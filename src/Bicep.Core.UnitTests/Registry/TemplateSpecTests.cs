// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Deployments.Core.Uri;
using Azure.ResourceManager.Resources.Models;
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
        public void FromGenericResource_InvalidTemplateSpecResource_ThrowsTemplateSpecException(GenericResource resource)
        {
            Invoking(() => TemplateSpec.FromGenericResource(resource))
                .Should()
                .Throw<TemplateSpecException>()
                .WithMessage("The referenced Template Spec is malformed.");
        }

        [TestMethod]
        public void FromGenericResource_ValidTemplateSpecResource_ReturnsTemplateSpec()
        {
            var resource = new GenericResource
            {
                Properties = new Dictionary<string, string>
                {
                    ["mainTemplate"] = "contents"
                },
            };

            var templateSpec = TemplateSpec.FromGenericResource(resource);

            templateSpec.MainTemplateContents.Should().Be("contents");
        }

        public static IEnumerable<object[]> GetInvalidData()
        {
            yield return new object[]
            {
                new GenericResource
                {
                    Properties = 123,
                },
            };

            yield return new object[]
            {
                new GenericResource
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
