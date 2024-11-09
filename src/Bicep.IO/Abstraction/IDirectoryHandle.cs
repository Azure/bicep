// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    public interface IDirectoryHandle : IResourceHandle
    {
        public IDirectoryHandle? GetParent();

        public IDirectoryHandle GetDirectory(string relativePath);

        public IFileHandle GetFile(string relativePath);
    }
}
