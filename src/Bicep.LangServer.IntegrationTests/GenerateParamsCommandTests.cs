// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.Emit.Options;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

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

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsListener.AddMessage),
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()).WithFeatureOverrides(new(TestContext)));
            var client = helper.Client;

            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
                TestContext,
                typeof(DataSet).Assembly,
                DataSets.Resources_CRLF.GetStreamPrefix());

            var bicepFilePath = Path.Combine(outputDirectory, "main.bicep");
            var expectedJson = File.ReadAllText(Path.Combine(outputDirectory, "main.parameters.json"));

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParamsFromFile(bicepFilePath, 1));
            await diagnosticsListener.WaitNext();

            var commandParams = new BicepGenerateParamsCommandParams(bicepFilePath, OutputFormatOption.Json, IncludeParamsOption.RequiredOnly);

            await client.Workspace.ExecuteCommand(new Command
            {
                Name = "generateParams",
                Arguments = new JArray(new[] { commandParams.ToJToken() })
            });

            var commandOutput = File.ReadAllText(Path.ChangeExtension(bicepFilePath, ".parameters.json"));
            commandOutput.Should().BeEquivalentToIgnoringNewlines(expectedJson);
        }
    }
}
