// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    public static class IFileHandleReadWriteExtensions
    {
        public static void WriteTo(this BinaryData binaryData, IFileHandle fileHandle)
        {
            using var dataStream = binaryData.ToStream();
            using var fileStream = fileHandle.OpenWrite();
            dataStream.CopyTo(fileStream);
        }

        public static void WriteTo(this Stream stream, IFileHandle fileHandle)
        {
            using var fileStream = fileHandle.OpenWrite();
            stream.CopyTo(fileStream);
        }

        public static void WriteAllText(this IFileHandle fileHandle, string content)
        {
            using var fileStream = fileHandle.OpenWrite();
            using var writer = new StreamWriter(fileStream);

            writer.Write(content);
        }

        public static string ReadAllText(this IFileHandle fileHandle)
        {
            using var fileStream = fileHandle.OpenRead();
            using var reader = new StreamReader(fileStream);
            return reader.ReadToEnd();
        }
    }
}
