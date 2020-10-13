// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;

namespace Bicep.Cli.Utils
{
    public static class PathHelper
    {
        private static readonly bool IsFileSystemCaseSensitive = CheckIfFileSystemIsCaseSensitive();

        private const string TemplateOutputExtension = ".json";

        public static StringComparer PathComparer => IsFileSystemCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

        public static StringComparison PathComparison => IsFileSystemCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Converts relative paths to absolute paths relative to current directory. Fully qualified paths are returned as-is.
        /// </summary>
        /// <param name="path">The path.</param>
        public static string ResolvePath(string path)
        {
            if (Path.IsPathFullyQualified(path))
            {
                return Path.GetFullPath(path);
            }

            return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, path));
        }

        public static string GetDefaultOutputPath(string path)
        {
            if (string.Equals(Path.GetExtension(path), TemplateOutputExtension, PathComparison))
            {
                // throwing because this could lead to us destroying the input file if extensions get mixed up.
                throw new ArgumentException($"The specified file already already has the '{TemplateOutputExtension}' extension.");
            }

            return Path.ChangeExtension(path, TemplateOutputExtension);
        }

        /// <summary>
        /// Returns true if the current file system is case sensitive (most Linux and MacOS X file systems). Returns false if the file system is case insensitive (Windows file systems.)
        /// </summary>
        private static bool CheckIfFileSystemIsCaseSensitive()
        {
            string tempUpperCasePath = Path.Combine(Path.GetTempPath(), $"BICEPFILESYSTEMTEST{Guid.NewGuid():N}");

            // file will be automatically deleted when stream is closed, which the using declaration will do when the variable goes out of scope
            using var stream = new FileStream(tempUpperCasePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);

            return File.Exists(tempUpperCasePath.ToLowerInvariant()) == false;
        }
    }
}

