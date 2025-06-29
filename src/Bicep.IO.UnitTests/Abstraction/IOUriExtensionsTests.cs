// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.IO.UnitTests.Abstraction
{
    [TestClass]
    public class IOUriExtensionsTests
    {
        [DataTestMethod]
        [DataRow("/a/b/c.txt", ".txt")]
        [DataRow("/a/b/c.tar.gz", ".gz")]
        [DataRow("/a/b/c", "")]
        [DataRow("/a/b/c.", "")]
        [DataRow("/a/b/c.d/e", "")]
        public void GetExtension_ValidPaths_ReturnsCorrectExtension(string path, string expectedExtension)
        {
            // Arrange.
            var resourceIdentifier = new IOUri("file", "", path);

            // Act.
            var extension = resourceIdentifier.GetExtension();

            // Assert.
            extension.ToString().Should().Be(expectedExtension);
        }

        [DataTestMethod]
        [DataRow("/a/b/c.txt", ".bak", "/a/b/c.bak")]
        [DataRow("/a/b/c.tar.gz", ".zip", "/a/b/c.tar.zip")]
        [DataRow("/a/b/c.tar.gz", "zip", "/a/b/c.tar.zip")]
        [DataRow("/a/b/c", ".txt", "/a/b/c.txt")]
        [DataRow("/a/b/c.", ".txt", "/a/b/c.txt")]
        [DataRow("/a/b/c.d/e", ".txt", "/a/b/c.d/e.txt")]
        public void WithExtension_ValidPaths_ReturnsPathWithNewExtension(string path, string newExtension, string expectedPath)
        {
            // Arrange
            var resourceIdentifier = new IOUri("file", "", path);

            // Act
            var newResourceIdentifier = resourceIdentifier.WithExtension(newExtension);

            // Assert
            newResourceIdentifier.Path.Should().Be(expectedPath);
        }

        [DataTestMethod]
        [DataRow("/a/b/c.txt", ".txt", true)]
        [DataRow("/a/b/c.txt", "txt", true)]
        [DataRow("/a/b/c.tar.gz", ".gz", true)]
        [DataRow("/a/b/c.tar.gz", "gz", true)]
        [DataRow("/a/b/c", ".txt", false)]
        [DataRow("/a/b/c.d/e", ".e", false)]
        [DataRow("/a/b/c.d/e", "e", false)]
        [DataRow("/a/b/c", "", false)]
        [DataRow("/a/b/c.txt", "", false)]
        public void HasExtension_ValidPaths_ReturnsCorrectResult(string path, string extension, bool expectedResult)
        {
            // Arrange
            var resourceIdentifier = new IOUri("file", "", path);

            // Act
            var result = resourceIdentifier.HasExtension(extension);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
