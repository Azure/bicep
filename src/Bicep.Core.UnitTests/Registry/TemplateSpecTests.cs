// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Azure.ResourceManager.Resources;
using Bicep.Core.Registry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FluentAssertions.FluentActions;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class TemplateSpecTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetInvalidData), DynamicDataSourceType.Method)]
        public void FromGenericResource_InvalidTemplateSpecResource_ThrowsTemplateSpecException(GenericResourceData data)
        {
            Invoking(() => TemplateSpec.FromGenericResourceData(data))
                .Should()
                .Throw<TemplateSpecException>()
                .WithMessage("The referenced Template Spec is malformed.");
        }

        [TestMethod]
        public void FromGenericResource_ValidTemplateSpecResource_ReturnsTemplateSpec()
        {
            var data = new GenericResourceData("westus")
            {
                Properties = new Dictionary<string, string>
                {
                    ["mainTemplate"] = "contents"
                },
            };

            var templateSpec = TemplateSpec.FromGenericResourceData(data);

            templateSpec.MainTemplateContents.Should().Be("contents");
        }

        public static IEnumerable<object[]> GetInvalidData()
        {
            yield return new object[]
            {
                new GenericResourceData("westus")
                {
                    Properties = 123,
                },
            };

            yield return new object[]
            {
                new GenericResourceData("westus")
                {
                    Properties = new Dictionary<string, string>
                    {
                        ["foo"] = "bar"
                    },
                },
            };
        }
    }
}
