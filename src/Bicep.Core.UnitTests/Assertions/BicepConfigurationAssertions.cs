// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using FluentAssertions;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class BicepConfigurationAssertionsExtensions
    {
        public static BicepConfigurationAssertions Should(this IBicepConfiguration configuration) => new(configuration);
    }

    public class BicepConfigurationAssertions : ReferenceTypeAssertions<IBicepConfiguration, BicepConfigurationAssertions>
    {
        public BicepConfigurationAssertions(IBicepConfiguration configuration)
            : base(configuration)
        {
        }

        protected override string Identifier => "BicepConfiguration";

        public AndConstraint<BicepConfigurationAssertions> HaveContents(string contents, string because = "", params object[] becauseArgs)
        {
            var actual = Subject.ToUtf8Json().ReplaceLineEndings();
            var expected = contents.ReplaceLineEndings();
            JToken.Parse(expected).Should().DeepEqual(JToken.Parse(actual));
            return new AndConstraint<BicepConfigurationAssertions>(this);
        }
    }
}
