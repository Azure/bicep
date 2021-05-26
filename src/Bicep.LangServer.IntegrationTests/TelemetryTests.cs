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
            var fileSystemDict = new Dictionary<Uri, string>();
            var telemetryReceived = new TaskCompletionSource<TelemetryEventParams>();

            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                options => options.OnTelemetryEvent(telemetry => {
                    telemetryReceived.SetResult(telemetry);
                }),
                fileResolver: new InMemoryFileResolver(fileSystemDict));

            var mainUri = DocumentUri.FromFileSystemPath("/main.bicep");
            fileSystemDict[mainUri.ToUri()] = string.Empty;

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier(mainUri),
                Position = new Position(0, 0),
            });

            CompletionItem completionItem = completions.Where(x => x.Kind == CompletionItemKind.Snippet && x.Label == "res-aks-cluster").First();
            Command? command = completionItem.Command;

            command!.Name.Should().Be(TelemetryConstants.CommandName);

            JArray? arguments = command!.Arguments;
            BicepTelemetryEvent? telemetryEvent = arguments!.First().ToObject<BicepTelemetryEvent>();

            telemetryEvent!.EventName.Should().Be(TelemetryConstants.EventNames.TopLevelDeclarationSnippetInsertion);
            telemetryEvent!.Properties!.Should().ContainKey("name");

            await client.ResolveCompletion(completionItem);
            await client.Workspace.ExecuteCommand(command);

            TelemetryEventParams telemetryEventParams = await IntegrationTestHelper.WithTimeoutAsync(telemetryReceived.Task);

            telemetryEventParams.Data.Keys.Count.Should().Be(2);
            telemetryEventParams.Data.Keys.Should().Contain("eventName");
            telemetryEventParams.Data.Keys.Should().Contain("properties");
            telemetryEventParams.Data["eventName"].ToString().Should().Be(TelemetryConstants.EventNames.TopLevelDeclarationSnippetInsertion);
            telemetryEventParams.Data["properties"].ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""name"": ""res-aks-cluster""
}");
        }
    }
}
