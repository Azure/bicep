// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
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
    }
}