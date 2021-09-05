// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Modules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Modules
{
    [TestClass]
    public class TemplateSpecModuleReferenceTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetEqualData), DynamicDataSourceType.Method)]
        public void Equals_SameReferences_ReturnsTrue(TemplateSpecModuleReference first, TemplateSpecModuleReference second) =>
            first.Equals(second).Should().BeTrue();

        [DataTestMethod]
        [DynamicData(nameof(GetNotEqualData), DynamicDataSourceType.Method)]
        public void Equals_DifferentReferences_ReturnsFalse(TemplateSpecModuleReference first, TemplateSpecModuleReference second) =>
            first.Equals(second).Should().BeFalse();

        [DataTestMethod]
        [DynamicData(nameof(GetEqualData), DynamicDataSourceType.Method)]
        public void GetHashCode_SameReferences_ReturnsEqualHashCode(TemplateSpecModuleReference first, TemplateSpecModuleReference second) =>
            first.GetHashCode().Should().Be(second.GetHashCode());

        [DataTestMethod]
        [DynamicData(nameof(GetNotEqualData), DynamicDataSourceType.Method)]
        public void GetHashCode_DifferentReferences_ReturnsEqualHashCode(TemplateSpecModuleReference first, TemplateSpecModuleReference second) =>
            first.GetHashCode().Should().NotBe(second.GetHashCode());

        [DataRow("D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG/myTemplateSpec1:v123")]
        [DataRow("5AA8419E-AFEB-45F2-9078-ED2167AAF51C/test-rg/deploy:1.0.0")]
        [DataRow("api-dogfood.resources.windows-int.net/D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG/myTemplateSpec1:v1")]
        [DataTestMethod]
        public void TryParse_ValidReference_ReturnsParsedReference(string value)
        {
            var reference = Parse(value);

            reference.UnqualifiedReference.Should().Be(value);
        }

        [DataRow("")]
        [DataRow("something/D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG./myTemplateSpec1:v1")]
        [DataRow("test.net/foobar/myRG/myTemplateSpec1:v1")]
        [DataRow("D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG./myTemplateSpec1:v1")]
        [DataRow("D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG/myTemplateSpec1:v1.")]
        [DataRow("D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG/myTemplateSpec1.:v1")]
        [DataRow("D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG/myTemplateSpec1:")]
        [DataRow("Test-RG/:mainVersion")]
        [DataRow("Test-RG/ts1:v1.")]
        [DataRow("Test-RG/.:v2")]
        [DataRow(":v100")]
        [DataTestMethod]
        public void TryParse_InvalidReference_ReturnsNullAndSetsFailureBuilder(string rawValue)
        {
            var parsed = TemplateSpecModuleReference.TryParse(rawValue, out var failureBuilder);

            parsed.Should().BeNull();
            failureBuilder.Should().NotBeNull();
        }

        public static IEnumerable<object[]> GetEqualData()
        {
            yield return new object[]
            {
                Parse("D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG/myTemplateSpec1:v123"),
                Parse("d9eec7db-8454-4ec1-8cd3-bb79d4cfebee/myrg/mytemplatespec1:v123"),
            };

            yield return new object[]
            {
                Parse("management.azure.com/010fb899-145c-44c0-97b8-83b2cb9202c5/rg1/ts1:v1"),
                Parse("management.AZURE.com/010FB899-145C-44C0-97B8-83B2CB9202C5/RG1/TS1:V1"),
            };

            yield return new object[]
            {
                Parse("api-dogfood.resources.windows-int.net/0243AB58-4881-4E71-A418-30C050B6F1C0/test-rg/spec1:v2"),
                Parse("api-dogfood.resources.windows-int.net/0243AB58-4881-4E71-A418-30C050B6F1C0/test-rg/SPEC1:V2"),
            };
        }

        public static IEnumerable<object[]> GetNotEqualData()
        {
            yield return new object[]
            {
                Parse("D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG1/myTemplateSpec1:v123"),
                Parse("D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG2/myTemplateSpec2:v123"),
            };

            yield return new object[]
            {
                Parse("115eaf54-2a8e-494c-accc-1ebdbf4d3c14/test-rg/test-ts:mainVersion"),
                Parse("115eaf54-2a8e-494c-accc-1ebdbf4d3c14/prod-RG/prod-ts:mainVersion")
            };

            yield return new object[]
            {
                Parse("api-dogfood.resources.windows-int.net/0243AB58-4881-4E71-A418-30C050B6F1C0/test-rg/spec1:v2"),
                Parse("management.azure.com/0243AB58-4881-4E71-A418-30C050B6F1C0/prod-rg/spec1:v2"),
            };
        }

        private static TemplateSpecModuleReference Parse(string rawValue)
        {
            var parsed = TemplateSpecModuleReference.TryParse(rawValue, out var failureBuilder);

            parsed.Should().NotBeNull();
            failureBuilder.Should().BeNull();

            return parsed!;
        }
    }
}
