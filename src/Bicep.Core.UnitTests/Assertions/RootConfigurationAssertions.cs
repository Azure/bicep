// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Parsing;
using FluentAssertions;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;


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
            var actual = JToken.Parse(Subject.ToUtf8Json());
            var expected = JToken.Parse(contents);
            actual.Should().DeepEqual(expected, because, becauseArgs);
            return new AndConstraint<RootConfigurationAssertions>(this);
        }
    }
}
