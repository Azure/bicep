// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    public interface IFileHandle : IIOHandle
    {
        IDirectoryHandle GetParent();

        IFileHandle EnsureExists();

        Stream OpenRead();

        Stream OpenWrite();

        string ReadAllText();

        Task<string> ReadAllTextAsync();

        void WriteAllText(string text);

        Task WriteAllTextAsync(string text);

        void Delete();

        void MakeExecutable();

        IFileLock? TryLock();
    }
}
