// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LocalFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.UnitTests.FileSystem
{
    [TestClass]
    public class FileResolverTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static IFileResolver GetFileResolver()
            => new FileResolver(new LocalFileSystem());

        [TestMethod]
        public void TryRead_should_return_expected_results()
        {
            var fileResolver = GetFileResolver();
            var tempFile = Path.Combine(Path.GetTempPath(), $"BICEP_TEST_{Guid.NewGuid()}");
            var tempFileUri = PathHelper.FilePathToFileUrl(tempFile);

            File.WriteAllText(tempFile, "abcd\r\ndef");
            fileResolver.TryRead(tempFileUri).IsSuccess(out var fileContents, out var failureMessage).Should().BeTrue();
            fileContents.Should().Be("abcd\r\ndef");
            failureMessage.Should().BeNull();

            File.Delete(tempFile);

            fileResolver.TryRead(tempFileUri).IsSuccess(out fileContents, out failureMessage).Should().BeFalse();
            fileContents.Should().BeNull();
            failureMessage.Should().NotBeNull();
        }

        [TestMethod]
        public void Diagnostic_should_be_raised_for_folder_instead_of_file()
        {
            var outputDir = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(outputDir);

            var outputUri = PathHelper.FilePathToFileUrl(outputDir);
            var fileResolver = GetFileResolver();

            fileResolver.TryRead(outputUri).IsSuccess(out var fileContents, out var failureBuilder).Should().BeFalse();
            fileContents.Should().BeNull();
            var err = failureBuilder!.Invoke(new DiagnosticBuilder.DiagnosticBuilderInternal(TextSpan.TextDocumentStart));
            err.Message.Should().Match("Unable to open file at path \"*\". Found a directory instead.");
            err.Code.Should().Be("BCP275");
        }
    }
}
