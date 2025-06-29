// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.SourceGraph;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class BicepFileExtensions
    {
        public static BicepFileAssertions Should(this BicepFile bicepFile)
        {
            return new BicepFileAssertions(bicepFile);
        }
    }

    public class BicepFileAssertions : ReferenceTypeAssertions<BicepFile, BicepFileAssertions>
    {
        public BicepFileAssertions(BicepFile bicepFile)
            : base(bicepFile)
        {
        }

        protected override string Identifier => "File";

        public AndConstraint<BicepFileAssertions> HaveSourceText(string expected, string because = "", params object[] becauseArgs)
        {
            var actual = Subject.ProgramSyntax.ToString().NormalizeNewlines();
            expected = expected.NormalizeNewlines();

            actual.Should().EqualWithLineByLineDiff(expected, because, becauseArgs);

            return new AndConstraint<BicepFileAssertions>(this);
        }

        public AndConstraint<BicepFileAssertions> HaveEquivalentSourceText(BicepFile other, string because = "", params object[] becauseArgs)
        {
            var expectedText = Subject.ProgramSyntax.ToString();
            var actualText = other.ProgramSyntax.ToString();

            expectedText.Should().EqualIgnoringNewlines(actualText, because, becauseArgs);

            return new AndConstraint<BicepFileAssertions>(this);
        }

        public AndConstraint<BicepFileAssertions> NotHaveParseErrors(string because = "", params object[] becauseArgs)
        {
            Subject.ParsingErrorLookup.Should().NotContain(d => d.IsError(), because, becauseArgs);

            return new AndConstraint<BicepFileAssertions>(this);
        }
    }
}
