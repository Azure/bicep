// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;

namespace Bicep.Core.FileSystem
{
    public class InMemoryFileResolver : IFileResolver
    {
        private readonly IReadOnlyDictionary<string, string> fileLookup;

        public InMemoryFileResolver(IReadOnlyDictionary<string, string> fileLookup)
        {
            this.fileLookup = fileLookup;
        }

        public string GetNormalizedFileName(string filePath)
            => filePath;

        public string? TryRead(string filePath, out string? failureMessage)
        {
            if (!fileLookup.TryGetValue(filePath, out var fileContents))
            {
                failureMessage = $"Could not find file {filePath}";
                return null;
            }

            failureMessage = null;
            return fileContents;
        }

        public string? TryResolveModulePath(string parentFileName, string childFileName)
            => Path.Combine(Path.GetDirectoryName(parentFileName), childFileName).Replace(Path.DirectorySeparatorChar, '/');

        public StringComparer PathComparer => StringComparer.Ordinal;
    }
}