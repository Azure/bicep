// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.Redirects;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Resolution;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Mock;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.UnitTests.Artifacts.Resolution;

/// <summary>
/// Characterization tests that pin how artifact reference text is resolved to an <see cref="ArtifactReference"/>
/// through the real resolver, registries and configuration discovery.
/// These protect the Phase 0 refactor of REP 0022 (artifact aliases and redirects) from unintended behavior changes.
/// </summary>
[TestClass]
public class ArtifactReferenceResolutionTests
{
    private const string Digest = "sha256:e207a69d02b3de40d48ede9fd208d80441a9e590a83a0bc915d46244c03310d4";

    private const string BicepConfig = """
        {
          "moduleAliases": {
            "br": {
              "contoso": { "registry": "contoso.azurecr.io", "modulePath": "bicep/modules" },
              "noModulePath": { "registry": "contoso.azurecr.io" },
              "noRegistry": { "modulePath": "bicep/modules" }
            },
            "ts": {
              "specs": { "subscription": "00000000-0000-0000-0000-000000000000", "resourceGroup": "specs-rg" },
              "noSubscription": { "resourceGroup": "specs-rg" }
            }
          },
          "moduleAliasesMock": {
            "br": {
              "mocked": { "mapToFilePath": "./mocks" }
            }
          },
          "extensions": {
            "localExt": "./extensions/local.tgz",
            "ociExt": "br:contoso.azurecr.io/extensions/foo:1.0.0",
            "ociAliasExt": "br/contoso:foo:1.0.0"
          }
        }
        """;

    private static readonly IOUri ConfigUri = CreateUri("/repo/bicepconfig.json");

    [TestMethod]
    [DataRow("fakeScheme:fakeModule", "BCP189: The specified module reference scheme \"fakeScheme\" is not recognized. Specify a path to a local module file.")]
    [DataRow("test.bicep", "LocalArtifactReference test.bicep -> /repo/sub/test.bicep")]
    public void NoRegistries_ValidateModuleReference_ShouldOnlyResolveLocalPaths(string referenceText, string expected)
    {
        // Local files do not go through registries, so they resolve even when no registries are configured.
        var (entryPoint, _, _, services) = Setup(entryContents: $"module foo '{referenceText}' = {{}}");
        var module = entryPoint.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>().Single();
        var resolver = new RedirectingArtifactResolver(
            new ArtifactReferenceResolver([], services.GetRequiredService<IFileExplorer>(), services.GetRequiredService<IBicepConfigurationManager>(), services.GetRequiredService<IRemoteArtifactContextFactory>()),
            []);

        Describe(ResolveDeclaration(resolver, entryPoint, module)).Should().Be(expected);
    }

    private static IOUri CreateUri(string path) => new("file", "", path);

    private static (BicepSourceFile EntryPoint, SourceFileGrouping Grouping, RedirectingArtifactResolver Resolver, IServiceProvider Services) Setup(string entryPath = "/repo/sub/main.bicep", string entryContents = "", string bicepConfig = BicepConfig)
    {
        var fileExplorer = new InMemoryFileExplorer();
        fileExplorer.GetFile(ConfigUri).WriteAllText(bicepConfig);
        fileExplorer.GetFile(CreateUri(entryPath)).WriteAllText(entryContents);

        var services = new ServiceCollection()
            .AddSingleton<IFileExplorer>(fileExplorer)
            .AddBicepCore()
            .BuildServiceProvider();

        var compilation = services.GetRequiredService<BicepCompiler>().CreateCompilationWithoutRestore(CreateUri(entryPath));
        var grouping = compilation.SourceFileGrouping;

        // The compiler resolves references through the redirect layer, which applies moduleAliasesMock.
        return (grouping.EntryPoint, grouping, services.GetRequiredService<RedirectingArtifactResolver>(), services);
    }

    private static ResultWithDiagnosticBuilder<ArtifactReference> ResolveDeclaration(RedirectingArtifactResolver resolver, BicepSourceFile referencingFile, IArtifactReferenceSyntax syntax) =>
        ArtifactReferenceText.TryGet(referencingFile, syntax).IsSuccess(out var referenceText, out var errorBuilder)
            ? resolver.TryResolve(referencingFile.FileHandle.Uri, syntax.GetArtifactType(), referenceText)
            : new(errorBuilder);

