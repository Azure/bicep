// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    public interface IFileHandle : IResourceHandle
    {
        IDirectoryHandle GetParent();

        Stream OpenRead();

        Stream OpenWrite();

        IFileLock? TryLock();
    }
}
