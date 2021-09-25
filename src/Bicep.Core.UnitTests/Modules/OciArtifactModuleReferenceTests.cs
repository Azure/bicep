// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
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

        [TestMethod]
        public void ExamplesShouldMatchExpectedConstraints()
        {
            ExampleTagOfMaxLength.Should().HaveLength(128);
            ExampleRepositoryOfMaxLength.Should().HaveLength(255);
            ExampleRegistryOfMaxLength.Should().HaveLength(255);
        }

        [DataRow("a/b:C", "a", "b", "C")]
        [DataRow("localhost/hello:V1", "localhost", "hello", "V1")]
        [DataRow("localhost:123/hello:V1", "localhost:123", "hello", "V1")]
        [DataRow("test.azurecr.io/foo/bar:latest", "test.azurecr.io", "foo/bar", "latest")]
        [DataRow("test.azurecr.io/foo/bar:" + ExampleTagOfMaxLength, "test.azurecr.io", "foo/bar", ExampleTagOfMaxLength)]
        [DataRow("example.com/" + ExamplePathSegment1 + "/" + ExamplePathSegment2 + ":1", "example.com", ExamplePathSegment1 + "/" + ExamplePathSegment2, "1")]
        [DataRow("example.com/" + ExampleRepositoryOfMaxLength + ":v3", "example.com", ExampleRepositoryOfMaxLength, "v3")]
        [DataRow(ExampleRegistryOfMaxLength + "/hello/there:1.0", ExampleRegistryOfMaxLength, "hello/there", "1.0")]
        [DataTestMethod]
        public void ValidReferencesShouldParseCorrectly(string value, string expectedRegistry, string expectedRepository, string expectedTag)
        {
            var parsed = Parse(value);

            using (new AssertionScope())
            {
                parsed.Registry.Should().Be(expectedRegistry);
                parsed.Repository.Should().Be(expectedRepository);
                parsed.Tag.Should().Be(expectedTag);
                parsed.ArtifactId.Should().Be(value);
            }
        }

        [DataRow("a/b:C")]
        [DataRow("localhost/hello:V1")]
        [DataRow("localhost:123/hello:V1")]
        [DataRow("test.azurecr.io/foo/bar:latest")]
        [DataRow("test.azurecr.io/foo/bar:" + ExampleTagOfMaxLength)]
        [DataRow("example.com/" + ExamplePathSegment1 + "/" + ExamplePathSegment2 + ":1")]
        [DataRow("example.com/" + ExampleRepositoryOfMaxLength + ":v3")]
        [DataRow(ExampleRegistryOfMaxLength + "/hello/there:1.0")]
        [DataTestMethod]
        public void ValidReferenceShouldBeEqualToItself(string value)
        {
            var first = Parse(value);
            var second = Parse(value);

            first.Equals(second).Should().Be(true);
            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        // bad
        [DataRow("", "BCP193", "The specified OCI artifact reference \"br:\" is not valid. Specify a reference in the format of \"br:<artifact uri>:<tag>\".")]
        [DataRow("a", "BCP193", "The specified OCI artifact reference \"br:a\" is not valid. Specify a reference in the format of \"br:<artifact uri>:<tag>\".")]
        [DataRow("a/", "BCP193", "The specified OCI artifact reference \"br:a/\" is not valid. Specify a reference in the format of \"br:<artifact uri>:<tag>\".")]
        [DataRow("a/b", "BCP196", "The specified OCI artifact reference \"br:a/b\" is not valid. The module tag is missing.")]
        [DataRow("a/b:", "BCP196", "The specified OCI artifact reference \"br:a/b:\" is not valid. The module tag is missing.")]
        [DataRow("a/b:$", "BCP198", "The specified OCI artifact reference \"br:a/b:$\" is not valid. The tag \"$\" is not valid. Valid characters are alphanumeric, \".\", \"_\", or \"-\" but the tag cannot begin with \".\", \"_\", or \"-\".")]
        [DataRow("example.com/hello.", "BCP195", "The specified OCI artifact reference \"br:example.com/hello.\" is not valid. The module path segment \"hello.\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello./there", "BCP195", "The specified OCI artifact reference \"br:example.com/hello./there\" is not valid. The module path segment \"hello.\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello./there:v1", "BCP195", "The specified OCI artifact reference \"br:example.com/hello./there:v1\" is not valid. The module path segment \"hello.\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello/there^", "BCP195", "The specified OCI artifact reference \"br:example.com/hello/there^\" is not valid. The module path segment \"there^\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello^/there:v1", "BCP195", "The specified OCI artifact reference \"br:example.com/hello^/there:v1\" is not valid. The module path segment \"hello^\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("test.azurecr.io/foo/bar:" + ExampleTagOfMaxLength + "a", "BCP197", "The specified OCI artifact reference \"br:test.azurecr.io/foo/bar:abcdefghijklmnopqrstuvxyz0123456789._-._-._-._-ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789._-._-._-._-abcdefghijklmnopqrstuvxyz012345678a\" is not valid. The tag \"abcdefghijklmnopqrstuvxyz0123456789._-._-._-._-ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789._-._-._-._-abcdefghijklmnopqrstuvxyz012345678a\" exceeds the maximum length of 128 characters.")]
        [DataRow("example.com/" + ExampleRepositoryOfMaxLength + "a:v3", "BCP199", "The specified OCI artifact reference \"br:example.com/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abca:v3\" is not valid. Module path \"abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abcdefghijklmnopqrstuvxyz0123456789/abca\" exceeds the maximum length of 255 characters.")]
        [DataRow(ExampleRegistryOfMaxLength + "a/hello/there:1.0", "BCP200", "The specified OCI artifact reference \"br:abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abca/hello/there:1.0\" is not valid. The registry \"abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789.abca\" exceeds the maximum length of 255 characters.")]
        [DataTestMethod]
        public void InvalidReferencesShouldProduceExpectedError(string value, string expectedCode, string expectedError)
        {
            OciArtifactModuleReference.TryParse(value, out var failureBuilder).Should().BeNull();
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
        [DataRow("one.azurecr.io/first/second:tag1","two.azurecr.io/third/fourth:tag2")]
        [DataTestMethod]
        public void MismatchedReferencesShouldNotBeEqual(string package1, string package2)
        {
            var (first, second) = ParsePair(package1, package2);
            first.Equals(second).Should().BeFalse();
            first.GetHashCode().Should().NotBe(second.GetHashCode());
        }

        private static OciArtifactModuleReference Parse(string package)
        {
            var parsed = OciArtifactModuleReference.TryParse(package, out var failureBuilder);
            failureBuilder!.Should().BeNull();
            parsed.Should().NotBeNull();
            return parsed!;
        }

        private static (OciArtifactModuleReference, OciArtifactModuleReference) ParsePair(string first, string second) => (Parse(first), Parse(second));
    }
}
