// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Reflection;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.SourceLink;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Handlers;
using Bicep.TextFixtures.Dummies;
using Bicep.TextFixtures.Utils;
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

#if WINDOWS_BUILD
    private const string ROOT = "c:\\";
#else
        private const string ROOT = "/";
#endif

        public static string GetDisplayName(MethodInfo info, object[] row)
        {
            row.Should().HaveCount(3);
            row[0].Should().BeOfType<DataSet>();
            row[1].Should().BeOfType<string>();
            row[2].Should().BeAssignableTo<IList<Position>>();

            return $"{info.Name}_{((DataSet)row[0]).Name}_{row[1]}";
        }

        // If entrypointSource is not null, then a source archive will be created with the given entrypointSource, otherwise no source archive will be created.
        private SharedLanguageHelperManager CreateServer(string? bicepModuleEntrypointPath, string? entrypointSource, string? sourceArchiveError = null)
        {
            var sourceTgzFileMock = StrictMock.Of<IFileHandle>();
            sourceTgzFileMock.Setup(x => x.Exists()).Returns(false);

            if (bicepModuleEntrypointPath is not null && entrypointSource is not null)
            {
                //var sourceArchive = new SourceArchiveBuilder(BicepTestConstants.SourceFileFactory).WithBicepFile(bicepModuleEntrypoint, entrypointSource).Build();
                var sourceArchive = DummySourceArchive.Create(bicepModuleEntrypointPath);
                sourceTgzFileMock.Setup(x => x.Exists()).Returns(true);

                if (sourceArchiveError is not null)
                {
                    sourceTgzFileMock.Setup(x => x.OpenRead()).Throws(new Exception(sourceArchiveError));
                }
                else
                {
                    sourceTgzFileMock.Setup(x => x.OpenRead()).Returns(sourceArchive.PackIntoBinaryData().ToStream());
                }
            }

            var cacheDirectoryMock = StrictMock.Of<IDirectoryHandle>();
            cacheDirectoryMock.Setup(x => x.GetFile("source.tgz")).Returns(sourceTgzFileMock.Object);

            var cacheRootDirectoryMock = StrictMock.Of<IDirectoryHandle>();
            cacheRootDirectoryMock.Setup(x => x.GetDirectory(It.IsAny<string>())).Returns(cacheDirectoryMock.Object);

            var moduleDispatcher = StrictMock.Of<IModuleDispatcher>();
            moduleDispatcher.Setup(x => x.RestoreArtifacts(It.IsAny<ImmutableArray<ArtifactReference>>(), It.IsAny<bool>())).
                ReturnsAsync(true);
            moduleDispatcher.Setup(x => x.PruneRestoreStatuses());

            MockRepository repository = new(MockBehavior.Strict);
            var provider = repository.Create<IArtifactRegistryProvider>();

            var defaultServer = new SharedLanguageHelperManager();
            defaultServer.Initialize(
                async () => await MultiFileLanguageServerHelper.StartLanguageServer(
                    TestContext,
                    services => services
                        .WithModuleDispatcher(moduleDispatcher.Object)
                        .WithFeatureOverrides(new(cacheRootDirectoryMock.Object))));
            return defaultServer;
        }

        [DataTestMethod]
        [DataRow($"file://{ROOT}path/to/localfile.bicep")]
        [DataRow($"file://{ROOT}path/to/localfile.json")]
        [DataRow($"file://{ROOT}path/to/localfile.bicepparam")]
        [DataRow("untitled:Untitled-1")]
        public async Task DisplayingLocalFile_NotExtSourceScheme_ShouldNotHaveCodeLens(string fileName)
        {
            var uri = DocumentUri.From($"{ROOT}{this.TestContext.TestName}");

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
            var uri = DocumentUri.From($"{ROOT}{this.TestContext.TestName}");
            var moduleEntrypointFileName = "module entrypoint.bicep";

            await using var server = CreateServer(moduleEntrypointFileName, "// module entrypoint");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var externalSourceUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", moduleEntrypointFileName).ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, externalSourceUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("bicep.internal.showModuleSourceFile");
            lens.Should().HaveCommandTitle("Show the compiled JSON for module \"module1\" (br:myregistry.azurecr.io/myrepo/bicep/module1:v1)");
            var target = new ExternalSourceReference(lens.CommandArguments().Single());
            target.IsRequestingCompiledJson.Should().BeTrue();
        }

        [TestMethod]
        public async Task DisplayingExternalModuleSource_BicepButNotEntrypointFile_ShouldHaveCodeLens_ToShowModuleCompiledJson()
        {
            var uri = DocumentUri.From($"{ROOT}{this.TestContext.TestName}");
            var moduleEntrypointFileName = "module entrypoint.bicep";

            await using var server = CreateServer(moduleEntrypointFileName, "// module entrypoint");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var externalSourceUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", "not the entrypoint.bicep").ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, externalSourceUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("bicep.internal.showModuleSourceFile");
            lens.Should().HaveCommandTitle("Show the compiled JSON for module \"module1\" (br:myregistry.azurecr.io/myrepo/bicep/module1:v1)");
            var target = new ExternalSourceReference(lens.CommandArguments().Single());
            target.IsRequestingCompiledJson.Should().BeTrue();
        }

        [TestMethod]
        public async Task DisplayingExternalModuleSource_JsonFileThatIsIncludedInSources_ShouldHaveCodeLens_ToShowCompiledJson_ForTheWholeModule()
        {
            var uri = DocumentUri.From($"{ROOT}{this.TestContext.TestName}");
            var moduleEntrypointFileName = "module entrypoint.bicep";

            await using var server = CreateServer(moduleEntrypointFileName, "// module entrypoint");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var externalSourceUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", "source file.json").ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, externalSourceUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("bicep.internal.showModuleSourceFile");
            lens.Should().HaveCommandTitle("Show the compiled JSON for module \"module1\" (br:myregistry.azurecr.io/myrepo/bicep/module1:v1)");
            var target = new ExternalSourceReference(lens.CommandArguments().Single());
            target.IsRequestingCompiledJson.Should().BeTrue();
        }

        [TestMethod]
        public async Task DisplayingModuleCompiledJsonFile_AndSourceIsAvailable_ShouldHaveCodeLens_ToShowBicepEntrypointFile()
        {
            var uri = DocumentUri.From($"{ROOT}{this.TestContext.TestName}");
            var moduleEntrypointFileName = "module entrypoint.bicep";

            await using var server = CreateServer(moduleEntrypointFileName, "// module entrypoint");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var externalSourceUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", null /* main.json */).ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, externalSourceUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("bicep.internal.showModuleSourceFile");
            lens.Should().HaveCommandTitle("Show Bicep source");
            var target = new ExternalSourceReference(lens.CommandArguments().Single());
            target.IsRequestingCompiledJson.Should().BeFalse();
            target.RequestedFile.Should().Be(moduleEntrypointFileName);
        }

        [TestMethod]
        public async Task DisplayingModuleCompiledJsonFile_AndSourceNotAvailable_ShouldHaveCodeLens_ToExplainWhyNoSources()
        {
            var uri = DocumentUri.From($"{ROOT}{this.TestContext.TestName}");
            var moduleEntrypointFileName = "module entrypoint.bicep";

            await using var server = CreateServer(moduleEntrypointFileName, null);
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var externalSourceUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", null /* main.json */).ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, externalSourceUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("");
            lens.Should().HaveCommandTitle("No source code is available for this module");
            lens.Should().HaveNoCommandArguments();
        }

        [TestMethod]
        public async Task HasBadUri_ShouldHaveCodeLens_ToExplainError()
        {
            var uri = DocumentUri.From($"{ROOT}{this.TestContext.TestName}");
            var moduleEntrypointName = "module entrypoint.bicep";

            await using var server = CreateServer(moduleEntrypointName, "// module entrypoint");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var badDocumentUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", null /* main.json */).ToUri().AbsoluteUri.Replace("v1", ""); // bad version string
            var lenses = await GetExternalSourceCodeLenses(helper, badDocumentUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("");
            lens.Should().HaveCommandTitle("There was an error retrieving source code for this module: Invalid module reference 'br:myregistry.azurecr.io/myrepo/bicep/module1:'. The specified OCI artifact reference \"br:myregistry.azurecr.io/myrepo/bicep/module1:\" is not valid. The module tag or digest is missing. (Parameter 'fullyQualifiedModuleReference')");
            lens.Should().HaveNoCommandArguments();
        }

        [TestMethod]
        public async Task SourceArchiveHasError_ShouldHaveCodeLensWithError()
        {
            var uri = DocumentUri.From($"{ROOT}{this.TestContext.TestName}");
            var moduleEntrypointName = "module entrypoint.bicep";

            await using var server = CreateServer(moduleEntrypointName, "// module entrypoint", sourceArchiveError: "Source archive is incompatible with this version of Bicep.");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var documentUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", null /* main.json */).ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, documentUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("");
            lens.Should().HaveCommandTitle("Cannot display source code for this module. Source archive is incompatible with this version of Bicep.");
            lens.Should().HaveNoCommandArguments();
        }

        private async Task<CodeLens[]> GetExternalSourceCodeLenses(MultiFileLanguageServerHelper helper, DocumentUri documentUri)
        {
            return (await helper.Client.RequestCodeLens(new CodeLensParams
            {
                TextDocument = new TextDocumentIdentifier(documentUri)
            }))?.Where(a => a.IsExternalSourceCodeLens()).ToArray()
            ?? [];
        }
    }
}
