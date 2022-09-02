// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepCodeActionHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task VerifyEditRuleInBicepCodeActionIsNotAvailable_WhenClientDoesNotSupportShowDocumentRequestAndWorkspaceFolders()
        {
            var bicepFileContents = @"param test string = 'abc'";
            var editRuleInBicepConfigCodeActionTitle = string.Format(LangServerResources.EditLinterRuleActionTitle, NoUnusedParametersRule.Code);

            var codeActions = await GetCodeActions(bicepFileContents, 0, 8, 0, 8, false);

            codeActions.Any(x => x.Title == editRuleInBicepConfigCodeActionTitle).Should().BeFalse();
        }

        [TestMethod]
        public async Task VerifyEditRuleInBicepCodeActionIsAvailable_WhenClientSupportsShowDocumentRequestAndWorkspaceFolders()
        {
            var bicepFileContents = @"param test string = 'abc'";
            var editRuleInBicepConfigCodeActionTitle = string.Format(LangServerResources.EditLinterRuleActionTitle, NoUnusedParametersRule.Code);

            var codeActions = await GetCodeActions(bicepFileContents, 0, 8, 0, 8, true);

            codeActions.Any(x => x.Title == editRuleInBicepConfigCodeActionTitle).Should().BeTrue();
        }

        private async Task<IEnumerable<CodeAction>> GetCodeActions(
            string bicepFileContents,
            int startLine,
            int startCharacter,
            int endLine,
            int endCharacter,
            bool clientSupportShowDocumentRequestAndWorkspaceFolders = true)
        {
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents, testOutputPath);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);

            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, upsertCompilation: true);

            var clientCapabilitiesProvider = StrictMock.Of<IClientCapabilitiesProvider>();
            clientCapabilitiesProvider.Setup(m => m.DoesClientSupportShowDocumentRequest()).Returns(clientSupportShowDocumentRequestAndWorkspaceFolders);
            clientCapabilitiesProvider.Setup(m => m.DoesClientSupportWorkspaceFolders()).Returns(clientSupportShowDocumentRequestAndWorkspaceFolders);

            var bicepEditLinterRuleHandler = new BicepCodeActionHandler(bicepCompilationManager, clientCapabilitiesProvider.Object);

            var range = new Range()
            {
                Start = new Position(startLine, startCharacter),
                End = new Position(endLine, endCharacter)
            };

            var codeActionParams = new CodeActionParams()
            {
                Context = new CodeActionContext(),
                Range = range,
                TextDocument = documentUri
            };

            var commandOrCodeActionContainer = await bicepEditLinterRuleHandler.Handle(codeActionParams, CancellationToken.None);
            return commandOrCodeActionContainer.GetCodeActions();
        }
    }
}
