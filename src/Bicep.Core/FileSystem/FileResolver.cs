// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Utils;

namespace Bicep.Core.FileSystem
{
    public class FileResolver : IFileResolver
    {
        private readonly IFileSystem fileSystem;

        public FileResolver(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public IDisposable? TryAcquireFileLock(Uri fileUri)
        {
            RequireFileUri(fileUri);
            return FileLock.TryAcquire(fileUri.LocalPath);
        }

        public ResultWithDiagnostic<string> TryRead(Uri fileUri)
        {
            if (!fileUri.IsFile)
            {
                return new(x => x.UnableToLoadNonFileUri(fileUri));
            }

            try
            {
                if (DirExists(fileUri))
                {
                    return new(x => x.FoundDirectoryInsteadOfFile(fileUri.LocalPath));
                }

                ApplyWindowsConFileWorkaround(fileUri.LocalPath);
                return new(fileSystem.File.ReadAllText(fileUri.LocalPath));
            }
            catch (Exception exception)
            {
                // I/O classes typically throw a large variety of exceptions
                // instead of handling each one separately let's just trust the message we get
                return new(x => x.ErrorOccurredReadingFile(exception.Message));
            }
        }

        public ResultWithDiagnostic<FileWithEncoding> TryRead(Uri fileUri, Encoding fileEncoding, int maxCharacters)
        {
            if (!fileUri.IsFile)
            {
                return new(x => x.UnableToLoadNonFileUri(fileUri));
            }

            try
            {
                if (DirExists(fileUri))
                {
                    return new(x => x.FoundDirectoryInsteadOfFile(fileUri.LocalPath));
                }

                ApplyWindowsConFileWorkaround(fileUri.LocalPath);
                using var fileStream = fileSystem.File.OpenRead(fileUri.LocalPath);
                using var sr = new StreamReader(fileStream, fileEncoding, true);

                Span<char> buffer = stackalloc char[LanguageConstants.MaxLiteralCharacterLimit + 1];
                var sb = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    var i = sr.ReadBlock(buffer);
                    sb.Append(new string(buffer.Slice(0, i)));
                    if (maxCharacters > 0 && sb.Length > maxCharacters)
                    {
                        return new(x => x.FileExceedsMaximumSize(fileUri.LocalPath, maxCharacters, "characters"));
                    }
                }

                return new(new FileWithEncoding(sb.ToString(), sr.CurrentEncoding));
            }
            catch (Exception exception)
            {
                // I/O classes typically throw a large variety of exceptions
                // instead of handling each one separately let's just trust the message we get
                return new(x => x.ErrorOccurredReadingFile(exception.Message));
            }
        }

        public ResultWithDiagnostic<string> TryReadAsBase64(Uri fileUri, int maxCharacters = -1)
        {
            if (!fileUri.IsFile)
            {
                return new(x => x.UnableToLoadNonFileUri(fileUri));
            }
            try
            {
                if (DirExists(fileUri))
                {
                    return new(x => x.FoundDirectoryInsteadOfFile(fileUri.LocalPath));
                }

                if (maxCharacters > 0)
                {
                    var maxFileSize = maxCharacters / 4 * 3; //each base64 character represents 6 bits
                    var fileInfo = fileSystem.FileInfo.New(fileUri.LocalPath);
                    fileInfo.Refresh();
                    if (fileInfo.Length > maxFileSize)
                    {
                        return new(x => x.FileExceedsMaximumSize(fileUri.LocalPath, maxFileSize, "bytes"));
                    }
                }

                ApplyWindowsConFileWorkaround(fileUri.LocalPath);
                using var fileStream = fileSystem.File.OpenRead(fileUri.LocalPath);

                Span<byte> buffer = stackalloc byte[102400];
                var sb = new StringBuilder();
                using var memoryStream = new MemoryStream(102400);
                var i = 0;
                while ((i = fileStream.Read(buffer)) > 0)
                {
                    memoryStream.Write(buffer.Slice(0, i));
                }

                return new(Convert.ToBase64String(memoryStream.ToArray(), Base64FormattingOptions.None));
            }
            catch (Exception exception)
            {
                // I/O classes typically throw a large variety of exceptions
                // instead of handling each one separately let's just trust the message we get
                return new(x => x.ErrorOccurredReadingFile(exception.Message));
            }
        }

