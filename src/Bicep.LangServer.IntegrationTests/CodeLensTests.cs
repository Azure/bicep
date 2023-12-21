// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Registry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class CodeLensTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        public static string GetDisplayName(MethodInfo info, object[] row)
        {
            row.Should().HaveCount(3);
            row[0].Should().BeOfType<DataSet>();
            row[1].Should().BeOfType<string>();
            row[2].Should().BeAssignableTo<IList<Position>>();

            return $"{info.Name}_{((DataSet)row[0]).Name}_{row[1]}";
        }

        // If entrypointSource is not null, then a source archive will be created with the given entrypointSource, otherwise no source archive will be created.
        private SharedLanguageHelperManager CreateServer(Uri? bicepModuleEntrypoint, string? entrypointSource, SourceArchiveResult? sourceArchiveResult = null)
        {
            var moduleRegistry = StrictMock.Of<IArtifactRegistry>();
            if (bicepModuleEntrypoint is not null && entrypointSource is not null)
            {
                BicepFile moduleEntrypointFile = SourceFileFactory.CreateBicepFile(bicepModuleEntrypoint, entrypointSource);
                sourceArchiveResult ??= SourceArchive.UnpackFromStream(SourceArchive.PackSourcesIntoStream(moduleEntrypointFile.FileUri, moduleEntrypointFile));
            }
            sourceArchiveResult ??= new();
            moduleRegistry.Setup(m => m.TryGetSource(It.IsAny<ArtifactReference>())).Returns(sourceArchiveResult);

            var moduleDispatcher = StrictMock.Of<IModuleDispatcher>();
            moduleDispatcher.Setup(x => x.RestoreModules(It.IsAny<ImmutableArray<ArtifactReference>>(), It.IsAny<bool>())).
                ReturnsAsync(true);
            moduleDispatcher.Setup(x => x.PruneRestoreStatuses());

            MockRepository repository = new(MockBehavior.Strict);
            var provider = repository.Create<IArtifactRegistryProvider>();

            var artifactRegistries = moduleRegistry.Object.AsArray();

            moduleDispatcher.Setup(m => m.TryGetModuleSources(It.IsAny<ArtifactReference>())).Returns((ArtifactReference reference) =>
                artifactRegistries.Select(r => r.TryGetSource(reference)).FirstOrDefault(s => s is not null) ?? new());

            var defaultServer = new SharedLanguageHelperManager();
            defaultServer.Initialize(
                async () => await MultiFileLanguageServerHelper.StartLanguageServer(
                    TestContext,
                    services => services
                        .WithModuleDispatcher(moduleDispatcher.Object)
                        .WithFeatureOverrides(new(TestContext, ExtensibilityEnabled: true))));
            return defaultServer;
        }

        [DataTestMethod]
        [DataRow("file://path/to/localfile.bicep")]
        [DataRow("file://path/to/localfile.json")]
        [DataRow("file://path/to/localfile.bicepparam")]
        [DataRow("untitled:Untitled-1")]
        public async Task DisplayingLocalFile_NotExtSourceScheme_ShouldNotHaveCodeLens(string fileName)
        {
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");

            await using var server = CreateServer(null, null);
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            // Local files will have a "file://" scheme
            var documentUri = DocumentUri.FromFileSystemPath(fileName);
            var lenses = await GetExternalSourceCodeLenses(helper, documentUri);

            lenses.Should().BeEmpty();
        }

        [TestMethod]
        public async Task DisplayingExternalModuleSource_EntrypointFile_ShouldHaveCodeLens_ToShowModuleCompiledJson()
        {
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");
            var moduleEntrypointUri = DocumentUri.From($"/module entrypoint.bicep");

            await using var server = CreateServer(moduleEntrypointUri.ToUriEncoded(), "// module entrypoint");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var externalSourceUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", Path.GetFileName(moduleEntrypointUri.Path)).ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, externalSourceUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("bicep.internal.showModuleSourceFile");
            lens.Should().HaveCommandTitle("Show compiled JSON");
            var target = new ExternalSourceReference(lens.CommandArguments().Single());
            target.IsRequestingCompiledJson.Should().BeTrue();
        }

        [TestMethod]
        public async Task DisplayingExternalModuleSource_BicepButNotEntrypointFile_ShouldHaveCodeLens_ToShowModuleCompiledJson()
        {
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");
            var moduleEntrypointUri = DocumentUri.From($"/module entrypoint.bicep");

            await using var server = CreateServer(moduleEntrypointUri.ToUriEncoded(), "// module entrypoint");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var externalSourceUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", "not the entrypoint.bicep").ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, externalSourceUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("bicep.internal.showModuleSourceFile");
            lens.Should().HaveCommandTitle("Show compiled JSON");
            var target = new ExternalSourceReference(lens.CommandArguments().Single());
            target.IsRequestingCompiledJson.Should().BeTrue();
        }

        [TestMethod]
        public async Task DisplayingExternalModuleSource_JsonFileThatIsIncludedInSources_ShouldHaveCodeLens_ToShowCompiledJson_ForTheWholeModule()
        {
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");
            var moduleEntrypointUri = DocumentUri.From($"/module entrypoint.bicep");

            await using var server = CreateServer(moduleEntrypointUri.ToUriEncoded(), "// module entrypoint");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var externalSourceUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", "source file.json").ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, externalSourceUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("bicep.internal.showModuleSourceFile");
            lens.Should().HaveCommandTitle("Show compiled JSON");
            var target = new ExternalSourceReference(lens.CommandArguments().Single());
            target.IsRequestingCompiledJson.Should().BeTrue();
        }

        [TestMethod]
        public async Task DisplayingModuleCompiledJsonFile_AndSourceIsAvailable_ShouldHaveCodeLens_ToShowBicepEntrypointFile()
        {
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");
            var moduleEntrypointUri = DocumentUri.From($"/module entrypoint.bicep");

            await using var server = CreateServer(moduleEntrypointUri.ToUriEncoded(), "// module entrypoint");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var externalSourceUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", null /* main.json */).ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, externalSourceUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("bicep.internal.showModuleSourceFile");
            lens.Should().HaveCommandTitle("Show Bicep source (experimental)");
            var target = new ExternalSourceReference(lens.CommandArguments().Single());
            target.IsRequestingCompiledJson.Should().BeFalse();
            target.RequestedFile.Should().Be(Path.GetFileName(moduleEntrypointUri.Path));
        }

        [TestMethod]
        public async Task DisplayingModuleCompiledJsonFile_AndSourceNotAvailable_ShouldHaveCodeLens_ToExplainWhyNoSources()
        {
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");
            var moduleEntrypointUri = DocumentUri.From($"/module entrypoint.bicep");

            await using var server = CreateServer(moduleEntrypointUri.ToUriEncoded(), null);
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var externalSourceUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", null /* main.json */).ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, externalSourceUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("");
            lens.Should().HaveCommandTitle("(Experimental) No source code is available for this module");
            lens.Should().HaveNoCommandArguments();
        }

        [TestMethod]
        public async Task HasBadUri_ShouldHaveCodeLens_ToExplainError()
        {
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");
            var moduleEntrypointUri = DocumentUri.From($"/module entrypoint.bicep");

            await using var server = CreateServer(moduleEntrypointUri.ToUriEncoded(), "// module entrypoint");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var badDocumentUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", null /* main.json */).ToUri().AbsoluteUri.Replace("v1", ""); // bad version string
            var lenses = await GetExternalSourceCodeLenses(helper, badDocumentUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("");
            lens.Should().HaveCommandTitle("(Experimental) There was an error retrieving source code for this module: Invalid module reference 'br:myregistry.azurecr.io/myrepo/bicep/module1:'. The specified OCI artifact reference \"br:myregistry.azurecr.io/myrepo/bicep/module1:\" is not valid. The module tag or digest is missing. (Parameter 'fullyQualifiedModuleReference')");
            lens.Should().HaveNoCommandArguments();
        }

        [TestMethod]
        public async Task SourceArchiveHasError_ShouldHaveCodeLensWithError()
        {
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");
            var moduleEntrypointUri = DocumentUri.From($"/module entrypoint.bicep");

            SourceArchiveResult sourceArchiveResult = new("Source archive is incompatible with this version of Bicep.");
            await using var server = CreateServer(moduleEntrypointUri.ToUriEncoded(), "// module entrypoint", sourceArchiveResult);
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var documentUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", null /* main.json */).ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, documentUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("");
            lens.Should().HaveCommandTitle("(Experimental) Cannot display source code for this module. Source archive is incompatible with this version of Bicep.");
            lens.Should().HaveNoCommandArguments();
        }

        private async Task<CodeLens[]> GetExternalSourceCodeLenses(MultiFileLanguageServerHelper helper, DocumentUri documentUri)
        {
            return (await helper.Client.RequestCodeLens(new CodeLensParams
            {
                TextDocument = new TextDocumentIdentifier(documentUri)
            }))?.Where(a => a.IsExternalSourceCodeLens()).ToArray()
            ?? Array.Empty<CodeLens>();
        }
    }
}