    private static string Describe(ResultWithDiagnosticBuilder<ArtifactReference> result) =>
        result.IsSuccess(out var reference, out var errorBuilder) ? Describe(reference) : Describe(errorBuilder);

    private static string Describe(ArtifactResolutionInfo resolutionInfo)
    {
        if (resolutionInfo.Reference is { } reference)
        {
            return Describe(reference);
        }

        resolutionInfo.Result.IsSuccess(out _, out var errorBuilder).Should().BeFalse();
        return Describe(errorBuilder!);
    }

    private static string Describe(ArtifactReference reference)
    {
        var description = $"{reference.GetType().Name} {reference.FullyQualifiedReference}";

        // Local files are anchored on the file system, so include where they resolve to.
        var localFile = reference switch
        {
            LocalArtifactReference local => local.File,
            _ => null,
        };

        return localFile is null ? description : $"{description} -> {localFile.Uri.Path}";
    }

    private static string Describe(DiagnosticBuilder.DiagnosticBuilderDelegate errorBuilder)
    {
        var diagnostic = errorBuilder(DiagnosticBuilder.ForDocumentStart());

        // The config path is formatted as a native file path, which differs between Windows and Linux.
        return $"{diagnostic.Code}: {diagnostic.Message.Replace(ConfigUri.ToString(), "<bicepconfig>")}";
    }

