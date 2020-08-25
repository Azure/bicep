// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Resources;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Resource
{
    [TestClass]
    public class ResourceTypeReferenceTests
    {
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("wrong")]
        [DataRow("Microsoft.Compute")]
        [DataRow("Microsoft.Compute/")]
        [DataRow("Microsoft.Compute/virtualMachines")]
        [DataRow("Microsoft.Compute/virtualMachines/")]
        [DataRow("Microsoft.Compute/virtualMachines@")]
        [DataRow("Microsoft.Compute/virtualMachines@foo")]
        [DataRow("Microsoft.Compute/virtualMachines@2020")]
        [DataRow("Microsoft.Compute/virtualMachines@2020-01")]
        [DataRow("Microsoft.Compute/virtualMachines@2020-02-0")]
        [DataRow("Microsoft.Compute/virtualMachines@2020-02-02-fake")]
        public void InvalidType_ShouldBeRejected(string value)
        {
            ResourceTypeReference.TryParse(value).Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01", "2019-06-01", "Microsoft.Compute", "virtualMachines")]
        [DataRow("Microsoft.Compute/virtualMachines/networkInterfaces@2019-06-01-alpha", "2019-06-01-alpha", "Microsoft.Compute", "virtualMachines", "networkInterfaces")]
        [DataRow("Microsoft.Blueprint/blueprints/versions/artifacts@2018-11-01-preview", "2018-11-01-preview", "Microsoft.Blueprint", "blueprints", "versions", "artifacts")]
        public void ValidType_ShouldReturnExpectedValue(string value, string expectedVersion, string expectedNamespace, params string[] expectedTypes)
        {
            // local function
            void AssertExpectations(ResourceTypeReference? typeRef)
            {
                typeRef.Should().NotBeNull();

                typeRef!.ApiVersion.Should().Be(expectedVersion);
                typeRef.Namespace.Should().Be(expectedNamespace);
                typeRef.Types.Should().Equal(expectedTypes);
            }

            var actual = ResourceTypeReference.TryParse(value);
            AssertExpectations(actual);
        }

        [DataTestMethod]
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01", "Microsoft.Compute/virtualMachines")]
        [DataRow("Microsoft.Blueprint/blueprints/versions/artifacts@2018-11-01-preview", "Microsoft.Blueprint/blueprints/versions/artifacts")]
        public void ValidType_FullyQualitifedTypeShouldBeCorrect(string value, string expectedFullyQualifiedType)
        {
            var actual = ResourceTypeReference.TryParse(value);

            actual.Should().NotBeNull();
            actual!.FullyQualifiedType.Should().Be(expectedFullyQualifiedType);
        }
    }
}

