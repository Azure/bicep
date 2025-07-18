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

        public ResultWithDiagnosticBuilder<string> TryRead(Uri fileUri)
            => TryReadInternal<string>(fileUri, 0, stream =>
            {
                using var reader = new StreamReader(stream);

                return new(reader.ReadToEnd());
            });

        private ResultWithDiagnosticBuilder<T> TryReadInternal<T>(Uri fileUri, int maxFileSize, Func<Stream, ResultWithDiagnosticBuilder<T>> readFunc)
        {
            if (!fileUri.IsFile)
            {
                return new(x => x.UnableToLoadNonFileUri(fileUri));
            }

            try
            {
                if (this.fileSystem.Directory.Exists(fileUri.LocalPath))
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
