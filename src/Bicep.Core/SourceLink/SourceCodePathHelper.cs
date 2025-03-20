// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.FileSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.SourceCode
{
    public static class SourceCodePathHelper
    {
        public static string Shorten(string path, int maxLength)
        {
            if (path.Length <= maxLength)
            {
                return path;
            }

            var extension = Path.GetExtension(path) ?? string.Empty;
            var tail = "__path_too_long__" + extension.Substring(0, Math.Min(10, extension.Length));
            var shortPath = path.Substring(0, maxLength - tail.Length) + tail;
            Debug.Assert(shortPath.Length == maxLength);
            return shortPath;
        }

        /// <summary>
        /// Finds the list of all distinctFolders in the given pathsArray that are not subfolders of any path,
        /// i.e. the smallest set of distinctFolders so that you can express any of the pathsArray as relative
        /// to one of the roots, without having to use "..".
        ///
        /// Excluding then handling of files under the cache root, each given path will be under one and only one root.
        ///
        /// The intention is to keep only the portion of the source user paths that is necessary.
        /// <returns>
        /// A mapping of the original paths to the root path that they should be relative to.
        /// </returns>
        /// </summary>
        /// <example>
        ///
        ///   c:/users/username/repos/deployment/src/main.bicep
        ///   c:/users/username/repos/deployment/src/modules/module1.bicep
        ///   c:/users/username/repos/deployment/src/modules/module2.bicep
        ///   c:/users/username/repos/deployment/shared/shared1.bicep
        ///   d:/repos/deployment/main.json
        ///
        /// the calculated distinct roots are:
        ///
        ///   c:/users/username/repos/deployment/src
        ///   c:/users/username/repos/deployment/shared
        ///   d:/repos/deployment
        ///
        /// so the returned map is:
        ///
        ///   c:/users/username/repos/deployment/src/main.bicep            => c:/users/username/repos/deployment/src
        ///   c:/users/username/repos/deployment/src/modules/module1.bicep => c:/users/username/repos/deployment/src
        ///   c:/users/username/repos/deployment/src/modules/module2.bicep => c:/users/username/repos/deployment/src
        ///   c:/users/username/repos/deployment/shared/shared1.bicep      => c:/users/username/repos/deployment/shared
        ///   d:/repos/deployment/main.json                                => d:/repos/deployment
        ///   
        /// </example>
        public static IDictionary<string, string> MapPathsToDistinctRoots(string? cacheRoot, string[] filePaths)
        {
            if (filePaths.Distinct().Count() != filePaths.Length)
            {
                throw new ArgumentException($"Paths should be distinct before calling {nameof(MapPathsToDistinctRoots)}");
            }

            if (filePaths.Any(p => p.Contains('\\')))
            {
                throw new ArgumentException($"Paths should be normalized before calling {nameof(MapPathsToDistinctRoots)}");
            }

            string[] folders = [.. filePaths.Select(path =>
            {
                if (!Path.IsPathFullyQualified(path) || Path.GetDirectoryName(path) is not string folder)
                {
                    throw new ArgumentException($"Path '{path}' should be a valid fully qualified path");
                }

                return folder;
            })
            .Select(NormalizeSlashes)];

            var distinctFolders = folders.Distinct(PathHelper.PathComparer).ToArray();

            var distinctRoots = distinctFolders.Where(path =>
                !distinctFolders.Any(path2 => path != path2 && IsDescendentOf(path, path2))
            ).ToArray();

            // Map path -> root to return
            var rootMapping = filePaths.Select(
                    filePath =>
                    {
                        var fileFolder = Path.GetDirectoryName(filePath)!; // (GetDirectoryName should always return non-null for a file path)
                        if (!string.IsNullOrEmpty(cacheRoot) && IsSameOrIsDescendentOf(fileFolder, cacheRoot))
                        {
                            // Treat everything under cacheRoot the same as cacheRoot to avoid having a root for each referenced module and for better source viewing
                            return (filePath, cacheRoot);
                        }

                        var matchingRoots = distinctRoots.Where(r => IsSameOrIsDescendentOf(fileFolder, r)).ToArray();
                        if (matchingRoots.Length != 1)
                        {
                            throw new ArgumentException($"{nameof(MapPathsToDistinctRoots)}: Path '{filePath}' should be under exactly one root");
                        }

                        return (filePath, matchingRoots.Single());
                    }
                );

            return rootMapping.ToDictionary(pair => pair.Item1, pair => pair.Item2);
        }

        public static string NormalizeSlashes(string path)
        {
            return path.Replace('\\', '/');
        }

        private static bool IsDescendentOf(string folder, string possibleParentFolder)
        {
            return folder != possibleParentFolder
                && IsSameOrIsDescendentOf(folder, possibleParentFolder);
        }

        private static bool IsSameOrIsDescendentOf(string folder, string possibleParentFolder)
        {
            var relativePath = NormalizeSlashes(Path.GetRelativePath(possibleParentFolder, folder));
            return relativePath != ".."
                && !relativePath.StartsWith("../")
                && PathHelper.PathComparer.Equals(Path.GetPathRoot(folder), Path.GetPathRoot(possibleParentFolder));
        }
    }
}
