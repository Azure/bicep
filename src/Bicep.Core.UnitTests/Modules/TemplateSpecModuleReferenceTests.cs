// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.UnitTests.Assertions;

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
        [DataRow("D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG/myTemplateSpec1:v1")]
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
        public void TryParse_InvalidReference_ReturnsFalseAndSetsFailureBuilder(string rawValue)
        {
            TemplateSpecModuleReference.TryParse(null, rawValue, BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled, RandomFileUri(), out var parsed, out var failureBuilder).Should().BeFalse();

            parsed.Should().BeNull();
            failureBuilder!.Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow("prodRG", "mySpec:v1", null, "BCP212", "The Template Spec module alias name \"prodRG\" does not exist in the built-in Bicep configuration.")]
        [DataRow("testRG", "myModule:v2", "bicepconfig.json", "BCP212", "The Template Spec module alias name \"testRG\" does not exist in the Bicep configuration \"bicepconfig.json\".")]
        public void TryParse_AliasNotInConfiguration_ReturnsFalseAndSetsError(string aliasName, string referenceValue, string? configurationPath, string expectedCode, string expectedMessage)
        {
            var configuration = BicepTestConstants.CreateMockConfiguration(configurationPath: configurationPath);

            TemplateSpecModuleReference.TryParse(aliasName, referenceValue, configuration, RandomFileUri(), out var reference, out var errorBuilder).Should().BeFalse();

            reference.Should().BeNull();
            errorBuilder!.Should().NotBeNull();
            errorBuilder!.Should().HaveCode(expectedCode);
            errorBuilder!.Should().HaveMessage(expectedMessage);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("****")]
        [DataRow("/")]
        [DataRow(":")]
        [DataRow("foo bar ÄÄÄ")]
        public void TryParse_InvalidAliasName_ReturnsFalseAndSetsErrorDiagnostic(string aliasName)
        {
            TemplateSpecModuleReference.TryParse(aliasName, "", BicepTestConstants.BuiltInConfiguration, RandomFileUri(), out var reference, out var errorBuilder).Should().BeFalse();

            reference.Should().BeNull();
            errorBuilder!.Should().HaveCode("BCP211");
            errorBuilder!.Should().HaveMessage($"The module alias name \"{aliasName}\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidData), DynamicDataSourceType.Method)]
        public void TryParse_InvalidAlias_ReturnsFalseAndSetsError(string aliasName, string referenceValue, RootConfiguration configuration, string expectedCode, string expectedMessage)
        {
            TemplateSpecModuleReference.TryParse(aliasName, referenceValue, configuration, RandomFileUri(), out var reference, out var errorBuilder).Should().BeFalse();

            reference.Should().BeNull();
            errorBuilder!.Should().NotBeNull();
            errorBuilder!.Should().HaveCode(expectedCode);
            errorBuilder!.Should().HaveMessage(expectedMessage);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidData), DynamicDataSourceType.Method)]
        public void TryGetModuleReference_ValidAlias_ReplacesReferenceValue(string aliasName, string referenceValue, string fullyQualifiedReferenceValue, RootConfiguration configuration)
        {
            TemplateSpecModuleReference.TryParse(aliasName, referenceValue, configuration, RandomFileUri(), out var reference, out var errorBuilder).Should().BeTrue();

            reference.Should().NotBeNull();
            reference!.FullyQualifiedReference.Should().Be(fullyQualifiedReferenceValue);
        }

        private static IEnumerable<object[]> GetEqualData()
        {
            yield return new object[]
            {
                Parse("D9EEC7DB-8454-4EC1-8CD3-BB79D4CFEBEE/myRG/myTemplateSpec1:v123"),
                Parse("d9eec7db-8454-4ec1-8cd3-bb79d4cfebee/myrg/mytemplatespec1:v123"),
            };

            yield return new object[]
            {
                Parse("010fb899-145c-44c0-97b8-83b2cb9202c5/rg1/ts1:v1"),
                Parse("010FB899-145C-44C0-97B8-83B2CB9202C5/RG1/TS1:V1"),
            };

            yield return new object[]
            {
                Parse("0243AB58-4881-4E71-A418-30C050B6F1C0/test-rg/spec1:v2"),
                Parse("0243AB58-4881-4E71-A418-30C050B6F1C0/test-rg/SPEC1:V2"),
            };
        }

        private static IEnumerable<object[]> GetNotEqualData()
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
                Parse("0243AB58-4881-4E71-A418-30C050B6F1C0/test-rg/spec1:v2"),
                Parse("0243AB58-4881-4E71-A418-30C050B6F1C0/prod-rg/spec1:v2"),
            };
        }

        private static IEnumerable<object[]> GetInvalidData()
        {
            yield return new object[]
            {
                "testRG",
                "mySpec:v1",
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["moduleAliases.ts.testRG.resourceGroup"] = "production-resource-group",
                }),
                "BCP214",
                "The Template Spec module alias \"testRG\" in the built-in Bicep configuration is in valid. The \"subscription\" property cannot be null or undefined.",
            };

            yield return new object[]
            {
                "prodRG",
                "mySpec:v1",
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["moduleAliases.ts.prodRG.subscription"] = "1E7593D0-FCD1-4570-B132-51E4FD254967",
                }, "bicepconfig.json"),
                "BCP215",
                "The Template Spec module alias \"prodRG\" in the Bicep configuration \"bicepconfig.json\" is in valid. The \"resourceGroup\" property cannot be null or undefined.",
            };
        }

        private static IEnumerable<object[]> GetValidData()
        {
            yield return new object[]
            {
                "prodRG",
                "mySpec:v1",
                "ts:1E7593D0-FCD1-4570-B132-51E4FD254967/production-resource-group/mySpec:v1",
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["moduleAliases.ts.prodRG.subscription"] = "1E7593D0-FCD1-4570-B132-51E4FD254967",
                    ["moduleAliases.ts.prodRG.resourceGroup"] = "production-resource-group",
                }),
            };

            yield return new object[]
            {
                "testRG",
                "mySpec:v2",
                "ts:1E7593D0-FCD1-4570-B132-51E4FD254967/test-resource-group/mySpec:v2",
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["moduleAliases.ts.testRG.subscription"] = "1E7593D0-FCD1-4570-B132-51E4FD254967",
                    ["moduleAliases.ts.testRG.resourceGroup"] = "test-resource-group",
                }),
            };
        }

        private static TemplateSpecModuleReference Parse(string rawValue)
        {
            TemplateSpecModuleReference.TryParse(null, rawValue, BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled, RandomFileUri(), out var parsed, out var failureBuilder).Should().BeTrue();

            parsed.Should().NotBeNull();
            failureBuilder!.Should().BeNull();

            return parsed!;
        }

        private static Uri RandomFileUri() => PathHelper.FilePathToFileUrl(Path.GetTempFileName());
    }
}
