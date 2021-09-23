// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

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

        public record ValidCase(string Value, string ExpectedRegistry, string ExpectedRepository, string ExpectedTag);

        [TestMethod]
        public void ExamplesShouldMatchExpectedConstraints()
        {
            ExampleTagOfMaxLength.Should().HaveLength(128);
            ExampleRepositoryOfMaxLength.Should().HaveLength(255);
            ExampleRegistryOfMaxLength.Should().HaveLength(255);
        }

        [DynamicData(nameof(GetValidCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [DataTestMethod]
        public void ValidReferencesShouldParseCorrectly(ValidCase @case)
        {
            var parsed = Parse(@case.Value);

            using (new AssertionScope())
            {
                parsed.Registry.Should().Be(@case.ExpectedRegistry);
                parsed.Repository.Should().Be(@case.ExpectedRepository);
                parsed.Tag.Should().Be(@case.ExpectedTag);
                parsed.ArtifactId.Should().Be(@case.Value);
            }
        }

        [DynamicData(nameof(GetValidCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [DataTestMethod]
        public void ValidReferenceShouldBeEqualToItself(ValidCase @case)
        {
            var first = Parse(@case.Value);
            var second = Parse(@case.Value);

            first.Equals(second).Should().Be(true);
            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [DynamicData(nameof(GetValidCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [DataTestMethod]
        public void ValidReferenceShouldBeUriParseable(ValidCase @case)
        {
            var parsed = Parse(@case.Value);

            // the go2def flow in the language server passes the reference through URIs
            // in cases of documents that come from the local module cache
            FluentActions.Invoking(() => new Uri(parsed.FullyQualifiedReference)).Should().NotThrow();

            // the unqualified reference should be parseable as a URI segment as well
            FluentActions.Invoking(() => new Uri("test://" + parsed.UnqualifiedReference)).Should().NotThrow();
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

        private static IEnumerable<object[]> GetValidCases()
        {
            static object[] CreateRow(string value, string expectedRegistry, string expectedRepository, string expectedTag) =>
                new object[] { new ValidCase(value, expectedRegistry, expectedRepository, expectedTag) };

            yield return CreateRow("a/b:C", "a", "b", "C");
            yield return CreateRow("localhost/hello:V1", "localhost", "hello", "V1");
            yield return CreateRow("localhost:123/hello:V1", "localhost:123", "hello", "V1");
            yield return CreateRow("test.azurecr.io/foo/bar:latest", "test.azurecr.io", "foo/bar", "latest");
            yield return CreateRow("test.azurecr.io/foo/bar:" + ExampleTagOfMaxLength, "test.azurecr.io", "foo/bar", ExampleTagOfMaxLength);
            yield return CreateRow("example.com/" + ExamplePathSegment1 + "/" + ExamplePathSegment2 + ":1", "example.com", ExamplePathSegment1 + "/" + ExamplePathSegment2, "1");
            yield return CreateRow("example.com/" + ExampleRepositoryOfMaxLength + ":v3", "example.com", ExampleRepositoryOfMaxLength, "v3");
            yield return CreateRow(ExampleRegistryOfMaxLength + "/hello/there:1.0", ExampleRegistryOfMaxLength, "hello/there", "1.0");
        }

        public static string GetDisplayName(MethodInfo info, object[] data) => $"{info.Name}_{((ValidCase)data[0]).Value}";
    }
}
