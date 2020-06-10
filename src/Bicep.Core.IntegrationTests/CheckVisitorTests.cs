using System.Collections.Generic;
using Bicep.Core.IntegrationTests.UnitSamples;
using Bicep.Core.IntegrationTests.Utils;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.Visitors;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class CheckVisitorTests
    {
        [DataTestMethod]
        [UnitSamplesDataSource(includeInvalid: false)]
        public void ValidProgram_ShouldProduceNoErrors(string displayName, string contents)
        {
            GetErrors(contents).Should().BeEmpty();
        }

        [DataTestMethod]
        [UnitSamplesDataSource(includeValid: false)]
        public void InvalidProgram_ShouldProduceErrors(string displayName, string contents)
        {
            var errors = GetErrors(contents);
            errors.Should().NotBeEmpty();
        }

        private static List<Error> GetErrors(string contents)
        {
            var program = ParserHelper.Parse(contents);
            program.Should().BeOfType<ProgramSyntax>();

            var errors = new List<Error>();
            var typeChecker = new CheckVisitor(errors);

            typeChecker.Visit(program);

            return errors;
        }
    }
}
