// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Resource
{
    [TestClass]
    public class ResourceTypeReferenceTests
    {
        [DataTestMethod]
        [DataRow("")]
        [DataRow("+-")]
        [DataRow("-/abc")]
        [DataRow("abc/-")]
        [DataRow("@")]
        [DataRow("@2020-01-01")]
        [DataRow("-@2020-01-01")]
        [DataRow("abc@+")]
        public void InvalidType_ShouldBeRejected(string value)
        {
            ResourceTypeReference.TryParse(value).Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01", "2019-06-01", "Microsoft.Compute", "virtualMachines")]
        [DataRow("Microsoft.Compute/virtualMachines/networkInterfaces@2019-06-01-alpha", "2019-06-01-alpha", "Microsoft.Compute", "virtualMachines", "networkInterfaces")]
        [DataRow("Microsoft.Blueprint/blueprints/versions/artifacts@2018-11-01-preview", "2018-11-01-preview", "Microsoft.Blueprint", "blueprints", "versions", "artifacts")]
        public void ValidType_ShouldReturnExpectedValue(string value, string expectedVersion, params string[] expectedTypes)
        {
            // local function
            void AssertExpectations(ResourceTypeReference? typeRef)
            {
                typeRef.Should().NotBeNull();

                typeRef!.ApiVersion.Should().Be(expectedVersion);
                typeRef.TypeSegments.Should().Equal(expectedTypes);
            }

            var actual = ResourceTypeReference.TryParse(value);
            AssertExpectations(actual);
        }

        [DataTestMethod]
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01", "Microsoft.Compute/virtualMachines")]
        [DataRow("Microsoft.Blueprint/blueprints/versions/artifacts@2018-11-01-preview", "Microsoft.Blueprint/blueprints/versions/artifacts")]
        public void ValidType_FullyQualifiedTypeShouldBeCorrect(string value, string expectedFullyQualifiedType)
        {
            var actual = ResourceTypeReference.TryParse(value);

            actual.Should().NotBeNull();
            actual!.FormatType().Should().Be(expectedFullyQualifiedType);
        }

        [DataTestMethod]
        [DataRow("virtualMachines", "virtualMachines", (string?)null)]
        [DataRow("virtualMachines@2019-06-01", "virtualMachines", "2019-06-01")]
        [DataRow("artifacts@2018-11-01-preview", "artifacts", "2018-11-01-preview")]
        public void Parse_permits_types_with_single_type_segment_and_optional_version(string value, string expectedType, string? expectedVersion)
        {
            var result = ResourceTypeReference.Parse(value);
            result.FormatType().Should().BeEquivalentTo(expectedType);
            result.ApiVersion.Should().BeEquivalentTo(expectedVersion);
        }

        [DataTestMethod]
        [DataRow("My.RP/someType@2020-01-01", "childType", "My.RP/someType/childType@2020-01-01")]
        [DataRow("My.RP/someType@2020-01-01", "childType/grandchildType", "My.RP/someType/childType/grandchildType@2020-01-01")]
        [DataRow("My.RP/someType@2020-01-01", "childType/grandchildType/greatGrandchildType", "My.RP/someType/childType/grandchildType/greatGrandchildType@2020-01-01")]
        [DataRow("My.RP/someType@2020-01-01", "childType@2020-01-02", "My.RP/someType/childType@2020-01-02")]
        [DataRow("My.RP/someType@2020-01-01", "childType/grandchildType@2020-01-03", "My.RP/someType/childType/grandchildType@2020-01-03")]
        public void Combine_CombinesValidTypeSegments(string baseTypeText, string childTypeText, string expected)
        {
            var baseType = ResourceTypeReference.Parse(baseTypeText);
            var childType = ResourceTypeReference.Parse(childTypeText);
            var actual = ResourceTypeReference.Combine(baseType, childType);

            actual.Should().NotBeNull();
            actual!.FormatName().Should().BeEquivalentTo(expected);
        }
    }
}

