// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;

namespace Bicep.Core.UnitTests.FileSystem
{
    public class InMemoryFileResolver : IFileResolver
    {
        private readonly IReadOnlyDictionary<Uri, string> fileLookup;
        private readonly Func<Uri, string> missingFileFailureBuilder;

        public InMemoryFileResolver(IReadOnlyDictionary<Uri, string> fileLookup, Func<Uri, string>? missingFileFailureBuilder = null)
        {
            this.fileLookup = fileLookup;
            this.missingFileFailureBuilder = missingFileFailureBuilder ?? (fileUri => $"Could not find file \"{fileUri.LocalPath}\"");
        }

        public bool TryRead(Uri fileUri, [NotNullWhen(true)] out string? fileContents, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (!fileLookup.TryGetValue(fileUri, out fileContents))
            {
                failureBuilder = x => x.ErrorOccurredLoadingModule(missingFileFailureBuilder(fileUri));
                fileContents = null;
                return false;
            }

            failureBuilder = null;
            return true;
        }

        public Uri? TryResolveModulePath(Uri parentFileUri, string childFilePath)
        {
            if (!Uri.TryCreate(parentFileUri, childFilePath, out var relativeUri))
            {
                return null;
            }

            return relativeUri;
        }
    }
}