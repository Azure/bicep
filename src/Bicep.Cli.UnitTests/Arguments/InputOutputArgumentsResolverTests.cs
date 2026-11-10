// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.Testing.Mocks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Cli.UnitTests.Arguments;

[TestClass]
public class InputOutputArgumentsResolverTests
{
    [TestMethod]
    [DataRow("**/*.bicep", "a.bicep,nested/b.bicep,z.bicep")]
    [DataRow("*.bicep", "a.bicep,z.bicep")]
    [DataRow("nested/*.bicep", "nested/b.bicep")]
    [DataRow("*/b.bicep", "nested/b.bicep")]
    [DataRow("**/b.bicep", "nested/b.bicep")]
    [DataRow("a.bicep", "a.bicep")]
    [DataRow("missing/**/*.bicep", "")]
    public void ResolveFilePatternInputArguments_MatchingFilesInInjectedFileSystem_ReturnsSortedMatches(string pattern, string expected)
    {
        var fileSystem = new MockFileSystem();
        var root = fileSystem.Path.GetFullPath("/virtual-pattern-root/");
        fileSystem.Directory.CreateDirectory(root);
        fileSystem.Directory.SetCurrentDirectory(root);
        fileSystem.AddFile(fileSystem.Path.Combine(root, "z.bicep"), "");
        fileSystem.AddFile(fileSystem.Path.Combine(root, "nested", "b.bicep"), "");
        fileSystem.AddFile(fileSystem.Path.Combine(root, "a.bicep"), "");
        fileSystem.AddFile(fileSystem.Path.Combine(root, "ignored.json"), "");
        var resolver = new InputOutputArgumentsResolver(fileSystem);

        var paths = resolver.ResolveFilePatternInputArguments(new RestoreArguments(null, pattern, false))
            .Select(uri => uri.GetPathRelativeTo(IOUri.FromFilePath(root)));

        paths.Should().Equal(expected.Split(',', StringSplitOptions.RemoveEmptyEntries));
    }

    [TestMethod]
    public void ResolveFilePatternInputArguments_DirectoryHasReparsePointAttribute_IncludesMatches()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/root/kept/main.bicep", "");
        fileSystem.AddFile("/root/linked/main.bicep", "");
        fileSystem.File.SetAttributes("/root/linked", FileAttributes.Directory | FileAttributes.ReparsePoint);
        var resolver = new InputOutputArgumentsResolver(fileSystem);

        var rootUri = IOUri.FromFilePath(fileSystem.Path.GetFullPath("/root/"));
        var paths = resolver.ResolveFilePatternInputArguments(new RestoreArguments(null, "/root/**/*.bicep", false))
            .Select(uri => uri.GetPathRelativeTo(rootUri));

