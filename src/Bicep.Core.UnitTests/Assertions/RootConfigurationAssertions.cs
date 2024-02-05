// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using FluentAssertions;
using FluentAssertions.Primitives;
using System.Text.Json.JsonDiffPatch.MsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var expected = contents.ReplaceLineEndings();
            var actual = Subject.ToUtf8Json().ReplaceLineEndings();

            Assert.That.JsonAreEqual(expected, actual, output: true);
            return new AndConstraint<RootConfigurationAssertions>(this);
        }
    }
}
