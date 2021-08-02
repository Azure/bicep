// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Modules
{
    [TestClass]
    public class LocalModuleReferenceTests
    {
        [DataRow("test.bicep","test.bicep")]
        [DataRow("../bar/foo.bicep", "../bar/foo.bicep")]
        [DataRow("./t.json", "./t.json")]
        [DataTestMethod]
        public void SameModulePathsShouldBeEqual(string package1, string package2)
        {
            var (first, second) = ParsePair(package1, package2);

            first.Equals(second).Should().BeTrue();
            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [DataRow("test.bicep", "Test.bicep")]
        [DataRow("../bar/foo.bicep", "foo.bicep")]
        [DataRow("./t.json", "./t.JSON")]
        [DataTestMethod]
        public void DifferentPathsShouldNotBeEqual(string package1, string package2)
        {
            var (first, second) = ParsePair(package1, package2);

            first.Equals(second).Should().BeFalse();
            first.GetHashCode().Should().NotBe(second.GetHashCode());
        }

        [DataRow("./test.bicep")]
        [DataRow("foo/bar/test.bicep")]
        [DataRow("../bar/test.bicep")]
        [DataTestMethod]
        public void TryParseModuleReference_ValidLocalReference_ShouldParse(string value)
        {
            var reference = Parse(value);
            reference.Path.Should().Be(value);
        }

        private static LocalModuleReference Parse(string package)
        {
            var parsed = LocalModuleReference.TryParse(package, out var failureBuilder);
            parsed.Should().NotBeNull();
            failureBuilder.Should().BeNull();
            return parsed!;
        }

        // TODO: Add equality tests
        private static (LocalModuleReference, LocalModuleReference) ParsePair(string first, string second) => (Parse(first), Parse(second));
    }
}
