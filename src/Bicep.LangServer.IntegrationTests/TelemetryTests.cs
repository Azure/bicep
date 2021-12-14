// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
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

        [TestMethod]
        public async Task VerifyDisableNextLineCodeActionInvocationFiresTelemetryEvent()
        {
            var bicepFileContents = @"param storageAccount string = 'testStorageAccount'";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var uri = documentUri.ToUri();

            var files = new Dictionary<Uri, string>
            {
                [uri] = bicepFileContents,
            };

            var compilation = new Compilation(BicepTestConstants.NamespaceProvider, SourceFileGroupingFactory.CreateForFiles(files, uri, BicepTestConstants.FileResolver, BicepTestConstants.BuiltInConfiguration), BicepTestConstants.BuiltInConfiguration);
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            var telemetryEventsListener = new MultipleMessageListener<BicepTelemetryEvent>();

            using var helper = await LanguageServerHelper.StartServerWithClientConnectionAsync(
                TestContext,
                options =>
                {
                    options.OnTelemetryEvent<BicepTelemetryEvent>(telemetry => telemetryEventsListener.AddMessage(telemetry));
                },
                new Server.CreationOptions(NamespaceProvider: BicepTestConstants.NamespaceProvider, FileResolver: new InMemoryFileResolver(files)));
            var client = helper.Client;

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, files[uri], 1));

            var bicepTelemetryEvent = await telemetryEventsListener.WaitNext();
            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.LinterRuleStateOnBicepFileOpen);

            bicepTelemetryEvent = await telemetryEventsListener.WaitNext();
            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.BicepFileOpen);

            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var codeActions = await client.RequestCodeAction(new CodeActionParams
            {
                TextDocument = new TextDocumentIdentifier(documentUri),
                Range = diagnostics.First().ToRange(lineStarts)
            });

            var disableNextLineCodeAction = codeActions.First(x => x.CodeAction!.Title == "Disable no-unused-params for this line").CodeAction;

            _ = await client!.ResolveCodeAction(disableNextLineCodeAction!);

            Command? command = disableNextLineCodeAction!.Command;
            JArray? arguments = command!.Arguments;

            await client.Workspace.ExecuteCommand(command);

            bicepTelemetryEvent = await telemetryEventsListener.WaitNext();

            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "code", "no-unused-params" }
            };

            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.DisableNextLineDiagnostics);
            bicepTelemetryEvent.Properties.Should().Equal(properties);
        }

        [TestMethod]
        public async Task BicepFileOpen_ShouldFireTelemetryEvent()
        {
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""warning""
        },
        ""no-unused-vars"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, testOutputPath);

            var fileSystemDict = new Dictionary<Uri, string>();

            var mainBicepFileContents = @"param appInsightsName string = 'testAppInsightsName'

var deployGroups = true

resource applicationInsights 'Microsoft.Insights/components@2015-05-01' = {
  name: appInsightsName
  location: resourceGroup().location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
  resource favorites 'favorites@2015-05-01'{
    name: 'testName'
  }
}

module apimGroups 'groups.bicep' = if (deployGroups) {
  name: 'apimGroups'
}

param location string = 'testLocation'

#disable-next-line";
            var mainBicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", mainBicepFileContents, testOutputPath);
            var mainUri = DocumentUri.FromFileSystemPath(mainBicepFilePath);
            fileSystemDict[mainUri.ToUri()] = mainBicepFileContents;

            var referencedBicepFileContents = @"resource parentAPIM 'Microsoft.ApiManagement/service@2019-01-01' existing = {
  name: 'testApimInstanceName'
}

resource apimGroup 'Microsoft.ApiManagement/service/groups@2020-06-01-preview' = {
  name: 'apiManagement/groups'
}

param storageAccount string = 'testStorageAccount'
param location string = 'testLocation'

var useDefaultSettings = true";
            var referencedBicepFilePath = FileHelper.SaveResultFile(TestContext, "groups.bicep", referencedBicepFileContents, testOutputPath);
            var moduleUri = DocumentUri.FromFileSystemPath(referencedBicepFilePath);
            fileSystemDict[moduleUri.ToUri()] = referencedBicepFileContents;

            var telemetryEventsListener = new MultipleMessageListener<BicepTelemetryEvent>();

            using var helper = await LanguageServerHelper.StartServerWithClientConnectionAsync(
                TestContext,
                options =>
                {
                    options.OnTelemetryEvent<BicepTelemetryEvent>(telemetry => telemetryEventsListener.AddMessage(telemetry));
                },
                creationOptions: new Server.CreationOptions(FileResolver: new InMemoryFileResolver(fileSystemDict)));
            var client = helper.Client;

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

            var bicepTelemetryEvent = await telemetryEventsListener.WaitNext();

            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "enabled", "true" },
                { "simplify-interpolation", "warning" },
                { "no-unused-vars", "info" },
                { "no-hardcoded-env-urls", "warning" },
                { "no-unused-params", "warning" },
                { "prefer-interpolation", "warning" },
                { "use-protectedsettings-for-commandtoexecute-secrets", "warning" },
                { "no-unnecessary-dependson", "warning" },
                { "adminusername-should-not-be-literal", "warning" },
                { "use-stable-vm-image", "warning" },
                { "secure-parameter-default", "warning" },
                { "outputs-should-not-contain-secrets", "warning" },
            };

            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.LinterRuleStateOnBicepFileOpen);
            bicepTelemetryEvent.Properties.Should().Contain(properties);

            properties = new Dictionary<string, string>
            {
                { "modules", "1" },
                { "parameters", "2" },
                { "resources", "2" },
                { "variables", "1" },
                { "fileSizeInBytes", "488" },
                { "lineCount", "23" },
                { "errors", "2" },
                { "warnings", "1" },
                { "modulesInReferencedFiles", "0" },
                { "parentResourcesInReferencedFiles", "2" },
                { "parametersInReferencedFiles", "2" },
                { "variablesInReferencedFiles", "1" }
            };

            bicepTelemetryEvent = await telemetryEventsListener.WaitNext();
            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.BicepFileOpen);
            bicepTelemetryEvent.Properties.Should().Contain(properties);
        }

        private async Task<BicepTelemetryEvent> ResolveCompletionAsync(string text, string prefix, Position position)
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            var telemetryEventsListener = new MultipleMessageListener<BicepTelemetryEvent>();

            using var helper = await LanguageServerHelper.StartServerWithClientConnectionAsync(
                TestContext,
                options =>
                {
                    options.OnTelemetryEvent<BicepTelemetryEvent>(telemetry => telemetryEventsListener.AddMessage(telemetry));
                },
                new Server.CreationOptions(NamespaceProvider: BicepTestConstants.NamespaceProvider, FileResolver: new InMemoryFileResolver(fileSystemDict)));
            var client = helper.Client;

            var mainUri = DocumentUri.FromFileSystemPath("/main.bicep");
            fileSystemDict[mainUri.ToUri()] = text;

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

            var bicepTelemetryEvent = await telemetryEventsListener.WaitNext();
            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.LinterRuleStateOnBicepFileOpen);

            bicepTelemetryEvent = await telemetryEventsListener.WaitNext();
            bicepTelemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.BicepFileOpen);

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

            return await telemetryEventsListener.WaitNext();
        }
    }
}
