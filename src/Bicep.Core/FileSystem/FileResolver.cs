// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.FileSystem
{
    public class FileResolver : IFileResolver
    {
        public bool TryRead(Uri fileUri, [NotNullWhen(true)] out string? fileContents, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (!fileUri.IsFile)
            {
                failureBuilder = x => x.UnableToLoadNonFileUri(fileUri);
                fileContents = null;
                return false;
            }

            try
            {
                failureBuilder = null;
                fileContents = File.ReadAllText(fileUri.LocalPath);
                return true;
            }
            catch (Exception exception)
            {
                // I/O classes typically throw a large variety of exceptions
                // instead of handling each one separately let's just trust the message we get
                failureBuilder = x => x.ErrorOccurredReadingFile(exception.Message);
                fileContents = null;
                return false;
            }
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