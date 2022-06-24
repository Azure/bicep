// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ParamsParserTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetParamData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Params_Parser_should_produce_expected_syntax(DataSet dataSet)
        {
            if (dataSet.BicepParam is null)
            {
                throw new InvalidOperationException($"Expected {nameof(dataSet.BicepParam)} to be non-null");
            }

            var program = ParamsParserHelper.ParamsParse(dataSet.BicepParam);
            var syntaxList = SyntaxCollectorVisitorHelper.SyntaxCollectorVisitor.Build(program);
            var syntaxByParent = syntaxList.ToLookup(x => x.Parent);

            string getLoggingString(SyntaxCollectorVisitorHelper.SyntaxCollectorVisitor.SyntaxItem data)
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

            TextSpan getSpan(SyntaxCollectorVisitorHelper.SyntaxCollectorVisitor.SyntaxItem data) => data.Syntax.Span;

            var sourceTextWithDiags = DataSet.AddDiagsToParamSourceText(dataSet, syntaxList, getSpan, getLoggingString);
            var resultsFile = FileHelper.SaveResultFile(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainParamSyntax), sourceTextWithDiags);

            sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                dataSet.ParamSyntax ?? throw new InvalidOperationException($"Expected {nameof(dataSet.ParamSyntax)} to be non-null."),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainParamSyntax),
                actualLocation: resultsFile);
        }

        private static IEnumerable<object[]> GetParamData()
        {
            return DataSets.ParamDataSets.ToDynamicTestData();
        }

    }
}
