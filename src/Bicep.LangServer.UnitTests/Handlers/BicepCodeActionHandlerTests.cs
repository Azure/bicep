// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.CodeAction.Fixes;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Options;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using Grpc.Net.Client.Balancer;
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
            var bicepfile = GetBicepFile(@"param test string = 'abc'");
            var editRuleInBicepConfigCodeActionTitle = string.Format(LangServerResources.EditLinterRuleActionTitle, NoUnusedParametersRule.Code);

            var codeActions = await GetCodeActions(bicepfile, new(0, 8, 0, 8), false);

            codeActions.Any(x => x.Title == editRuleInBicepConfigCodeActionTitle).Should().BeFalse();
        }

        [TestMethod]
        public async Task VerifyEditRuleInBicepCodeActionIsAvailable_WhenClientSupportsShowDocumentRequestAndWorkspaceFolders()
        {
            var bicepfile = GetBicepFile(@"param test string = 'abc'");
            var editRuleInBicepConfigCodeActionTitle = string.Format(LangServerResources.EditLinterRuleActionTitle, NoUnusedParametersRule.Code);

            var codeActions = await GetCodeActions(bicepfile, new(0, 8, 0, 8), true);

            codeActions.Any(x => x.Title == editRuleInBicepConfigCodeActionTitle).Should().BeTrue();
        }

        [TestMethod]
        public async Task Multiline_string_code_action_suggests_fix()
        {
            var (contents, cursor) = ParserHelper.GetFileWithSingleCursor("""
var foo = 'abc\nde|f\nghi'
""");
            var bicepFile = GetBicepFile(contents);
            var codeAction = await GetSingleCodeAction(bicepFile, cursor, MultilineStringCodeFixProvider.Title);

            LspRefactoringHelper.ApplyCodeAction(bicepFile, codeAction).Text.Should().Be("""
var foo = '''abc
def
ghi'''
""");
        }

        [TestMethod]
        public async Task Multiline_string_code_action_handles_escaping_correctly()
        {
            var (contents, cursor) = ParserHelper.GetFileWithSingleCursor("""
var foo = 'abc\'d\\e|f\n\u{1F4AA}!!'
""");
            var bicepFile = GetBicepFile(contents);
            var codeAction = await GetSingleCodeAction(bicepFile, cursor, MultilineStringCodeFixProvider.Title);

            LspRefactoringHelper.ApplyCodeAction(bicepFile, codeAction).Text.Should().Be("""
var foo = '''abc'd\ef
💪!!'''
""");
        }

        [TestMethod]
        public async Task Multiline_string_suggestion_skips_strings_that_cannot_be_represented_as_multiline()
        {
            var (contents, cursor) = ParserHelper.GetFileWithSingleCursor("""
var foo = 'abc\'|\'\'def'
""");
            var codeActions = await GetCodeActions(GetBicepFile(contents), cursor);

            codeActions.Should().NotContain(x => x.Title == MultilineStringCodeFixProvider.Title);
        }

        [TestMethod]
        public async Task Multiline_string_suggestion_skips_strings_with_interpolation()
        {
            var (contents, cursor) = ParserHelper.GetFileWithSingleCursor("""
var foo = 'abc\nd${123}e|f\nghi'
""");
            var codeActions = await GetCodeActions(GetBicepFile(contents), cursor);

            codeActions.Should().NotContain(x => x.Title == MultilineStringCodeFixProvider.Title);
        }

        [TestMethod]
        public async Task Multiline_string_suggestion_skips_strings_that_are_already_multiline()
        {
            var (contents, cursor) = ParserHelper.GetFileWithSingleCursor("""
var foo = '''abc
d|ef
ghi'''
""");
            var codeActions = await GetCodeActions(GetBicepFile(contents), cursor);

            codeActions.Should().NotContain(x => x.Title == MultilineStringCodeFixProvider.Title);
        }

        [TestMethod]
        public async Task Multiline_string_suggestion_skips_strings_without_newline_chars()
        {
            var (contents, cursor) = ParserHelper.GetFileWithSingleCursor("""
var foo = 'i am just a |normal string!'
""");
            var codeActions = await GetCodeActions(GetBicepFile(contents), cursor);

            codeActions.Should().NotContain(x => x.Title == MultilineStringCodeFixProvider.Title);
        }

        private static LanguageClientFile GetBicepFile(string contents)
            => new(new Uri("file:///main.bicep"), contents);

        private async Task<IEnumerable<CodeAction>> GetCodeActions(
            LanguageClientFile bicepFile,
            Range range,
            bool clientSupportShowDocumentRequestAndWorkspaceFolders = true)
        {
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(bicepFile.Uri, bicepFile.Text, upsertCompilation: true);

            var clientCapabilitiesProvider = StrictMock.Of<IClientCapabilitiesProvider>();
            clientCapabilitiesProvider.Setup(m => m.DoesClientSupportShowDocumentRequest()).Returns(clientSupportShowDocumentRequestAndWorkspaceFolders);
            clientCapabilitiesProvider.Setup(m => m.DoesClientSupportWorkspaceFolders()).Returns(clientSupportShowDocumentRequestAndWorkspaceFolders);

            var bicepEditLinterRuleHandler = new BicepCodeActionHandler(bicepCompilationManager, clientCapabilitiesProvider.Object, new DocumentSelectorFactory(BicepLangServerOptions.Default));

            var codeActionParams = new CodeActionParams()
            {
                Context = new CodeActionContext(),
                Range = range,
                TextDocument = bicepFile.Uri,
            };

            var commandOrCodeActionContainer = await bicepEditLinterRuleHandler.Handle(codeActionParams, CancellationToken.None);
            return commandOrCodeActionContainer!.GetCodeActions();
        }

        private async Task<IEnumerable<CodeAction>> GetCodeActions(
            LanguageClientFile bicepFile,
            int cursor)
        {
            Position position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor);

            return await GetCodeActions(bicepFile, new Range(position, position));
        }

        private async Task<CodeAction> GetSingleCodeAction(
            LanguageClientFile bicepFile,
            int cursor,
            string title)
        {
            var codeActions = await GetCodeActions(bicepFile, cursor);

            return codeActions.Should().ContainSingle(x => x.Title == title).Subject;
        }
    }
}
