// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.Samples
{
    [TestClass]
    public class DataSetsTests
    {
        private static readonly Regex Pattern_CRLF = new Regex(@"^(\r\n)+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex Pattern_LF = new Regex(@"^(\n)+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void DataSetShouldBeValid(DataSet dataSet)
        {
            dataSet.Name.Should().NotBeNullOrWhiteSpace();
            dataSet.DisplayName.Should().NotBeNullOrWhiteSpace();
            
            // Bicep files may be empty
            dataSet.Bicep.Should().NotBeNull();
            dataSet.Tokens.Should().NotBeNull();
            dataSet.Diagnostics.Should().NotBeNull();
            dataSet.Symbols.Should().NotBeNull();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void DataSetBicepLineEndingsShouldMatchDataSetNameSuffix(DataSet dataSet)
        {
            var lineEndingTokens = GetLineEndingTokens(dataSet.Bicep);
            lineEndingTokens.Select(token => token.Type).Should().AllBeEquivalentTo(TokenType.NewLine);

            var expectedPattern = dataSet.HasCrLfNewlines() ? Pattern_CRLF : Pattern_LF;
            lineEndingTokens.All(token => expectedPattern.IsMatch(token.Text)).Should().BeTrue();
        }

        private IEnumerable<Token> GetLineEndingTokens(string contents)
        {
            var lexer = new Lexer(new SlidingTextWindow(contents), ToListDiagnosticWriter.Create());
            lexer.Lex();

            return lexer.GetTokens().Where(token => token.Type == TokenType.NewLine);
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }
    }
}

