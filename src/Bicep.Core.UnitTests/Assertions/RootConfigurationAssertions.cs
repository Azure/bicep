// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class RootConfigurationExtensions
    {
        public static RootConfigurationAssertions Should(this RootConfiguration configuration) => new(configuration);
    }

    public class RootConfigurationAssertions(RootConfiguration configuration) : ReferenceTypeAssertions<RootConfiguration, RootConfigurationAssertions>(configuration)
    {
        protected override string Identifier => "RootConfiguration";

        public AndConstraint<RootConfigurationAssertions> HaveContents(string contents, string because = "", params object[] becauseArgs)
        {
            var actual = Subject.ToUtf8Json().ReplaceLineEndings();
            var expected = contents.ReplaceLineEndings();
            actual.Should().Be(expected, because, becauseArgs);
            return new AndConstraint<RootConfigurationAssertions>(this);
        }
    }
}
