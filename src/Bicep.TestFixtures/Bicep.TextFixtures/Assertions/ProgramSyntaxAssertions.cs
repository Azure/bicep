// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Bicep.TextFixtures.Assertions
{
    public static class ProgramSyntaxAssertionsExtensions
    {
        public static ProgramSyntaxAssertions Should2(this ProgramSyntax subject)
        {
            return new ProgramSyntaxAssertions(subject);
        }
    }

    public class ProgramSyntaxAssertions : ReferenceTypeAssertions<ProgramSyntax, ProgramSyntaxAssertions>
    {
        public ProgramSyntaxAssertions(ProgramSyntax subject)
            : base(subject)
        {
        }

        protected override string Identifier => nameof(ProgramSyntax);

        public AndConstraint<ProgramSyntaxAssertions> NotHaveAnySyntaxErrors(string because = "", params object[] becauseArgs)
        {
            var programText = this.Subject.ToString();

            TestParser.Parse(programText, out var syntaxErrors);

            syntaxErrors.Should().BeEmpty(because, becauseArgs);

            return new(this);
        }
    }
}
