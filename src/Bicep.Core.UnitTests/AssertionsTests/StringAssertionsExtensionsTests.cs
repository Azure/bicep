// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.AssertionsTests
{
    [TestClass]
    public class StringAssertionsExtensionsTests
    {
        [DataRow(
            "hello there",
            new[] { "Hello", "Dolly" },
            StringComparison.InvariantCulture,
            null)]
        [DataRow(
            "hello there",
            new[] { "Hello", "Dolly" },
            StringComparison.InvariantCultureIgnoreCase,
            "Did not expect string \"hello there\" to contain any of the strings: {\"Hello\"} because I said so.")]
        [DataTestMethod]
        public void NotContainAny_WithStringComparison(string text, IEnumerable<string> values, StringComparison stringComparison, string? expectedFailureMessage)
        {
            string? actualMessage;
            try
            {
                text.Should().NotContainAny(values, stringComparison, "because I said so");
                actualMessage = null;
            }
            catch (Exception ex)
            {
                actualMessage = ex.Message;
            }

            actualMessage.Should().Be(expectedFailureMessage);
        }

        [TestMethod]
        public void EqualWithLineByLineDiff()
        {
            string s1 = """
                    abc
                    def
                    ghi
                    jkl
                    mno
                    """;
            string s2 = """
                    abc
                    def
                    jkl
                    mnop
                    """;
            string expectedMessage = """
                    Expected strings to be equal because I said so, but they are not.
                    ===== DIFF (--actual, ++expected) =====
                    "[3] ++ ghi
                    [] -- mnop
                    [5] ++ mno"
                    ===== ACTUAL (length 19) =====
                    "abc
                    def
                    ghi
                    jkl
                    mno"
                    ===== EXPECTED (length 16) =====
                    "abc
                    def
                    jkl
                    mnop"
                    ===== END =====
                    """;

            string? actualMessage;
            try
            {
                s1.Should().EqualWithLineByLineDiff(s2, "because I said so");
                actualMessage = null;
            }
            catch (Exception ex)
            {
                actualMessage = ex.Message;
            }

            actualMessage.Should().Be(expectedMessage);

        }
    }
}
