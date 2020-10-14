// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.FileSystem
{
    public interface IFileResolver
    {
        /// <summary>
        /// Tries to read a file contents to string. If an exception is encoutered, returns null and sets a non-null failureMessage.
        /// </summary>
        /// <param name="filePath">The fully-qualified file path to read.</param>
        /// <param name="failureMessage">The failure message returned if reading the file failed.</param>
        string? TryRead(string filePath, out string? failureMessage);
        
        /// <summary>
        /// Returns a normalized absolute path. Relative paths are converted to absolute paths relative to current directory prior to normalization.
        /// </summary>
        /// <param name="filePath">The file path to nornalize.</param>
        string GetNormalizedFileName(string filePath);

        /// <summary>
        /// Tries to resolve a child file path relative to a parent module file path.
        /// </summary>
        /// <param name="parentFilePath">The file path of the parent. Must be a fully-qualified path.</param>
        /// <param name="childFilePath">The file path of the child.</param>
        string? TryResolveModulePath(string parentFilePath, string childFilePath);

        /// <summary>
        /// The string comparer to use for normalized path comparisons.
        /// </summary>
        StringComparer PathComparer { get; }
    }
}