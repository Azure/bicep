// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class TelemetryTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task VerifyTopLevelDeclarationSnippetInsertionFiresTelemetryEvent()
        {
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "name", "res-aks-cluster" }
            };

            TaskCompletionSource<TelemetryEventParams> telemetryReceived = await ResolveCompletionAsync(string.Empty, "res-aks-cluster", TelemetryConstants.EventNames.TopLevelDeclarationSnippetInsertion, properties);
            TelemetryEventParams telemetryEventParams = await IntegrationTestHelper.WithTimeoutAsync(telemetryReceived.Task);
            IDictionary<string, object> extensionData = telemetryEventParams.ExtensionData;
            ICollection<string> keys = extensionData.Keys;

            keys.Count.Should().Be(2);
            keys.Should().Contain("eventName");
            keys.Should().Contain("properties");
            extensionData["eventName"].ToString().Should().Be(TelemetryConstants.EventNames.TopLevelDeclarationSnippetInsertion);
            extensionData["properties"].ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""name"": ""res-aks-cluster""
}");
        }

        [DataRow("required-properties", "{\r\n  \"name\": \"required-properties\",\r\n  \"type\": \"Microsoft.ContainerService/managedClusters@2021-03-01\"\r\n}")]
        [DataRow("snippet", "{\r\n  \"name\": \"snippet\",\r\n  \"type\": \"Microsoft.ContainerService/managedClusters@2021-03-01\"\r\n}")]
        [DataRow("{}", "{\r\n  \"name\": \"{}\",\r\n  \"type\": \"Microsoft.ContainerService/managedClusters@2021-03-01\"\r\n}")]
        [DataTestMethod]
        public async Task VerifyResourceBodySnippetInsertionFiresTelemetryEvent(string prefix, string expectedProperties)
        {
            string text = "resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = ";
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "name", prefix },
                { "type", "Microsoft.ContainerService/managedClusters@2021-03-01" }
            };

            TaskCompletionSource<TelemetryEventParams> telemetryReceived = await ResolveCompletionAsync(text, prefix, TelemetryConstants.EventNames.ResourceBodySnippetInsertion, properties);
            TelemetryEventParams telemetryEventParams = await IntegrationTestHelper.WithTimeoutAsync(telemetryReceived.Task);
            IDictionary<string, object> extensionData = telemetryEventParams.ExtensionData;
            ICollection<string> keys = extensionData.Keys;

            keys.Count.Should().Be(2);
            keys.Should().Contain("eventName");
            keys.Should().Contain("properties");
            extensionData["eventName"].ToString().Should().Be(TelemetryConstants.EventNames.ResourceBodySnippetInsertion);
            extensionData["properties"].ToString().Should().BeEquivalentToIgnoringNewlines(expectedProperties);
        }

        [TestMethod]
        public async Task VerifyModuleBodySnippetInsertionFiresTelemetryEvent()
        {
            string text = "module foo 'test.bicep' = ";
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "name", "{}" }
            };

            TaskCompletionSource<TelemetryEventParams> telemetryReceived = await ResolveCompletionAsync(text, "{}", TelemetryConstants.EventNames.ModuleBodySnippetInsertion, properties);
            TelemetryEventParams telemetryEventParams = await IntegrationTestHelper.WithTimeoutAsync(telemetryReceived.Task);
            IDictionary<string, object> extensionData = telemetryEventParams.ExtensionData;
            ICollection<string> keys = extensionData.Keys;

            keys.Count.Should().Be(2);
            keys.Should().Contain("eventName");
            keys.Should().Contain("properties");
            extensionData["eventName"].ToString().Should().Be(TelemetryConstants.EventNames.ModuleBodySnippetInsertion);
            extensionData["properties"].ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""name"": ""{}""
}");
        }

        private async Task<TaskCompletionSource<TelemetryEventParams>> ResolveCompletionAsync(string text, string prefix, string eventName, IDictionary<string, string> properties)
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            var telemetryReceived = new TaskCompletionSource<TelemetryEventParams>();

            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                TestContext,
                options =>
                {
                    options.OnTelemetryEvent(telemetry => telemetryReceived.SetResult(telemetry));
                },
                fileResolver: new InMemoryFileResolver(fileSystemDict));

            var mainUri = DocumentUri.FromFileSystemPath("/main.bicep");
            fileSystemDict[mainUri.ToUri()] = text;

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier(mainUri),
                Position = new Position(0, text.Length),
            });

            CompletionItem completionItem = completions.Where(x => x.Kind == CompletionItemKind.Snippet && x.Label == prefix).First();
            Command? command = completionItem.Command;

            command!.Name.Should().Be(TelemetryConstants.CommandName);

            JArray? arguments = command!.Arguments;
            BicepTelemetryEvent? telemetryEvent = arguments!.First().ToObject<BicepTelemetryEvent>();

            telemetryEvent!.EventName.Should().Be(eventName);
            telemetryEvent!.Properties!.Should().Equal(properties);

            await client.ResolveCompletion(completionItem);
            await client.Workspace.ExecuteCommand(command);

            return telemetryReceived;
        }
    }
}
