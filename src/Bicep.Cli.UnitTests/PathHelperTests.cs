using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Bicep.Cli.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.UnitTests
{
    [TestClass]
    public class PathHelperTests
    {
#if WINDOWS_BUILD
        [TestMethod]
        public void WindowsFileSystem_ShouldBeCaseInsensitive()
        {
            PathHelper.PathComparison.Should().Be(StringComparison.OrdinalIgnoreCase);
            PathHelper.PathComparer.Should().BeSameAs(StringComparer.OrdinalIgnoreCase);
        }

        [DataTestMethod]
        [DataRow("foo.json")]
        [DataRow("foo.JSON")]
        [DataRow("foo.JsOn")]
        public void GetOutputPath_ShouldThrowOnJsonExtensions_Windows(string path)
        {
            Action badExtension = () => PathHelper.GetDefaultOutputPath(path);
            badExtension.Should().Throw<ArgumentException>().WithMessage("The specified file already already has the '.json' extension.");
        }
#else
        [TestMethod]
        public void NonWindowsFileSystem_ShouldBeCaseInsensitive()
        {
            PathHelper.PathComparison.Should().Be(StringComparison.Ordinal);
            PathHelper.PathComparer.Should().BeSameAs(StringComparer.Ordinal);
        }

        [DataTestMethod]
        [DataRow("foo.json")]
        public void GetOutputPath_ShouldThrowOnJsonExtensions_NonWindows(string path)
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
            yield return CreateFullPathRow(@"C:\test\foo.arm");
            yield return CreateFullPathRow(@"\\reddog\Builds\branches\git_mgmt_governance_blueprint_master\test.arm");
            yield return CreateFullPathRow(@"C:\test");

            yield return CreateRelativePathRow(@"test");
            yield return CreateRelativePathRow(@"test.arm");
            yield return CreateRelativePathRow(@"folder\test.arm");
            yield return CreateRelativePathRow(@"deeper\folder\test.arm");
            yield return CreateRelativePathRow(@"\deeper\folder\test.arm");
#else
            yield return CreateFullPathRow(@"/lib");
            yield return CreateFullPathRow(@"/lib.arm");
            yield return CreateFullPathRow(@"/lib/var/test.arm");
            yield return CreateFullPathRow(@"/lib/var/something/test.arm");

            yield return CreateRelativePathRow(@"test");
            yield return CreateRelativePathRow(@"test.arm");
            yield return CreateRelativePathRow(@"folder/test.arm");
            yield return CreateRelativePathRow(@"deeper/folder/test.arm");
            yield return CreateRelativePathRow(@"/deeper/folder/test.arm");
#endif
        }

        private static IEnumerable<object[]> GetOutputPathData()
        {
            yield return CreateRow(@"foo.arm", @"foo.json");

#if WINDOWS_BUILD
            yield return CreateRow(@"C:\foo.arm", @"C:\foo.json");
            yield return CreateRow(@"D:\a\b\c\foo.arm", @"D:\a\b\c\foo.json");
            
            yield return CreateRow(@"/foo.arm", @"/foo.json");

#else
            yield return CreateRow(@"/lib/bar/foo.arm", @"/lib/bar/foo.json");

            // these will throw on Windows
            yield return CreateRow(@"/lib/bar/foo.JSON", @"/lib/bar/foo.json");
            yield return CreateRow(@"/bar/foo.jSoN", @"/bar/foo.json");
#endif
        }

        private static object[] CreateRow(string input, string expectedOutput) => new object[] {input, expectedOutput};
    }
}
