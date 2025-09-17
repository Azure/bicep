// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.SourceGraph
{
    [TestClass]
    public class RelativePathTests
    {
        [DataTestMethod]
        [DataRow("C:\\Code\\file.txt")]
        [DataRow("d:/code/file.txt")]
        [DataRow("/home/code/file.txt")]
        public void Absolute_file_paths_fail(string filePath)
        {
            var relativePath = RelativePath.TryCreate(filePath);
            relativePath.IsSuccess(out _, out var errorBuilder).Should().BeFalse();
            var error = errorBuilder!(DiagnosticBuilder.ForDocumentStart());
            error.Should().HaveCodeAndSeverity("BCP051", DiagnosticLevel.Error);
            error.Should().HaveMessage("The specified path seems to reference an absolute path. Files must be referenced using relative paths.");
        }

        [DataTestMethod]
        [DataRow("code/file.txt")]
        public void Relative_file_paths_succeed(string filePath)
        {
            var relativePath = RelativePath.TryCreate(filePath);
            relativePath.IsSuccess().Should().BeTrue();
        }


        [DataTestMethod]
        [DataRow("code\\file.txt")]
        public void File_paths_with_backslash_fail(string filePath)
        {
            var relativePath = RelativePath.TryCreate(filePath);
            relativePath.IsSuccess(out _, out var errorBuilder).Should().BeFalse();
            var error = errorBuilder!(DiagnosticBuilder.ForDocumentStart());
            error.Should().HaveCodeAndSeverity("BCP098", DiagnosticLevel.Error);
            error.Should().HaveMessage("The specified file path contains a \"\\\" character. Use \"/\" instead as the directory separator character.");
        }
    }
}
