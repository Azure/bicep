// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ParamsParserTests
    {
        private class SyntaxCollectorVisitor : SyntaxVisitor
        {
            public record SyntaxItem(SyntaxBase Syntax, SyntaxBase? Parent, int Depth);

            private readonly IList<SyntaxItem> syntaxList = new List<SyntaxItem>();
            private SyntaxBase? parent = null;
            private int depth = 0;

            private SyntaxCollectorVisitor()
            {
            }

            public static ImmutableArray<SyntaxItem> Build(ProgramSyntax syntax)
            {
                var visitor = new SyntaxCollectorVisitor();
                visitor.Visit(syntax);

                return visitor.syntaxList.ToImmutableArray();
            }

            protected override void VisitInternal(SyntaxBase syntax)
            {
                syntaxList.Add(new(Syntax: syntax, Parent: parent, Depth: depth));

                var prevParent = parent;
                parent = syntax;
                depth++;
                base.VisitInternal(syntax);
                depth--;
                parent = prevParent;
            }
        }

        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Params_Parser_should_produce_expected_syntax(DataSet dataSet)
        {
            if (dataSet.BicepParam is null)
            {
                return;
            }

            var program = ParamsParserHelper.ParamsParse(dataSet.BicepParam); // TODO: make dataSet.Bicep nullable (rename to BicepParam)
            var syntaxList = SyntaxCollectorVisitor.Build(program);
            var syntaxByParent = syntaxList.ToLookup(x => x.Parent);

            string getLoggingString(SyntaxCollectorVisitor.SyntaxItem data)
            {
                // Build a visual graph with lines to help understand the syntax hierarchy
                var graphPrefix = "";
                if (data.Depth > 0)
                {
                    var lastSibling = syntaxByParent[data.Parent].Last();
                    var isLast = data.Syntax == lastSibling.Syntax;

                    graphPrefix = string.Concat(Enumerable.Repeat("| ", data.Depth - 1));
                    graphPrefix += isLast switch {
                        true => "└─",
                        _ => "├─",
                    };
                }

                return data.Syntax switch {
                    Token token => $"{graphPrefix}Token({token.Type}) |{OutputHelper.EscapeWhitespace(token.Text)}|",
                    _ => $"{graphPrefix}{data.Syntax.GetType().Name}",
                };
            }

            TextSpan getSpan(SyntaxCollectorVisitor.SyntaxItem data) => data.Syntax.Span;

            var sourceTextWithDiags = DataSet.AddDiagsToSourceText(dataSet, syntaxList, getSpan, getLoggingString);
            var resultsFile = FileHelper.SaveResultFile(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainParamSyntax), sourceTextWithDiags);

            sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                dataSet.Syntax,
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainParamSyntax), // and here
                actualLocation: resultsFile);
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.ParamDataSets.ToDynamicTestData();
        }

    }
}
