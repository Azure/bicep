// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.IO.UnitTests.Abstraction
{
    [TestClass]
    public class ResourceIdentifierTests
    {
        [DataTestMethod]
        [DataRow("/a/b/c", "/a/b/c")]
        [DataRow("/a/b/../c", "/a/c")]
        [DataRow("/a/./b/c", "/a/b/c")]
        [DataRow("/a/b/c/", "/a/b/c/")]
        [DataRow("/a//b/c", "/a/b/c")]
        [DataRow("/a/b/c/..", "/a/b")]
        public void ResourceIdentifier_ByDefault_CanonicalizePath(string inputPath, string expectedPath)
        {
            // Act
            var resourceIdentifier = new ResourceIdentifier("http", "example.com", inputPath);

            // Assert
            Assert.AreEqual(expectedPath, resourceIdentifier.Path);
        }

        [DataTestMethod]
        [DataRow("http", "example.com", "/a/b/c", "http://example.com/a/b/c")]
        [DataRow("https", "example.com", "/a/b/c", "https://example.com/a/b/c")]
        [DataRow("file", "", "/a/b/c", "/a/b/c")]
        public void ResourceIdentifier_ToString_ReturnsCorrectUriOrPath(string scheme, string authority, string path, string expectedOutput)
        {
            // Act
            var resourceIdentifier = new ResourceIdentifier(scheme, authority, path);

            // Assert
            Assert.AreEqual(expectedOutput, resourceIdentifier.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ResourceIdentifier_InvalidPath_ThrowsArgumentException()
        {
            // Act
            var resourceIdentifier = new ResourceIdentifier("http", "example.com", "a/b/c");
        }

        [TestMethod]
        public void ResourceIdentifier_Equals_ReturnsTrueForIdenticalIdentifiers()
        {
            // Arrange
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b/c");
            var identifier2 = new ResourceIdentifier("http", "example.com", "/a/b/c");

            // Act & Assert
            Assert.IsTrue(identifier1.Equals(identifier2));
            Assert.IsTrue(identifier1 == identifier2);
            Assert.IsFalse(identifier1 != identifier2);
        }

        [TestMethod]
        public void ResourceIdentifier_Equals_ReturnsFalseForDifferentIdentifiers()
        {
            // Arrange
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b/c");
            var identifier2 = new ResourceIdentifier("http", "example.com", "/a/b/d");

            // Act & Assert
            Assert.IsFalse(identifier1.Equals(identifier2));
            Assert.IsFalse(identifier1 == identifier2);
            Assert.IsTrue(identifier1 != identifier2);
        }

        [TestMethod]
        public void ResourceIdentifier_GetHashCode_ReturnsConsistentHashCode()
        {
            // Arrange
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b/c");
            var identifier2 = new ResourceIdentifier("http", "example.com", "/a/b/c");

            // Act & Assert
            Assert.AreEqual(identifier1.GetHashCode(), identifier2.GetHashCode());
        }

        [TestMethod]
        public void ResourceIdentifier_GetHashCode_ReturnsDifferentHashCodesForDifferentIdentifiers()
        {
            // Arrange
            var identifier1 = new ResourceIdentifier("http", "example.com", "/a/b/c");
            var identifier2 = new ResourceIdentifier("http", "example.com", "/a/b/d");

            // Act & Assert
            Assert.AreNotEqual(identifier1.GetHashCode(), identifier2.GetHashCode());
        }
    }
}
