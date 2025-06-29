// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    public interface IFileExplorer
    {
        IDirectoryHandle GetDirectory(IOUri uri);

        IFileHandle GetFile(IOUri uri);
    }
}
