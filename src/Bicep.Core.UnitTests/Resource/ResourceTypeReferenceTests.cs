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
        public void ValidType_FullyQualifiedTypeShouldBeCorrect(string value, string expectedFullyQualifiedType)
        {
            var actual = ResourceTypeReference.TryParse(value);

            actual.Should().NotBeNull();
            actual!.FullyQualifiedType.Should().Be(expectedFullyQualifiedType);
        }

        [DataTestMethod]
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01")] // has a slash
        [DataRow("Microsoft.Blueprint/blueprints/versions/artifacts@2018-11-01-preview")] // full type name
        [DataRow("/artifacts@2018-11-01-preview")] // leading slash
        [DataRow("artifacts@")] // version delimiter but no version
        [DataRow("artifacts@2018-11-012222-preview")] // invalid version
        public void TryParseSingleTypeSegment_InvalidTypeSegmentIsRejected(string value)
        {
            var success = ResourceTypeReference.TryParseSingleTypeSegment(value, out var type, out var version);
            success.Should().BeFalse($"For input '{value}': type was '{type}', version was '{version}'");
            type.Should().BeNull();
            version.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("virtualMachines", "virtualMachines", (string?)null)]
        [DataRow("virtualMachines@2019-06-01", "virtualMachines", "2019-06-01")]
        [DataRow("artifacts@2018-11-01-preview", "artifacts", "2018-11-01-preview")]
        public void TryParseSingleTypeSegment_TypeSegmentIsParsed(string value, string expectedType, string expectedVersion)
        {
            var success = ResourceTypeReference.TryParseSingleTypeSegment(value, out var type, out var version);
            success.Should().BeTrue($"For input '{value}': type was '{type}', version was '{version}'");
            type.Should().BeEquivalentTo(expectedType);
            version.Should().BeEquivalentTo(expectedVersion);
        }

        [TestMethod]
        public void TryCombine_RejectsInvalidTypeSegment()
        {
            var baseType = ResourceTypeReference.Parse("My.RP/someType@2020-01-01");
            var typeSegments = new [] { "childType@2019-06-01", "childType/grandChildType", };
            var actual = ResourceTypeReference.TryCombine(baseType, typeSegments);

            actual.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("My.RP/someType@2020-01-01", new string[]{ "childType", }, "My.RP/someType/childType@2020-01-01")]
        [DataRow("My.RP/someType@2020-01-01", new string[]{ "childType", "grandchildType",}, "My.RP/someType/childType/grandchildType@2020-01-01")]
        [DataRow("My.RP/someType@2020-01-01", new string[]{ "childType", "grandchildType", "greatGrandchildType"}, "My.RP/someType/childType/grandchildType/greatGrandchildType@2020-01-01")]
        [DataRow("My.RP/someType@2020-01-01", new string[]{ "childType@2020-01-02", }, "My.RP/someType/childType@2020-01-02")]
        [DataRow("My.RP/someType@2020-01-01", new string[]{ "childType", "grandchildType@2020-01-03", }, "My.RP/someType/childType/grandchildType@2020-01-03")]
        [DataRow("My.RP/someType@2020-01-01", new string[]{ "childType@2020-01-02", "grandchildType", }, "My.RP/someType/childType/grandchildType@2020-01-02")]
        public void TryCombine_CombinesValidTypeSegments(string baseTypeText, string[] typeSegments, string expected)
        {
            var baseType = ResourceTypeReference.Parse(baseTypeText);
            var actual = ResourceTypeReference.TryCombine(baseType, typeSegments);

            actual.Should().NotBeNull();
            actual!.FormatName().Should().BeEquivalentTo(expected);
        }
    }
}

