// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.IO.UnitTests.Abstraction;

[TestClass]
public class FileSystemExceptionExtensionsTests
{
    [DataTestMethod]
    [DataRow(typeof(IOException), true, true)]
    [DataRow(typeof(UnauthorizedAccessException), true, true)]
    [DataRow(typeof(ArgumentException), false, true)]
    [DataRow(typeof(NotSupportedException), false, true)]
    [DataRow(typeof(InvalidOperationException), false, false)]
    public void Exception_classification_matches_the_supported_operations(
        Type exceptionType,
        bool isFileSystemException,
        bool isPathException)
    {
        var exception = (Exception)Activator.CreateInstance(exceptionType)!;

        exception.IsFileSystemException().Should().Be(isFileSystemException);
        exception.IsPathException().Should().Be(isPathException);
    }
}
