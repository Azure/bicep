// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;

namespace Bicep.IO.FileSystem
{
    public abstract class FileSystemIOHandle : IIOHandle
    {
        protected FileSystemIOHandle(IFileSystem fileSystem, IOUri uri)
        {
            this.FileSystem = fileSystem;
            this.FilePath = uri.GetLocalFilePath();
            this.Uri = uri;
        }

        protected IFileSystem FileSystem { get; }

        protected string FilePath { get; }

        public IOUri Uri { get; }

        public IOUri ParentUri => this.Uri.Resolve("..");

        public abstract bool Exists();

        public override int GetHashCode() => HashCode.Combine(this.GetType(), Uri);

        public override bool Equals(object? @object) => Equals(@object as FileSystemIOHandle);

        public bool Equals(IIOHandle? other)
        {
            if (other is null)
            {
                return false;
            }

            return this.GetType() == other.GetType() && Uri.Equals(other.Uri);
        }
    }
}
