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
        private readonly Func<string, string> missingFileFailureBuilder;

        public InMemoryFileResolver(IReadOnlyDictionary<string, string> fileLookup, Func<string, string>? missingFileFailureBuilder = null)
        {
            this.fileLookup = fileLookup;
            this.missingFileFailureBuilder = missingFileFailureBuilder ?? (filePath => $"Could not find file {filePath}");
        }

        public string GetNormalizedFileName(string filePath)
            => filePath;

        public string? TryRead(string filePath, out string? failureMessage)
        {
            if (!fileLookup.TryGetValue(filePath, out var fileContents))
            {
                failureMessage = missingFileFailureBuilder(filePath);
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