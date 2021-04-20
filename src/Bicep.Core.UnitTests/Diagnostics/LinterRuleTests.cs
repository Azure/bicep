// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics
{
    [TestClass]
    public class LinterRuleTests
    {

        private void CompileAndTestForCode(string code, string text, int diagnosticCount)
        {
            var compilationResult = CompilationHelper.Compile(text);
            Assert.AreEqual(diagnosticCount, compilationResult.Diagnostics.Count(d => d.Code == code));
        }

        [DataRow(1, @"
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(0, @"
param password string
var sum = 1 + 3
output sub int = sum
")]
        [DataTestMethod]
        public void ParametersAreRequired_BCPL1000(int diagnosticCount, string text)
        {
            CompileAndTestForCode(new BCPL1000().Code, text, diagnosticCount);
        }


        [DataRow(1, @"
param password string
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(3, @"
param param1 string
param param2 string
param param3 string
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(2, @"
param param1 string
param param2 int = 4
param param3 string
var sum = 1 + 3
output sub int = sum + param2
")]
        [DataRow(0, @"
param param2 int = 4
var sum = 1 + 3
output sub int = sum + param2
")]
        [DataRow(0, @"
var sum = 1 + 3
output sub int = sum
")]
        [DataTestMethod]
        public void ParametersAreReferenced_BCPL1010(int diagnosticCount, string text)
        {
            CompileAndTestForCode(new BCPL1010().Code, text, diagnosticCount);
        }

    }
}
