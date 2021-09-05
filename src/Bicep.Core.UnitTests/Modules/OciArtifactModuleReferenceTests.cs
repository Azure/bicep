// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Modules
{
    [TestClass]
    public class OciArtifactModuleReferenceTests
    {
        [DataRow("test.azurecr.io/foo/bar:latest", "test.azurecr.io/foo/bar:latest")]
        [DataRow("localhost:5000/test/ssss:v1", "localhost:5000/test/ssss:v1")]
        [DataRow("test.azurecr.io/foo/bar:latest", "test.azurecr.IO/foo/bar:latest")]
        [DataRow("localhost:5000/test/ssss:v1", "localHost:5000/test/ssss:v1")]
        [DataTestMethod]
        public void PackagesWithIdOrVersionCasingDifferencesShouldBeEqual(string package1, string package2)
        {
            var (first, second) = ParsePair(package1, package2);

            first.Equals(second).Should().BeTrue();
            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [DataRow("test.azurecr.io/foo/bar:latest", "test.azurecr.io/fOO/bar:latest")]
        [DataRow("test.azurecr.io/foo/bar:latest", "test.azurecr.io/foo/bar:LAtest")]
        [DataRow("localhost:5000/test/ssss:v1", "localhost:5000/Test/ssss:v1")]
        [DataRow("localhost:5000/test/ssss:v1", "localhost:5000/test/ssss:V1")]
        [DataTestMethod]
        public void MismatchedPackagesShouldNotBeEqual(string package1, string package2)
        {
            var (first, second) = ParsePair(package1, package2);
            first.Equals(second).Should().BeFalse();
            first.GetHashCode().Should().NotBe(second.GetHashCode());
        }

        private static OciArtifactModuleReference Parse(string package)
        {
            var parsed = OciArtifactModuleReference.TryParse(package, out var failureBuilder);
            failureBuilder.Should().BeNull();
            parsed.Should().NotBeNull();
            return parsed!;
        }

        private static (OciArtifactModuleReference, OciArtifactModuleReference) ParsePair(string first, string second) => (Parse(first), Parse(second));
    }
}
