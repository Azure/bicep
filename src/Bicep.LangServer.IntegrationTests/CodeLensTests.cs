// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.SourceCode;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Completions;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using CompilationHelper = Bicep.Core.UnitTests.Utils.CompilationHelper;
using IOFileSystem = System.IO.Abstractions.FileSystem;
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

        private SharedLanguageHelperManager CreateServer(Uri? bicepModuleEntrypoint, string? entrypointSource)
        {
            var registry = StrictMock.Of<IArtifactRegistry>();
            SourceArchive? sourceArchive = null;
            if (bicepModuleEntrypoint is not null && entrypointSource is not null)
            {
                BicepFile moduleEntrypointFile = SourceFileFactory.CreateBicepFile(bicepModuleEntrypoint, entrypointSource);
                sourceArchive = SourceArchive.FromStream(SourceArchive.PackSourcesIntoStream(moduleEntrypointFile.FileUri, moduleEntrypointFile));
            }
            registry.Setup(m => m.TryGetSource(It.IsAny<ArtifactReference>())).Returns(sourceArchive);

            var defaultServer = new SharedLanguageHelperManager();
            defaultServer.Initialize(
                async () => await MultiFileLanguageServerHelper.StartLanguageServer(
                    TestContext,
                    services => services.WithFeatureOverrides(new(TestContext, ExtensibilityEnabled: true)),
                    registry.Object.AsArray()));//asdfg remove? 
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

            var documentUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", Path.GetFileName(moduleEntrypointUri.Path)).ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, documentUri);

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

            var documentUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", "not the entrypoint.bicep").ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, documentUri);

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

            var documentUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", "source file.json").ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, documentUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("bicep.internal.showModuleSourceFile");
            lens.Should().HaveCommandTitle("Show compiled JSON");
            var target = new ExternalSourceReference(lens.CommandArguments().Single());
            target.IsRequestingCompiledJson.Should().BeTrue();
        }

        //public async Task DisplayingExternalModuleSource_TemplateSpecFile_ShouldHaveCodeLens_ToShowModuleCompiledJson_ForTheWholeModule()//asdfg
        //{
        //    var uri = DocumentUri.From($"/{this.TestContext.TestName}");
        //    var moduleEntrypointUri = DocumentUri.From($"/module entrypoint.bicep").ToUriEncoded();

        //    await using var server = CreateServer(moduleEntrypointUri, "// module entrypoint");
        //    var helper = await server.GetAsync();
        //    await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

        //    var documentUri = new ExternalSourceReference("title", "ts:myregistry.azurecr.io/myrepo/bicep/module1:v1", "main.json").ToUri();
        //    var lenses = await GetExternalSourceCodeLenses(helper, documentUri);

        //    lenses.Should().HaveCount(1);
        //    var lens = lenses.First();
        //    lens.Should().HaveRange(new Range(0, 0, 0, 0));
        //    lens.Should().HaveCommandName("bicep.internal.showModuleSourceFile");
        //    lens.Should().HaveCommandTitle("Show the compiled JSON for this module");
        //    var target = new ExternalSourceReference(lens.CommandArguments().Single());
        //    target.IsRequestingCompiledJson.Should().BeTrue();
        //}

        [TestMethod]
        public async Task DisplayingModuleCompiledJsonFile_AndSourceIsAvailable_ShouldHaveCodeLens_ToShowBicepEntrypointFile()//asdfg
        {
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");
            var moduleEntrypointUri = DocumentUri.From($"/module entrypoint.bicep");

            await using var server = CreateServer(moduleEntrypointUri.ToUriEncoded(), "// module entrypoint");
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var documentUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", null /* main.json */).ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, documentUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("bicep.internal.showModuleSourceFile");
            lens.Should().HaveCommandTitle("Show Bicep source");
            var target = new ExternalSourceReference(lens.CommandArguments().Single());
            target.IsRequestingCompiledJson.Should().BeFalse();
            target.RequestedFile.Should().Be(Path.GetFileName(moduleEntrypointUri.Path));
        }

        [TestMethod]
        public async Task DisplayingModuleCompiledJsonFile_AndSourceNotAvailable_ShouldHaveCodeLens_ToExplainWhyNoSources()//asdfg
        {
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");
            var moduleEntrypointUri = DocumentUri.From($"/module entrypoint.bicep");

            await using var server = CreateServer(moduleEntrypointUri.ToUriEncoded(), null);
            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var documentUri = new ExternalSourceReference("title", "br:myregistry.azurecr.io/myrepo/bicep/module1:v1", null /* main.json */).ToUri();
            var lenses = await GetExternalSourceCodeLenses(helper, documentUri);

            lenses.Should().HaveCount(1);
            var lens = lenses.First();
            lens.Should().HaveRange(new Range(0, 0, 0, 0));
            lens.Should().HaveCommandName("");
            lens.Should().HaveCommandTitle("Source code has not been published for this module");
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
