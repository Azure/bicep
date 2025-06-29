// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.IO.InMemory
{
    public abstract class InMemoryIOHandle : IIOHandle
    {
        protected InMemoryIOHandle(InMemoryFileExplorer.FileStore fileStore, IOUri uri)
        {
            this.FileStore = fileStore;
            this.Uri = uri;
        }

        protected InMemoryFileExplorer.FileStore FileStore { get; }

        public IOUri Uri { get; }

        public abstract bool Exists();

        public override int GetHashCode() => HashCode.Combine(this.GetType(), Uri);

        public override bool Equals(object? @object) => Equals(@object as InMemoryIOHandle);

        public bool Equals(IIOHandle? other)
        {
            if (other is null)
            {
                return false;
            }

            return this.GetType() == other.GetType() && Uri == other.Uri;
        }
    }
}
