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
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task Secure_parameter_basic_test()
        {
            var fileWithCursors = @"
param fo|o string
";

            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///main.bicep"), file);
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(TestContext, file, bicepFile.FileUri);
            var client = helper.Client;

            var codeActions = await RequestCodeActions(client, bicepFile, cursors.Single());
            codeActions.Should().Contain(x => x.Title == "Add @secure");

            var updatedFile = ApplyCodeAction(bicepFile, codeActions.Single(x => x.Title == "Add @secure"));
            updatedFile.Should().HaveSourceText(@"
@secure()
param foo string
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