        paths.Should().Equal("kept/main.bicep", "linked/main.bicep");
    }

    [TestMethod]
    public void ResolveFilePatternInputArguments_NonRecursivePattern_DoesNotEnumerateChildDirectories()
    {
        var paths = new MockFileSystem();
        var rootPath = paths.Path.GetFullPath("/root/");
        var fileSystem = StrictMock.Of<IFileSystem>();
        var factory = StrictMock.Of<IDirectoryInfoFactory>();
        var root = StrictMock.Of<IDirectoryInfo>();
        var child = StrictMock.Of<IDirectoryInfo>();
        var file = StrictMock.Of<IFileInfo>();
        fileSystem.SetupGet(system => system.Path).Returns(paths.Path);
        fileSystem.SetupGet(system => system.DirectoryInfo).Returns(factory.Object);
        factory.Setup(info => info.New(rootPath)).Returns(root.Object);
        root.SetupGet(info => info.Exists).Returns(true);
        root.SetupGet(info => info.FullName).Returns(rootPath);
        root.Setup(info => info.EnumerateFileSystemInfos()).Returns([child.Object, file.Object]);
        child.SetupGet(info => info.Name).Returns("nested");
        file.SetupGet(info => info.Name).Returns("main.bicep");
        var resolver = new InputOutputArgumentsResolver(fileSystem.Object);

        var matches = resolver.ResolveFilePatternInputArguments(new RestoreArguments(null, "/root/*.bicep", false));

        matches.Should().Equal(IOUri.FromFilePath(paths.Path.Combine(rootPath, "main.bicep")));
        child.Verify(info => info.EnumerateFileSystemInfos(), Times.Never);
    }

    [TestMethod]
    public void ResolveFilePatternInputOutputArguments_OutputDirectorySpecified_PreservesRelativeOutputPaths()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/root/a.bicep", "");
        fileSystem.AddFile("/root/nested/b.bicep", "");
        var resolver = new InputOutputArgumentsResolver(fileSystem);
        var arguments = new BuildArguments(null, false, false, "/output", null, "/root/**/*.bicep", null);

        var matches = resolver.ResolveFilePatternInputOutputArguments(arguments);

        matches.Select(match => match.OutputUri).Should().Equal(
            IOUri.FromFilePath(fileSystem.Path.GetFullPath("/output/a.json")),
            IOUri.FromFilePath(fileSystem.Path.GetFullPath("/output/nested/b.json")));
    }

    [TestMethod]
    [DataRow("*.bicep", "")]
    [DataRow("*/main.bicep", "link/main.bicep,target/main.bicep")]
    [DataRow("**/*.bicep", "link/main.bicep,target/main.bicep")]
    public void ResolveFilePatternInputArguments_DirectorySymbolicLink_MatchesExistingGlobbingBehavior(string pattern, string expectedPaths)
    {
        var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var target = Path.Combine(root, "target");
        Directory.CreateDirectory(target);
        try
        {
            File.WriteAllText(Path.Combine(target, "main.bicep"), "");
            try
            {
                Directory.CreateSymbolicLink(Path.Combine(root, "link"), target);
            }
            catch (UnauthorizedAccessException) when (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Creating symbolic links requires Developer Mode or elevated privileges on Windows.");
            }

            var matcher = new Matcher();
            matcher.AddInclude(pattern);
            var expected = matcher.GetResultsInFullPath(root)
                .Select(IOUri.FromFilePath)
                .OrderBy(uri => uri.Path, StringComparer.Ordinal);
            var resolver = new InputOutputArgumentsResolver(new FileSystem());

            var matches = resolver.ResolveFilePatternInputArguments(new RestoreArguments(null, Path.Combine(root, pattern), false));

            matches.Should().Equal(expected);
            var rootUri = IOUri.FromFilePath(root + Path.DirectorySeparatorChar);
            matches.Select(uri => uri.GetPathRelativeTo(rootUri))
                .Should().Equal(expectedPaths.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    // Forward slash patterns - should work on all platforms
    [DataRow("file.txt", "", "file.txt")]
    [DataRow("path/to/file.txt", "path/to", "file.txt")]
    [DataRow("/path/to/file.txt", "/path/to", "file.txt")]
    [DataRow("/path/**/file.txt", "/path", "**/file.txt")]
    [DataRow("/path/to/*.txt", "/path/to", "*.txt")]
    [DataRow("/path/to/foo*.txt", "/path/to", "foo*.txt")]
    [DataRow("/path/to/foo*bar/file.txt", "/path/to", "foo*bar/file.txt")]
    // Edge cases
    [DataRow("", "", "")]
    [DataRow("/", "", "")]
    [DataRow("abc/", "abc", "")]
    [DataRow("/abc", "", "abc")]
    [DataRow("*", "", "*")]
    [DataRow("foo/*", "foo", "*")]
    [DataRow("*/foo", "", "*/foo")]
    [DataRow("C:", "", "C:")]
    [DataRow(":", "", ":")]
    [DataRow("**", "", "**")]
    public void SplitFilePatternOnWildcard_should_return_path_components(string filePattern, string expectedFullPath, string expectedRemainingPath)
    {
        var fileSystem = new MockFileSystem();
        var inputOutputArgumentsResolver = new InputOutputArgumentsResolver(fileSystem);

        var (fullPath, remainingPath) = inputOutputArgumentsResolver.SplitFilePatternOnWildcard(filePattern);

        using (new AssertionScope())
        {
            expectedFullPath = expectedFullPath.Length == 0 ? fileSystem.Directory.GetCurrentDirectory() : fileSystem.Path.GetFullPath(expectedFullPath);
            fullPath.TrimEnd(fileSystem.Path.DirectorySeparatorChar).Should().Be(expectedFullPath.TrimEnd(fileSystem.Path.DirectorySeparatorChar));
            remainingPath.Should().Be(expectedRemainingPath);
        }
    }

#if !WINDOWS_BUILD
    [TestMethod]
    [DataRow("path\\to\\file*.txt")]
    [DataRow("C:\\Users\\**\\*.bicep")]
    [DataRow("C:\\path\\to\\file.bicep")]
    [DataRow("path/to\\file*.txt")]
    [DataRow("folder\\subfolder\\*.json")]
    public void SplitFilePatternOnWildcard_should_throw_CommandLineException_when_pattern_contains_backslash_on_non_windows(string filePatternWithBackslash)
    {
        // Arrange
        var resolver = new InputOutputArgumentsResolver(new FileSystem());

        // Act & Assert
        FluentActions.Invoking(() => resolver.SplitFilePatternOnWildcard(filePatternWithBackslash))
            .Should()
            .Throw<CommandLineException>()
            .WithMessage($"The file pattern '{filePatternWithBackslash}' contains '\\'. Bicep does not support '\\' in file paths on non-Windows platforms when used as command-line inputs.");
    }
#else
    [TestMethod]
    // Windows platform - backslashes should be treated as path separators
    [DataRow("path\\to\\file*.txt", "path\\to\\", "file*.txt")]
    [DataRow("C:\\Users\\**\\*.bicep", "C:\\Users\\", "**\\*.bicep")]
    [DataRow("C:\\path\\to\\file.bicep", "C:\\path\\to\\", "file.bicep")]
    [DataRow("path/to\\file*.txt", "path/to/", "file*.txt")] // Mixed separators
    public void SplitFilePatternOnWildcard_should_handle_backslash_patterns_on_windows(string filePattern, string expectedFullPath, string expectedRemainingPath)
    {
        var fileSystem = new MockFileSystem();
        var inputOutputArgumentsResolver = new InputOutputArgumentsResolver(fileSystem);

        var (fullPath, remainingPath) = inputOutputArgumentsResolver.SplitFilePatternOnWildcard(filePattern);

        using (new AssertionScope())
        {
            expectedFullPath = expectedFullPath.Length == 0 ? fileSystem.Directory.GetCurrentDirectory() : fileSystem.Path.GetFullPath(expectedFullPath);
            fullPath.TrimEnd(fileSystem.Path.DirectorySeparatorChar).Should().Be(expectedFullPath.TrimEnd(fileSystem.Path.DirectorySeparatorChar));
            remainingPath.Should().Be(expectedRemainingPath);
        }
    }
#endif

#if !WINDOWS_BUILD
    [TestMethod]
    [DataRow("path\\with\\backslash.bicep")]
    [DataRow("C:\\Windows\\file.txt")]
    [DataRow("folder\\subfolder\\file.json")]
    [DataRow("\\\\server\\share\\file.bicep")]
    [DataRow("single\\backslash.txt")]
    public void PathToUri_should_throw_CommandLineException_when_path_contains_backslash_on_non_windows(string pathWithBackslash)
    {
        // Arrange
        var resolver = new InputOutputArgumentsResolver(new FileSystem());

        // Act & Assert
        FluentActions.Invoking(() => resolver.PathToUri(pathWithBackslash))
            .Should()
            .Throw<CommandLineException>()
            .WithMessage($"The file path '{pathWithBackslash}' contains '\\'. Bicep does not support '\\' in file paths on non-Windows platforms when used as command-line inputs.");
    }
#else
    [TestMethod]
    [DataRow("C:/Windows\\file.txt")]
    [DataRow("path\\with\\backslash.bicep")]
    [DataRow("folder\\subfolder\\file.json")]
    [DataRow("single\\backslash.txt")]
    public void PathToUri_should_accept_backslash_paths_on_windows(string windowsPath)
    {
        // Arrange
        var mockFileSystem = StrictMock.Of<IFileSystem>();
        var mockPath = StrictMock.Of<IPath>();
        mockFileSystem.Setup(fs => fs.Path).Returns(mockPath.Object);
        mockPath.Setup(p => p.GetFullPath(It.IsAny<string>())).Returns((string p) => Path.GetFullPath(p));
        
        var resolver = new InputOutputArgumentsResolver(mockFileSystem.Object);

        // Act
        var result = resolver.PathToUri(windowsPath);

        // Assert
        result.Should().NotBeNull();
        result.IsFile.Should().BeTrue();
        mockPath.Verify(p => p.GetFullPath(windowsPath), Times.Once);
    }
#endif

    [TestMethod]
    [DataRow("path/with/backslash.bicep")]
    [DataRow("folder/subfolder/file.json")]
    [DataRow("single/backslash.txt")]
    public void PathToUri_should_accept_slash_on_all_platforms(string path)
    {
        // Arrange
        var mockFileSystem = StrictMock.Of<IFileSystem>();
        var mockPath = StrictMock.Of<IPath>();
        mockFileSystem.Setup(fs => fs.Path).Returns(mockPath.Object);
        mockPath.Setup(p => p.GetFullPath(It.IsAny<string>())).Returns((string p) => Path.GetFullPath(p));

        var resolver = new InputOutputArgumentsResolver(mockFileSystem.Object);

        // Act
        var result = resolver.PathToUri(path);

        // Assert
        result.Should().NotBeNull();
        result.IsFile.Should().BeTrue();
        mockPath.Verify(p => p.GetFullPath(path), Times.Once);
    }
}
