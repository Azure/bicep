// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    public interface IDirectoryHandle : IIOHandle
    {
        IDirectoryHandle EnsureExists();

        void Delete();

        IDirectoryHandle? GetParent();

        IDirectoryHandle GetDirectory(string relativePath);

        IFileHandle GetFile(string relativePath);

        IEnumerable<IDirectoryHandle> EnumerateDirectories(string searchPattern = "");

        IEnumerable<IFileHandle> EnumerateFiles(string searchPattern = "");
    }
}
