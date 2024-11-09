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
        public IDirectoryHandle GetDirectory(ResourceIdentifier identifier);

        public IFileHandle GetFile(ResourceIdentifier identifier);
    }
}
