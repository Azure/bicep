// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;

namespace Bicep.Core.FileSystem
{
    public static class PathHelper
    {
        private static readonly bool IsFileSystemCaseSensitive = CheckIfFileSystemIsCaseSensitive();

        private const string TemplateOutputExtension = ".json";

        private const string BicepExtension = LanguageConstants.LanguageFileExtension;

        private const string BicepParamsExtension = LanguageConstants.ParamsFileExtension;

        public static StringComparer PathComparer => IsFileSystemCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

        public static StringComparison PathComparison => IsFileSystemCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Converts relative paths to absolute paths relative to current directory. Fully qualified paths are returned as-is.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="baseDirectory">The base directory to use when resolving relative paths. Set to null to use CWD.</param>
        public static string ResolvePath(string path, string? baseDirectory = null)
        {
            if (Path.IsPathFullyQualified(path))
            {
                return path;
            }

            baseDirectory ??= Environment.CurrentDirectory;
            return Path.Combine(baseDirectory, path);
        }

        /// <summary>
        /// Returns a normalized absolute path. Relative paths are converted to absolute paths relative to current directory prior to normalization.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="baseDirectory">The base directory to use when resolving relative paths. Set to null to use CWD.</param>
        public static string ResolveAndNormalizePath(string path, string? baseDirectory = null)
        {
            var resolvedPath = ResolvePath(path, baseDirectory);

            return Path.GetFullPath(resolvedPath);
        }

        public static string ResolveDefaultOutputPath(string inputPath, string? outputDir, string? outputFile, Func<string, string> defaultOutputPath)
        {
            if (outputDir is not null)
            {
                var dir = ResolvePath(outputDir);
                var file = Path.GetFileName(inputPath);
                var path = Path.Combine(dir, file);

                return defaultOutputPath(path);
            }
            else if (outputFile is not null)
            {
                return ResolvePath(outputFile);
            }
            else
            {
                return defaultOutputPath(inputPath);
            }
        }

        public static string ResolveParametersFileOutputPath(string path)
        {
            var folder = ResolvePath(path);

            var pathWithoutFileName = Path.GetDirectoryName(folder);

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);

            if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                fileNameWithoutExtension = "output";
            }

            var outputPath = $"{pathWithoutFileName}{Path.DirectorySeparatorChar}{fileNameWithoutExtension}.parameters.json";

            return outputPath;
        }

        public static string GetDefaultBuildOutputPath(string path)
        {
            if (string.Equals(Path.GetExtension(path), TemplateOutputExtension, PathComparison))
            {
                // throwing because this could lead to us destroying the input file if extensions get mixed up.
                throw new ArgumentException($"The specified file already has the '{TemplateOutputExtension}' extension.");
            }

            return Path.ChangeExtension(path, TemplateOutputExtension);
        }
        public static string GetDefaultDecompileOutputPath(string path)
        {
            if (string.Equals(Path.GetExtension(path), BicepExtension, PathComparison))
            {
                // throwing because this could lead to us destroying the input file if extensions get mixed up.
                throw new ArgumentException($"The specified file already has the '{BicepExtension}' extension.");
            }

            return Path.ChangeExtension(path, BicepExtension);
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

        public static Uri FilePathToFileUrl(string filePath)
        {
            filePath = filePath.Replace(Path.DirectorySeparatorChar, '/');
            if (!filePath.StartsWith('/'))
            {
                filePath = "/" + filePath;
            }
            filePath = filePath.Replace("%", "%25");

            var uriBuilder = new UriBuilder
            {
                Scheme = "file",
                Host = null,
                Path = filePath,
            };

            return uriBuilder.Uri;
        }
        
        public static Uri ChangeExtension(Uri uri, string? newExtension)
        {
            var uriString = uri.ToString();
            var finalDotIndex = uriString.LastIndexOf('.');

            newExtension = newExtension is null ? "" : NormalizeExtension(newExtension);
            uriString = (finalDotIndex >= 0 ? uriString.Substring(0, finalDotIndex) : uriString) + newExtension;

            return new Uri(uriString);
        }

        public static bool HasAnyExtension(Uri uri)
        {
            var path = GetNormalizedPath(uri);

            return Path.HasExtension(path);
        }

        public static bool HasExtension(Uri uri, string extension)
        {
            var path = GetNormalizedPath(uri);
            extension = NormalizeExtension(extension);

            return path.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
        }

        public static Uri RemoveExtension(Uri uri) => ChangeExtension(uri, null);

        public static Uri ChangeToBicepExtension(Uri uri) => ChangeExtension(uri, BicepExtension);

        public static bool HasBicepExtension(Uri uri) => HasExtension(uri, BicepExtension);

        public static bool HasBicepparamsExension(Uri uri) => HasExtension(uri, BicepParamsExtension);

        public static bool HasArmTemplateLikeExtension(Uri uri) =>
                HasExtension(uri, LanguageConstants.JsonFileExtension) ||
                HasExtension(uri, LanguageConstants.JsoncFileExtension) ||
                HasExtension(uri, LanguageConstants.ArmTemplateFileExtension);

        private static string GetNormalizedPath(Uri uri) =>
            uri.Scheme != Uri.UriSchemeFile ? uri.AbsoluteUri.TrimEnd('/') : uri.AbsolutePath;

        private static string NormalizeExtension(string extension) =>
            extension.StartsWith(".") ? extension : $".{extension}";
    }
}
