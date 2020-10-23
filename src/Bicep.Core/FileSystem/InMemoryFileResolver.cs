// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;

namespace Bicep.Core.FileSystem
{
    public class InMemoryFileResolver : IFileResolver
    {
        private readonly IReadOnlyDictionary<string, string> fileLookup;
        private readonly Func<string, string> missingFileFailureBuilder;

        public InMemoryFileResolver(IReadOnlyDictionary<string, string> fileLookup, Func<string, string>? missingFileFailureBuilder = null)
        {
            foreach (var (filePath, fileContents) in fileLookup)
            {
                AssertPathFullyQualified(filePath);
            }

            this.fileLookup = fileLookup;
            this.missingFileFailureBuilder = missingFileFailureBuilder ?? (filePath => $"Could not find file {filePath}");
        }

        public string GetNormalizedFileName(string filePath)
            => new Uri($"file://{filePath}").AbsolutePath;

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

        public string? TryResolveModulePath(string parentFilePath, string childFilePath)
        {
            AssertPathFullyQualified(parentFilePath);

            var parentDirUri = new Uri(new Uri($"file://{parentFilePath}"), ".");
            var moduleUri = new Uri(parentDirUri, childFilePath);

            return moduleUri.AbsolutePath;
        }

        public StringComparer PathComparer => StringComparer.Ordinal;

        private static void AssertPathFullyQualified(string filePath)
        {
            if (!filePath.StartsWith("/"))
            {
                throw new InvalidOperationException($"Expected a fully qualified path, but got \"{filePath}\"");
            }
        }
    }
}