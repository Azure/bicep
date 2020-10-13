// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using Bicep.Cli.Utils;
using Bicep.Core.FileSystem;

namespace Bicep.Cli.FileSystem
{
    public class FileResolver : IFileResolver
    {
        public string GetNormalizedFileName(string fileName)
            => PathHelper.ResolvePath(fileName);

        public string? TryRead(string fileName, out string? failureMessage)
        {
            try
            {
                failureMessage = null;
                return File.ReadAllText(fileName);
            }
            catch (Exception exception)
            {
                // I/O classes typically throw a large variety of exceptions
                // instead of handling each one separately let's just trust the message we get
                failureMessage = exception.Message;
                return null;
            }
        }

        public string? TryResolveModulePath(string childFileName, string parentFileName)
        {
            if (Path.IsPathFullyQualified(childFileName))
            {
                return GetNormalizedFileName(childFileName);
            }

            var parentDirectoryName = Path.GetDirectoryName(parentFileName);
            if (parentDirectoryName == null)
            {
                return null;
            }

            if (Path.IsPathFullyQualified(parentDirectoryName))
            {
                return GetNormalizedFileName(Path.GetFullPath(childFileName, parentDirectoryName));
            }

            return GetNormalizedFileName(Path.Combine(parentDirectoryName, childFileName));
        }
    }
}

