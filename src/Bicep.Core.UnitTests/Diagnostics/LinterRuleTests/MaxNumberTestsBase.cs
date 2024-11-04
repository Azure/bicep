// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Extensions;
using CommandLine;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    public class MaxNumberTestsBase : LinterRuleTestsBase
    {
        protected void CompileAndTest(string text, string expectedCode, DiagnosticLevel expectedLevel, string[] expectedMessages, Options? options = null)
        {
            AssertLinterRuleDiagnostics(expectedCode, text, diags => diags.Should().HaveDiagnostics(expectedMessages.Select(msg => (expectedCode, expectedLevel, msg)).ToArray()), options);
        }

        // e.g.: GenerateText(1, 256, "var v% = %") => "var v1 = 1\nvar v2 = 2\n...\nvar v256 = 256"
        protected string GenerateText(int i, int j, string pattern)
        {
            var text = string.Join(
                "\n",
                Enumerable.Range(i, j - i + 1).Select(i => pattern.Replace("%", i.ToString())).ToArray()
            );

            return text;
        }
    }
}
