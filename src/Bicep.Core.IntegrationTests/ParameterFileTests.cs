// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ParameterFileTests
    {
        [TestMethod]
        public void Parameters_file_cannot_reference_non_bicep_files()
        {
            var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using './foo.bicepparam'
"),
("foo.bicepparam", ""));

            result.Should().OnlyContainDiagnostic("BCP276", DiagnosticLevel.Error, "A using declaration can only reference a Bicep file.");
        }

        [TestMethod]
        public void Parameters_file_cannot_self_reference()
        {
            var result = CompilationHelper.CompileParams(("parameters.bicepparam", @"
using './parameters.bicepparam'
"));

            result.Should().OnlyContainDiagnostic("BCP278", DiagnosticLevel.Error, "This parameters file references itself, which is not allowed.");
        }

        [TestMethod]
        public void Parameters_file_cycles_should_be_detected()
        {
            var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using './one.bicepparam'
"),
("one.bicepparam", @"
using './two.bicepparam'
"),
("two.bicepparam", @"
using './one.bicepparam'
"));

            result.Diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Code.Should().Be("BCP095");
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Message.Should().StartWith("The file is involved in a cycle (\"");
                    x.Message.Should().EndWith("\").");
                    x.Message.Should().ContainAll("one.bicepparam\" -> \"", "two.bicepparam\").");
                });
        }
    }
}
