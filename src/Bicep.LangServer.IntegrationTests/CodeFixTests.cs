// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class CodeFixTests
    {
        private const string SecureTitle = "Add @secure";
        private const string DescriptionTitle = "Add @description";
        private const string AllowedTitle = "Add @allowed";
        private const string MinLengthTitle = "Add @minLength";
        private const string MaxLengthTitle = "Add @maxLength";
        private const string MinValueTitle = "Add @minValue";
        private const string MaxValueTitle = "Add @maxValue";

        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataRow("string", "@secure()", SecureTitle)]
        [DataRow("object", "@secure()", SecureTitle)]
        [DataRow("string", "@description('')", DescriptionTitle)]
        [DataRow("object", "@description('')", DescriptionTitle)]
        [DataRow("array", "@description('')", DescriptionTitle)]
        [DataRow("bool", "@description('')", DescriptionTitle)]
        [DataRow("int", "@description('')", DescriptionTitle)]
        [DataRow("string", "@allowed([])", AllowedTitle)]
        [DataRow("object", "@allowed([])", AllowedTitle)]
        [DataRow("array", "@allowed([])", AllowedTitle)]
        [DataRow("bool", "@allowed([])", AllowedTitle)]
        [DataRow("int", "@allowed([])", AllowedTitle)]
        [DataRow("string", "@minLength()", MinLengthTitle)]
        [DataRow("array", "@minLength()", MinLengthTitle)]
        [DataRow("string", "@maxLength()", MaxLengthTitle)]
        [DataRow("array", "@maxLength()", MaxLengthTitle)]
        [DataRow("int", "@minValue()", MinValueTitle)]
        [DataRow("int", "@maxValue()", MaxValueTitle)]
        [DataTestMethod]
        public async Task Parameter_basic_test(string type, string decorator, string title)
        {
            await AssertBasicParameterTest(type, title, decorator);
        }

        [DataRow("string", "@secure()", SecureTitle)]
        [DataRow("object", "@secure()", SecureTitle)]
        [DataRow("string", "@description()", DescriptionTitle)]
        [DataRow("object", "@description()", DescriptionTitle)]
        [DataRow("array", "@description()", DescriptionTitle)]
        [DataRow("bool", "@description()", DescriptionTitle)]
        [DataRow("int", "@description()", DescriptionTitle)]
        [DataRow("string", "@allowed([])", AllowedTitle)]
        [DataRow("object", "@allowed([])", AllowedTitle)]
        [DataRow("array", "@allowed([])", AllowedTitle)]
        [DataRow("bool", "@allowed([])", AllowedTitle)]
        [DataRow("int", "@allowed([])", AllowedTitle)]
        [DataRow("string", "@minLength()", MinLengthTitle)]
        [DataRow("object", "@minLength()", MinLengthTitle)]
        [DataRow("string", "@maxLength()", MaxLengthTitle)]
        [DataRow("object", "@maxLength()", MaxLengthTitle)]
        [DataRow("int", "@minValue()", MinValueTitle)]
        [DataRow("int", "@maxValue()", MaxValueTitle)]
        [DataTestMethod]
        public async Task Parameter_do_not_add_duplicate(string type, string decorator, string title)
        {
            (var codeActions, var bicepFile) = await TestCodeAction(type, decorator);
            codeActions.Should().NotContain(x => x.Title == title);
        }

        [DataRow("array", SecureTitle)]
        [DataRow("bool", SecureTitle)]
        [DataRow("int", SecureTitle)]
        [DataRow("object", MinLengthTitle)]
        [DataRow("bool", MinLengthTitle)]
        [DataRow("int", MinLengthTitle)]
        [DataRow("object", MaxLengthTitle)]
        [DataRow("bool", MaxLengthTitle)]
        [DataRow("int", MaxLengthTitle)]
        [DataRow("object", MinValueTitle)]
        [DataRow("bool", MinValueTitle)]
        [DataRow("string", MinValueTitle)]
        [DataRow("array", MinValueTitle)]
        [DataRow("object", MaxValueTitle)]
        [DataRow("bool", MaxValueTitle)]
        [DataRow("string", MaxValueTitle)]
        [DataRow("array", MaxValueTitle)]
        [DataTestMethod]
        public async Task Parameter_do_not_add_for_unsupported_type(string type, string title)
        {
            (var codeActions, var bicepFile) = await TestCodeAction(type);
            codeActions.Should().NotContain(x => x.Title == title);
        }

        private async Task AssertBasicParameterTest(string type, string title, string expectedDecorator)
        {
            (var codeActions, var bicepFile) = await TestCodeAction(type);
            codeActions.Should().Contain(x => x.Title == title);

            var updatedFile = ApplyCodeAction(bicepFile, codeActions.Single(x => x.Title == title));
            updatedFile.Should().HaveSourceText($@"
{expectedDecorator}
param foo {type}
");
        }

        private static async Task<IEnumerable<CodeAction>> RequestCodeActions(ILanguageClient client, BicepFile bicepFile, int cursor)
        {
            var startPosition = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor);
            var endPosition = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor);

            var result = await client.RequestCodeAction(new CodeActionParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.FileUri),
                Range = new Range(startPosition, endPosition),
            });

            return result.Select(x => x.CodeAction).WhereNotNull();
        }

        private async Task<(IEnumerable<CodeAction> codeActions, BicepFile bicepFile)> TestCodeAction(string type, string decorator = "")
        {
            string fileWithCursors = @$"
param fo|o {type}
";
            if (!String.IsNullOrWhiteSpace(decorator))
            {
                fileWithCursors = @$"
{decorator}
param fo|o {type}
";
            }
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///main.bicep"), file);
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(TestContext, file, bicepFile.FileUri);
            var client = helper.Client;

            var codeActions = await RequestCodeActions(client, bicepFile, cursors.Single());
            return (codeActions, bicepFile);
        }

        private static BicepFile ApplyCodeAction(BicepFile bicepFile, CodeAction codeAction, params string[] tabStops)
        {
            // only support a small subset of possible edits for now - can always expand this later on
            codeAction.Edit!.Changes.Should().NotBeNull();
            codeAction.Edit.Changes.Should().HaveCount(1);
            codeAction.Edit.Changes.Should().ContainKey(bicepFile.FileUri);

            var changes = codeAction.Edit.Changes![bicepFile.FileUri];
            changes.Should().HaveCount(1);

            var replacement = changes.Single();

            var start = PositionHelper.GetOffset(bicepFile.LineStarts, replacement.Range.Start);
            var end = PositionHelper.GetOffset(bicepFile.LineStarts, replacement.Range.End);
            var textToInsert = replacement.NewText;

            // the handler can contain tabs. convert to double space to simplify printing.
            textToInsert = textToInsert.Replace("\t", "  ");

            var originalFile = bicepFile.ProgramSyntax.ToTextPreserveFormatting();
            var replaced = originalFile.Substring(0, start) + textToInsert + originalFile.Substring(end);

            return SourceFileFactory.CreateBicepFile(bicepFile.FileUri, replaced);
        }
    }
}
