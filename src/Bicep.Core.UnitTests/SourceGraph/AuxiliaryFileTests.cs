// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests.Assertions;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.SourceGraph
{
    [TestClass]
    public class AuxiliaryFileTests
    {
        [TestMethod]
        public void TryDetectEncodingFromByteOrderMarks_NoBOM_ReturnsNull()
        {
            // Arrange
            var uri = new IOUri("file", "", "/path/to/file");
            var data = new BinaryData(Encoding.UTF8.GetBytes("Hello, World!"));
            var auxiliaryFile = new AuxiliaryFile(uri, data);

            // Act
            var encoding = auxiliaryFile.TryDetectEncodingFromByteOrderMarks();

            // Assert
            encoding.Should().BeNull();
        }

        [TestMethod]
        public void TryDetectEncodingFromByteOrderMarks_BOMPresent_ReturnsEncoding()
        {
            // Arrange
            var uri = new IOUri(new IOUriScheme("file"), null, "/path/to/file");
            var data = new BinaryData([.. Encoding.UTF8.GetPreamble(), .. Encoding.UTF8.GetBytes("Hello, World!")]);
            var auxiliaryFile = new AuxiliaryFile(uri, data);

            // Act
            var encoding = auxiliaryFile.TryDetectEncodingFromByteOrderMarks();

            // Assert
            encoding.Should().NotBeNull();
            encoding.Should().Be(Encoding.UTF8);
        }

        [TestMethod]
        public void TryReadText_WithinLimit_ReturnsText()
        {
            // Arrange
            var uri = new IOUri(new IOUriScheme("file"), null, "/path/to/file");
            var data = new BinaryData(Encoding.UTF8.GetBytes("Hello, World!"));
            var auxiliaryFile = new AuxiliaryFile(uri, data);

            // Act
            var result = auxiliaryFile.TryReadText(Encoding.UTF8, 20);

            // Assert
            result.IsSuccess(out var text, out _).Should().BeTrue();
            text.Should().Be("Hello, World!");
        }

        [TestMethod]
        public void TryReadText_ExceedsLimit_ReturnsError()
        {
            // Arrange
            var uri = new IOUri(new IOUriScheme("file"), null, "/path/to/file");
            var data = new BinaryData(Encoding.UTF8.GetBytes("Hello, World!"));
            var auxiliaryFile = new AuxiliaryFile(uri, data);

            // Act
            var result = auxiliaryFile.TryReadText(Encoding.UTF8, 5);

            // Assert
            result.IsSuccess(out _, out var errorBuilder).Should().BeFalse();
            var error = errorBuilder!(DiagnosticBuilder.ForDocumentStart());
            error.Should().HaveCodeAndSeverity("BCP184", DiagnosticLevel.Error);
            error.Should().HaveMessage("File '/path/to/file' exceeded maximum size of 5 characters.");
        }

        [TestMethod]
        public void TryReadBytes_WithinLimit_ReturnsBytes()
        {
            // Arrange
            var uri = new IOUri(new IOUriScheme("file"), null, "/path/to/file");
            var data = new BinaryData(Encoding.UTF8.GetBytes("Hello, World!"));
            var auxiliaryFile = new AuxiliaryFile(uri, data);

            // Act
            var result = auxiliaryFile.TryReadBytes(20);

            // Assert
            result.IsSuccess(out var bytes, out _).Should().BeTrue();
            bytes.ToArray().Should().Equal(data.ToArray());
        }

        [TestMethod]
        public void TryReadBytes_ExceedsLimit_ReturnsError()
        {
            // Arrange
            var uri = new IOUri(new IOUriScheme("file"), null, "/path/to/file");
            var data = new BinaryData(Encoding.UTF8.GetBytes("Hello, World!"));
            var auxiliaryFile = new AuxiliaryFile(uri, data);

            // Act
            var result = auxiliaryFile.TryReadBytes(5);

            // Assert
            result.IsSuccess(out _, out var errorBuilder).Should().BeFalse();
            var error = errorBuilder!(DiagnosticBuilder.ForDocumentStart());
            error.Should().HaveCodeAndSeverity("BCP184", DiagnosticLevel.Error);
            error.Should().HaveMessage("File '/path/to/file' exceeded maximum size of 5 bytes.");
        }
    }
}
