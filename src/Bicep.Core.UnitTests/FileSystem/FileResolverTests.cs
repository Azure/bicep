// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.FileSystem
{
    [TestClass]
    public class FileResolverTests
    {
        [DataTestMethod]
        [DynamicData(nameof(TryResolveModulePathData), DynamicDataSourceType.Method)]
        public void TryResolveModulePath_should_return_expected_results(string parentFilePath, string childFilePath, string? expectedResult)
        {
            var fileResolver = new FileResolver();
            fileResolver.TryResolveModulePath(new Uri(parentFilePath), childFilePath).Should().Be(expectedResult != null ? new Uri(expectedResult) : null, $"{nameof(fileResolver.TryResolveModulePath)}(\"{parentFilePath}\", \"{childFilePath}\") should produce expected result");
        }

        private static IEnumerable<object?[]> TryResolveModulePathData()
        {
            if (CompilerFlags.IsWindowsBuild)
            {
                yield return new [] { "file:///C:/abc/file.bicep", "def/ghi.bicep", "file:///C:/abc/def/ghi.bicep", };
                yield return new [] { "file:///C:/abc/file.bicep", "./def/ghi.bicep", "file:///C:/abc/def/ghi.bicep", };
                yield return new [] { "file:///C:/abc/file.bicep", "../ghi.bicep", "file:///C:/ghi.bicep", }; // directory is resolved relative to parent path
                yield return new [] { "file:///C:/abc/file.bicep", "./def/../ghi.bicep", "file:///C:/abc/ghi.bicep", }; // directory is resolved
                yield return new [] { "file:///D:/abc/file.bicep", "/def.bicep", "file:///D:/def.bicep", }; // root child path is used as-is
                yield return new [] { "file:///E:/abc/file.bicep", "/abc/../def/../ghi.bicep", "file:///E:/ghi.bicep", }; // root child path is used as-is with normalization
            }
            else
            {
                yield return new [] { "file:///abc/file.bicep", "def/ghi.bicep", "file:///abc/def/ghi.bicep", };
                yield return new [] { "file:///abc/file.bicep", "./def/ghi.bicep", "file:///abc/def/ghi.bicep", };
                yield return new [] { "file:///abc/file.bicep", "../ghi.bicep", "file:///ghi.bicep", }; // directory is resolved relative to parent path
                yield return new [] { "file:///abc/file.bicep", "./def/../ghi.bicep", "file:///abc/ghi.bicep", }; // directory is resolved
                yield return new [] { "file:///abc/file.bicep", "/def.bicep", "file:///def.bicep", }; // root child path is used as-is
                yield return new [] { "file:///abc/file.bicep", "/abc/../def/../ghi.bicep", "file:///ghi.bicep", }; // root child path is used as-is with normalization
            }
        }

        [TestMethod]
        public void TryRead_should_return_expected_results()
        {
            var fileResolver = new FileResolver();
            var tempFile = Path.Combine(Path.GetTempPath(), $"BICEP_TEST_{Guid.NewGuid()}");
            var tempFileUri = PathHelper.FilePathToFileUrl(tempFile);

            File.WriteAllText(tempFile, "abcd\r\ndef");
            fileResolver.TryRead(tempFileUri, out var fileContents, out var failureMessage).Should().BeTrue();
            fileContents.Should().Equals("abc\r\ndef");
            failureMessage.Should().BeNull();

            File.Delete(tempFile);

            fileResolver.TryRead(tempFileUri, out fileContents, out failureMessage).Should().BeFalse();
            fileContents.Should().BeNull();
            failureMessage.Should().NotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(IOException), AllowDerivedTypes=true)]
        public void GetDirectories_should_return_expected_results()
        {
            var fileResolver = new FileResolver();
            var tempDir = Path.Combine(Path.GetTempPath(), $"BICEP_TESTDIR_{Guid.NewGuid()}");
            var tempFile = Path.Combine(tempDir, $"BICEP_TEST_{Guid.NewGuid()}");
            var tempChildDir = Path.Combine(tempDir, $"BICEP_TESTCHILDDIR_{Guid.NewGuid()}");

            // make parent dir
            Directory.CreateDirectory(tempDir);
            fileResolver.GetDirectories(PathHelper.FilePathToFileUrl(tempDir)).Count().Should().Be(0);
            // make child dir
            Directory.CreateDirectory(tempChildDir);
            fileResolver.GetDirectories(PathHelper.FilePathToFileUrl(tempDir)).Count().Should().Be(1);
            // add a file to parent dir
            File.WriteAllText(tempFile, "abcd\r\ndef");
            fileResolver.GetDirectories(PathHelper.FilePathToFileUrl(tempDir)).Count().Should().Be(1);
            // check child dir
            fileResolver.GetDirectories(PathHelper.FilePathToFileUrl(tempChildDir)).Count().Should().Be(0);
            // should throw an IOException when called with a file path
            fileResolver.GetDirectories(PathHelper.FilePathToFileUrl(Path.Join(Path.GetTempPath(), tempFile)));
        }

        [TestMethod]
        [ExpectedException(typeof(IOException), AllowDerivedTypes=true)]
        public void GetFiles_should_return_expected_results()
        {
            var fileResolver = new FileResolver();
            var tempDir = Path.Combine(Path.GetTempPath(), $"BICEP_TESTDIR_{Guid.NewGuid()}");
            var tempFile = Path.Combine(tempDir, $"BICEP_TEST_{Guid.NewGuid()}");
            var tempChildDir = Path.Combine(tempDir, $"BICEP_TESTCHILDDIR_{Guid.NewGuid()}");

            // make parent dir
            Directory.CreateDirectory(tempDir);
            fileResolver.GetFiles(PathHelper.FilePathToFileUrl(tempDir)).Count().Should().Be(0);
            // add a file to parent dir
            File.WriteAllText(tempFile, "abcd\r\ndef");
            fileResolver.GetFiles(PathHelper.FilePathToFileUrl(tempDir)).Count().Should().Be(1);
            // make child dir
            Directory.CreateDirectory(tempChildDir);
            fileResolver.GetFiles(PathHelper.FilePathToFileUrl(tempDir)).Count().Should().Be(1);
            // check child dir
            fileResolver.GetFiles(PathHelper.FilePathToFileUrl(tempChildDir)).Count().Should().Be(0);
            // should throw an IOException when called with a file path
            fileResolver.GetDirectories(PathHelper.FilePathToFileUrl(Path.Join(Path.GetTempPath(), tempFile)));
        }

        [TestMethod]
        public void DirExists_should_return_expected_results()
        {
            var fileResolver = new FileResolver();
            var tempDir = Path.Combine(Path.GetTempPath(), $"BICEP_TESTDIR_{Guid.NewGuid()}");
            var tempFile = Path.Combine(tempDir, $"BICEP_TEST_{Guid.NewGuid()}");
            var tempChildDir = Path.Combine(tempDir, $"BICEP_TESTCHILDDIR_{Guid.NewGuid()}");

            // make parent dir
            Directory.CreateDirectory(tempDir);
            fileResolver.TryDirExists(PathHelper.FilePathToFileUrl(tempDir)).Should().BeTrue();
            fileResolver.TryDirExists(PathHelper.FilePathToFileUrl(tempFile)).Should().BeFalse();
            // add a file to parent dir
            File.WriteAllText(tempFile, "abcd\r\ndef");
            fileResolver.TryDirExists(PathHelper.FilePathToFileUrl(tempDir)).Should().BeTrue();
            fileResolver.TryDirExists(PathHelper.FilePathToFileUrl(tempFile)).Should().BeFalse();
            // make child dir
            Directory.CreateDirectory(tempChildDir);
            fileResolver.TryDirExists(PathHelper.FilePathToFileUrl(tempChildDir)).Should().BeTrue();
        }
    }
}