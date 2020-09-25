// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    [TestClass]
    public class CompletionTestDirectiveParserTests
    {
        [TestMethod]
        public void ParserShouldFindExpectedDirectives()
        {
            const string text = @"// an int variable
var myInt = 42

// a string variable #completionTest(0) -> example
var myStr = 'str'
var curliesWithNoInterp = '}{1}{'
var interp1 = 'abc${123}def'
var interp2 = '${123}def'
var interp3 = 'abc${123}'
// #completionTest(3) -> foo
var interp4 = 'abc${123}${456}jk$l${789}p$'
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
var curliesInInterp = '{${123}{0}${true}}'

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
var bracketAtBeginning = '[test'
var enclosingBrackets = '[test]'
var emptyJsonArray = '[]'
/*
  #completionTest(3,4,5) -> test
  #completionTest(41) -> other
*/
var interpolatedBrackets = '[${myInt}]'
var nestedBrackets = '[test[]test2]'
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
var bracketStringInExpression = concat('[', '\'test\'',']')
";

            var triggers = CompletionTestDirectiveParser.GetTriggers(text);
            triggers.Should().SatisfyRespectively(
                t =>
                {
                    t.Position.Should().Be(new Position(4, 0));
                    t.SetName.Should().Be("example");
                },
                t =>
                {
                    t.Position.Should().Be(new Position(10, 3));
                    t.SetName.Should().Be("foo");
                },
                t =>
                {
                    t.Position.Should().Be(new Position(23, 3));
                    t.SetName.Should().Be("test");
                },
                t =>
                {
                    t.Position.Should().Be(new Position(23, 4));
                    t.SetName.Should().Be("test");
                },
                t =>
                {
                    t.Position.Should().Be(new Position(23, 5));
                    t.SetName.Should().Be("test");
                },
                t =>
                {
                    t.Position.Should().Be(new Position(23, 41));
                    t.SetName.Should().Be("other");
                });
        }

        [TestMethod]
        public void ParserShouldThrowOnInvalidCharIndex()
        {
            const string text = @"// an int variable
var myInt = 42

// a string variable #completionTest(0) -> example
var myStr = 'str'
var curliesWithNoInterp = '}{1}{'
var interp1 = 'abc${123}def'
var interp2 = '${123}def'
var interp3 = 'abc${123}'
// #completionTest(3) -> foo
var interp4 = 'abc${123}${456}jk$l${789}p$'
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
var curliesInInterp = '{${123}{0}${true}}'

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
var bracketAtBeginning = '[test'
var enclosingBrackets = '[test]'
var emptyJsonArray = '[]'
/*
  #completionTest(3,4,53333333333333333333333333333333333333333333333333333333333) -> test
  #completionTest(41) -> other
*/
var interpolatedBrackets = '[${myInt}]'
var nestedBrackets = '[test[]test2]'
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
var bracketStringInExpression = concat('[', '\'test\'',']')
";
            Action badIndex = () => CompletionTestDirectiveParser.GetTriggers(text);
            badIndex.Should().Throw<FormatException>().WithMessage(@"Comment '/*
  #completionTest(3,4,53333333333333333333333333333333333333333333333333333333333) -> test
  #completionTest(41) -> other
*/' contains a completion test directive with an invalid character index '53333333333333333333333333333333333333333333333333333333333'. Please specify a valid 32-bit integer.");
        }
    }
}
