// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.UnitTests.Utils
{
    [TestClass]
    public class TempFileTests
    {
        [TestMethod]
        public void Dispose_WhenCalled_DeletesFile()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();

            // Act.
            using (var tempFile = new TempFile(fileSystem))
            {
                using (fileSystem.File.Create(tempFile.Path)) { }
            }

            // Assert.
            fileSystem.AllFiles.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void NewTempFile_WhenGarbageCollected_DeletesFile(bool createFile)
        {
            // Arrange.
            var fileSystem = new MockFileSystem();

            var action = new Action(() =>
            {
                var tempFile = new TempFile(fileSystem);

                using (fileSystem.File.Create(tempFile.Path)) { }
            });

            // Act.
            action();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Assert.
            fileSystem.AllFiles.Should().BeEmpty();
        }

    }
}
