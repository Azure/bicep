// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Bicep.Core.FileSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.FileSystem
{
    /// <summary>
    /// Some of the tests in this class are specific to Windows or Unix file systems and require to be run on a machine with that type of OS.
    /// </summary>
    [TestClass]
    public class PathHelperTests
    {
#if LINUX_BUILD
        [TestMethod]
        public void LinuxFileSystem_ShouldBeCaseSensitive()
        {
            PathHelper.PathComparison.Should().Be(StringComparison.Ordinal);
            PathHelper.PathComparer.Should().BeSameAs(StringComparer.Ordinal);
        }

        [DataTestMethod]
        [DataRow("foo.json")]
        public void GetBuildOutputPath_ShouldThrowOnJsonExtensions_Linux(string path)
        {
            Action badExtension = () => PathHelper.GetDefaultBuildOutputPath(path);
            badExtension.Should().Throw<ArgumentException>().WithMessage("The specified file already has the '.json' extension.");
        }

        [DataTestMethod]
        [DataRow("foo.bicep")]
        public void GetDecompileOutputPath_ShouldThrowOnBicepExtensions_Linux(string path)
        {
            Action badExtension = () => PathHelper.GetDefaultDecompileOutputPath(path);
            badExtension.Should().Throw<ArgumentException>().WithMessage("The specified file already has the '.bicep' extension.");
        }
#else
        [TestMethod]
        public void WindowsAndMacFileSystem_ShouldBeCaseInsensitive()
        {
            PathHelper.PathComparison.Should().Be(StringComparison.OrdinalIgnoreCase);
            PathHelper.PathComparer.Should().BeSameAs(StringComparer.OrdinalIgnoreCase);
        }

        [DataTestMethod]
        [DataRow("foo.json")]
        [DataRow("foo.JSON")]
        [DataRow("foo.JsOn")]
        public void GetBuildOutputPath_ShouldThrowOnJsonExtensions_WindowsAndMac(string path)
        {
            Action badExtension = () => PathHelper.GetDefaultBuildOutputPath(path);
            badExtension.Should().Throw<ArgumentException>().WithMessage("The specified file already has the '.json' extension.");
        }

        [DataTestMethod]
        [DataRow("foo.bicep")]
        [DataRow("foo.BICEP")]
        [DataRow("foo.BiCeP")]
        public void GetDecompileOutputPath_ShouldThrowOnBicepExtensions_WindowsAndMac(string path)
        {
            Action badExtension = () => PathHelper.GetDefaultDecompileOutputPath(path);
            badExtension.Should().Throw<ArgumentException>().WithMessage("The specified file already has the '.bicep' extension.");
        }
#endif

        [DataTestMethod]
        [DynamicData(nameof(GetResolvePathData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void ResolvePath_ShouldResolveCorrectly(string path, string expectedPath)
        {
            PathHelper.ResolvePath(path).Should().Be(expectedPath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetFilePathToFileUrlData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void FilePathToFileUrl_ShouldResolveCorrectly(string path, string expectedPath)
        {
            PathHelper.FilePathToFileUrl(path).LocalPath.Should().Be(expectedPath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetBuildOutputPathData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void GetDefaultBuildOutputPath_ShouldChangeExtensionCorrectly(string path, string expectedPath)
        {
            PathHelper.GetDefaultBuildOutputPath(path).Should().Be(expectedPath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetDecompileOutputPathData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void GetDefaultDecompileOutputPath_ShouldChangeExtensionCorrectly(string path, string expectedPath)
        {
            PathHelper.GetDefaultDecompileOutputPath(path).Should().Be(expectedPath);
        }

        public static string GetDisplayName(MethodInfo info, object[] row)
        {
            row.Should().HaveCount(2);
            row[0].Should().BeOfType<string>();

            return $"{info.Name} - {row[0]}";
        }

        private static IEnumerable<object[]> GetResolvePathData()
        {
            // local function
            object[] CreateRelativePathRow(string path) => CreateRow(path, Path.Combine(Environment.CurrentDirectory, path));

            object[] CreateFullPathRow(string path) => CreateRow(path, path);

#if WINDOWS_BUILD
            yield return CreateFullPathRow(@"C:\test");
            yield return CreateFullPathRow(@"C:\test.json");
            yield return CreateFullPathRow(@"C:\test\foo");
            yield return CreateFullPathRow(@"C:\test\foo.bicep");
            yield return CreateFullPathRow(@"\\reddog\Builds\branches\git_mgmt_governance_blueprint_master\test.bicep");
            yield return CreateFullPathRow(@"C:\test");
            yield return CreateFullPathRow(@"C:\te st");
            yield return CreateFullPathRow(@"C:\te%20st");
            yield return CreateFullPathRow(@"C:\test\te%20st");
            yield return CreateFullPathRow(@"C:\test\te st");
            yield return CreateFullPathRow(@"C:\test\te%20st\foo.bicep");
            yield return CreateFullPathRow(@"C:\test\te st\foo.bicep");

            yield return CreateRelativePathRow(@"test");
            yield return CreateRelativePathRow(@"test.bicep");
            yield return CreateRelativePathRow(@"folder\test.bicep");
            yield return CreateRelativePathRow(@"deeper\folder\test.bicep");
            yield return CreateRelativePathRow(@"\deeper\folder\test.bicep");
#else
            yield return CreateFullPathRow(@"/lib");
            yield return CreateFullPathRow(@"/lib.bicep");
            yield return CreateFullPathRow(@"/lib/var/test.bicep");
            yield return CreateFullPathRow(@"/lib/var/something/test.bicep");

            yield return CreateRelativePathRow(@"test");
            yield return CreateRelativePathRow(@"test.bicep");
            yield return CreateRelativePathRow(@"folder/test.bicep");
            yield return CreateRelativePathRow(@"deeper/folder/test.bicep");
            yield return CreateRelativePathRow(@"/deeper/folder/test.bicep");
#endif
        }

        private static IEnumerable<object[]> GetFilePathToFileUrlData()
        {
            // local function
            object[] CreateFullPathRow(string path) => CreateRow(path, path);

#if WINDOWS_BUILD
            yield return CreateFullPathRow(@"C:\test");
            yield return CreateFullPathRow(@"C:\te st");
            yield return CreateFullPathRow(@"C:\te%20st");
            yield return CreateFullPathRow(@"C:\test\te%20st");
            yield return CreateFullPathRow(@"C:\test\te st");
            yield return CreateFullPathRow(@"C:\test\te%20st\foo.bicep");
            yield return CreateFullPathRow(@"C:\test\te st\foo.bicep");
            yield return CreateFullPathRow(@"C:\test\te%20st\foo.bicep");
            yield return CreateFullPathRow(@"C:\test\te%2Ast\foo.bicep");
            yield return CreateFullPathRow(@"C:\test\te%00st\foo.bicep");
            yield return CreateFullPathRow(@"C:\test\te%ZZst\foo.bicep");
#else
            yield return CreateFullPathRow(@"/lib/var/test");
            yield return CreateFullPathRow(@"/lib/var/te st");
            yield return CreateFullPathRow(@"/lib/var/te%20st");
            yield return CreateFullPathRow(@"/lib/var/test/te%20st");
            yield return CreateFullPathRow(@"/lib/var/test/te st");
            yield return CreateFullPathRow(@"/lib/var/test/te%20st/foo.bicep");
            yield return CreateFullPathRow(@"/lib/var/test/te st/foo.bicep");
            yield return CreateFullPathRow(@"/lib/var/test/te%20st/foo.bicep");
            yield return CreateFullPathRow(@"/lib/var/test/te%2Ast/foo.bicep");
            yield return CreateFullPathRow(@"/lib/var/test/te%00st/foo.bicep");
            yield return CreateFullPathRow(@"/lib/var/test/te%ZZst/foo.bicep");
#endif
        }

        private static IEnumerable<object[]> GetBuildOutputPathData()
        {
            yield return CreateRow(@"foo.bicep", @"foo.json");

#if LINUX_BUILD
            yield return CreateRow(@"/lib/bar/foo.bicep", @"/lib/bar/foo.json");

            // these will throw on Windows
            yield return CreateRow(@"/lib/bar/foo.JSON", @"/lib/bar/foo.json");
            yield return CreateRow(@"/bar/foo.jSoN", @"/bar/foo.json");
#else
            yield return CreateRow(@"C:\foo.bicep", @"C:\foo.json");
            yield return CreateRow(@"D:\a\b\c\foo.bicep", @"D:\a\b\c\foo.json");

            yield return CreateRow(@"/foo.bicep", @"/foo.json");
#endif
        }

        private static IEnumerable<object[]> GetDecompileOutputPathData()
        {
            yield return CreateRow(@"foo.json", @"foo.bicep");

#if LINUX_BUILD
            yield return CreateRow(@"/lib/bar/foo.json", @"/lib/bar/foo.bicep");

            // these will throw on Windows
            yield return CreateRow(@"/lib/bar/foo.BICEP", @"/lib/bar/foo.bicep");
            yield return CreateRow(@"/bar/foo.bIcEp", @"/bar/foo.bicep");
#else
            yield return CreateRow(@"C:\foo.json", @"C:\foo.bicep");
            yield return CreateRow(@"D:\a\b\c\foo.json", @"D:\a\b\c\foo.bicep");

            yield return CreateRow(@"/foo.json", @"/foo.bicep");
#endif
        }

        private static object[] CreateRow(string input, string expectedOutput) => new object[] { input, expectedOutput };
    }
}

