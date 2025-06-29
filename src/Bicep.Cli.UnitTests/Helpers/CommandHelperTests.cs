// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Helpers;
using Bicep.Core.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Cli.UnitTests.Helpers;

[TestClass]
public class CommandHelperTests
{
    [TestMethod]
    // unix path separator, pattern specified with "/"
    [DataRow("file.txt", '/', "", "file.txt")]
    [DataRow("path/to/file.txt", '/', "path/to", "file.txt")]
    [DataRow("/path/to/file.txt", '/', "/path/to", "file.txt")]
    [DataRow("/path/**/file.txt", '/', "/path", "**/file.txt")]
    [DataRow("/path/to/*.txt", '/', "/path/to", "*.txt")]
    [DataRow("/path/to/foo*.txt", '/', "/path/to", "foo*.txt")]
    [DataRow("/path/to/foo*bar/file.txt", '/', "/path/to", "foo*bar/file.txt")]
    // windows path separator, pattern specified with "/"
    // we intentionally still split on '/'
    [DataRow("file.txt", '\\', "", "file.txt")]
    [DataRow("path/to/file.txt", '\\', "path/to", "file.txt")]
    [DataRow("/path/to/file.txt", '\\', "/path/to", "file.txt")]
    [DataRow("/path/**/file.txt", '\\', "/path", "**/file.txt")]
    [DataRow("/path/to/*.txt", '\\', "/path/to", "*.txt")]
    [DataRow("/path/to/foo*.txt", '\\', "/path/to", "foo*.txt")]
    [DataRow("/path/to/foo*bar/file.txt", '\\', "/path/to", "foo*bar/file.txt")]
    // unix path separator, pattern contains "\"
    [DataRow("path/to\\file*.txt", '/', "path", "to\\file*.txt")]
    [DataRow("C:\\Users\\**\\*.bicep", '/', "", "C:\\Users\\**\\*.bicep")]
    [DataRow("C:\\path\\to\\file.bicep", '/', "", "C:\\path\\to\\file.bicep")]
    // windows path separator, pattern contains "\"
    [DataRow("path/to\\file*.txt", '\\', "path/to", "file*.txt")]
    [DataRow("C:\\Users\\**\\*.bicep", '\\', "C:\\Users", "**\\*.bicep")]
    [DataRow("C:\\path\\to\\file.bicep", '\\', "C:\\path\\to", "file.bicep")]
    // edge cases
    [DataRow("", '/', "", "")]
    [DataRow("/", '/', "", "")]
    [DataRow("abc/", '/', "abc", "")]
    [DataRow("/abc", '/', "", "abc")]
    [DataRow("*", '/', "", "*")]
    [DataRow("foo/*", '/', "foo", "*")]
    [DataRow("*/foo", '/', "", "*/foo")]
    [DataRow("C:", '\\', "", "C:")]
    [DataRow(":", '\\', "", ":")]
    [DataRow("**", '\\', "", "**")]
    public void SplitFilePatternOnWildcard_should_return_path_components(string filePattern, char osPathSeparator, string expectedFullPath, string expectedRemainingPath)
    {
        var (fullPath, remainingPath) = CommandHelper.SplitFilePatternOnWildcard(filePattern, osPathSeparator);
        using (new AssertionScope())
        {
            fullPath.Should().Be(expectedFullPath);
            remainingPath.Should().Be(expectedRemainingPath);
        }
    }
}