        public ResultWithDiagnostic<string> TryReadAtMostNCharacters(Uri fileUri, Encoding fileEncoding, int n)
        {
            if (n <= 0)
            {
                throw new InvalidOperationException($"Cannot read {n} characters");
            }

            if (!fileUri.IsFile)
            {
                return new(x => x.UnableToLoadNonFileUri(fileUri));
            }

            try
            {
                if (DirExists(fileUri))
                {
                    return new(x => x.FoundDirectoryInsteadOfFile(fileUri.LocalPath));
                }

                ApplyWindowsConFileWorkaround(fileUri.LocalPath);
                using var fileStream = fileSystem.File.OpenRead(fileUri.LocalPath);
                using var sr = new StreamReader(fileStream, fileEncoding, true);

                var buffer = new char[n];
                n = sr.ReadBlock(buffer, 0, n);

                return new(new string(buffer.Take(n).ToArray()));
            }
            catch (Exception exception)
            {
                // I/O classes typically throw a large variety of exceptions
                // instead of handling each one separately let's just trust the message we get
                return new(x => x.ErrorOccurredReadingFile(exception.Message));
            }
        }

        public void Write(Uri fileUri, Stream contents)
        {
            RequireFileUri(fileUri);

            using var fileStream = fileSystem.File.Open(fileUri.LocalPath, FileMode.Create);
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

        public string GetRelativePath(string relativeTo, string path)
        {
            return fileSystem.Path.GetRelativePath(relativeTo, path).Replace('\\', '/');
        }

        public IEnumerable<Uri> GetDirectories(Uri fileUri, string pattern)
        {
            if (!fileUri.IsFile)
            {
                return Enumerable.Empty<Uri>();
            }
            return fileSystem.Directory.GetDirectories(fileUri.LocalPath, pattern).Select(s => new Uri(s + "/"));
        }

        public IEnumerable<Uri> GetFiles(Uri fileUri, string pattern)
        {
            if (!fileUri.IsFile)
            {
                return Enumerable.Empty<Uri>();
            }
            return fileSystem.Directory.GetFiles(fileUri.LocalPath, pattern).Select(s => new Uri(s));
        }

        public bool DirExists(Uri fileUri) => fileUri.IsFile && fileSystem.Directory.Exists(fileUri.LocalPath);

        public bool FileExists(Uri uri) => uri.IsFile && fileSystem.File.Exists(uri.LocalPath);

        private static void RequireFileUri(Uri uri)
        {
            if (!uri.IsFile)
            {
                throw new ArgumentException($"Non-file URI is not supported by this file resolver.");
            }
        }

        private static void ApplyWindowsConFileWorkaround(string localPath)
        {
            /*
             * Win10 (and possibly older versions) will block without returning when
             * reading a file whose name is CON or CON.<any extension> which breaks the language server
             *
             * as a workaround, we will simulate Win11+ behavior that throws a
             * FileNotFoundException
             *
             * https://github.com/Azure/bicep/issues/6224
             */

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // not on windows - no need for the workaround
                return;
            }

            string fileName = Path.GetFileNameWithoutExtension(localPath);
            if (!string.Equals(fileName, "CON", StringComparison.InvariantCultureIgnoreCase))
            {
                // file is not named CON or CON.<any extension>, so we can proceed normally
                return;
            }

            // on win11+ file not found exception is thrown
            // simulate similar behavior
            throw new FileNotFoundException($"Could not find file '{localPath}'.");
        }
    }
}
