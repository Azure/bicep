// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.IO.Abstraction;

public static class FileSystemExceptionExtensions
{
    public static bool IsFileSystemException(this Exception exception) =>
        exception is IOException or UnauthorizedAccessException;

    public static bool IsPathException(this Exception exception) =>
        exception.IsFileSystemException() ||
        exception is ArgumentException or NotSupportedException;
}
