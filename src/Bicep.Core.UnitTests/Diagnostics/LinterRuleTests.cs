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

        [DataRow(0, @"
param password string
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(0, @"
param param1 string
var location = 'somehost.com'
output sub int = sum
")]
        [DataRow(1, @"
param param1 string
var location = 'management.core.windows.net'
output sub int = sum
")]
        [DataTestMethod]
        public void DisallowedHostsDetected_BCPL1020(int diagnosticCount, string text)
        {
            CompileAndTestForCode(new BCPL1020().Code, text, diagnosticCount);
        }

        [DataRow(0, @"
@secure()
param password string
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(1, @"
@secure()
param param1 string = 'val1'
output sub int = sum
")]
        [DataRow(1, @"
@secure()
param param1 string
@secure()
param param2 string = 'val'
param param3 int
output sub int = sum
")]
        [DataRow(2, @"
@secure()
param param1 string = 'val'
@secure()
param param2 string = 'val'
param param3 int
output sub int = sum
")]
        [DataRow(2, @"
@secure()
param param1 string = 'val'
@secure()
param param2 string = 'val'
@secure()
param param3 int
output sub int = sum
")]
        [DataRow(3, @"
@secure()
param param1 string = 'val'
@secure()
param param2 string = 'val'
@secure()
param param3 int = 5
output sub int = sum
")]
        [DataTestMethod]
        public void SecureParametersNoDefaults_BCPL1030(int diagnosticCount, string text)
        {
            CompileAndTestForCode(new BCPL1020().Code, text, diagnosticCount);
        }


    }

}
