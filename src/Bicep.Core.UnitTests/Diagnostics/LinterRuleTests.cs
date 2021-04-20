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

        [TestMethod]
        public void ParametersAreRequired_BCPL1000()
        {
            var compilationResult = CompilationHelper.Compile(@"
var sum = 1 + 3
var mult = sum * 5
var modulo = mult % 7
var div = modulo / 11
var sub = div - 13

output sum int = sum
output mult int = mult
output modulo int = modulo
output div int = div
output sub int = sub
");

            Assert.IsTrue(compilationResult.Diagnostics.Any(d => d.Code == new BCPL1000().Code));
        }
    }
}
