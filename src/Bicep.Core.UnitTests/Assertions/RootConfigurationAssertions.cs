// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Parsing;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class RootConfigurationExtensions
    {
        public static RootConfigurationAssertions Should(this RootConfiguration configuration) => new(configuration);
    }

    public class RootConfigurationAssertions : ReferenceTypeAssertions<RootConfiguration, RootConfigurationAssertions>
    {
        public RootConfigurationAssertions(RootConfiguration configuration)
            : base(configuration)
        {
        }

        protected override string Identifier => "RootConfiguration";

        public AndConstraint<RootConfigurationAssertions> HaveContents(string contents, string because = "", params object[] becauseArgs)
        {
            string actual = StringUtils.ReplaceNewlines(Subject.ToUtf8Json(), "\n");
            string expected = StringUtils.ReplaceNewlines(contents, "\n");

            actual.Should().Be(expected, because, becauseArgs);

            return new AndConstraint<RootConfigurationAssertions>(this);
        }
    }
}
