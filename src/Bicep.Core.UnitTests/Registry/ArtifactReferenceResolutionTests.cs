// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Registry;

/// <summary>
/// Characterization tests that pin how artifact reference text is resolved to an <see cref="ArtifactReference"/>
/// through the real module dispatcher, registries and configuration discovery.
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

    private static IOUri CreateUri(string path) => new("file", "", path);

    private static (BicepSourceFile EntryPoint, SourceFileGrouping Grouping, IModuleDispatcher Dispatcher) Setup(string entryPath = "/repo/sub/main.bicep", string entryContents = "", string bicepConfig = BicepConfig)
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

        return (grouping.EntryPoint, grouping, services.GetRequiredService<IModuleDispatcher>());
    }

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
            LocalModuleReference local => local.TryGetRelativeFile(local.Path).TryUnwrap(),
            OciArtifactMockedReference mocked => mocked.TryGetEntryPointFileHandle().TryUnwrap(),
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
    [DataRow("mod.bicep", "LocalModuleReference mod.bicep -> /repo/sub/mod.bicep")]
    [DataRow("./nested/mod.bicep", "LocalModuleReference ./nested/mod.bicep -> /repo/sub/nested/mod.bicep")]
    [DataRow("../mod.bicep", "LocalModuleReference ../mod.bicep -> /repo/mod.bicep")]
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
    [DataRow("br/mocked:network/vnet:1.0.0", "OciArtifactMockedReference br:network/vnet -> /repo/mocks/network/vnet.bicep")]
    [DataRow("br/mocked:network/vnet@" + Digest, "OciArtifactMockedReference br:network/vnet -> /repo/mocks/network/vnet.bicep")]
    [DataRow("br/mocked:Network:1.0.0", "BCP195: The OCI artifact reference \"Network:1.0.0\" after resolving alias \"mocked\" is not valid. The artifact path segment \"Network\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
    [DataRow("br-mock:network/vnet:1.0.0", "BCP449: The 'br-mock' module reference scheme is for internal use only. Use a 'br/<alias>:' reference with a configured 'mapToFilePath' alias instead.")]
    // Template Specs
    [DataRow("ts:00000000-0000-0000-0000-000000000000/specs-rg/webapp:v1", "TemplateSpecModuleReference ts:00000000-0000-0000-0000-000000000000/specs-rg/webapp:v1")]
    [DataRow("ts/specs:webapp:v1", "TemplateSpecModuleReference ts:00000000-0000-0000-0000-000000000000/specs-rg/webapp:v1")]
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
    public void Module_reference_text_resolves_to_expected_reference(string referenceText, string expected)
    {
        var (entryPoint, _, dispatcher) = Setup();

        Describe(dispatcher.TryGetArtifactReference(entryPoint, ArtifactType.Module, referenceText)).Should().Be(expected);
    }

    [TestMethod]
    [DataRow("./extensions/local.tgz", "LocalModuleReference ./extensions/local.tgz -> /repo/sub/extensions/local.tgz")]
    [DataRow("br:contoso.azurecr.io/extensions/foo:1.0.0", "OciArtifactReference br:contoso.azurecr.io/extensions/foo:1.0.0")]
    [DataRow("br/public:foo:1.0.0", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("br/contoso:foo:1.0.0", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("br/mocked:foo:1.0.0", "BCP448: The OCI artifact module alias \"mocked\" has a \"mapToFilePath\" property which is only supported for modules, not extensions.")]
    [DataRow("ts:00000000-0000-0000-0000-000000000000/specs-rg/webapp:v1", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("builtin:", "BCP189: The specified module reference scheme \"builtin\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    public void Extension_reference_text_resolves_to_expected_reference(string referenceText, string expected)
    {
        var (entryPoint, _, dispatcher) = Setup();

        Describe(dispatcher.TryGetArtifactReference(entryPoint, ArtifactType.Extension, referenceText)).Should().Be(expected);
    }

    [TestMethod]
    // Bare paths in the "extensions" map are relative to bicepconfig.json and are rewritten relative to the referencing file.
    [DataRow("extension localExt", "LocalModuleReference ../extensions/local.tgz -> /repo/extensions/local.tgz")]
    [DataRow("extension ociExt", "OciArtifactReference br:contoso.azurecr.io/extensions/foo:1.0.0")]
    [DataRow("extension ociAliasExt", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("extension unknownExt", "BCP204: Extension \"unknownExt\" is not recognized.")]
    [DataRow("extension 'br:contoso.azurecr.io/extensions/foo:1.0.0'", "OciArtifactReference br:contoso.azurecr.io/extensions/foo:1.0.0")]
    public void Extension_declaration_resolves_to_expected_reference(string declaration, string expected)
    {
        var (entryPoint, grouping, dispatcher) = Setup(entryContents: declaration);
        var syntax = entryPoint.ProgramSyntax.Declarations.OfType<ExtensionDeclarationSyntax>().Single();

        Describe(dispatcher.TryGetArtifactReference(entryPoint, syntax)).Should().Be(expected);

        // The source file grouping stores the same reference that the dispatcher produces.
        Describe(grouping.ArtifactLookup[syntax]).Should().Be(expected);
    }

    [TestMethod]
    public void Implicit_extension_with_a_bare_path_is_resolved_relative_to_the_referencing_file()
    {
        // Pins current behavior: unlike "extension localExt", an implicit extension passes the raw "extensions" value
        // to the dispatcher, so a bare path is anchored on the referencing file rather than on bicepconfig.json.
        var (entryPoint, grouping, _) = Setup(bicepConfig: """
            {
              "extensions": { "localExt": "./extensions/local.tgz" },
              "implicitExtensions": ["az", "localExt"]
            }
            """);

        var implicitExtension = grouping.ImplicitExtensions[entryPoint].Single(x => x.Name == "localExt");

        Describe(implicitExtension.Artifact!).Should().Be("LocalModuleReference ./extensions/local.tgz -> /repo/sub/extensions/local.tgz");
    }

    [TestMethod]
    public void Local_module_references_with_the_same_text_from_different_folders_are_equal()
    {
        // Pins current behavior: equality only compares the relative path text, not the file it resolves to.
        // Phase 0.5 of the REP 0022 implementation plan is expected to change this to compare resolved file URIs.
        var (first, _, dispatcher) = Setup("/repo/a/main.bicep");
        var (second, _, _) = Setup("/repo/b/main.bicep");

        var firstReference = dispatcher.TryGetArtifactReference(first, ArtifactType.Module, "mod.bicep").Unwrap();
        var secondReference = dispatcher.TryGetArtifactReference(second, ArtifactType.Module, "mod.bicep").Unwrap();

        firstReference.TryGetEntryPointFileHandle().Unwrap().Uri.Path.Should().Be("/repo/a/mod.bicep");
        secondReference.TryGetEntryPointFileHandle().Unwrap().Uri.Path.Should().Be("/repo/b/mod.bicep");
        firstReference.Should().Be(secondReference);
    }
}
