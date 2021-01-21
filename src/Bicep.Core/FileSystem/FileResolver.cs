// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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

        public IEnumerable<Uri> GetDirectories(Uri fileUri, string pattern="")
        {
            if (!fileUri.IsFile) 
            {
                return Enumerable.Empty<Uri>();
            }
            if (pattern == string.Empty)
            {
                return Directory.GetDirectories(fileUri.LocalPath).Select(s => new Uri(s + "/"));
            }
            else 
            {
                return Directory.GetDirectories(fileUri.LocalPath, pattern).Select(s => new Uri(s + "/"));
            }    
        }

        public IEnumerable<Uri> GetFiles(Uri fileUri, string pattern)
        {
            if (!fileUri.IsFile) 
            {
                return Enumerable.Empty<Uri>();
            }
            return Directory.GetFiles(fileUri.LocalPath, pattern).Select(s => new Uri(s));
        }

        public bool DirExists(Uri fileUri)
        {
            return fileUri.IsFile && Directory.Exists(fileUri.LocalPath);
        }

        public bool FileExists(Uri fileUri)
        {
            return fileUri.IsFile && File.Exists(fileUri.LocalPath);
        }

        public Uri GetParentDirectory(Uri uri)
        {
            return new Uri(uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length));
        }
    }
}