// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;
using FluentAssertions;

namespace Bicep.IO.UnitTests.Abstraction
{
    [TestClass]
    public class ResourceIdentifierTests
    {
        [DataTestMethod]
        [DataRow("http", "EXAMPLE.COM", "example.com")]
        [DataRow("http", "Example.Com", "example.com")]
        [DataRow("http", "example.com", "example.com")]
        [DataRow("https", "EXAMPLE.COM:80", "example.com:80")]
        [DataRow("https", "Example.Com:443", "example.com:443")]
        [DataRow("file", "localhost", "")]
        [DataRow("file", "", "")]
        [DataRow("file", null, "")]
        public void ResourceIdentifier_ByDefault_NormalizesAuthority(string scheme, string? authority, string expectedAuthority)
        {
            // Arrange & Act.
            var resourceIdentifier = new ResourceIdentifier(scheme, authority, "/a/b/c");

            // Assert.
            resourceIdentifier.Authority.Should().Be(expectedAuthority);
        }

        [DataTestMethod]
        [DataRow("/a/b/c", "/a/b/c")]
        [DataRow("/a/b/../c", "/a/c")]
        [DataRow("/a/./b/c", "/a/b/c")]
        [DataRow("/a/b/c/", "/a/b/c/")]
        [DataRow("/a//b/c", "/a/b/c")]
        [DataRow("/a/b/c/..", "/a/b")]
        public void ResourceIdentifier_ByDefault_NormalizesPath(string inputPath, string expectedPath)
        {
            // Arrange & Act.
            var resourceIdentifier = new ResourceIdentifier("http", "example.com", inputPath);

            // Assert.
            resourceIdentifier.Path.Should().Be(expectedPath);
        }

        [DataTestMethod]
        [DataRow("https", "")]
        [DataRow("https", null)]
        [DataRow("http", "")]
        [DataRow("http", null)]
        public void ResourceIdentifier_NullOrEmptyHttpOrHttpsAuthority_ThrowsArgumentException(string scheme, string? authority)
        {
            FluentActions
                .Invoking(() => new ResourceIdentifier(scheme, authority, "/a/b/c"))
                .Should().Throw<ArgumentException>();
        }

        [DataTestMethod]
        [DataRow("http", "example.com", "a/b/c")]
        [DataRow("http", null, "//a/b/c")]
        [DataRow("file", null, "a/b/c")]
        public void ResourceIdentifier_InvalidPath_ThrowsArgumentException(string scheme, string? authorty, string path)
        {
            FluentActions
                .Invoking(() => new ResourceIdentifier(scheme, authorty, path))
                .Should().Throw<ArgumentException>();
        }

        [DataTestMethod]
        [DataRow("http", "example.com", "/a/b/c", "http://example.com/a/b/c")]
        [DataRow("https", "example.com", "/a/b/c", "https://example.com/a/b/c")]
        [DataRow("file", "", "/a/b/c", "/a/b/c")]
        public void ToString_ByDefault_ReturnsUriOrLocalFilePath(string scheme, string authority, string path, string expectedOutput)
        {
            // Arrange & Act.
            var resourceIdentifier = new ResourceIdentifier(scheme, authority, path);

            // Assert
            resourceIdentifier.ToString().Should().Be(expectedOutput);
        }

        [TestMethod]
        public void Equals_IdenticalIdentifiers_ReturnsTrue()
        {
            // Arrange
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b/c");
            var identifier2 = new ResourceIdentifier("http", "example.com", "/a/b/c");

            // Act & Assert
            identifier1.Equals(identifier2).Should().BeTrue();
            (identifier1 == identifier2).Should().BeTrue();
            (identifier1 != identifier2).Should().BeFalse();
        }

        [TestMethod]
        public void Equals_DifferentIdentifiers_ReturnsFalse()
        {
            // Arrange
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b/c");
            var identifier2 = new ResourceIdentifier("http", "example.com", "/a/b/d");

            // Act & Assert
            identifier1.Equals(identifier2).Should().BeFalse();
            (identifier1 == identifier2).Should().BeFalse();
            (identifier1 != identifier2).Should().BeTrue();
        }

