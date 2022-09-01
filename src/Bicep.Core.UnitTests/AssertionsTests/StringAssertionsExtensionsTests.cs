// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

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
    }
}
