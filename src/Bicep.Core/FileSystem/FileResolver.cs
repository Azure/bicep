// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.FileSystem
{
    public class FileResolver : IFileResolver
    {
        public IDisposable? TryAcquireFileLock(Uri fileUri)
        {
            RequireFileUri(fileUri);
            return FileLock.TryAcquire(fileUri.LocalPath);
        }

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
                if (Directory.Exists(fileUri.LocalPath))
                {
                    // Docs suggest this is the error to throw when we give a directory.
                    // A trailing backslash causes windows not to throw this exception.
                    throw new UnauthorizedAccessException($"Access to the path '{fileUri.LocalPath}' is denied.");
                }
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

        public bool TryRead(Uri fileUri, [NotNullWhen(true)] out string? fileContents, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder, Encoding fileEncoding, int maxCharacters, [NotNullWhen(true)] out Encoding? detectedEncoding)
        {
            if (!fileUri.IsFile)
            {
                failureBuilder = x => x.UnableToLoadNonFileUri(fileUri);
                fileContents = null;
                detectedEncoding = null;
                return false;
            }

            try
            {
                failureBuilder = null;
                if (Directory.Exists(fileUri.LocalPath))
                {
                    // Docs suggest this is the error to throw when we give a directory.
                    // A trailing backslash causes windows not to throw this exception.
                    throw new UnauthorizedAccessException($"Access to the path '{fileUri.LocalPath}' is denied.");
                }
                using var fileStream = File.OpenRead(fileUri.LocalPath);
                using var sr = new StreamReader(fileStream, fileEncoding, true);

                Span<char> buffer = stackalloc char[LanguageConstants.MaxLiteralCharacterLimit + 1];
                var sb = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    var i = sr.ReadBlock(buffer);
                    sb.Append(new string(buffer.Slice(0, i)));
                    if (maxCharacters > 0 && sb.Length > maxCharacters)
                    {
                        failureBuilder = x => x.FileExceedsMaximumSize(fileUri.LocalPath, maxCharacters, "characters");
                        fileContents = null;
                        detectedEncoding = null;
                        return false;
                    }
                }
                fileContents = sb.ToString();
                detectedEncoding = sr.CurrentEncoding;
                return true;
            }
            catch (Exception exception)
            {
                // I/O classes typically throw a large variety of exceptions
                // instead of handling each one separately let's just trust the message we get
                failureBuilder = x => x.ErrorOccurredReadingFile(exception.Message);
                fileContents = null;
                detectedEncoding = null;
                return false;
            }
        }

        public bool TryReadAsBase64(Uri fileUri, [NotNullWhen(true)] out string? fileBase64, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder, int maxCharacters = -1)
        {
            if (!fileUri.IsFile)
            {
                failureBuilder = x => x.UnableToLoadNonFileUri(fileUri);
                fileBase64 = null;
                return false;
            }
            try
            {
                failureBuilder = null;
                if (Directory.Exists(fileUri.LocalPath))
                {
                    // Docs suggest this is the error to throw when we give a directory.
                    // A trailing backslash causes windows not to throw this exception.
                    throw new UnauthorizedAccessException($"Access to the path '{fileUri.LocalPath}' is denied.");
                }

                if (maxCharacters > 0)
                {
                    var maxFileSize = maxCharacters / 4 * 3; //each base64 character represents 6 bits
                    var fileInfo = new FileInfo(fileUri.LocalPath);
                    fileInfo.Refresh();
                    if (fileInfo.Length > maxFileSize)
                    {
                        failureBuilder = x => x.FileExceedsMaximumSize(fileUri.LocalPath, maxFileSize, "bytes");
                        fileBase64 = null;
                        return false;
                    }
                }

                using var fileStream = File.OpenRead(fileUri.LocalPath);

                Span<byte> buffer = stackalloc byte[102400];
                var sb = new StringBuilder();
                using var memoryStream = new MemoryStream(102400);
                var i = 0;
                while ((i = fileStream.Read(buffer)) > 0)
                {
                    memoryStream.Write(buffer.Slice(0, i));
                }

                fileBase64 = new string(Convert.ToBase64String(memoryStream.ToArray(), Base64FormattingOptions.None));

                return true;
            }
            catch (Exception exception)
            {
                // I/O classes typically throw a large variety of exceptions
                // instead of handling each one separately let's just trust the message we get
                failureBuilder = x => x.ErrorOccurredReadingFile(exception.Message);
                fileBase64 = null;
                return false;
            }
        }

        public bool TryReadAtMostNCharaters(Uri fileUri, Encoding fileEncoding, int n, [NotNullWhen(true)] out string? fileContents)
        {
            if (!fileUri.IsFile || n <= 0)
            {
                fileContents = null;
                return false;
            }

            try
            {
                if (Directory.Exists(fileUri.LocalPath))
                {
                    // Docs suggest this is the error to throw when we give a directory.
                    // A trailing backslash causes windows not to throw this exception.
                    throw new UnauthorizedAccessException($"Access to the path '{fileUri.LocalPath}' is denied.");
                }

                using var fileStream = File.OpenRead(fileUri.LocalPath);
                using var sr = new StreamReader(fileStream, fileEncoding, true);

                var buffer = new char[n];
                n = sr.ReadBlock(buffer, 0, n);

                fileContents = new string(buffer.Take(n).ToArray());
                return true;
            }
            catch (Exception)
            {
                fileContents = null;
                return false;
            }
        }

        public void Write(Uri fileUri, Stream contents)
        {
            RequireFileUri(fileUri);

            using var fileStream = new FileStream(fileUri.LocalPath, FileMode.Create);
            contents.CopyTo(fileStream);
        }

        public Uri? TryResolveFilePath(Uri parentFileUri, string childFilePath)
        {
            if (!Uri.TryCreate(parentFileUri, childFilePath, out var relativeUri))
            {
                return null;
            }

            return relativeUri;
        }

        public IEnumerable<Uri> GetDirectories(Uri fileUri, string pattern = "")
        {
            if (!fileUri.IsFile)
            {
                return Enumerable.Empty<Uri>();
            }
            return Directory.GetDirectories(fileUri.LocalPath, pattern).Select(s => new Uri(s + "/"));
        }

        public IEnumerable<Uri> GetFiles(Uri fileUri, string pattern = "")
        {
            if (!fileUri.IsFile)
            {
                return Enumerable.Empty<Uri>();
            }
            return Directory.GetFiles(fileUri.LocalPath, pattern).Select(s => new Uri(s));
        }

        public bool DirExists(Uri fileUri) => fileUri.IsFile && Directory.Exists(fileUri.LocalPath);

        public bool FileExists(Uri uri) => uri.IsFile && File.Exists(uri.LocalPath);

        private static void RequireFileUri(Uri uri)
        {
            if (!uri.IsFile)
            {
                throw new ArgumentException($"Non-file URI is not supported by this file resolver.");
            }
        }
    }
}
