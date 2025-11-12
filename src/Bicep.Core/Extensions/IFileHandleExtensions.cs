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
using Bicep.Core.SourceGraph;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Extensions
{
    public static class IFileHandleExtensions
    {
        public static bool IsArmTemplateLikeFile(this IFileHandle fileHandle) => fileHandle.Uri.HasArmTemplateLikeExtension();

        public static bool IsBicepFile(this IFileHandle fileHandle) => fileHandle.Uri.HasBicepExtension();

        public static bool IsBicepParamFile(this IFileHandle fileHandle) => fileHandle.Uri.HasBicepParamExtension();

        public static ResultWithDiagnosticBuilder<IFileHandle> TryGetRelativeFile(this IFileHandle fileHandle, RelativePath path)
        {
            try
            {
                var currentDirectory = fileHandle.GetParent();
                return currentDirectory.TryGetRelativeFile(path);
            }
            catch (IOException exception)
            {
                return new(x => x.ErrorOccurredReadingFile(exception.Message));
            }
        }

        public static ResultWithDiagnosticBuilder<string> TryPeekText(this IFileHandle fileHandle, int length, Encoding? encoding = null) => HandleFileReadError(() =>
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

            using var stream = fileHandle.OpenRead();
            using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);

            var buffer = new char[length];
            length = reader.ReadBlock(buffer, 0, length);

            return new string(buffer, 0, length);
        });

        public static ResultWithDiagnosticBuilder<string> TryReadAllText(this IFileHandle fileHandle, Encoding? encoding = null) =>
            HandleFileReadError(() =>
            {
                using var fileStream = fileHandle.OpenRead();
                using var reader = new StreamReader(fileStream, encoding ?? Encoding.UTF8);

                return reader.ReadToEnd();
            });

        public static ResultWithDiagnosticBuilder<BinaryData> TryReadBinaryData(this IFileHandle fileHandle) =>
            HandleFileReadError(() =>
            {
                using var fileStream = fileHandle.OpenRead();

                return BinaryData.FromStream(fileStream);
            });

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
            using var writer = new StreamWriter(fileStream, leaveOpen: true);

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
