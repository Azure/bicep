// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.Extensions
{
    internal sealed class TempFile : IDisposable
    {
        private readonly IFileSystem fileSystem;

        private string? path;

        public TempFile(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            this.path = fileSystem.Path.GetTempFileName();
        }

        public string Path => this.path ?? throw new ObjectDisposedException(this.GetType().Name);

        ~TempFile() => this.DisposeInternal();

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            this.DisposeInternal();
        }

        private void DisposeInternal()
        {
            if (path is null)
            {
                // Already disposed.
                return;
            }

            try
            {
                this.fileSystem.File.Delete(path);
            }
            catch
            {
                // There's nothing we can do.
            }

            this.path = null;
        }
    }
}
