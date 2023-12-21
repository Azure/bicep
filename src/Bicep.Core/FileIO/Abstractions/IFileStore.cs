// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Constants;
using Bicep.Core.Utils;

namespace Bicep.Core.FileIO.Abstractions;

public interface IFileStore : IComparer<FilePointer>, IEqualityComparer<FilePointer>
{
    FilePointer Parse(string pointer);

    FilePointer Combine(FilePointer basePointer, FilePointer relativePointer);

    FileKind GetFileKind(FilePointer pointer);

    IFilelock? TryAquireFileLock(FilePointer pointer);

    public string ReadAllText(FilePointer pointer);

    public IEnumerable<string> ReadLines(FilePointer pointer);

    public void WriteStream(FilePointer pointer, Stream stream);
}
