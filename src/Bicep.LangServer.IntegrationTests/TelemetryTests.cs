// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class TelemetryTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task ValidateDeclarationSnippetCompletionItemContainsCommandWithTelemetryInformation()
        {
            var syntaxTree = SyntaxTree.Create(new Uri("file:///main.bicep"), string.Empty);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(string.Empty, syntaxTree.FileUri);

            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier(syntaxTree.FileUri),
                Position = TextCoordinateConverter.GetPosition(syntaxTree.LineStarts, 0),
            });

            CompletionItem completionItem = completions.Where(x => x.Kind == CompletionItemKind.Snippet && x.Label == "res-aks-cluster").First();
            Command? command = completionItem.Command;
            TelemetryEvent? telemetryEvent = command?.Arguments?.First().ToObject<TelemetryEvent>();

            command?.Name.Should().Be(TelemetryConstants.CommandName);
            telemetryEvent?.EventName.Should().Be(TelemetryConstants.EventNames.DeclarationSnippetCompletion);
            telemetryEvent?.Properties?.ContainsKey("label");
        }
    }
}
