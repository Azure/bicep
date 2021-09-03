// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Modules
{
    [TestClass]
    public class OciArtifactModuleReferenceTests
    {
        public const string ExampleTagOfMaxLength = "abcdefghijklmnopqrstuvxyz0123456789._-._-._-._-ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789._-._-._-._-abcdefghijklmnopqrstuvxyz012345678";

        public const string ExamplePathSegment1 = "abcdefghijklmnopqrstuvxyz0123456789.abcdefghijklmnopqrstuvxyz0123456789-abcdefghijklmnopqrstuvxyz0123456789_abcdefghijklmnopqrstuvxyz0123456789";

        public const string ExamplePathSegment2 = "a.b-0_1";

        [TestMethod]
        public void ExamplesShouldMatchExpectedConstraints()
        {
            ExampleTagOfMaxLength.Should().HaveLength(128);
        }

        [DataRow("a/b:C", "a", "b", "C")]
        [DataRow("localhost/hello:V1", "localhost", "hello", "V1")]
        [DataRow("localhost:123/hello:V1", "localhost:123", "hello", "V1")]
        [DataRow("test.azurecr.io/foo/bar:latest", "test.azurecr.io", "foo/bar", "latest")]
        [DataRow("test.azurecr.io/foo/bar:" + ExampleTagOfMaxLength, "test.azurecr.io", "foo/bar", ExampleTagOfMaxLength)]
        [DataRow("example.com/"+ ExamplePathSegment1 + "/" + ExamplePathSegment2 + ":1", "example.com", ExamplePathSegment1 + "/" + ExamplePathSegment2, "1")]
        [DataTestMethod]
        public void ValidReferencesShouldParseCorrectly(string value, string expectedRegistry, string expectedRepository, string expectedTag)
        {
            var parsed = Parse(value);
            parsed.Registry.Should().Be(expectedRegistry);
            parsed.Repository.Should().Be(expectedRepository);
            parsed.Tag.Should().Be(expectedTag);
            parsed.ArtifactId.Should().Be(value);
        }

        [DataRow("a/b:C")]
        [DataRow("localhost/hello:V1")]
        [DataRow("localhost:123/hello:V1")]
        [DataRow("test.azurecr.io/foo/bar:latest")]
        [DataRow("test.azurecr.io/foo/bar:" + ExampleTagOfMaxLength)]
        [DataRow("example.com/" + ExamplePathSegment1 + "/" + ExamplePathSegment2 + ":1")]
        public void ValidReferenceShouldBeEqualToItself(string value)
        {
            var first = Parse(value);
            var second = Parse(value);

            first.Equals(second).Should().Be(true);
            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        // bad
        [DataRow("", "The specified OCI artifact reference \"br:\" is not valid. Specify a reference in the format of \"br:<artifact uri>:<tag>\".")]
        [DataRow("a", "The specified OCI artifact reference \"br:a\" is not valid. Specify a reference in the format of \"br:<artifact uri>:<tag>\".")]
        [DataRow("a/", "The specified OCI artifact reference \"br:a/\" is not valid. Specify a reference in the format of \"br:<artifact uri>:<tag>\".")]
        [DataRow("a/b", "The specified OCI artifact reference \"br:a/b\" is not valid. The module tag is missing.")]
        [DataRow("a/b:", "The specified OCI artifact reference \"br:a/b:\" is not valid. The tag \"\" is not valid. The tag must be a string with maximum length 128 characters. Valid characters are alphanumeric, \".\", \"_\", or \"-\" but the tag cannot begin with \".\", \"_\", or \"-\".")]
        [DataRow("example.com/hello.", "The specified OCI artifact reference \"br:example.com/hello.\" is not valid. The mpdule path segment \"hello.\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello./there", "The specified OCI artifact reference \"br:example.com/hello./there\" is not valid. The mpdule path segment \"hello.\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello./there:v1", "The specified OCI artifact reference \"br:example.com/hello./there:v1\" is not valid. The mpdule path segment \"hello.\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello/there^", "The specified OCI artifact reference \"br:example.com/hello/there^\" is not valid. The mpdule path segment \"there^\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataRow("example.com/hello^/there:v1", "The specified OCI artifact reference \"br:example.com/hello^/there:v1\" is not valid. The mpdule path segment \"hello^\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".")]
        [DataTestMethod]
        public void InvalidReferencesShouldProduceExpectedError(string value, string expectedError)
        {
            OciArtifactModuleReference.TryParse(value, out var failureBuilder).Should().BeNull();
            failureBuilder!.Should().NotBeNull();

            failureBuilder!.Should().HaveCode("BCP193");
            failureBuilder!.Should().HaveMessage(expectedError);
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