    [TestMethod]
    // relative paths
    [DataRow("mod.bicep", "LocalArtifactReference mod.bicep -> /repo/sub/mod.bicep")]
    [DataRow("./nested/mod.bicep", "LocalArtifactReference ./nested/mod.bicep -> /repo/sub/nested/mod.bicep")]
    [DataRow("../mod.bicep", "LocalArtifactReference ../mod.bicep -> /repo/mod.bicep")]
    [DataRow("nested\\mod.bicep", "BCP098: The specified file path contains a \"\\\" character. Use \"/\" instead as the directory separator character.")]
    [DataRow("/abs/mod.bicep", "BCP051: The specified path seems to reference an absolute path. Files must be referenced using relative paths.")]
    [DataRow("C:/abs/mod.bicep", "BCP189: The specified module reference scheme \"C\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("", "BCP050: The specified path is empty.")]
    // fully qualified OCI references
    [DataRow("br:contoso.azurecr.io/bicep/modules/storage:1.0.0", "OciArtifactReference br:contoso.azurecr.io/bicep/modules/storage:1.0.0")]
    [DataRow("br:CONTOSO.azurecr.io/bicep/modules/storage:1.0.0", "OciArtifactReference br:contoso.azurecr.io/bicep/modules/storage:1.0.0")]
    [DataRow("br:contoso.azurecr.io/storage@" + Digest, "OciArtifactReference br:contoso.azurecr.io/storage@" + Digest)]
    [DataRow("br:localhost:5000/storage:v1", "OciArtifactReference br:localhost:5000/storage:v1")]
    [DataRow("br:contoso.azurecr.io/storage", "BCP196: The specified OCI artifact reference \"br:contoso.azurecr.io/storage\" is not valid. The module tag or digest is missing.")]
    [DataRow("br:contoso.azurecr.io/Storage:v1", "BCP195: The specified OCI artifact reference \"br:contoso.azurecr.io/Storage:v1\" is not valid. The artifact path segment \"Storage\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
    [DataRow("br:", "BCP193: The specified OCI artifact reference \"br:\" is not valid. Specify a reference in the format of \"br:<artifact-uri>:<tag>\", or \"br/<module-alias>:<module-name-or-path>:<tag>\".")]
    // scheme-qualified OCI aliases
    [DataRow("br/public:avm/res/storage/storage-account:0.32.1", "OciArtifactReference br:mcr.microsoft.com/bicep/avm/res/storage/storage-account:0.32.1")]
    [DataRow("br/contoso:storage:1.0.0", "OciArtifactReference br:contoso.azurecr.io/bicep/modules/storage:1.0.0")]
    [DataRow("br/noModulePath:storage:1.0.0", "OciArtifactReference br:contoso.azurecr.io/storage:1.0.0")]
    [DataRow("br/missing:storage:1.0.0", "BCP213: The OCI artifact module alias name \"missing\" does not exist in the Bicep configuration \"<bicepconfig>\".")]
    [DataRow("br/noRegistry:storage:1.0.0", "BCP216: The OCI artifact module alias \"noRegistry\" in the Bicep configuration \"<bicepconfig>\" is invalid. The \"registry\" property must be specified.")]
    [DataRow("br/inv@lid:storage:1.0.0", "BCP211: The module alias name \"inv@lid\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".")]
    [DataRow("br/:storage:1.0.0", "BCP211: The module alias name \"\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".")]
    [DataRow("br/contoso:Storage:1.0.0", "BCP195: The OCI artifact reference \"br:contoso.azurecr.io/bicep/modules/Storage:1.0.0\" after resolving alias \"contoso\" is not valid. The artifact path segment \"Storage\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
    // module alias mocks
    [DataRow("br/mocked:network/vnet:1.0.0", "LocalArtifactReference br:network/vnet -> /repo/mocks/network/vnet.bicep")]
    [DataRow("br/mocked:network/vnet@" + Digest, "LocalArtifactReference br:network/vnet -> /repo/mocks/network/vnet.bicep")]
    [DataRow("br/mocked:Network:1.0.0", "BCP195: The OCI artifact reference \"Network:1.0.0\" after resolving alias \"mocked\" is not valid. The artifact path segment \"Network\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
    [DataRow("br-mock:network/vnet:1.0.0", "BCP449: The 'br-mock' module reference scheme is for internal use only. Use a 'br/<alias>:' reference with a configured 'mapToFilePath' alias instead.")]
    // Template Specs
    [DataRow("ts:00000000-0000-0000-0000-000000000000/specs-rg/webapp:v1", "TemplateSpecArtifactReference ts:00000000-0000-0000-0000-000000000000/specs-rg/webapp:v1")]
    [DataRow("ts/specs:webapp:v1", "TemplateSpecArtifactReference ts:00000000-0000-0000-0000-000000000000/specs-rg/webapp:v1")]
    [DataRow("ts/missing:webapp:v1", "BCP212: The Template Spec module alias name \"missing\" does not exist in the Bicep configuration \"<bicepconfig>\".")]
    [DataRow("ts/noSubscription:webapp:v1", "BCP214: The Template Spec module alias \"noSubscription\" in the Bicep configuration \"<bicepconfig>\" is in valid. The \"subscription\" property cannot be null or undefined.")]
    [DataRow("ts:not-a-guid/specs-rg/webapp:v1", "BCP217: The specified Template Spec reference \"ts:not-a-guid/specs-rg/webapp:v1\" is not valid. The subscription ID \"not-a-guid\" in is not a GUID.")]
    // unsupported schemes and syntax introduced by REP 0022
    [DataRow("unknown:foo", "BCP189: The specified module reference scheme \"unknown\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("oci:contoso.azurecr.io/storage:1.0.0", "BCP189: The specified module reference scheme \"oci\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("file:mod.bicep", "BCP189: The specified module reference scheme \"file\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("avm::res/storage/storage-account:0.32.1", "BCP189: The specified module reference scheme \"avm\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("br::storage:1.0.0", "BCP193: The specified OCI artifact reference \"br::storage:1.0.0\" is not valid. Specify a reference in the format of \"br:<artifact-uri>:<tag>\", or \"br/<module-alias>:<module-name-or-path>:<tag>\".")]
    [DataRow("oci/contoso:storage:1.0.0", "BCP189: The specified module reference scheme \"oci\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    // splitting edge cases
    [DataRow(":storage:1.0.0", "BCP189: The specified module reference scheme \"\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("br/contoso/nested:storage:1.0.0", "BCP211: The module alias name \"contoso/nested\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".")]
    [DataRow("../mod:1.bicep", "BCP189: The specified module reference scheme \"..\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("unknown/alias:storage:1.0.0", "BCP189: The specified module reference scheme \"unknown\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    public void Module_reference_text_resolves_to_expected_reference(string referenceText, string expected)
    {
        var (entryPoint, _, resolver, _) = Setup();

        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, referenceText)).Should().Be(expected);
    }

    [TestMethod]
    [DataRow("./extensions/local.tgz", "LocalArtifactReference ./extensions/local.tgz -> /repo/sub/extensions/local.tgz")]
    [DataRow("br:contoso.azurecr.io/extensions/foo:1.0.0", "OciArtifactReference br:contoso.azurecr.io/extensions/foo:1.0.0")]
    [DataRow("br/public:foo:1.0.0", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("br/contoso:foo:1.0.0", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("br/mocked:foo:1.0.0", "BCP448: The OCI artifact module alias \"mocked\" has a \"mapToFilePath\" property which is only supported for modules, not extensions.")]
    [DataRow("ts:00000000-0000-0000-0000-000000000000/specs-rg/webapp:v1", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("builtin:", "BCP189: The specified module reference scheme \"builtin\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    public void Extension_reference_text_resolves_to_expected_reference(string referenceText, string expected)
    {
        var (entryPoint, _, resolver, _) = Setup();

        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Extension, referenceText)).Should().Be(expected);
    }

    [TestMethod]
    // Bare paths in the "extensions" map are relative to bicepconfig.json and are rewritten relative to the referencing file.
    [DataRow("extension localExt", "LocalArtifactReference ../extensions/local.tgz -> /repo/extensions/local.tgz")]
    [DataRow("extension ociExt", "OciArtifactReference br:contoso.azurecr.io/extensions/foo:1.0.0")]
    [DataRow("extension ociAliasExt", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("extension unknownExt", "BCP204: Extension \"unknownExt\" is not recognized.")]
    [DataRow("extension 'br:contoso.azurecr.io/extensions/foo:1.0.0'", "OciArtifactReference br:contoso.azurecr.io/extensions/foo:1.0.0")]
    public void Extension_declaration_resolves_to_expected_reference(string declaration, string expected)
    {
        var (entryPoint, grouping, resolver, _) = Setup(entryContents: declaration);
        var syntax = entryPoint.ProgramSyntax.Declarations.OfType<ExtensionDeclarationSyntax>().Single();

        Describe(ResolveDeclaration(resolver, entryPoint, syntax)).Should().Be(expected);

        // The source file grouping stores the same reference that the resolver produces.
        Describe(grouping.ArtifactLookup[syntax]).Should().Be(expected);
    }

    [TestMethod]
    public void Implicit_extension_with_a_bare_path_is_resolved_relative_to_the_referencing_file()
    {
        // Pins current behavior: unlike "extension localExt", an implicit extension passes the raw "extensions" value
        // to the resolver, so a bare path is anchored on the referencing file rather than on bicepconfig.json.
        var (entryPoint, grouping, _, _) = Setup(bicepConfig: """
            {
              "extensions": { "localExt": "./extensions/local.tgz" },
              "implicitExtensions": ["az", "localExt"]
            }
            """);

        var implicitExtension = grouping.ImplicitExtensions[entryPoint].Single(x => x.Name == "localExt");

        Describe(implicitExtension.Artifact!).Should().Be("LocalArtifactReference ./extensions/local.tgz -> /repo/sub/extensions/local.tgz");
    }

    [TestMethod]
    [DataRow("br/mocked:network/vnet:1.0.0", "BCP213: The OCI artifact module alias name \"mocked\" does not exist in the Bicep configuration \"<bicepconfig>\".")]
    [DataRow("br/contoso:storage:1.0.0", "OciArtifactReference br:contoso.azurecr.io/bicep/modules/storage:1.0.0")]
    public void Module_alias_mocks_only_apply_to_what_the_compiler_loads(string referenceText, string expected)
    {
        // The resolver never applies mocks. The compiler applies them through the redirect layer, and
        // `bicep publish` applies the mock rule only to keep rejecting mocked aliases as targets.
        var (entryPoint, _, _, services) = Setup();
        var resolver = services.GetRequiredService<IArtifactReferenceResolver>();

        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, referenceText)).Should().Be(expected);
    }

    [TestMethod]
    public void Local_module_references_with_the_same_text_from_different_folders_are_not_equal()
    {
        // Equality compares the files that references resolve to, not the reference text.
        var (first, _, resolver, _) = Setup("/repo/a/main.bicep");
        var (second, _, _, _) = Setup("/repo/b/main.bicep");

        var firstReference = resolver.TryResolve(first.FileHandle.Uri, ArtifactType.Module, "mod.bicep").Unwrap();
        var secondReference = resolver.TryResolve(second.FileHandle.Uri, ArtifactType.Module, "mod.bicep").Unwrap();

        firstReference.Should().BeOfType<LocalArtifactReference>().Which.File.Uri.Path.Should().Be("/repo/a/mod.bicep");
        secondReference.Should().BeOfType<LocalArtifactReference>().Which.File.Uri.Path.Should().Be("/repo/b/mod.bicep");
        firstReference.Should().NotBe(secondReference);
    }

    [TestMethod]
    [DataRow("br:contoso.azurecr.io/bicep/modules/foo:v1")]
    [DataRow("ts:00000000-0000-0000-0000-000000000000/specs-rg/foo:v1")]
    public void Remote_references_to_the_same_artifact_are_equal_only_if_restored_with_the_same_cloud(string referenceText)
    {
        // The external artifact context comes from each referencing file's configuration, so an artifact reached from folders with
        // different clouds is restored once per cloud.
        var (first, _, resolver, services) = Setup("/repo/a/main.bicep");
        var fileExplorer = services.GetRequiredService<IFileExplorer>();
        fileExplorer.GetFile(CreateUri("/repo/china/bicepconfig.json")).WriteAllText("""{ "cloud": { "currentProfile": "AzureChinaCloud" } }""");

        var sameCloudReference = resolver.TryResolve(CreateUri("/repo/b/main.bicep"), ArtifactType.Module, referenceText).Unwrap();
        var otherCloudReference = resolver.TryResolve(CreateUri("/repo/china/main.bicep"), ArtifactType.Module, referenceText).Unwrap();
        var reference = resolver.TryResolve(first.FileHandle.Uri, ArtifactType.Module, referenceText).Unwrap();

        reference.Should().Be(sameCloudReference);
        reference.GetHashCode().Should().Be(sameCloudReference.GetHashCode());
        reference.Should().NotBe(otherCloudReference);
        reference.FullyQualifiedReference.Should().Be(otherCloudReference.FullyQualifiedReference);
        ((RemoteArtifactReference)otherCloudReference).Context.Cloud.ResourceManagerEndpointUri.Should().Be(new Uri("https://management.chinacloudapi.cn"));
    }

    [TestMethod]
    public void Only_OCI_references_depend_on_the_ORAS_setting()
    {
        // The ORAS setting only changes how OCI registries are reached, so it is part of the identity of OCI references only.
        var (entryPoint, _, resolver, services) = Setup("/repo/a/main.bicep");
        services.GetRequiredService<IFileExplorer>().GetFile(CreateUri("/repo/oras/bicepconfig.json")).WriteAllText("""{ "experimentalFeaturesEnabled": { "ociEnabled": true } }""");
        var orasUri = CreateUri("/repo/oras/main.bicep");

        const string OciText = "br:contoso.azurecr.io/bicep/modules/foo:v1";
        var ociReference = resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, OciText).Unwrap().Should().BeOfType<OciArtifactReference>().Subject;
        var orasOciReference = resolver.TryResolve(orasUri, ArtifactType.Module, OciText).Unwrap().Should().BeOfType<OciArtifactReference>().Subject;

        ociReference.UseOrasSdk.Should().BeFalse();
        orasOciReference.UseOrasSdk.Should().BeTrue();
        ociReference.Should().NotBe(orasOciReference);

        const string TemplateSpecText = "ts:00000000-0000-0000-0000-000000000000/specs-rg/foo:v1";
        var templateSpecReference = resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, TemplateSpecText).Unwrap();
        var orasTemplateSpecReference = resolver.TryResolve(orasUri, ArtifactType.Module, TemplateSpecText).Unwrap();

        templateSpecReference.Should().Be(orasTemplateSpecReference);
        templateSpecReference.GetHashCode().Should().Be(orasTemplateSpecReference.GetHashCode());
    }
}
