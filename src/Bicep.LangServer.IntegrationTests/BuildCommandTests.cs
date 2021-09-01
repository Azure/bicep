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

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class BuildCommandTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task Build_command_should_generate_template_with_symbolic_names_if_enabled()
        {
            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var featuresProvider = BicepTestConstants.CreateFeaturesProvider(TestContext, registryEnabled: false, symbolicNameCodegenEnabled: true);
            
            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsParams => diagnosticsListener.AddMessage(diagnosticsParams)),
                new LanguageServer.Server.CreationOptions(
                    ResourceTypeProvider: BuiltInTestTypes.Create(),
                    Features: featuresProvider));

            var bicepContents = @"
resource res1 'Test.Rp/basicTests@2020-01-01' = {
  name: 'res1'
}
";

            var bicepFile = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepContents);
            var bicepDocumentUri = DocumentUri.FromFileSystemPath(bicepFile);

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(bicepDocumentUri, bicepContents, 1));
            await diagnosticsListener.WaitNext();

            await client.Workspace.ExecuteCommand(new Command {
                Name = "build",
                Arguments = new JArray {
                    bicepFile,
                }
            });

            var expectedJson = File.ReadAllText(Path.ChangeExtension(bicepFile, ".json"));
            expectedJson.Should().Match(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""languageVersion"": ""1.9-experimental"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""EXPERIMENTAL_WARNING"": ""Symbolic name support in ARM is experimental, and should be enabled for testing purposes only. Do not enable this setting for any production usage, or you may be unexpectedly broken at any time!"",
    ""_generator"": {*}
  },
  ""functions"": [],
  ""resources"": {
    ""res1"": {
      ""type"": ""Test.Rp/basicTests"",
      ""apiVersion"": ""2020-01-01"",
      ""name"": ""res1""
    }
  }
}");
            
        }
    }
}
