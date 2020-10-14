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
        public void GetOutputPath_ShouldThrowOnJsonExtensions_Linux(string path)
        {
            Action badExtension = () => PathHelper.GetDefaultOutputPath(path);
            badExtension.Should().Throw<ArgumentException>().WithMessage("The specified file already already has the '.json' extension.");
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
        public void GetOutputPath_ShouldThrowOnJsonExtensions_WindowsAndMac(string path)
        {
            Action badExtension = () => PathHelper.GetDefaultOutputPath(path);
            badExtension.Should().Throw<ArgumentException>().WithMessage("The specified file already already has the '.json' extension.");
        }
#endif

        [DataTestMethod]
        [DynamicData(nameof(GetResolvePathData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void ResolvePath_ShouldResolveCorrectly(string path, string expectedPath)
        {
            PathHelper.ResolvePath(path).Should().Be(expectedPath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetOutputPathData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void GetOutputPath_ShouldChangeExtensionCorrectly(string path, string expectedPath)
        {
            PathHelper.GetDefaultOutputPath(path).Should().Be(expectedPath);
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

        private static IEnumerable<object[]> GetOutputPathData()
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

        private static object[] CreateRow(string input, string expectedOutput) => new object[] {input, expectedOutput};
    }
}

