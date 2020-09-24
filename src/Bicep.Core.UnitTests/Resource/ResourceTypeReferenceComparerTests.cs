// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Resources;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Resource
{
    [TestClass]
    public class ResourceTypeReferenceComparerTests
    {
        [DataTestMethod]
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01", "Microsoft.Compute/virtualMachines@2019-06-01")] // same string
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01", "Microsoft.Compute/vIrTuAlMaChiNeS@2019-06-01")] // different type casing
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01-preview", "Microsoft.Compute/vIrTuAlMaChiNeS@2019-06-01-PREVIEW")] // different api version casing
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01", "Microsoft.COMPUTE/vIrTuAlMaChiNeS@2019-06-01")] // different provider name casing
        [DataRow("Microsoft.Compute/virtualMachines/extensions@2019-06-01", "Microsoft.COMPUTE/vIrTuAlMaChiNeS/eXtEnSIONs@2019-06-01")] // different child name casing
        public void ResourceTypeReferenceComparer_should_determine_equality_correctly_for_equal_types(string first, string second)
        {
            var firstReference = ResourceTypeReference.Parse(first);
            var secondReference = ResourceTypeReference.Parse(second);

            ResourceTypeReferenceComparer.Instance.Equals(firstReference, secondReference).Should().BeTrue($"'{firstReference.FormatName()}' and '{secondReference.FormatName()}' should be considered equal");
            var firstHashCode = ResourceTypeReferenceComparer.Instance.GetHashCode(firstReference);
            var secondHashCode = ResourceTypeReferenceComparer.Instance.GetHashCode(secondReference);
            firstHashCode.Should().Be(secondHashCode, $"calculated hash codes of '{firstReference.FormatName()}' and '{secondReference.FormatName()}' should be equal");
        }

        [DataTestMethod]
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01", "Microsoft.Compute/virtualMachines@2019-07-01")] // different api version
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01", "Microsoft.Compute/virtualMachineScaleSets@2019-06-01")] // different type
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01", "Microsoft.Copmute/virtualMachines@2019-06-01")] // different provider name
        [DataRow("Microsoft.Compute/virtualMachines@2019-06-01", "Microsoft.COMPUTE/vIrTuAlMaChiNeS/extensions@2019-06-01")] // different number of child types
        [DataRow("Microsoft.Compute/virtualMachines/extensions@2019-06-01", "Microsoft.COMPUTE/virtualMachines/etxensions@2019-06-01")] // different child type
        public void ResourceTypeReferenceComparer_should_determine_equality_correctly_for_inequal_types(string first, string second)
        {
            var firstReference = ResourceTypeReference.Parse(first);
            var secondReference = ResourceTypeReference.Parse(second);

            ResourceTypeReferenceComparer.Instance.Equals(firstReference, secondReference).Should().BeFalse($"'{firstReference.FormatName()}' and '{secondReference.FormatName()}' should not be considered equal");
        }
    }
}