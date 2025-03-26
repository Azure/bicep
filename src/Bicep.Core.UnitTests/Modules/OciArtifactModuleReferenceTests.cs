// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Data;
using System.Reflection;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Modules
{
    [TestClass]
    public class OciArtifactModuleReferenceTests
    {
        public const string ExampleTagOfMaxLength = "abcdefghijklmnopqrstuvxyz0123456789._-._-._-._-ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789._-._-._-._-abcdefghijklmnopqrstuvxyz012345678";

        public const string ExampleRepositoryOfMaxLength = "abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abc";

        public const string ExampleRegistryOfMaxLength = "abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abc";

        public const string ExamplePathSegment1 = "abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789-abcdefghijklmnopqrstuvxyz0123456789_abcdefghijklmnopqrstuvxyz0123456789";

        public const string ExamplePathSegment2 = "a.b-0_1";

        private static void VerifyEqual(OciArtifactReference first, OciArtifactReference second)
        {
            first.Equals(second).Should().BeTrue();
            second.Equals(first).Should().BeTrue();

            // It's technically possible for the hash codes to be equal, but it's unlikely.
            first.GetHashCode().Should().Be(second.GetHashCode());
            second.GetHashCode().Should().Be(first.GetHashCode());

            object secondAsObject = second;
            first.Equals(secondAsObject).Should().BeTrue();
            first.GetHashCode().Should().Be(secondAsObject.GetHashCode());
        }

        private static void VerifyNotEqual(OciArtifactReference first, OciArtifactReference second)
        {
            first.Equals(second).Should().BeFalse();
            second.Equals(first).Should().BeFalse();

            // It's technically possible for the hash codes to be equal, but it's unlikely.
            first.GetHashCode().Should().NotBe(second.GetHashCode());
            second.GetHashCode().Should().NotBe(first.GetHashCode());

            object secondAsObject = second;
            first.Equals(secondAsObject).Should().BeFalse();
            first.GetHashCode().Should().NotBe(secondAsObject.GetHashCode());
        }

        [DynamicData(nameof(ArtifactAddressComponentsTests.GetValidCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [DataTestMethod]
        public void ValidReferencesShouldParseCorrectly(ArtifactAddressComponentsTests.ValidCase @case)
        {
            var parsed = Parse(@case.Value);

            using (new AssertionScope())
            {
                parsed.Registry.Should().Be(@case.ExpectedRegistry);
                parsed.Repository.Should().Be(@case.ExpectedRepository);
                parsed.Tag.Should().Be(@case.ExpectedTag);
                parsed.Digest.Should().Be(@case.ExpectedDigest);
                parsed.ArtifactId.Should().Be(@case.Value);
                parsed.UnqualifiedReference.Should().Be(@case.Value);
            }
        }

        [DynamicData(nameof(GetValidCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [DataTestMethod]
        public void ValidReferenceShouldBeEqualToItself(ArtifactAddressComponentsTests.ValidCase @case)
        {
            OciArtifactReference first = Parse(@case.Value);
            OciArtifactReference second = Parse(@case.Value);
            VerifyEqual(first, second);
        }

        [DynamicData(nameof(GetValidCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [DataTestMethod]
        public void ValidReferenceShouldBeEqualWithCaseChanged(ArtifactAddressComponentsTests.ValidCase @case)
        {
            OciArtifactReference first = Parse(@case.Value);
            OciArtifactReference firstLower = Parse((@case with { ExpectedDigest = @case.Value.ToLower() }).Value);
            OciArtifactReference firstUpper = Parse((@case with { ExpectedDigest = @case.Value.ToUpper() }).Value);

            VerifyEqual(first, firstLower);
            VerifyEqual(first, firstUpper);
            VerifyEqual(firstLower, firstUpper);
        }

        [DynamicData(nameof(GetValidCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [DataTestMethod]
        public void CharacterChanged_ShouldNotBeEqual(ArtifactAddressComponentsTests.ValidCase @case)
        {
            string ModifyCharAt(string a, int index)
            {
                char newChar = a[index] == 'q' ? 'z' : 'q';
                return a.Substring(0, index) + newChar + a.Substring(index + 1);
            }

            for (int i = 0; i < @case.Value.Length - 1; ++i)
            {
                OciArtifactReference first = Parse(@case.Value);
                var modified = ModifyCharAt(@case.Value, i);
                if (IsValid(modified))
                {
                    OciArtifactReference second = Parse(modified);
                    VerifyNotEqual(first, second);
                }
            }
        }

        [DynamicData(nameof(GetValidCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [DataTestMethod]
        public void ValidReferenceShouldBeUriParseable(ArtifactAddressComponentsTests.ValidCase @case)
        {
            var parsed = Parse(@case.Value);

            // the go2def flow in the language server passes the reference through URIs
            // in cases of documents that come from the local module cache
            FluentActions.Invoking(() => new Uri(parsed.FullyQualifiedReference)).Should().NotThrow();

            // the unqualified reference should be parseable as a URI segment as well
            FluentActions.Invoking(() => new Uri("test://" + parsed.UnqualifiedReference)).Should().NotThrow();
        }

        // bad
        [DataRow("", "BCP193", "The specified OCI artifact reference \"br:\" is not valid. Specify a reference in the format of \"br:<artifact-uri>:<tag>\", or \"br/<module-alias>:<module-name-or-path>:<tag>\".")]
        [DataRow("a", "BCP193", "The specified OCI artifact reference \"br:a\" is not valid. Specify a reference in the format of \"br:<artifact-uri>:<tag>\", or \"br/<module-alias>:<module-name-or-path>:<tag>\".")]
        [DataRow("a/", "BCP193", "The specified OCI artifact reference \"br:a/\" is not valid. Specify a reference in the format of \"br:<artifact-uri>:<tag>\", or \"br/<module-alias>:<module-name-or-path>:<tag>\".")]
        [DataRow("a/b", "BCP196", "The specified OCI artifact reference \"br:a/b\" is not valid. The module tag or digest is missing.")]
        [DataRow("a/b:", "BCP196", "The specified OCI artifact reference \"br:a/b:\" is not valid. The module tag or digest is missing.")]
        [DataRow("a/b:$", "BCP198", "The specified OCI artifact reference \"br:a/b:$\" is not valid. The tag \"$\" is not valid. Valid characters are alphanumeric, \".\", \"_\", or \"-\" but the tag cannot begin with \".\", \"_\", or \"-\".")]
        [DataRow("example.com/hello.", "BCP195", "The specified OCI artifact reference \"br:example.com/hello.\" is not valid. The artifact path segment \"hello.\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello./there", "BCP195", "The specified OCI artifact reference \"br:example.com/hello./there\" is not valid. The artifact path segment \"hello.\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello./there:v1", "BCP195", "The specified OCI artifact reference \"br:example.com/hello./there:v1\" is not valid. The artifact path segment \"hello.\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello/there^", "BCP195", "The specified OCI artifact reference \"br:example.com/hello/there^\" is not valid. The artifact path segment \"there^\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello^/there:v1", "BCP195", "The specified OCI artifact reference \"br:example.com/hello^/there:v1\" is not valid. The artifact path segment \"hello^\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("test.azurecr.io/foo/bar:" + ExampleTagOfMaxLength + "a", "BCP197", "The specified OCI artifact reference \"br:test.azurecr.io/foo/bar:abcdefghijklmnopqrstuvxyz0123456789._-._-._-._-ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789._-._-._-._-abcdefghijklmnopqrstuvxyz012345678a\" is not valid. The tag \"abcdefghijklmnopqrstuvxyz0123456789._-._-._-._-ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789._-._-._-._-abcdefghijklmnopqrstuvxyz012345678a\" exceeds the maximum length of 128 characters.")]
        [DataRow("example.com/" + ExampleRepositoryOfMaxLength + "a:v3", "BCP199", "The specified OCI artifact reference \"br:example.com/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abca:v3\" is not valid. Module path \"abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abca\" exceeds the maximum length of 255 characters.")]
        [DataRow(ExampleRegistryOfMaxLength + "a/hello/there:1.0", "BCP200", "The specified OCI artifact reference \"br:abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abca/hello/there:1.0\" is not valid. The registry \"abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abca\" exceeds the maximum length of 255 characters.")]
        [DataRow("example.com/hello/there@wrong", "BCP224", "The specified OCI artifact reference \"br:example.com/hello/there@wrong\" is not valid. The digest \"wrong\" is not valid. The valid format is a string \"sha256:\" followed by exactly 64 lowercase hexadecimal digits.")]
        // valid digest plus 1 char
        [DataRow("example.com/hello/there@sha256:9aeb50c4b1a84de2315e2272c03bf940fa76c7c15e95dd6c5faabdb0945e6f8f1", "BCP224", "The specified OCI artifact reference \"br:example.com/hello/there@sha256:9aeb50c4b1a84de2315e2272c03bf940fa76c7c15e95dd6c5faabdb0945e6f8f1\" is not valid. The digest \"sha256:9aeb50c4b1a84de2315e2272c03bf940fa76c7c15e95dd6c5faabdb0945e6f8f1\" is not valid. The valid format is a string \"sha256:\" followed by exactly 64 lowercase hexadecimal digits.")]
        [DataRow("example.com/hello/there@sha256:9AEB50C4B1A84DE2315E2272C03BF940FA76C7C15E95DD6C5FAABDB0945E6F8F", "BCP224", "The specified OCI artifact reference \"br:example.com/hello/there@sha256:9AEB50C4B1A84DE2315E2272C03BF940FA76C7C15E95DD6C5FAABDB0945E6F8F\" is not valid. The digest \"sha256:9AEB50C4B1A84DE2315E2272C03BF940FA76C7C15E95DD6C5FAABDB0945E6F8F\" is not valid. The valid format is a string \"sha256:\" followed by exactly 64 lowercase hexadecimal digits.")]
        [DataTestMethod]
        public void InvalidReferencesShouldProduceExpectedError(string value, string expectedCode, string expectedError)
        {
            TryParseOciArtifactReference(value).IsSuccess(out var @ref, out var failureBuilder).Should().BeFalse();
            @ref.Should().BeNull();
            failureBuilder!.Should().NotBeNull();

            using (new AssertionScope())
            {
                failureBuilder!.Should().HaveCode(expectedCode);
                failureBuilder!.Should().HaveMessage(expectedError);
            }
        }

        [DataRow("TEST.azurecr.IO/foo/bar:latest", "test.azurecr.io/foo/bar:latest")]
        [DataRow("LOCALHOST:5000/test/ssss:v1", "localhost:5000/test/ssss:v1")]
        [DataTestMethod]
        public void ReferencesWithRegistryCasingDifferencesShouldBeEqual(string package1, string package2)
        {
            var (first, second) = ParsePair(package1, package2);

            first.Equals(second).Should().BeTrue();
            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [DataRow("test.azurecr.io/foo/bar:latest", "test.azurecr.io/foo/bar:LATEST")]
        [DataRow("localhost:5000/test/ssss:version1", "localhost:5000/test/ssss:VERSION1")]
        [DataRow("one.azurecr.io/first/second:tag1", "two.azurecr.io/third/fourth:tag2")]
        [DataTestMethod]
        public void MismatchedReferencesShouldNotBeEqual(string package1, string package2)
        {
            var (first, second) = ParsePair(package1, package2);
            first.Equals(second).Should().BeFalse();
            first.GetHashCode().Should().NotBe(second.GetHashCode());
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
            TryParseOciArtifactReference("", aliasName).IsSuccess(out var reference, out var errorBuilder).Should().BeFalse();

            reference.Should().BeNull();
            errorBuilder!.Should().HaveCode("BCP211");
            errorBuilder!.Should().HaveMessage($"The module alias name \"{aliasName}\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".");
        }

        [DataTestMethod]
        [DataRow("myRegistry", "path/to/module:v1", null, "BCP213", "The OCI artifact module alias name \"myRegistry\" does not exist in the built-in Bicep configuration.")]
        [DataRow("myModulePath", "myModule:v2", "/bicepconfig.json", "BCP213", "The OCI artifact module alias name \"myModulePath\" does not exist in the Bicep configuration \"/bicepconfig.json\".")]
        public void TryParse_AliasNotInConfiguration_ReturnsFalseAndSetsErrorDiagnostic(string aliasName, string referenceValue, string? configurationPath, string expectedCode, string expectedMessage)
        {
            var configuration = BicepTestConstants.CreateMockConfiguration(configFilePath: configurationPath);

            TryParseOciArtifactReference(referenceValue, aliasName, configuration).IsSuccess(out var reference, out var errorBuilder).Should().BeFalse();

            reference.Should().BeNull();
            errorBuilder!.Should().NotBeNull();
            errorBuilder!.Should().HaveCode(expectedCode);
            errorBuilder!.Should().HaveMessage(expectedMessage);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidAliasData), DynamicDataSourceType.Method)]
        public void TryParse_InvalidAlias_ReturnsFalseAndSetsErrorDiagnostic(string aliasName, string referenceValue, RootConfiguration configuration, string expectedCode, string expectedMessage)
        {
            TryParseOciArtifactReference(referenceValue, aliasName, configuration).IsSuccess(out var reference, out var errorBuilder).Should().BeFalse();

            reference.Should().BeNull();
            errorBuilder!.Should().NotBeNull();
            errorBuilder!.Should().HaveCode(expectedCode);
            errorBuilder!.Should().HaveMessage(expectedMessage);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidAliasData), DynamicDataSourceType.Method)]
        public void TryGetModuleReference_ValidAlias_ReplacesReferenceValue(string aliasName, string referenceValue, string fullyQualifiedReferenceValue, RootConfiguration configuration)
        {
            TryParseOciArtifactReference(referenceValue, aliasName, configuration).IsSuccess(out var reference, out var errorBuilder).Should().BeTrue();

            reference.Should().NotBeNull();
            reference!.FullyQualifiedReference.Should().Be(fullyQualifiedReferenceValue);
        }


        public static bool IsValid(string package)
        {
            return TryParseOciArtifactReference(package).IsSuccess(out var _, out var _);
        }

        public static OciArtifactReference Parse(string package)
        {
            TryParseOciArtifactReference(package).IsSuccess(out var parsed, out var failureBuilder).Should().BeTrue();
            failureBuilder!.Should().BeNull();
            parsed.Should().NotBeNull();
            return parsed!;
        }

        private static (OciArtifactReference, OciArtifactReference) ParsePair(string first, string second) => (Parse(first), Parse(second));

        private static ResultWithDiagnosticBuilder<OciArtifactReference> TryParseOciArtifactReference(string value, string? aliasName = null, RootConfiguration? configuration = null) =>
            OciArtifactReference.TryParse(BicepTestConstants.CreateDummyBicepFile(configuration), ArtifactType.Module, aliasName, value);

        private static IEnumerable<object[]> GetValidCases()
        {
            return ArtifactAddressComponentsTests.GetValidCases();
        }

        private static IEnumerable<object[]> GetInvalidAliasData()
        {
            yield return new object[]
            {
                "myModulePath",
                "myModule:v1",
                BicepTestConstants.CreateMockConfiguration(
                    new()
                    {
                        ["moduleAliases.br.myModulePath.modulePath"] = "path",
                    }),
                "BCP216",
                "The OCI artifact module alias \"myModulePath\" in the built-in Bicep configuration is invalid. The \"registry\" property cannot be null or undefined.",
            };

            yield return new object[]
            {
                "myModulePath2",
                "myModule:v2",
                BicepTestConstants.CreateMockConfiguration(
                    new()
                    {
                        ["moduleAliases.br.myModulePath2.modulePath"] = "path2",
                    },
                    "/bicepconfig.json"),
                "BCP216",
                "The OCI artifact module alias \"myModulePath2\" in the Bicep configuration \"/bicepconfig.json\" is invalid. The \"registry\" property cannot be null or undefined.",
            };
        }

        private static IEnumerable<object[]> GetValidAliasData()
        {
            yield return new object[]
            {
                "myModulePath",
                "mymodule:v1",
                "br:example.com/path/mymodule:v1",
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["moduleAliases.br.myModulePath.registry"] = "example.com",
                    ["moduleAliases.br.myModulePath.modulePath"] = "path",
                }),
            };

            yield return new object[]
            {
                "myModulePath2",
                "mymodule:v2",
                "br:localhost:8000/root/parent/mymodule:v2",
                BicepTestConstants.CreateMockConfiguration(
                    new()
                    {
                        ["moduleAliases.br.myModulePath2.registry"] = "localhost:8000",
                        ["moduleAliases.br.myModulePath2.modulePath"] = "root/parent",
                    },
                    "/bicepconfig.json"),
            };
        }

        public static string GetDisplayName(MethodInfo info, object[] data) => ArtifactAddressComponentsTests.GetDisplayName(info, data);
    }
}
