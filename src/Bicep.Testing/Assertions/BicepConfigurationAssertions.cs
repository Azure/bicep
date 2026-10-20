// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using FluentAssertions;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;

namespace Bicep.Testing.Assertions
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
            var actualToken = JToken.Parse(actual);
            var expectedToken = JToken.Parse(expected);
            JToken.DeepEquals(actualToken, expectedToken).Should().BeTrue(
                string.IsNullOrEmpty(because)
                    ? $"configurations should match.\n\nExpected:\n{expectedToken}\n\nActual:\n{actualToken}"
                    : because,
                becauseArgs);
            return new AndConstraint<BicepConfigurationAssertions>(this);
        }
    }
}
