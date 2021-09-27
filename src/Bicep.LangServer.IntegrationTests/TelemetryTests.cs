// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
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

            BicepTelemetryEvent bicepTelemetryEvent = await ResolveCompletionAsync(string.Empty, "res-aks-cluster", new Position(0, 0));

            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.TopLevelDeclarationSnippetInsertion);
            bicepTelemetryEvent.Properties.Should().Equal(properties);
        }

        [TestMethod]
        public async Task VerifyNestedResourceDeclarationSnippetInsertionFiresTelemetryEvent()
        {
            string text = @"resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
  location: resourceGroup().location

}";
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "name", "res-automation-cert" }
            };

            BicepTelemetryEvent bicepTelemetryEvent = await ResolveCompletionAsync(text, "res-automation-cert", new Position(3, 0));

            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.NestedResourceDeclarationSnippetInsertion);
            bicepTelemetryEvent.Properties.Should().Equal(properties);
        }

        [DataRow("required-properties")]
        [DataRow("snippet")]
        [DataRow("{}")]
        [DataTestMethod]
        public async Task VerifyResourceBodySnippetInsertionFiresTelemetryEvent(string prefix)
        {
            string text = "resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = ";
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "name", prefix },
                { "type", "Microsoft.ContainerService/managedClusters@2021-03-01" }
            };

            BicepTelemetryEvent bicepTelemetryEvent = await ResolveCompletionAsync(text, prefix, new Position(0, text.Length));

            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.ResourceBodySnippetInsertion);
            bicepTelemetryEvent.Properties.Should().Equal(properties);
        }

        [DataRow("required-properties")]
        [DataRow("{}")]
        [DataTestMethod]
        public async Task VerifyObjectBodySnippetInsertionFiresTelemetryEvent(string prefix)
        {
            string text = @"resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: 'name'
  properties: 
}";
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "name", prefix }
            };

            BicepTelemetryEvent bicepTelemetryEvent = await ResolveCompletionAsync(text, prefix, new Position(2, 13));

            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.ObjectBodySnippetInsertion);
            bicepTelemetryEvent.Properties.Should().Equal(properties);
        }

        [DataRow("required-properties")]
        [DataRow("{}")]
        [DataTestMethod]
        public async Task VerifyObjectBodySnippetInsertionInsideExistingArrayOfObjectsFiresTelemetryEvent(string prefix)
        {
            string text = @"resource applicationGatewayFirewall 'Microsoft.Network/ApplicationGatewayWebApplicationFirewallPolicies@2020-11-01' = {
  name: 'name'
  properties: {
    managedRules: {
      managedRuleSets: [
        
      ]
    }
  }
}";
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "name", prefix }
            };

            BicepTelemetryEvent bicepTelemetryEvent = await ResolveCompletionAsync(text, prefix, new Position(5, 0));

            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.ObjectBodySnippetInsertion);
            bicepTelemetryEvent.Properties.Should().Equal(properties);
        }

        [TestMethod]
        public async Task VerifyModuleBodySnippetInsertionFiresTelemetryEvent()
        {
            string text = "module foo 'test.bicep' = ";
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "name", "{}" }
            };

            BicepTelemetryEvent bicepTelemetryEvent = await ResolveCompletionAsync(text, "{}", new Position(0, text.Length));

            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.ModuleBodySnippetInsertion);
            bicepTelemetryEvent.Properties.Should().Equal(properties);
        }

        private async Task<BicepTelemetryEvent> ResolveCompletionAsync(string text, string prefix, Position position)
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            var telemetryReceived = new TaskCompletionSource<BicepTelemetryEvent>();

            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                TestContext,
                options =>
                {
                    options.OnTelemetryEvent<BicepTelemetryEvent>(telemetry => telemetryReceived.SetResult(telemetry));
                },
                new LanguageServer.Server.CreationOptions(NamespaceProvider: BicepTestConstants.NamespaceProvider, FileResolver: new InMemoryFileResolver(fileSystemDict)));

            var mainUri = DocumentUri.FromFileSystemPath("/main.bicep");
            fileSystemDict[mainUri.ToUri()] = text;

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier(mainUri),
                Position = position,
            });

            CompletionItem completionItem = completions.Where(x => x.Kind == CompletionItemKind.Snippet && x.Label == prefix).First();
            Command? command = completionItem.Command;
            JArray? arguments = command!.Arguments;
            BicepTelemetryEvent? telemetryEvent = arguments!.First().ToObject<BicepTelemetryEvent>();

            await client.ResolveCompletion(completionItem);
            await client.Workspace.ExecuteCommand(command);

            return await IntegrationTestHelper.WithTimeoutAsync(telemetryReceived.Task);
        }
    }
}
