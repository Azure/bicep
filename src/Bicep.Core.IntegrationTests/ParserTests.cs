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
            public class SyntaxOrToken
            {
                public SyntaxOrToken(SyntaxBase syntax, int depth)
                {
                    Syntax = syntax;
                    Token = null;
                    Depth = depth;
                }

                public SyntaxOrToken(Token token, int depth)
                {
                    Syntax = null;
                    Token = token;
                    Depth = depth;
                }

                public SyntaxBase? Syntax { get; }

                public Token? Token { get; }

                public int Depth { get; }
            }

            private int depth = 0;
            private readonly IList<SyntaxOrToken> syntaxList = new List<SyntaxOrToken>();

            private SyntaxCollectorVisitor()
            {
            }

            public static ImmutableArray<SyntaxOrToken> Build(ProgramSyntax syntax)
            {
                var visitor = new SyntaxCollectorVisitor();
                visitor.VisitProgramSyntax(syntax);

                return visitor.syntaxList.ToImmutableArray();
            }

            protected override void VisitTokenInternal(Token token)
            {
                syntaxList.Add(new SyntaxOrToken(token, depth));

                base.VisitTokenInternal(token);
            }

            protected override void VisitInternal(SyntaxBase syntax)
            {
                syntaxList.Add(new SyntaxOrToken(syntax, depth));

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
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void Parser_should_produce_expected_syntax(DataSet dataSet)
        {
            var program = SyntaxFactory.CreateFromText(dataSet.Bicep);
            var syntaxList = SyntaxCollectorVisitor.Build(program);

            string getLoggingString(SyntaxCollectorVisitor.SyntaxOrToken data)
            {
                var depthPrefix = new string(' ', data.Depth);
                if (data.Syntax != null)
                {
                    return $"{depthPrefix}{data.Syntax.GetType().Name}";
                }
                
                if (data.Token != null)
                {
                    return $"{depthPrefix}{data.Token.Type} |{OutputHelper.EscapeWhitespace(data.Token.Text)}|";
                }

                throw new NotImplementedException();
            }

            TextSpan getSpan(SyntaxCollectorVisitor.SyntaxOrToken data)
            {
                var positionable = (data.Syntax as IPositionable ?? data.Token as IPositionable)!;

                return positionable.Span;
            }

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
    }
}

