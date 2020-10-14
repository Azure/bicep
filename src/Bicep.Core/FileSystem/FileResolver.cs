// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using Bicep.Core.FileSystem;

namespace Bicep.Core.FileSystem
{
    public class FileResolver : IFileResolver
    {
        public string GetNormalizedFileName(string filePath)
            => PathHelper.ResolveAndNormalizePath(filePath);

        public string? TryRead(string filePath, out string? failureMessage)
        {
            try
            {
                failureMessage = null;
                return File.ReadAllText(filePath);
            }
            catch (Exception exception)
            {
                // I/O classes typically throw a large variety of exceptions
                // instead of handling each one separately let's just trust the message we get
                failureMessage = exception.Message;
                return null;
            }
        }

        public string? TryResolveModulePath(string parentFilePath, string childFilePath)
        {
            if (Path.IsPathFullyQualified(childFilePath))
            {
                return GetNormalizedFileName(childFilePath);
            }

            var parentDirectoryName = Path.GetDirectoryName(parentFilePath);
            if (parentDirectoryName == null || !Path.IsPathFullyQualified(parentDirectoryName))
            {
                return null;
            }

            return GetNormalizedFileName(Path.GetFullPath(childFilePath, parentDirectoryName));
        }

        public StringComparer PathComparer => PathHelper.PathComparer;
    }
}