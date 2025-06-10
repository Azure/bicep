// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph.Artifacts
{
    public abstract class CacheableArtifact
    {
        private IDirectoryHandle cacheDirectory;

        protected CacheableArtifact(IDirectoryHandle cacheDirectory)
        {
            this.cacheDirectory = cacheDirectory;
        }

        protected IFileHandle GetFile(string fileName) => this.cacheDirectory.GetFile(fileName);
    }
}
