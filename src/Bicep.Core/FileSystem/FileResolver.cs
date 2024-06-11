// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using System.Text;
using Bicep.Core.Diagnostics;

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
            return FileLock.TryAcquire(fileSystem, fileUri.LocalPath);
        }

        public ResultWithDiagnostic<string> TryRead(Uri fileUri)
            => TryReadInternal<string>(fileUri, 0, stream =>
            {
                using var reader = new StreamReader(stream);

                return new(reader.ReadToEnd());
            });

        public static ResultWithDiagnostic<FileWithEncoding> ReadWithEncoding(BinaryData data, Encoding fileEncoding, int maxCharacters, Uri fileUri)
        {
            using var sr = new StreamReader(data.ToStream(), fileEncoding, true);

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

        public ResultWithDiagnostic<BinaryData> TryReadAsBinaryData(Uri fileUri, int? maxFileSize)
            => TryReadInternal<BinaryData>(fileUri, maxFileSize ?? 0, stream => new(BinaryData.FromStream(stream)));

        public ResultWithDiagnostic<string> TryReadAtMostNCharacters(Uri fileUri, Encoding fileEncoding, int n)
            => TryReadInternal<string>(fileUri, 0, stream =>
            {
                if (n <= 0)
                {
                    throw new InvalidOperationException($"Cannot read {n} characters");
                }

                using var sr = new StreamReader(stream, fileEncoding, true);

                var buffer = new char[n];
                n = sr.ReadBlock(buffer, 0, n);

                return new(new string(buffer.Take(n).ToArray()));
            });

        private ResultWithDiagnostic<T> TryReadInternal<T>(Uri fileUri, int maxFileSize, Func<Stream, ResultWithDiagnostic<T>> readFunc)
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

                if (maxFileSize > 0)
                {
                    var fileInfo = fileSystem.FileInfo.New(fileUri.LocalPath);
                    fileInfo.Refresh();
                    if (fileInfo.Length > maxFileSize)
                    {
                        return new(x => x.FileExceedsMaximumSize(fileUri.LocalPath, maxFileSize, "bytes"));
                    }
                }

                ApplyWindowsConFileWorkaround(fileUri.LocalPath);
                using var fileStream = fileSystem.File.OpenRead(fileUri.LocalPath);

                return readFunc(fileStream);
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
            => PathHelper.TryResolveFilePath(parentFileUri, childFilePath);

        public IEnumerable<Uri> GetDirectories(Uri fileUri, string pattern)
        {
            if (!fileUri.IsFile)
            {
                return [];
            }
            return fileSystem.Directory.GetDirectories(fileUri.LocalPath, pattern).Select(s => new Uri(s + "/"));
        }

        public IEnumerable<Uri> GetFiles(Uri fileUri, string pattern)
        {
            if (!fileUri.IsFile)
            {
                return [];
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
