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
        [TestMethod]
        public void PathComparer_should_be_ordinal_for_linux_otherwise_ordinal_insensitive()
        {
            var fileResolver = new FileResolver();
            if (CompilerFlags.IsLinuxBuild)
            {
                fileResolver.PathComparer.Should().Be(StringComparer.Ordinal);
            }
            else
            {
                fileResolver.PathComparer.Should().Be(StringComparer.OrdinalIgnoreCase);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetNormalizedFileNameData), DynamicDataSourceType.Method)]
        public void GetNormalizedFileName_should_return_expected_results(string pathToNormalize, string expectedNormalizedPath)
        {
            var fileResolver = new FileResolver();
            fileResolver.GetNormalizedFileName(pathToNormalize).Should().Be(expectedNormalizedPath, $"{nameof(fileResolver.GetNormalizedFileName)}(\"{pathToNormalize}\") should produce expected result");
        }

        private static IEnumerable<object[]> GetNormalizedFileNameData()
        {
            if (CompilerFlags.IsWindowsBuild)
            {
                yield return new [] { "C:\\abcdefg", "C:\\abcdefg", }; // fully-qualified path returned as-is
                yield return new [] { "C:\\abc\\def\\ghi\\..\\..\\jkl\\..\\mno", "C:\\abc\\mno", }; // path is resolved
                yield return new [] { ".\\abc\\", $"{Directory.GetCurrentDirectory()}\\abc\\", }; // unqualified path assumes current directory
                yield return new [] { ".", $"{Directory.GetCurrentDirectory()}", }; // unqualified path assumes current directory
            }
            else
            {
                yield return new [] { "/abcdefg", "/abcdefg", }; // fully-qualified path returned as-is
                yield return new [] { "/abc/def/ghi/../../jkl/../mno", "/abc/mno", }; // path is resolved
                yield return new [] { "./abc/", $"{Directory.GetCurrentDirectory()}/abc/", }; // unqualified path assumes current directory
                yield return new [] { ".", $"{Directory.GetCurrentDirectory()}", }; // unqualified path assumes current directory
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(TryResolveModulePathData), DynamicDataSourceType.Method)]
        public void TryResolveModulePath_should_return_expected_results(string parentFilePath, string childFilePath, string? expectedResult)
        {
            var fileResolver = new FileResolver();
            fileResolver.TryResolveModulePath(parentFilePath, childFilePath).Should().Be(expectedResult, $"{nameof(fileResolver.TryResolveModulePath)}(\"{parentFilePath}\", \"{childFilePath}\") should produce expected result");
        }

        private static IEnumerable<object?[]> TryResolveModulePathData()
        {
            if (CompilerFlags.IsWindowsBuild)
            {
                yield return new [] { "C:\\abc\\file.bicep", "def/ghi.bicep", "C:\\abc\\def\\ghi.bicep", };
                yield return new [] { "C:\\abc\\file.bicep", "./def/ghi.bicep", "C:\\abc\\def\\ghi.bicep", };
                yield return new [] { "C:\\abc\\file.bicep", "../ghi.bicep", "C:\\ghi.bicep", }; // directory is resolved relative to parent path
                yield return new [] { "C:\\abc\\file.bicep", "./def/../ghi.bicep", "C:\\abc\\ghi.bicep", }; // directory is resolved
                yield return new [] { "C:\\abc\\file.bicep", "D:\\def.bicep", "D:\\def.bicep", }; // root child path is used as-is
                yield return new [] { "C:\\abc\\file.bicep", "D:\\abc\\..\\def\\..\\ghi.bicep", "D:\\ghi.bicep", }; // root child path is used as-is with normalization
                yield return new [] { "C:\\", "def.bicep", null, }; // parent path *is* root
                yield return new [] { "abc\\file.bicep", "def.bicep", null, }; // non-rooted parent path
                yield return new [] { "abc.bicep", "def.bicep", null, }; // non-rooted parent path
            }
            else
            {
                yield return new [] { "/abc/file.bicep", "def/ghi.bicep", "/abc/def/ghi.bicep", };
                yield return new [] { "/abc/file.bicep", "./def/ghi.bicep", "/abc/def/ghi.bicep", };
                yield return new [] { "/abc/file.bicep", "../ghi.bicep", "/ghi.bicep", }; // directory is resolved relative to parent path
                yield return new [] { "/abc/file.bicep", "./def/../ghi.bicep", "/abc/ghi.bicep", }; // directory is resolved
                yield return new [] { "/abc/file.bicep", "/def.bicep", "/def.bicep", }; // root child path is used as-is
                yield return new [] { "/abc/file.bicep", "/abc/../def/../ghi.bicep", "/ghi.bicep", }; // root child path is used as-is with normalization
                yield return new [] { "/", "def.bicep", null, }; // parent path *is* root
                yield return new [] { "abc/file.bicep", "def.bicep", null, }; // non-rooted parent path
                yield return new [] { "abc.bicep", "def.bicep", null, }; // non-rooted parent path
            }
        }

        [TestMethod]
        public void TryRead_should_return_expected_results()
        {
            var fileResolver = new FileResolver();
            var tempFile = Path.Combine(Path.GetTempPath(), $"BICEP_TEST_{Guid.NewGuid()}");

            File.WriteAllText(tempFile, "abcd\r\ndef");
            fileResolver.TryRead(tempFile, out var failureMessage).Should().Equals("abc\r\ndef");
            failureMessage.Should().BeNull();

            File.Delete(tempFile);

            fileResolver.TryRead(tempFile, out failureMessage).Should().BeNull();
            failureMessage.Should().NotBeNullOrWhiteSpace();
        }
    }
}