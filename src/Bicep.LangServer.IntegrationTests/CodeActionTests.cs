// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Bicep.Core.CodeAction;
using Bicep.Core.Parser;
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
            var fixables = compilation.GetEntrypointSemanticModel().GetAllDiagnostics().OfType<IFixable>();

            foreach (IFixable fixable in fixables)
            {
                foreach (var span in GetOverlappingSpans(fixable.Span))
                {
                    CommandOrCodeActionContainer? quickFixes = await client.RequestCodeAction(new CodeActionParams
                    {
                        TextDocument = new TextDocumentIdentifier(uri),
                        Range = span.ToRange(lineStarts)
                    });

                    // Assert.
                    quickFixes.Should().NotBeNull();

                    var quickFixList = quickFixes.ToList();
                    var bicepFixList = fixable.Fixes.ToList();

                    quickFixList.Should().HaveSameCount(bicepFixList);

                    for (int i = 0; i < quickFixList.Count; i++)
                    {
                        var quickFix = quickFixList[i];
                        var bicepFix = bicepFixList[i];

                        quickFix.IsCodeAction.Should().BeTrue();
                        quickFix.CodeAction.Kind.Should().Be(CodeActionKind.QuickFix);
                        quickFix.CodeAction.Title.Should().Be(bicepFix.Description);
                        quickFix.CodeAction.Edit.Changes.Should().ContainKey(uri);

                        var textEditList = quickFix.CodeAction.Edit.Changes[uri].ToList();
                        var replacementList = bicepFix.Replacements.ToList();

                        for (int j = 0; j < textEditList.Count; j++)
                        {
                            var textEdit = textEditList[j];
                            var replacement = replacementList[j];

                            textEdit.Range.Should().Be(replacement.ToRange(lineStarts));
                            textEdit.NewText.Should().Be(replacement.Text);
                        }
                    }
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
            var nonFixables = compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Where(diagnostic => !(diagnostic is IFixable));

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

        private static IEnumerable<TextSpan> GetOverlappingSpans(TextSpan span)
        {
            // Same span.
            yield return span;

            // Adjacent spans before.
            int startOffset = Math.Max(0, span.Position - 10);
            yield return new TextSpan(startOffset, 10);
            yield return new TextSpan(span.Position, 0);

            // Adjacent spans after.
            yield return new TextSpan(span.Position + span.Length, 10);
            yield return new TextSpan(span.Position + span.Length, 0);

            // Overlapping spans.
            yield return new TextSpan(startOffset, 11);
            yield return new TextSpan(span.Position + 1, span.Length);
            yield return new TextSpan(startOffset, span.Length + 10);
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }
    }
}
