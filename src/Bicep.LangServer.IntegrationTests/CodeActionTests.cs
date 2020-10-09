// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Bicep.Core.Linter;
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class CodeActionTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task RequestingCodeActionWithFixableDiagnosticsShouldProduceQuickFixes(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");
            var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);

            // construct a parallel compilation
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(dataSet.Bicep));
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.Bicep);
            var fixables = compilation.GetSemanticModel().GetAllDiagnostics().OfType<IFixable>();

            foreach (IFixable fixable in fixables)
            {
                CommandOrCodeActionContainer? quickFixes = await client.RequestCodeAction(new CodeActionParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Range = fixable.Span.ToRange(lineStarts)
                });

                // Assert.
                quickFixes.Should().NotBeNull();
                quickFixes.Should().HaveCount(fixable.Fixes.Count);

                int fixIndex = 0;

                foreach (CommandOrCodeAction quickFix in quickFixes)
                {
                    var fix = fixable.Fixes[fixIndex];

                    quickFix.IsCodeAction.Should().BeTrue();
                    quickFix.CodeAction.Kind.Should().Be(CodeActionKind.QuickFix);
                    quickFix.CodeAction.Title.Should().Be(fix.Description);
                    quickFix.CodeAction.Edit.Changes.Should().ContainKey(uri);

                    var textEdits = quickFix.CodeAction.Edit.Changes[uri];
                    int editIndex = 0;

                    foreach (var textEdit in textEdits)
                    {
                        var fixEdit = fix.Edits[editIndex];

                        textEdit.Range.Should().Be(fixEdit.ToRange(lineStarts));
                        textEdit.NewText.Should().Be(fixEdit.Text);

                        editIndex++;
                    }

                    fixIndex++;
                }
            }

        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task RequestingCodeActionWithNonFixableDiagnosticsShouldProduceEmptyQuickFixes(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");
            var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);

            // construct a parallel compilation
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(dataSet.Bicep));
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.Bicep);
            var nonFixables = compilation.GetSemanticModel().GetAllDiagnostics().Where(diagnostic => !(diagnostic is IFixable));

            foreach (var nonFixable in nonFixables)
            {
                CommandOrCodeActionContainer? quickFixes = await client.RequestCodeAction(new CodeActionParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Range = nonFixable.Span.ToRange(lineStarts)
                });

                // Assert.
                quickFixes.Should().NotBeNull();
                quickFixes.Should().BeEmpty();
            }
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }
    }
}
