// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.Redirects;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Resolution;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
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
          "experimentalFeaturesEnabled": { "artifactAliasesAndRedirects": true },
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
          "artifacts": {
            "aliases": {
              "company": { "type": "oci", "registry": "contoso.azurecr.io", "repositoryPrefix": "bicep/modules" },
              "noPrefix": { "type": "oci", "registry": "contoso.azurecr.io" },
              "storage": { "type": "oci", "registry": "mcr.microsoft.com", "repositoryPrefix": "bicep/avm/res/storage/storage-account" },
              "contoso": { "type": "oci", "registry": "other.azurecr.io" },
              "specs": { "type": "ts", "subscriptionId": "00000000-0000-0000-0000-000000000000", "resourceGroup": "specs-rg" },
              "modules": { "type": "file", "path": "./modules" },
              "noType": { "registry": "contoso.azurecr.io" },
              "badType": { "type": "blob" },
              "ociNoRegistry": { "type": "oci" },
              "emptyRegistry": { "type": "oci", "registry": "" },
              "absolutePath": { "type": "file", "path": "/modules" },
              "notAnObject": "contoso.azurecr.io",
              "ts": { "type": "oci", "registry": "contoso.azurecr.io" }
            }
          },
          "extensions": {
            "localExt": "./extensions/local.tgz",
            "ociExt": "br:contoso.azurecr.io/extensions/foo:1.0.0",
            "ociAliasExt": "br/contoso:foo:1.0.0",
            "ociSchemeExt": "oci:contoso.azurecr.io/extensions/foo:1.0.0",
            "fileExt": "file:./extensions/local.tgz",
            "absoluteFileExt": "file:/extensions/local.tgz",
            "artifactAliasExt": "company::foo:1.0.0"
          }
        }
        """;

    private static readonly IOUri ConfigUri = CreateUri("/repo/bicepconfig.json");

    [TestMethod]
    [DataRow("fakeScheme:fakeModule", "BCP189: The specified module reference scheme \"fakeScheme\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"file\"")]
    [DataRow("test.bicep", "LocalArtifactReference test.bicep -> /repo/sub/test.bicep")]
    public void NoRegistries_ValidateModuleReference_ShouldOnlyResolveLocalPaths(string referenceText, string expected)
    {
        // Local files do not go through registries, so they resolve even when no registries are configured.
        var (entryPoint, _, _, services) = Setup(entryContents: $"module foo '{referenceText}' = {{}}");
        var module = entryPoint.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>().Single();
        var resolver = new RedirectingArtifactResolver(
            new ArtifactReferenceResolver([], services.GetRequiredService<IFileExplorer>(), services.GetRequiredService<IBicepConfigurationManager>(), services.GetRequiredService<IFeatureProviderFactory>(), services.GetRequiredService<IRemoteArtifactContextFactory>()),
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

    private static string Describe(ArtifactDependency dependency)
    {
        if (dependency.Reference is { } reference)
        {
            return Describe(reference);
        }

        dependency.TryGetArtifact().IsSuccess(out _, out var errorBuilder).Should().BeFalse();
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
    [DataRow("C:/abs/mod.bicep", "BCP189: The specified module reference scheme \"C\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"file\", \"oci\", \"ts\"")]
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
    // the oci scheme: another spelling of br, persisted as br
    [DataRow("oci:contoso.azurecr.io/storage:1.0.0", "OciArtifactReference br:contoso.azurecr.io/storage:1.0.0")]
    [DataRow("oci:localhost:5000/storage@" + Digest, "OciArtifactReference br:localhost:5000/storage@" + Digest)]
    [DataRow("oci:contoso.azurecr.io/Storage:v1", "BCP195: The specified OCI artifact reference \"oci:contoso.azurecr.io/Storage:v1\" is not valid. The artifact path segment \"Storage\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
    [DataRow("oci/contoso:storage:1.0.0", "BCP460: The \"oci\" scheme does not support aliases in the form \"<scheme>/<alias>:\", which only the \"br\" and \"ts\" schemes support. To use an alias defined in \"artifacts.aliases\", write \"<alias>::\".")]
    // the file scheme: a path relative to the referencing file
    [DataRow("file:mod.bicep", "LocalArtifactReference file:mod.bicep -> /repo/sub/mod.bicep")]
    [DataRow("file:../nested/mod.json", "LocalArtifactReference file:../nested/mod.json -> /repo/nested/mod.json")]
    [DataRow("file:main.bicepparam", "LocalArtifactReference file:main.bicepparam -> /repo/sub/main.bicepparam")]
    [DataRow("file:template.arm", "LocalArtifactReference file:template.arm -> /repo/sub/template.arm")]
    [DataRow("file:mod", "BCP461: The reference \"file:mod\" is not valid. A module referenced with the \"file\" scheme or a file alias must be a Bicep file (\".bicep\" or \".bicepparam\") or an ARM template (\".json\", \".jsonc\" or \".arm\").")]
    [DataRow("file:mod.txt", "BCP461: The reference \"file:mod.txt\" is not valid. A module referenced with the \"file\" scheme or a file alias must be a Bicep file (\".bicep\" or \".bicepparam\") or an ARM template (\".json\", \".jsonc\" or \".arm\").")]
    [DataRow("file:/abs/mod.bicep", "BCP051: The specified path seems to reference an absolute path. Files must be referenced using relative paths.")]
    [DataRow("file:C:/abs/mod.bicep", "BCP051: The specified path seems to reference an absolute path. Files must be referenced using relative paths.")]
    [DataRow("file:", "BCP050: The specified path is empty.")]
    [DataRow("file/contoso:mod.bicep", "BCP460: The \"file\" scheme does not support aliases in the form \"<scheme>/<alias>:\", which only the \"br\" and \"ts\" schemes support. To use an alias defined in \"artifacts.aliases\", write \"<alias>::\".")]
    // artifact aliases: predefined
    [DataRow("avm::res/storage/storage-account:0.32.1", "OciArtifactReference br:mcr.microsoft.com/bicep/avm/res/storage/storage-account:0.32.1")]
    [DataRow("mar::avm/res/storage/storage-account:0.32.1", "OciArtifactReference br:mcr.microsoft.com/bicep/avm/res/storage/storage-account:0.32.1")]
    // artifact aliases: OCI
    [DataRow("company::storage:1.0.0", "OciArtifactReference br:contoso.azurecr.io/bicep/modules/storage:1.0.0")]
    [DataRow("company::storage@" + Digest, "OciArtifactReference br:contoso.azurecr.io/bicep/modules/storage@" + Digest)]
    [DataRow("noPrefix::storage:1.0.0", "OciArtifactReference br:contoso.azurecr.io/storage:1.0.0")]
    [DataRow("storage:::0.32.1", "OciArtifactReference br:mcr.microsoft.com/bicep/avm/res/storage/storage-account:0.32.1")]
    [DataRow("storage::@" + Digest, "OciArtifactReference br:mcr.microsoft.com/bicep/avm/res/storage/storage-account@" + Digest)]
    [DataRow("company::Storage:1.0.0", "BCP195: The OCI artifact reference \"oci:contoso.azurecr.io/bicep/modules/Storage:1.0.0\" after resolving alias \"company\" is not valid. The artifact path segment \"Storage\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
    [DataRow("company::storage", "BCP196: The OCI artifact reference \"oci:contoso.azurecr.io/bicep/modules/storage\" after resolving alias \"company\" is not valid. The module tag or digest is missing.")]
    [DataRow("company::", "BCP464: The reference \"company::\" is not valid. Specify an artifact path after \"company::\".")]
    [DataRow("noPrefix:::1.0.0", "BCP464: The reference \"noPrefix:::1.0.0\" is not valid. A tag or digest can only follow \"noPrefix::\" directly if the alias sets \"repositoryPrefix\" to a complete repository.")]
    // artifact aliases: Template Spec
    [DataRow("specs::webapp:v1", "TemplateSpecArtifactReference ts:00000000-0000-0000-0000-000000000000/specs-rg/webapp:v1")]
    [DataRow("specs:::v1", "BCP464: The reference \"specs:::v1\" is not valid. Specify an artifact path after \"specs::\".")]
    // artifact aliases: file, relative to the bicepconfig.json that declares them
    [DataRow("modules::network/vnet.bicep", "LocalArtifactReference modules::network/vnet.bicep -> /repo/modules/network/vnet.bicep")]
    [DataRow("modules::../shared/main.json", "LocalArtifactReference modules::../shared/main.json -> /repo/shared/main.json")]
    [DataRow("modules::network/vnet", "BCP461: The reference \"modules::network/vnet\" is not valid. A module referenced with the \"file\" scheme or a file alias must be a Bicep file (\".bicep\" or \".bicepparam\") or an ARM template (\".json\", \".jsonc\" or \".arm\").")]
    [DataRow("modules::/abs/vnet.bicep", "BCP051: The specified path seems to reference an absolute path. Files must be referenced using relative paths.")]
    [DataRow("modules::", "BCP464: The reference \"modules::\" is not valid. Specify an artifact path after \"modules::\".")]
    // artifact aliases and module aliases are independent, even with the same name
    [DataRow("contoso::storage:1.0.0", "OciArtifactReference br:other.azurecr.io/storage:1.0.0")]
    [DataRow("br/company:storage:1.0.0", "BCP213: The OCI artifact module alias name \"company\" does not exist in the Bicep configuration \"<bicepconfig>\".")]
    [DataRow("noModulePath::storage:1.0.0", "BCP462: The artifact alias \"noModulePath\" does not exist in the Bicep configuration \"<bicepconfig>\". Define it in \"artifacts.aliases\".")]
    // artifact aliases: scheme names are reserved, even if they're defined
    [DataRow("br::storage:1.0.0", "BCP465: \"br\" can't be used as an artifact alias because the name is reserved for a scheme. A fully qualified reference has a single \":\" after the scheme, as in \"oci:<registry>/<repository>:<tag>\".")]
    [DataRow("oci::storage:1.0.0", "BCP465: \"oci\" can't be used as an artifact alias because the name is reserved for a scheme. A fully qualified reference has a single \":\" after the scheme, as in \"oci:<registry>/<repository>:<tag>\".")]
    [DataRow("ts::webapp:v1", "BCP465: \"ts\" can't be used as an artifact alias because the name is reserved for a scheme. A fully qualified reference has a single \":\" after the scheme, as in \"oci:<registry>/<repository>:<tag>\".")]
    [DataRow("file::mod.bicep", "BCP465: \"file\" can't be used as an artifact alias because the name is reserved for a scheme. A fully qualified reference has a single \":\" after the scheme, as in \"oci:<registry>/<repository>:<tag>\".")]
    [DataRow("br-mock::storage:1.0.0", "BCP465: \"br-mock\" can't be used as an artifact alias because the name is reserved for a scheme. A fully qualified reference has a single \":\" after the scheme, as in \"oci:<registry>/<repository>:<tag>\".")]
    [DataRow("OCI::storage:1.0.0", "BCP465: \"OCI\" can't be used as an artifact alias because the name is reserved for a scheme. A fully qualified reference has a single \":\" after the scheme, as in \"oci:<registry>/<repository>:<tag>\".")]
    // artifact aliases: unknown or invalid
    [DataRow("missing::storage:1.0.0", "BCP462: The artifact alias \"missing\" does not exist in the Bicep configuration \"<bicepconfig>\". Define it in \"artifacts.aliases\".")]
    [DataRow("Company::storage:1.0.0", "BCP462: The artifact alias \"Company\" does not exist in the Bicep configuration \"<bicepconfig>\". Define it in \"artifacts.aliases\".")]
    [DataRow("noType::storage:1.0.0", "BCP463: The \"artifacts.aliases.noType\" property in the Bicep configuration \"<bicepconfig>\" is invalid. The \"type\" property must be \"oci\", \"ts\" or \"file\".")]
    [DataRow("badType::storage:1.0.0", "BCP463: The \"artifacts.aliases.badType\" property in the Bicep configuration \"<bicepconfig>\" is invalid. The \"type\" property must be \"oci\", \"ts\" or \"file\".")]
    [DataRow("ociNoRegistry::storage:1.0.0", "BCP463: The \"artifacts.aliases.ociNoRegistry\" property in the Bicep configuration \"<bicepconfig>\" is invalid. The \"registry\" property must be specified.")]
    [DataRow("emptyRegistry::storage:1.0.0", "BCP463: The \"artifacts.aliases.emptyRegistry.registry\" property in the Bicep configuration \"<bicepconfig>\" is invalid. The value must be a non-empty string.")]
    [DataRow("absolutePath::vnet.bicep", "BCP463: The \"artifacts.aliases.absolutePath\" property in the Bicep configuration \"<bicepconfig>\" is invalid. The \"path\" property must be a relative path that uses \"/\" as the separator, such as \"./modules\".")]
    [DataRow("notAnObject::storage:1.0.0", "BCP463: The \"artifacts.aliases.notAnObject\" property in the Bicep configuration \"<bicepconfig>\" is invalid. The value must be an object with a \"type\" property.")]
    // unknown schemes
    [DataRow("unknown:foo", "BCP189: The specified module reference scheme \"unknown\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"file\", \"oci\", \"ts\"")]
    // splitting edge cases
    [DataRow(":storage:1.0.0", "BCP189: The specified module reference scheme \"\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"file\", \"oci\", \"ts\"")]
    [DataRow("br/contoso/nested:storage:1.0.0", "BCP211: The module alias name \"contoso/nested\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".")]
    [DataRow("../mod:1.bicep", "BCP189: The specified module reference scheme \"..\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"file\", \"oci\", \"ts\"")]
    [DataRow("unknown/alias:storage:1.0.0", "BCP189: The specified module reference scheme \"unknown\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"file\", \"oci\", \"ts\"")]
    public void Module_reference_text_resolves_to_expected_reference(string referenceText, string expected)
    {
        var (entryPoint, _, resolver, _) = Setup();

        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, referenceText)).Should().Be(expected);
    }

    [TestMethod]
    [DataRow("./extensions/local.tgz", "LocalArtifactReference ./extensions/local.tgz -> /repo/sub/extensions/local.tgz")]
    [DataRow("br:contoso.azurecr.io/extensions/foo:1.0.0", "OciArtifactReference br:contoso.azurecr.io/extensions/foo:1.0.0")]
    [DataRow("oci:contoso.azurecr.io/extensions/foo:1.0.0", "OciArtifactReference br:contoso.azurecr.io/extensions/foo:1.0.0")]
    [DataRow("file:./extensions/local.tgz", "LocalArtifactReference file:./extensions/local.tgz -> /repo/sub/extensions/local.tgz")]
    [DataRow("file:./extensions/local", "LocalArtifactReference file:./extensions/local -> /repo/sub/extensions/local")]
    [DataRow("br/public:foo:1.0.0", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("br/contoso:foo:1.0.0", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("br/mocked:foo:1.0.0", "BCP448: The OCI artifact module alias \"mocked\" has a \"mapToFilePath\" property which is only supported for modules, not extensions.")]
    [DataRow("ts:00000000-0000-0000-0000-000000000000/specs-rg/webapp:v1", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("company::foo:1.0.0", "OciArtifactReference br:contoso.azurecr.io/bicep/modules/foo:1.0.0")]
    [DataRow("specs::webapp:v1", "BCP380: Artifacts of type: \"Extension\" are not supported.")]
    [DataRow("modules::extensions/local.tgz", "LocalArtifactReference modules::extensions/local.tgz -> /repo/modules/extensions/local.tgz")]
    [DataRow("builtin:", "BCP189: The specified module reference scheme \"builtin\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"file\", \"oci\", \"ts\"")]
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
    [DataRow("extension ociSchemeExt", "OciArtifactReference br:contoso.azurecr.io/extensions/foo:1.0.0")]
    // A file: path in the "extensions" map is relative to bicepconfig.json, like a bare path.
    [DataRow("extension fileExt", "LocalArtifactReference file:../extensions/local.tgz -> /repo/extensions/local.tgz")]
    [DataRow("extension absoluteFileExt", "BCP051: The specified path seems to reference an absolute path. Files must be referenced using relative paths.")]
    [DataRow("extension 'file:./extensions/local.tgz'", "LocalArtifactReference file:./extensions/local.tgz -> /repo/sub/extensions/local.tgz")]
    [DataRow("extension artifactAliasExt", "OciArtifactReference br:contoso.azurecr.io/bicep/modules/foo:1.0.0")]
    [DataRow("extension 'modules::extensions/local.tgz'", "LocalArtifactReference modules::extensions/local.tgz -> /repo/modules/extensions/local.tgz")]
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

        Describe(implicitExtension.Dependency!).Should().Be("LocalArtifactReference ./extensions/local.tgz -> /repo/sub/extensions/local.tgz");
    }

    [TestMethod]
    public void ImplicitExtension_FileAliasValue_ResolvesRelativeToAliasDeclaringConfiguration()
    {
        var (entryPoint, grouping, _, _) = Setup(bicepConfig: """
            {
              "experimentalFeaturesEnabled": { "artifactAliasesAndRedirects": true },
              "artifacts": { "aliases": { "exts": { "type": "file", "path": "./extensions" } } },
              "extensions": { "localExt": "exts::local.tgz" },
              "implicitExtensions": ["az", "localExt"]
            }
            """);

        var implicitExtension = grouping.ImplicitExtensions[entryPoint].Single(x => x.Name == "localExt");

        Describe(implicitExtension.Dependency!).Should().Be("LocalArtifactReference exts::local.tgz -> /repo/extensions/local.tgz");
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

    [TestMethod]
    public void Oci_and_br_spellings_name_the_same_artifact()
    {
        var (entryPoint, _, resolver, _) = Setup();

        var brReference = resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "br:contoso.azurecr.io/bicep/modules/storage:1.0.0").Unwrap();
        var ociReference = resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "oci:CONTOSO.azurecr.io/bicep/modules/storage:1.0.0").Unwrap();
        var aliasReference = resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "br/contoso:storage:1.0.0").Unwrap();
        var artifactAliasReference = resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "company::storage:1.0.0").Unwrap();

        // Equal references are restored and cached once, and persisted with the br scheme.
        ociReference.Should().Be(brReference).And.Be(aliasReference).And.Be(artifactAliasReference);
        ociReference.GetHashCode().Should().Be(brReference.GetHashCode());
        ociReference.FullyQualifiedReference.Should().Be("br:contoso.azurecr.io/bicep/modules/storage:1.0.0");
    }

    [TestMethod]
    // With the experimental feature off, every REP 0022 spelling fails as it does without the feature.
    [DataRow("module foo 'oci:contoso.azurecr.io/storage:1.0.0' = {}", "BCP189: The specified module reference scheme \"oci\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("module foo 'file:mod.bicep' = {}", "BCP189: The specified module reference scheme \"file\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("module foo 'oci/contoso:storage:1.0.0' = {}", "BCP189: The specified module reference scheme \"oci\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("module foo 'avm::res/storage/storage-account:0.32.1' = {}", "BCP189: The specified module reference scheme \"avm\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("extension fileExt", "BCP189: The specified module reference scheme \"file\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")]
    [DataRow("module foo 'br:contoso.azurecr.io/storage:1.0.0' = {}", "OciArtifactReference br:contoso.azurecr.io/storage:1.0.0")]
    public void Experimental_spellings_are_rejected_when_the_feature_is_disabled(string declaration, string expected)
    {
        var (entryPoint, grouping, _, _) = Setup(entryContents: declaration, bicepConfig: BicepConfig.Replace("\"artifactAliasesAndRedirects\": true", "\"artifactAliasesAndRedirects\": false"));
        var syntax = entryPoint.ProgramSyntax.Declarations.OfType<IArtifactReferenceSyntax>().Single();

        Describe(grouping.ArtifactLookup[syntax]).Should().Be(expected);
    }

    private const string AliasesBaseConfig = """
        {
          "experimentalFeaturesEnabled": { "artifactAliasesAndRedirects": true },
          "artifacts": {
            "aliases": {
              "modules": { "type": "file", "path": "./modules" },
              "broken": { "type": "oci" }
            }
          }
        }
        """;

    [TestMethod]
    public void TryResolve_InheritedArtifactAlias_ResolvesRelativeToDeclaringConfiguration()
    {
        // /repo/bicepconfig.json declares the aliases; /repo/sub/bicepconfig.json extends it.
        var (entryPoint, _, resolver, services) = Setup(bicepConfig: AliasesBaseConfig);
        var childConfigUri = CreateUri("/repo/sub/bicepconfig.json");
        services.GetRequiredService<IFileExplorer>().GetFile(childConfigUri).WriteAllText("""
            {
              "extends": "../bicepconfig.json",
              "artifacts": { "aliases": { "local": { "type": "file", "path": "./local" } } }
            }
            """);
        services.GetRequiredService<IBicepConfigurationManager>().InvalidateAll();

        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "modules::vnet.bicep")).Should().Be("LocalArtifactReference modules::vnet.bicep -> /repo/modules/vnet.bicep");
        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "local::vnet.bicep")).Should().Be("LocalArtifactReference local::vnet.bicep -> /repo/sub/local/vnet.bicep");

        // An invalid alias names the file that declared it; a missing one names the leaf, where it would be added.
        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "broken::vnet:1.0.0")).Should().Be(
            "BCP463: The \"artifacts.aliases.broken\" property in the Bicep configuration \"<bicepconfig>\" is invalid. The \"registry\" property must be specified.");
        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "missing::vnet:1.0.0")).Should().Be(
            $"BCP462: The artifact alias \"missing\" does not exist in the Bicep configuration \"{childConfigUri}\". Define it in \"artifacts.aliases\".");
    }

    [TestMethod]
    public void TryResolve_AliasDefinedDifferentlyPerFolder_UsesReferencingFileConfiguration()
    {
        var (entryPoint, _, resolver, services) = Setup(bicepConfig: AliasesBaseConfig);
        services.GetRequiredService<IFileExplorer>().GetFile(CreateUri("/repo/other/bicepconfig.json")).WriteAllText("""
            {
              "experimentalFeaturesEnabled": { "artifactAliasesAndRedirects": true },
              "artifacts": { "aliases": { "modules": { "type": "file", "path": "../shared" } } }
            }
            """);

        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "modules::vnet.bicep")).Should().Be("LocalArtifactReference modules::vnet.bicep -> /repo/modules/vnet.bicep");
        Describe(resolver.TryResolve(CreateUri("/repo/other/main.bicep"), ArtifactType.Module, "modules::vnet.bicep")).Should().Be("LocalArtifactReference modules::vnet.bicep -> /repo/shared/vnet.bicep");
    }

    [TestMethod]
    // An override replaces the whole predefined entry, so "repositoryPrefix" isn't inherited.
    [DataRow("""{ "type": "oci", "registry": "contoso.azurecr.io" }""", "OciArtifactReference br:contoso.azurecr.io/res/storage/storage-account:0.32.1")]
    [DataRow("""{ "type": "oci", "registry": "contoso.azurecr.io", "repositoryPrefix": "mirror/avm" }""", "OciArtifactReference br:contoso.azurecr.io/mirror/avm/res/storage/storage-account:0.32.1")]
    [DataRow("""{ "type": "file", "path": "./avm" }""", "BCP463: The \"artifacts.aliases.avm\" property in the Bicep configuration \"<bicepconfig>\" is invalid. \"avm\" is a predefined alias, so its \"type\" must be \"oci\".")]
    public void TryResolve_PredefinedAliasOverride_ReplacesWholeEntryAndRequiresOciType(string avmAlias, string expected)
    {
        var (entryPoint, _, resolver, _) = Setup(bicepConfig: $$"""
            {
              "experimentalFeaturesEnabled": { "artifactAliasesAndRedirects": true },
              "artifacts": { "aliases": { "avm": {{avmAlias}} } }
            }
            """);

        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "avm::res/storage/storage-account:0.32.1")).Should().Be(expected);
    }

    [TestMethod]
    [DataRow("\"artifacts\": 5", "BCP463: The \"artifacts\" property in the Bicep configuration \"<bicepconfig>\" is invalid. The value must be an object.")]
    [DataRow("\"artifacts\": { \"aliases\": [] }", "BCP463: The \"artifacts.aliases\" property in the Bicep configuration \"<bicepconfig>\" is invalid. The value must be an object.")]
    public void TryResolve_InvalidArtifactsSection_ReportsErrorOnlyWhenAliasIsUsed(string artifactsProperty, string expected)
    {
        var (entryPoint, _, resolver, services) = Setup(bicepConfig: $$"""
            {
              "experimentalFeaturesEnabled": { "artifactAliasesAndRedirects": true },
              {{artifactsProperty}}
            }
            """);

        // The rest of the configuration still loads, and nothing is reported until an alias is used.
        var configuration = services.GetRequiredService<IBicepConfigurationManager>().GetEffectiveConfiguration(entryPoint.FileHandle.Uri);
        configuration.GetDiagnostics().Should().BeEmpty();
        configuration.ExperimentalFeaturesEnabled.ArtifactAliasesAndRedirects.Should().BeTrue();

        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "avm::res/storage/storage-account:0.32.1")).Should().Be(expected);
        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "br/public:avm/res/storage/storage-account:0.32.1")).Should().Be("OciArtifactReference br:mcr.microsoft.com/bicep/avm/res/storage/storage-account:0.32.1");
    }

    [TestMethod]
    public void TryResolve_InvalidAliasDefinitions_ReportsErrorsOnlyWhenUsed()
    {
        var (entryPoint, _, resolver, services) = Setup(bicepConfig: """
            {
              "experimentalFeaturesEnabled": { "artifactAliasesAndRedirects": true },
              "artifacts": {
                "aliases": {
                  "modules": { "type": "file", "path": "./modules" },
                  "broken": { "type": "oci" },
                  "oci": { "type": "oci", "registry": "contoso.azurecr.io" }
                }
              }
            }
            """);

        // The configuration loads without diagnostics, and the valid aliases work.
        var chain = services.GetRequiredService<IBicepConfigurationManager>().GetConfigurationChain(entryPoint.FileHandle.Uri);
        chain.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        chain.EnumerateDiagnosticsPerFile().SelectMany(entry => entry.Value).Should().BeEmpty();
        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "modules::vnet.bicep")).Should().Be("LocalArtifactReference modules::vnet.bicep -> /repo/modules/vnet.bicep");

        Describe(resolver.TryResolve(entryPoint.FileHandle.Uri, ArtifactType.Module, "broken::vnet:1.0.0")).Should().Be(
            "BCP463: The \"artifacts.aliases.broken\" property in the Bicep configuration \"<bicepconfig>\" is invalid. The \"registry\" property must be specified.");
    }
}