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
    public class IOUriTests
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
        [DataRow("file", "server", "server")]
        public void IOUri_ByDefault_NormalizesAuthority(string scheme, string? authority, string expectedAuthority)
        {
            // Arrange & Act.
            var resourceIdentifier = new IOUri(scheme, authority, "/a/b/c");

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
        public void IOUri_ByDefault_NormalizesNonFilePath(string inputPath, string expectedPath)
        {
            // Arrange & Act.
            var resourceIdentifier = new IOUri("http", "example.com", inputPath);

            // Assert.
            resourceIdentifier.Path.Should().Be(expectedPath);
        }

        [DataTestMethod]
        [DataRow("https", "")]
        [DataRow("https", null)]
        [DataRow("http", "")]
        [DataRow("http", null)]
        public void IOUri_NullOrEmptyHttpOrHttpsAuthority_ThrowsArgumentException(string scheme, string? authority)
        {
            FluentActions
                .Invoking(() => new IOUri(scheme, authority, "/a/b/c"))
                .Should().Throw<ArgumentException>();
        }

        [DataTestMethod]
        [DataRow("http", "example.com", "a/b/c")]
        [DataRow("http", null, "//a/b/c")]
        [DataRow("file", null, "a/b/c")]
        public void IOUri_InvalidPath_ThrowsArgumentException(string scheme, string? authorty, string path)
        {
            FluentActions
                .Invoking(() => new IOUri(scheme, authorty, path))
                .Should().Throw<ArgumentException>();
        }

        [DataTestMethod]
        [DataRow("http", "example.com", "/a/b/c", "http://example.com/a/b/c")]
        [DataRow("https", "example.com", "/a/b/c", "https://example.com/a/b/c")]
        [DataRow("inmemory", null, "a/b/c", "inmemory:a/b/c")]
        [DataRow("inmemory", null, "a/b/../c", "inmemory:a/c")]
        [DataRow("file", "", "/a/b/c", "/a/b/c")]
        [DataRow("file", null, "/a/b/c", "/a/b/c")]
        public void ToString_ByDefault_ReturnsUriOrFilePath(string scheme, string? authority, string path, string expectedOutput)
        {
            // Arrange & Act.
            var resourceIdentifier = new IOUri(scheme, authority, path);

            // Assert
            resourceIdentifier.ToString().Should().Be(expectedOutput);
        }

        [TestMethod]
        public void Equals_IdenticalIdentifiers_ReturnsTrue()
        {
            // Arrange
            var identifier1 = new IOUri("http", "example.com", "/a/b/c");
            var identifier2 = new IOUri("http", "example.com", "/a/b/c");

            // Act & Assert
            identifier1.Equals(identifier2).Should().BeTrue();
            (identifier1 == identifier2).Should().BeTrue();
            (identifier1 != identifier2).Should().BeFalse();
        }

        [TestMethod]
        public void Equals_DifferentIdentifiers_ReturnsFalse()
        {
            // Arrange
            var identifier1 = new IOUri("http", "example.com", "/a/b/c");
            var identifier2 = new IOUri("http", "example.com", "/a/b/d");

            // Act & Assert
            identifier1.Equals(identifier2).Should().BeFalse();
            (identifier1 == identifier2).Should().BeFalse();
            (identifier1 != identifier2).Should().BeTrue();
        }

        [TestMethod]
        public void GetHashCode_IdenticalIdentifiers_ReturnsConsistentHashCode()
        {
            // Arrange
            var identifier1 = new IOUri("http", "example.com", "/a/b/c");
            var identifier2 = new IOUri("http", "example.com", "/a/b/c");

            // Act & Assert
            identifier1.GetHashCode().Should().Be(identifier2.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_DifferentIdentifiers_ReturnsDifferentHashCodes()
        {
            // Arrange
            var identifier1 = new IOUri("http", "example.com", "/a/b/c");
            var identifier2 = new IOUri("http", "example.com", "/a/b/d");

            // Act & Assert
            identifier1.GetHashCode().Should().NotBe(identifier2.GetHashCode());
        }

        [DataTestMethod]
        [DataRow("http", "example.com", "/a/b/c", "/a/b/", "c")]
        [DataRow("http", "example.com", "/a/b/c/", "/a/b/", "c/")]
        [DataRow("http", "example.com", "/a/b/c", "/a/b/c", "")]
        [DataRow("http", "example.com", "/a/b/c/f", "/a/b/d/", "../c/f")]
        [DataRow("http", "example.com", "/a/b/c/f", "/a/b/d", "c/f")]
        [DataRow("http", "example.com", "/a/b/c", "/a/b/d", "c")]
        [DataRow("http", "example.com", "/a/b/c/d", "/a/b/", "c/d")]
        [DataRow("http", "example.com", "/a/b/c", "/a/b/e/d", "../c")]
        public void GetPathRelativeTo_ValidPaths_ReturnsCorrectRelativePath(string scheme, string authority, string path, string otherPath, string expectedRelativePath)
        {
            // Arrange.
            var identifier = new IOUri(scheme, authority, path);
            var otherIdentifier = new IOUri(scheme, authority, otherPath);

            // Act.
            var relativePath = identifier.GetPathRelativeTo(otherIdentifier);

            // Assert.
            relativePath.Should().Be(expectedRelativePath);
        }

        [TestMethod]
        public void GetPathRelativeTo_DifferentSchemes_ThrowsInvalidOperationException()
        {
            // Arrange.
            var identifier1 = new IOUri("http", "example.com", "/a/b/c");
            var identifier2 = new IOUri("https", "example.com", "/a/b/c");

            // Act.
            Action act = () => identifier1.GetPathRelativeTo(identifier2);

            // Assert.
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void GetPathRelativeTo_DifferentAuthorities_ThrowsInvalidOperationException()
        {
            // Arrange.
            var identifier1 = new IOUri("http", "example.com", "/a/b/c");
            var identifier2 = new IOUri("http", "example.org", "/a/b/c");

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
            var baseIdentifier = new IOUri(scheme, authority, basePath);
            var otherIdentifier = new IOUri(scheme, authority, otherPath);

            // Act.
            var result = baseIdentifier.IsBaseOf(otherIdentifier);

            // Assert.
            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void IsBaseOf_DifferentSchemes_ReturnsFalse()
        {
            // Arrange.
            var identifier1 = new IOUri("http", "example.com", "/a/b");
            var identifier2 = new IOUri("https", "example.com", "/a/b/c");

            // Act.
            var result = identifier1.IsBaseOf(identifier2);

            // Assert.
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsBaseOf_DifferentAuthorities_ReturnsFalse()
        {
            // Arrange.
            var identifier1 = new IOUri("http", "example.com", "/a/b");
            var identifier2 = new IOUri("http", "example.org", "/a/b/c");

            // Act.
            var result = identifier1.IsBaseOf(identifier2);

            // Assert.
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("http", "example.com", "/a/b/c", "d/e", "http://example.com/a/b/d/e")]
        [DataRow("http", "example.com", "/a/b/c/", "d/e", "http://example.com/a/b/c/d/e")]
        [DataRow("http", "example.com", "/a/b/c", "../d/e", "http://example.com/a/d/e")]
        [DataRow("http", "example.com", "/a/b/c/", "../d/e", "http://example.com/a/b/d/e")]
        [DataRow("http", "example.com", "/a/b/c/", "./d/e", "http://example.com/a/b/c/d/e")]
        [DataRow("file", "", "/a/b/c", "d/e", "/a/b/d/e")]
        [DataRow("file", "", "/a/b/c/", "d/e", "/a/b/c/d/e")]
        [DataRow("file", "", "/a/b/c", ".", "/a/b/")]
        [DataRow("file", "", "/a/b/c", "..", "/a/")]
        [DataRow("file", "", "/a/b/c", "../d/e", "/a/d/e")]
        [DataRow("file", "", "/a/b/c/", "../d/e", "/a/b/d/e")]
        [DataRow("http", "example.com", "/a/b/c/", "/d/e", "http://example.com/d/e")]
        [DataRow("file", "", "/a/b/c/", "/d/e", "/d/e")]
        [DataRow("file", "", "/a/b/c/", "/../d/e", "/d/e")]
        [DataRow("file", "", "/a/b/c/", "/./d/e", "/d/e")]
        public void Resolve_RelativeOrAbsolutePath_ReturnsExpectedUri(string scheme, string? authority, string path, string relativeOrAbsolutePath, string expectedUri)
        {
            // Arrange
            var baseUri = new IOUri(scheme, authority, path);

            // Act
            var resolvedUri = baseUri.Resolve(relativeOrAbsolutePath);

            // Assert
            resolvedUri.ToString().Should().Be(expectedUri);
        }

        [TestMethod]
        public void FromFilePath_RelativePath_ThrowsIOException()
        {
            // Arrange
            var filePath = "a/b/c";

            // Act
            Action act = () => IOUri.FromFilePath(filePath);

            // Assert
            act.Should().Throw<IOException>().WithMessage("File path must be absolute.");
        }

        [TestMethod]
        public void FromFilePath_ValidAbsolutePath_ReturnsExpectedUri()
        {
            // Arrange
            var filePath = "/a/b/c";

            // Act
            var uri = IOUri.FromFilePath(filePath);

            // Assert
            uri.Scheme.Should().Be(IOUriScheme.File);
            uri.Authority.Should().Be("");
            uri.Path.Should().Be("/a/b/c");
        }

#if WINDOWS_BUILD
        [TestMethod]
        public void FromFilePath_WindowsAbsolutePath_ReturnsExpectedUri()
        {
            // Arrange
            var filePath = "C:\\a\\b\\c";

            // Act
            var uri = IOUri.FromFilePath(filePath);

            // Assert
            uri.Scheme.Should().Be(IOUriScheme.File);
            uri.Authority.Should().Be("");
            uri.Path.Should().Be("/C:/a/b/c");
        }

        [DataTestMethod]
        [DataRow(@"\\server\share\file.txt", "server", "/share/file.txt")]
        [DataRow(@"\\myserver\documents\folder\file.bicep", "myserver", "/documents/folder/file.bicep")]
        [DataRow(@"\\SERVER\SHARE\file.txt", "server", "/SHARE/file.txt")]
        [DataRow(@"\\file-server\public\docs\readme.md", "file-server", "/public/docs/readme.md")]
        public void FromFilePath_UncPath_ReturnsExpectedUri(string uncPath, string expectedAuthority, string expectedPath)
        {
            // Act
            var uri = IOUri.FromFilePath(uncPath);

            // Assert
            uri.Scheme.Should().Be(IOUriScheme.File);
            uri.Authority.Should().Be(expectedAuthority);
            uri.Path.Should().Be(expectedPath);
        }

        [DataTestMethod]
        [DataRow(@"\\server\share\file.txt")]
        [DataRow(@"\\myserver\documents\folder\file.bicep")]
        [DataRow(@"\\file-server\public\docs\readme.md")]
        public void ToString_UncPath_ReturnsUncPath(string uncPath)
        {
            // Arrange
            var uri = IOUri.FromFilePath(uncPath);

            // Act
            var result = uri.ToString();

            // Assert
            result.Should().Be(uncPath);
        }

        [DataTestMethod]
        [DataRow(@"\\server\share\file.txt", @"\\server\share\file.txt")]
        [DataRow(@"\\myserver\documents\folder\file.bicep", @"\\myserver\documents\folder\file.bicep")]
        [DataRow(@"\\file-server\public\docs\readme.md", @"\\file-server\public\docs\readme.md")]
        public void TryGetFilePath_UncPath_ReturnsCorrectLocalPath(string originalUncPath, string expectedLocalPath)
        {
            // Arrange
            var uri = IOUri.FromFilePath(originalUncPath);

            // Act
            var localPath = uri.TryGetFilePath();

            // Assert
            localPath.Should().Be(expectedLocalPath);
        }
#endif
    }
}
