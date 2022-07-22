// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using System.Text;
using System.IO;

namespace Bicep.Core.FileSystem
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

        public IDisposable? TryAcquireFileLock(Uri fileUri)
        {
            throw new NotImplementedException();
        }

        public bool TryRead(Uri fileUri, [NotNullWhen(true)] out string? fileContents, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (!fileLookup.TryGetValue(fileUri, out fileContents))
            {
                failureBuilder = x => x.ErrorOccurredReadingFile(missingFileFailureBuilder(fileUri));
                fileContents = null;
                return false;
            }
            failureBuilder = null;
            return true;
        }

        public bool TryRead(Uri fileUri, [NotNullWhen(true)] out string? fileContents, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder, Encoding fileEncoding, int maxCharacters, [NotNullWhen(true)] out Encoding? detectedEncoding)
        {
            if (!fileLookup.TryGetValue(fileUri, out fileContents))
            {
                failureBuilder = x => x.ErrorOccurredReadingFile(missingFileFailureBuilder(fileUri));
                fileContents = null;
                detectedEncoding = null;
                return false;
            }
            if (maxCharacters > 0)
            {
                if (fileContents.Length > maxCharacters)
                {
                    failureBuilder = x => x.FileExceedsMaximumSize(fileUri.LocalPath, maxCharacters, "characters");
                    fileContents = null;
                    detectedEncoding = null;
                    return false;
                }
            }
            failureBuilder = null;
            detectedEncoding = fileEncoding;
            return true;
        }

        public bool TryReadAtMostNCharaters(Uri fileUri, Encoding fileEncoding, int n, [NotNullWhen(true)] out string? fileContents)
        {
            if (!fileLookup.TryGetValue(fileUri, out fileContents))
            {
                fileContents = null;
                return false;
            }

            fileContents = new string(fileContents.Take(n).ToArray());
            return true;
        }

        public Uri? TryResolveFilePath(Uri parentFileUri, string childFilePath)
        {
            if (!Uri.TryCreate(parentFileUri, childFilePath, out var relativeUri))
            {
                return null;
            }

            return relativeUri;
        }

        public string GetRelativePath(string relativeTo, string path)
        {
            return Path.GetRelativePath(relativeTo, path).Replace('\\', '/');
        }

        public bool DirExists(Uri fileUri) => this.fileLookup.Keys.Any(key => key.ToString().StartsWith(fileUri.ToString()));

        public bool FileExists(Uri uri) => this.fileLookup.ContainsKey(uri);

        public IEnumerable<Uri> GetDirectories(Uri fileUri, string pattern)
        {
            if (!fileUri.IsFile)
            {
                return Enumerable.Empty<Uri>();
            }

            HashSet<Uri> dirUris = new();

            foreach (var knownFileUri in fileLookup.Keys)
            {
                if (fileUri.IsBaseOf(knownFileUri) && knownFileUri.Segments.Count() > fileUri.Segments.Count() + 1) 
                {
                    var IndexOfNextSlash = knownFileUri.AbsoluteUri.IndexOf("/", fileUri.AbsoluteUri.Length + 1);
                    var dirUri = new Uri(knownFileUri.AbsoluteUri.Substring(0, IndexOfNextSlash));
                    dirUris.Add(dirUri);
                }
            }
            return dirUris.ToList();
        }

        public IEnumerable<Uri> GetFiles(Uri fileUri, string pattern)
        {
            return fileLookup.Keys.Where(uri => fileUri.IsBaseOf(uri) && fileUri.Segments.Count() + 1 == uri.Segments.Count());
        }

        public bool TryReadAsBase64(Uri fileUri, [NotNullWhen(true)] out string? fileBase64, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder, int maxCharacters = -1)
        {
            if (!fileLookup.TryGetValue(fileUri, out var fileContents))
            {
                failureBuilder = x => x.ErrorOccurredReadingFile(missingFileFailureBuilder(fileUri));
                fileBase64 = null;
                return false;
            }

            failureBuilder = null;
            var bytes = Encoding.UTF8.GetBytes(fileContents);
            if (maxCharacters > 0)
            {
                var maxBytes = maxCharacters / 4 * 3; //each base64 character represents 6 bits
                if (bytes.Length > maxBytes)
                {
                    failureBuilder = x => x.FileExceedsMaximumSize(fileUri.LocalPath, maxBytes, "bytes");
                    fileBase64 = null;
                    return false;
                }
            }
            fileBase64 = Convert.ToBase64String(bytes);
            return true;
        }

        public void Write(Uri fileUri, Stream contents)
        {
            throw new NotImplementedException();
        }
    }
}
