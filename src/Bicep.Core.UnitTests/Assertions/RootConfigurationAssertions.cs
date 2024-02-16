// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
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
            var actual = Subject.ToUtf8Json().ReplaceLineEndings();
            var expected = contents.ReplaceLineEndings();
            JToken.Parse(expected).Should().DeepEqual(JToken.Parse(actual));
            return new AndConstraint<RootConfigurationAssertions>(this);
        }
    }
}
