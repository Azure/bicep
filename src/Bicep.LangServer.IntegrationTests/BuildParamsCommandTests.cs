// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class BuildParamsCommandTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task BuildParams_command_should_generate_template()
        {
            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsListener.AddMessage),
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()).WithFeatureOverrides(new(TestContext)));
            var client = helper.Client;

            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
                TestContext,
                typeof(DataSet).Assembly,
                "Files/baselines_bicepparam/Parameters");

            var bicepFilePath = Path.Combine(outputDirectory, "parameters.bicepparam");
            var expectedJson = File.ReadAllText(Path.Combine(outputDirectory, "parameters.json"));

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParamsFromFile(bicepFilePath, 1));
            await diagnosticsListener.WaitNext();

            await client.Workspace.ExecuteCommand(new Command
            {
                Name = "buildParams",
                Arguments = new JArray {
                    DocumentUri.FromFileSystemPath(bicepFilePath).ToString(),
                }
            });

            var buildCommandOutput = File.ReadAllText(Path.ChangeExtension(bicepFilePath, ".json"));
            buildCommandOutput.Should().BeEquivalentToIgnoringNewlines(expectedJson);
        }
    }
}
