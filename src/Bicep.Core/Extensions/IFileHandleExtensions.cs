// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Extensions
{
    public static class IFileHandleExtensions
    {
        public static bool IsArmTemplateLikeFile(this IFileHandle fileHandle) =>
                fileHandle.Uri.HasExtension(LanguageConstants.JsonFileExtension) ||
                fileHandle.Uri.HasExtension(LanguageConstants.JsoncFileExtension) ||
                fileHandle.Uri.HasExtension(LanguageConstants.ArmTemplateFileExtension);

        public static bool IsBicepFile(this IFileHandle fileHandle) => fileHandle.Uri.HasExtension(LanguageConstants.LanguageFileExtension);

        public static bool IsBicepParamFile(this IFileHandle fileHandle) => fileHandle.Uri.HasExtension(LanguageConstants.ParamsFileExtension);

        public static ResultWithDiagnosticBuilder<Encoding> TryDetectEncoding(this IFileHandle fileHandle) => HandleFileReadError(() =>
        {
            using var stream = fileHandle.OpenRead();
            using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);

            reader.Peek();

            return reader.CurrentEncoding;
        });

        public static ResultWithDiagnosticBuilder<string> TryPeek(this IFileHandle fileHandle, int length, Encoding? encoding = null) => HandleFileReadError(() =>
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

            using var stream = fileHandle.OpenRead();
            using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);

            var buffer = new char[length];
            length = reader.Read(buffer, 0, length);
        
            return new string(buffer, 0, length);
        });

        public static ResultWithDiagnosticBuilder<string> TryRead(this IFileHandle fileHandle, Encoding? encoding = null) =>
            HandleFileReadError(() =>
            {
                using var fileStream = fileHandle.OpenRead();
                using var reader = new StreamReader(fileStream, encoding ?? Encoding.UTF8);

                return reader.ReadToEnd();
            });

        public static ResultWithDiagnosticBuilder<string> TryRead(this IFileHandle fileHandle, int lengthLimit, Encoding? encoding = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(lengthLimit);

            return HandleFileReadError(() =>
            {
                using var stream = fileHandle.OpenRead();
                using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);

                char[] buffer = new char[lengthLimit];
                int lengthRead = reader.Read(buffer, 0, lengthLimit);

                if (!reader.EndOfStream)
                {
                    return new ResultWithDiagnosticBuilder<string>(x => x.FileExceedsMaximumSize(fileHandle.Uri, lengthLimit, "characters"));
                }

                return new ResultWithDiagnosticBuilder<string>(new string(buffer, 0, lengthRead));
            });
        }

        public static void Write(this IFileHandle fileHandle, BinaryData data)
        {
            using var dataStream = data.ToStream();
            fileHandle.Write(dataStream);
        }

        public static void Write(this IFileHandle fileHandle, Stream stream)
        {
            using var fileStream = fileHandle.OpenWrite();
            stream.CopyTo(fileStream);
        }

        public static void Write(this IFileHandle fileHandle, string text)
        {
            using var fileStream = fileHandle.OpenWrite();
            using var writer = new StreamWriter(fileStream);
            writer.Write(text);
        }

        private static ResultWithDiagnosticBuilder<T> HandleFileReadError<T>(Func<T> readOperation) => HandleFileReadError(() => new ResultWithDiagnosticBuilder<T>(readOperation()));

        private static ResultWithDiagnosticBuilder<T> HandleFileReadError<T>(Func<ResultWithDiagnosticBuilder<T>> operation)
        {
            try
            {
                return operation();
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                return new(x => x.ErrorOccurredReadingFile(exception.Message));
            }
        }
    }
}
