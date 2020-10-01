// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.Parser;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ParserTests
    {
        private class SyntaxCollectorVisitor : SyntaxVisitor
        {
            public class SyntaxItem
            {
                public SyntaxItem(SyntaxBase syntax, int depth)
                {
                    Syntax = syntax;
                    Depth = depth;
                }

                public SyntaxBase Syntax { get; }

                public int Depth { get; }
            }

            private int depth = 0;
            private readonly IList<SyntaxItem> syntaxList = new List<SyntaxItem>();

            private SyntaxCollectorVisitor()
            {
            }

            public static ImmutableArray<SyntaxItem> Build(ProgramSyntax syntax)
            {
                var visitor = new SyntaxCollectorVisitor();
                visitor.VisitProgramSyntax(syntax);

                return visitor.syntaxList.ToImmutableArray();
            }

            protected override void VisitInternal(SyntaxBase syntax)
            {
                syntaxList.Add(new SyntaxItem(syntax, depth));

                depth++;
                base.VisitInternal(syntax);
                depth--;
            }
        }

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
        public void Parser_should_produce_expected_syntax(DataSet dataSet)
        {
            var program = SyntaxFactory.CreateFromText(dataSet.Bicep);
            var syntaxList = SyntaxCollectorVisitor.Build(program);

            string getLoggingString(SyntaxCollectorVisitor.SyntaxItem data)
            {
                var depthPrefix = new string(' ', data.Depth);
                
                if (data.Syntax is Token token)
                {
                    return $"{depthPrefix}{token.Type} |{OutputHelper.EscapeWhitespace(token.Text)}|";
                }

                return $"{depthPrefix}{data.Syntax.GetType().Name}";
            }

            TextSpan getSpan(SyntaxCollectorVisitor.SyntaxItem data) => data.Syntax.Span;

            var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(dataSet, syntaxList, getSpan, getLoggingString);
            var resultsFile = FileHelper.SaveResultFile(this.TestContext!, Path.Combine(dataSet.Name, DataSet.TestFileMainSyntax), sourceTextWithDiags);

            sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                dataSet.Syntax,
                expectedLocation: OutputHelper.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainSyntax),
                actualLocation: resultsFile);
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }

        private static void RunRoundTripTest(string contents)
        {
            var program = SyntaxFactory.CreateFromText(contents);
            program.Should().BeOfType<ProgramSyntax>();

            var buffer = new StringBuilder();
            var visitor = new PrintVisitor(buffer);

            visitor.Visit(program);

            buffer.ToString().Should().Be(contents);
        }

        private static void RunSpanConsistencyTest(string text)
        {
            var program = SyntaxFactory.CreateFromText(text);
            program.Should().BeOfType<ProgramSyntax>();

            var visitor = new SpanConsistencyVisitor();
            visitor.Visit(program);
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

                this.maxPosition.Should().Be(span.Position + span.Length);
            }

            public override void VisitToken(Token token)
            {
                base.VisitToken(token);

                var span = token.Span;
                span.Position.Should().BeGreaterOrEqualTo(this.maxPosition);
                this.maxPosition = span.Position + span.Length;
            }
        }
    }
}