        [TestMethod]
        public void GetHashCode_IdenticalIdentifiers_ReturnsConsistentHashCode()
        {
            // Arrange
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b/c");
            var identifier2 = new ResourceIdentifier("http", "example.com", "/a/b/c");

            // Act & Assert
            identifier1.GetHashCode().Should().Be(identifier2.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_DifferentIdentifiers_ReturnsDifferentHashCodes()
        {
            // Arrange
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b/c");
            var identifier2 = new ResourceIdentifier("http", "example.com", "/a/b/d");

            // Act & Assert
            identifier1.GetHashCode().Should().NotBe(identifier2.GetHashCode());
        }

        [DataTestMethod]
        [DataRow("http", "example.com", "/a/b/c", "/a/b", "c")]
        [DataRow("http", "example.com", "/a/b/c/", "/a/b", "c/")]
        [DataRow("http", "example.com", "/a/b/c", "/a/b/c", "")]
        [DataRow("http", "example.com", "/a/b/c", "/a/b/d", "../c")]
        [DataRow("http", "example.com", "/a/b/c/d", "/a/b", "c/d")]
        [DataRow("http", "example.com", "/a/b/c", "/a/b/c/d", "..")]
        public void GetPathRelativeTo_ValidPaths_ReturnsCorrectRelativePath(string scheme, string authority, string path, string otherPath, string expectedRelativePath)
        {
            // Arrange.
            var identifier = new ResourceIdentifier(scheme, authority, path);
            var otherIdentifier = new ResourceIdentifier(scheme, authority, otherPath);

            // Act.
            var relativePath = identifier.GetPathRelativeTo(otherIdentifier);

            // Assert.
            relativePath.Should().Be(expectedRelativePath);
        }

        [TestMethod]
        public void GetPathRelativeTo_DifferentSchemes_ThrowsInvalidOperationException()
        {
            // Arrange.
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b/c");
            var identifier2 = new ResourceIdentifier("https", "example.com", "/a/b/c");

            // Act.
            Action act = () => identifier1.GetPathRelativeTo(identifier2);

            // Assert.
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void GetPathRelativeTo_DifferentAuthorities_ThrowsInvalidOperationException()
        {
            // Arrange.
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b/c");
            var identifier2 = new ResourceIdentifier("http", "example.org", "/a/b/c");

            // Act.
            Action act = () => identifier1.GetPathRelativeTo(identifier2);

            // Assert.
            act.Should().Throw<InvalidOperationException>();
        }

        [DataTestMethod]
        [DataRow("http", "example.com", "/a/b", "/a/b/c", true)]
        [DataRow("http", "example.com", "/a/b", "/a/b/c/d", true)]
        [DataRow("http", "example.com", "/a/b", "/a/b", true)]
        [DataRow("http", "example.com", "/a/b", "/a/c", false)]
        [DataRow("http", "example.com", "/a/b", "/a/bc", false)]
        [DataRow("http", "example.com", "/a/b", "/a/bc/d", false)]
        [DataRow("http", "example.com", "/a/b", "/a/b/../c", false)]
        [DataRow("http", "example.com", "/a/b", "/a/b/./c", true)]
        [DataRow("http", "example.com", "/a/b", "/a/b/c/..", true)]
        public void IsBaseOf_ValidPaths_ReturnsExpectedResult(string scheme, string authority, string basePath, string otherPath, bool expectedResult)
        {
            // Arrange.
            var baseIdentifier = new ResourceIdentifier(scheme, authority, basePath);
            var otherIdentifier = new ResourceIdentifier(scheme, authority, otherPath);

            // Act.
            var result = baseIdentifier.IsBaseOf(otherIdentifier);

            // Assert.
            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void IsBaseOf_DifferentSchemes_ReturnsFalse()
        {
            // Arrange.
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b");
            var identifier2 = new ResourceIdentifier("https", "example.com", "/a/b/c");

            // Act.
            var result = identifier1.IsBaseOf(identifier2);

            // Assert.
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsBaseOf_DifferentAuthorities_ReturnsFalse()
        {
            // Arrange.
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b");
            var identifier2 = new ResourceIdentifier("http", "example.org", "/a/b/c");

            // Act.
            var result = identifier1.IsBaseOf(identifier2);

            // Assert.
            result.Should().BeFalse();
        }
    }
}
