// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Modules
{
    [TestClass]
    public class ArtifactAddressComponentsTests
    {
        public const string ExampleTagOfMaxLength = "abcdefghijklmnopqrstuvxyz0123456789._-._-._-._-ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789._-._-._-._-abcdefghijklmnopqrstuvxyz012345678";

        public const string ExampleRepositoryOfMaxLength = "abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abc";

        public const string ExampleRegistryOfMaxLength = "abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abc";

        public const string ExamplePathSegment1 = "abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789-abcdefghijklmnopqrstuvxyz0123456789_abcdefghijklmnopqrstuvxyz0123456789";

        public const string ExamplePathSegment2 = "a.b-0_1";

        public record ValidCase(string Value, string ExpectedRegistry, string ExpectedRepository, string? ExpectedTag, string? ExpectedDigest);

        private static void VerifyEqual(OciArtifactAddressComponents first, OciArtifactAddressComponents second)
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

        private static void VerifyNotEqual(OciArtifactAddressComponents first, OciArtifactAddressComponents second)
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

        [TestMethod]
        public void ExamplesShouldMatchExpectedConstraints()
        {
            ExampleTagOfMaxLength.Should().HaveLength(128);
            ExampleRepositoryOfMaxLength.Should().HaveLength(255);
            ExampleRegistryOfMaxLength.Should().HaveLength(255);
        }

        [DynamicData(nameof(GetValidCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [DataTestMethod]
        public void ValidReferenceShouldBeEqualToItself(ValidCase @case)
        {
            OciArtifactAddressComponents first = Parse(@case.Value);
            OciArtifactAddressComponents second = Parse(@case.Value);
            VerifyEqual(first, second);
        }

        [DynamicData(nameof(GetValidCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [DataTestMethod]
        public void ValidReferenceShouldBeEqualWithCaseChanged(ValidCase @case)
        {
            OciArtifactAddressComponents first = Parse(@case.Value);
            OciArtifactAddressComponents firstLower = Parse((@case with { ExpectedDigest = @case.Value.ToLower() }).Value);
            OciArtifactAddressComponents firstUpper = Parse((@case with { ExpectedDigest = @case.Value.ToUpper() }).Value);

            VerifyEqual(first, firstLower);
            VerifyEqual(first, firstUpper);
            VerifyEqual(firstLower, firstUpper);
        }

        [DynamicData(nameof(GetValidCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [DataTestMethod]
        public void CharacterChanged_ShouldNotBeEqual(ValidCase @case)
        {
            string ModifyCharAt(string a, int index)
            {
                char newChar = a[index] == 'q' ? 'z' : 'q';
                return a.Substring(0, index) + newChar + a.Substring(index + 1);
            }

            for (int i = 0; i < @case.Value.Length - 1; ++i)
            {
                OciArtifactAddressComponents first = Parse(@case.Value);
                var modified = ModifyCharAt(@case.Value, i);
                if (IsValid(modified))
                {
                    OciArtifactAddressComponents second = Parse(modified);
                    VerifyNotEqual(first, second);
                }
            }
        }

        private static bool IsValid(string package)
        {
            // NOTE: ArtifactAddressComponents doesn't currently have a parser separate from OciArtifactReference.
            return OciArtifactReference.TryParse(BicepTestConstants.DummyBicepFile, ArtifactType.Module, null, package).IsSuccess(out var _, out var _);
        }

        private static OciArtifactAddressComponents Parse(string package)
        {
            // NOTE: ArtifactAddressComponents doesn't currently have a parser separate from OciArtifactReference.
            OciArtifactReference.TryParse(BicepTestConstants.DummyBicepFile, ArtifactType.Module, null, package).IsSuccess(out var parsed, out var failureBuilder).Should().BeTrue();
            failureBuilder!.Should().BeNull();
            parsed.Should().NotBeNull();
            return (OciArtifactAddressComponents)parsed!.AddressComponents;
        }

        public static IEnumerable<object[]> GetValidCases()
        {
            static object[] CreateRow(string value, string expectedRegistry, string expectedRepository, string? expectedTag, string? expectedDigest) =>
                [new ValidCase(value, expectedRegistry, expectedRepository, expectedTag, expectedDigest)];

            yield return CreateRow("a/b:C", "a", "b", "C", null);
            yield return CreateRow("localhost/hello:V1", "localhost", "hello", "V1", null);
            yield return CreateRow("localhost:123/hello:V1", "localhost:123", "hello", "V1", null);
            yield return CreateRow("test.azurecr.io/foo/bar:latest", "test.azurecr.io", "foo/bar", "latest", null);
            yield return CreateRow("test.azurecr.io/foo/bar:" + ExampleTagOfMaxLength, "test.azurecr.io", "foo/bar", ExampleTagOfMaxLength, null);
            yield return CreateRow("example.com/" + ExamplePathSegment1 + "/" + ExamplePathSegment2 + ":1", "example.com", ExamplePathSegment1 + "/" + ExamplePathSegment2, "1", null);
            yield return CreateRow("example.com/" + ExampleRepositoryOfMaxLength + ":v3", "example.com", ExampleRepositoryOfMaxLength, "v3", null);
            yield return CreateRow(ExampleRegistryOfMaxLength + "/hello/there:1.0", ExampleRegistryOfMaxLength, "hello/there", "1.0", null);
            yield return CreateRow("hello-there.azurecr.io/general/kenobi@sha256:b131a80d6764593360293a4a0a55e6850356c16754c4b5eb9a2286293fddcdfb", "hello-there.azurecr.io", "general/kenobi", null, "sha256:b131a80d6764593360293a4a0a55e6850356c16754c4b5eb9a2286293fddcdfb");
        }

        public static string GetDisplayName(MethodInfo info, object[] data) => $"{info.Name}_{((ValidCase)data[0]).Value}";
    }
}
