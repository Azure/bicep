// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.Features;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Newtonsoft.Json.Linq;
using Bicep.Core.UnitTests;
using Bicep.LangServer.IntegrationTests.Helpers;
using OmniSharp.Extensions.LanguageServer.Protocol;
using FluentAssertions;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Assertions;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class BuildCommandTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task Build_command_should_generate_template()
        {
            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var features = BicepTestConstants.CreateFeaturesProvider(
                TestContext,
                assemblyFileVersion: BicepTestConstants.DevAssemblyFileVersion);
            
            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsParams => diagnosticsListener.AddMessage(diagnosticsParams)),
                new LanguageServer.Server.CreationOptions(
                    NamespaceProvider: BuiltInTestTypes.Create(),
                    Features: features));

            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
                TestContext,
                typeof(ExamplesTests).Assembly,
                "Bicep.Core.Samples/Resources_CRLF");

            var bicepFilePath = Path.Combine(outputDirectory, "main.bicep");
            var expectedJson = File.ReadAllText(Path.Combine(outputDirectory, "main.json"));

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParamsFromFile(bicepFilePath, 1));
            await diagnosticsListener.WaitNext();

            await client.Workspace.ExecuteCommand(new Command {
                Name = "build",
                Arguments = new JArray {
                    bicepFilePath,
                }
            });

            var buildCommandOutput = File.ReadAllText(Path.ChangeExtension(bicepFilePath, ".json"));
            buildCommandOutput.Should().BeEquivalentToIgnoringNewlines(expectedJson);
        }

        [TestMethod]
        public async Task Build_command_should_generate_template_with_symbolic_names_if_enabled()
        {
            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var features = BicepTestConstants.CreateFeaturesProvider(
                TestContext,
                symbolicNameCodegenEnabled: true,
                assemblyFileVersion: BicepTestConstants.DevAssemblyFileVersion);
            
            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsParams => diagnosticsListener.AddMessage(diagnosticsParams)),
                new LanguageServer.Server.CreationOptions(
                    NamespaceProvider: BuiltInTestTypes.Create(),
                    Features: features));

            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
                TestContext,
                typeof(ExamplesTests).Assembly,
                "Bicep.Core.Samples/Resources_CRLF");

            var bicepFilePath = Path.Combine(outputDirectory, "main.bicep");
            var expectedJson = File.ReadAllText(Path.Combine(outputDirectory, "main.symbolicnames.json"));

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParamsFromFile(bicepFilePath, 1));
            await diagnosticsListener.WaitNext();

            await client.Workspace.ExecuteCommand(new Command {
                Name = "build",
                Arguments = new JArray {
                    bicepFilePath,
                }
            });

            var buildCommandOutput = File.ReadAllText(Path.ChangeExtension(bicepFilePath, ".json"));
            buildCommandOutput.Should().BeEquivalentToIgnoringNewlines(expectedJson);
        }
    }
}
