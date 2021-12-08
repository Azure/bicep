// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Workspaces;
using FluentAssertions;
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
            Subject.ProgramSyntax.ToTextPreserveFormatting().Should().EqualIgnoringNewlines(expected, because, becauseArgs);

            return new AndConstraint<BicepFileAssertions>(this);
        }
    }
}
