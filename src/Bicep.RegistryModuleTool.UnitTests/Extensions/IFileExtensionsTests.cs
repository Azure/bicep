// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.UnitTests.Extensions
{
    [TestClass]
    public class IFileExtensionsTests
    {
        [TestMethod]
        public void CreateTempFile_Disposed_DeletesTempFile()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();

            // Act.
            using (var tempFile = fileSystem.File.CreateTempFile())
            {
                using (fileSystem.File.Create(tempFile.Path)) { }
            }

            // Assert.
            fileSystem.AllFiles.Should().BeEmpty();
        }

        [TestMethod]
        public void CreateTempFile_GarbageCollected_DeletesTempFile()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();

            var action = new Action(() =>
            {
                var tempFile = fileSystem.File.CreateTempFile();

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
