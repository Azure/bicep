// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using Bicep.Core.UnitTests.Syntax;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ParserTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void FilesShouldRoundTripSuccessfully(DataSet dataSet)
        {
            RunRoundTripTest(dataSet.Bicep);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void FileTreeNodesShouldHaveConsistentSpans(DataSet dataSet)
        {
            RunSpanConsistencyTest(dataSet.Bicep);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("param")]
        [DataRow("param\r\n")]
        [DataRow("param ")]
        [DataRow("param foo")]
        [DataRow("param foo bar")]
        [DataRow("param foo bar =")]
        [DataRow("param foo bar = 1")]
        public void Oneliners_ShouldRoundTripSuccessfully(string contents)
        {
            RunRoundTripTest(contents);
            RunSpanConsistencyTest(contents);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Parser_should_produce_expected_syntax(DataSet dataSet)
        {
            var program = ParserHelper.Parse(dataSet.Bicep);
            var syntaxList = SyntaxCollectorVisitor.Build(program);
            var syntaxByParent = syntaxList.ToLookup(x => x.Parent);

            TextSpan getSpan(SyntaxCollectorVisitor.SyntaxItem data) => data.Syntax.Span;

            var sourceTextWithDiags = DataSet.AddDiagsToSourceText(dataSet, syntaxList, getSpan, syntax => GetSyntaxLoggingString(syntaxByParent, syntax));
            var resultsFile = FileHelper.SaveResultFile(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainSyntax), sourceTextWithDiags);

            sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                dataSet.Syntax,
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainSyntax),
                actualLocation: resultsFile);
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData()]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Params_Parser_should_produce_expected_syntax(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);
            var program = ParamsParserHelper.ParamsParse(data.Parameters.EmbeddedFile.Contents);
            var syntaxList = SyntaxCollectorVisitor.Build(program);
            var syntaxByParent = syntaxList.ToLookup(x => x.Parent);

            TextSpan getSpan(SyntaxCollectorVisitor.SyntaxItem data) => data.Syntax.Span;

            var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(data.Parameters.EmbeddedFile.Contents, "\n", syntaxList, getSpan, syntax => GetSyntaxLoggingString(syntaxByParent, syntax));

            data.Syntax.WriteToOutputFolder(sourceTextWithDiags);
            data.Syntax.ShouldHaveExpectedValue();
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }

        private static void RunRoundTripTest(string contents)
        {
            var program = ParserHelper.Parse(contents);
            program.Should().BeOfType<ProgramSyntax>();

            program.ToTextPreserveFormatting().Should().Be(contents);
        }

        private static void RunSpanConsistencyTest(string text)
        {
            var program = ParserHelper.Parse(text);
            program.Should().BeOfType<ProgramSyntax>();

            var visitor = new SpanConsistencyVisitor();
            visitor.Visit(program);
        }

        private static string GetSyntaxLoggingString(
            ILookup<SyntaxCollectorVisitor.SyntaxItem?, SyntaxCollectorVisitor.SyntaxItem> syntaxByParent,
            SyntaxCollectorVisitor.SyntaxItem syntax)
        {
            // Build a visual graph with lines to help understand the syntax hierarchy
            var graphPrefix = new StringBuilder();

            foreach (var ancestor in syntax.GetAncestors().Reverse().Skip(1))
            {
                var isLast = (ancestor.Depth > 0 && ancestor == syntaxByParent[ancestor.Parent].Last());
                graphPrefix.Append(isLast switch {
                    true => "  ",
                    _ => "| ",
                });
            }

            if (syntax.Depth > 0)
            {
                var isLast = syntax == syntaxByParent[syntax.Parent].Last();
                graphPrefix.Append(isLast switch {
                    true => "└─",
                    _ => "├─",
                });
            }

            return syntax.Syntax switch {
                Token token => $"{graphPrefix}Token({token.Type}) |{OutputHelper.EscapeWhitespace(token.Text)}|",
                _ => $"{graphPrefix}{syntax.Syntax.GetType().Name}",
            };
        }

        private sealed class SpanConsistencyVisitor : SyntaxVisitor
        {
            private int maxPosition = 0;

            protected override void VisitInternal(SyntaxBase node)
            {
                // trivia cause the spans of any particular node to be discontinuous
                // but should be consistently increasing as we visit the tree
                var span = node.Span;
                span.Position.Should().BeGreaterOrEqualTo(this.maxPosition);
                this.maxPosition = node.Span.Position;

                base.VisitInternal(node);

                this.maxPosition.Should().Be(span.GetEndPosition());
            }

            public override void VisitToken(Token token)
            {
                base.VisitToken(token);

                var span = token.Span;
                span.Position.Should().BeGreaterOrEqualTo(this.maxPosition);
                this.maxPosition = span.GetEndPosition();
            }
        }
    }
}

