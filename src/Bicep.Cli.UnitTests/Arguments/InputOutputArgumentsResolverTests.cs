// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Core.Utils;
using Bicep.TextFixtures.Mocks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Cli.UnitTests.Arguments;

[TestClass]
public class InputOutputArgumentsResolverTests
{
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
        result.IsLocalFile.Should().BeTrue();
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
        result.IsLocalFile.Should().BeTrue();
        mockPath.Verify(p => p.GetFullPath(path), Times.Once);
    }
}
