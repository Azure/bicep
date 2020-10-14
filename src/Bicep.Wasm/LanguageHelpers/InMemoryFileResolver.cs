// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.FileSystem;

namespace Bicep.Wasm.LanguageHelpers
{
    public class InMemoryFileResolver : IFileResolver
    {
        private readonly IReadOnlyDictionary<string, string> fileLookup;

        public InMemoryFileResolver(IReadOnlyDictionary<string, string> fileLookup)
        {
            this.fileLookup = fileLookup;
        }

        public string GetNormalizedFileName(string fileName)
            => fileName;

        public string? TryRead(string fileName, out string? failureMessage)
        {
            if (!fileLookup.TryGetValue(fileName, out var fileContents))
            {
                failureMessage = $"Failed to find {fileName}";
                return null;
            }

            failureMessage = null;
            return fileContents;
        }

        public string? TryResolveModulePath(string parentFileName, string childFileName)
            => childFileName;

        public StringComparer PathComparer => StringComparer.OrdinalIgnoreCase;
    }
}