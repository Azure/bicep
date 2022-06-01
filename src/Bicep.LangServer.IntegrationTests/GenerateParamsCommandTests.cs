// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Newtonsoft.Json.Linq;
using Bicep.Core.UnitTests;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Assertions;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class GenerateParamsCommandTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task GenerateParams_command_should_generate_paramsfile()
        {
            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var features = BicepTestConstants.CreateFeaturesProvider(
                TestContext,
                assemblyFileVersion: BicepTestConstants.DevAssemblyFileVersion);

            using var helper = await LanguageServerHelper.StartServerWithClientConnectionAsync(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsParams => diagnosticsListener.AddMessage(diagnosticsParams)),
                new LanguageServer.Server.CreationOptions(
                    NamespaceProvider: BuiltInTestTypes.Create(),
                    Features: features));
            var client = helper.Client;

            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
                TestContext,
                typeof(ExamplesTests).Assembly,
                "Bicep.Core.Samples/Resources_CRLF");

            var bicepFilePath = Path.Combine(outputDirectory, "main.bicep");
            var expectedJson = File.ReadAllText(Path.Combine(outputDirectory, "main.parameters.json"));

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParamsFromFile(bicepFilePath, 1));
            await diagnosticsListener.WaitNext();

            await client.Workspace.ExecuteCommand(new Command
            {
                Name = "generateParams",
                Arguments = new JArray {
                    bicepFilePath,
                }
            });

            var commandOutput = File.ReadAllText(Path.ChangeExtension(bicepFilePath, ".parameters.json"));
            commandOutput.Should().BeEquivalentToIgnoringNewlines(expectedJson);
        }
   }
}
