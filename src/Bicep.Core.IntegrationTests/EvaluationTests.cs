// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class EvaluationTests
    {
        [TestMethod]
        public void Basic_arithmetic_expressions_are_evaluated_successfully()
        {
            var (template, _, _) = CompilationHelper.Compile(@"
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

            using (new AssertionScope())
            {
                var evaluated = TemplateEvaluator.Evaluate(template);

                evaluated.Should().HaveValueAtPath("$.outputs['sum'].value", 4);
                evaluated.Should().HaveValueAtPath("$.outputs['mult'].value", 20);
                evaluated.Should().HaveValueAtPath("$.outputs['modulo'].value", 6);
                evaluated.Should().HaveValueAtPath("$.outputs['div'].value", 0);
                evaluated.Should().HaveValueAtPath("$.outputs['sub'].value", -13);         
            }
        }
    }
}
