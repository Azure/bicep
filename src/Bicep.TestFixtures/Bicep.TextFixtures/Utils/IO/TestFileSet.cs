// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Extensions;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.IO.InMemory;

namespace Bicep.TextFixtures.Utils.IO
{
    public abstract class TestFileSet
    {
        protected TestFileSet(IFileExplorer fileExplorer)
        {
            this.FileExplorer = fileExplorer;
            this.FileExplorer.GetDirectory(this.GetUri("")).EnsureExists();
        }

        public IFileExplorer FileExplorer { get; }

        public IOUri GetEntryPointUri()
        {
            var entryPoint = this.FileExplorer.GetFile(this.GetUri("main.bicep"));

            if (!entryPoint.Exists())
            {
                throw new InvalidOperationException("TestFileSet does not contain an entry point file 'main.bicep'.");
            }

            return entryPoint.Uri;
        }

        public TestFileSet AddDirectory(string path)
        {
            this.FileExplorer.GetDirectory(this.GetUri(path)).EnsureExists();

            return this;
        }

        public TestFileSet AddFile(string path, TestFileData data)
        {
            if (data.IsDirectory)
            {
                return this.AddDirectory(path);
            }

            this.FileExplorer.GetFile(this.GetUri(path)).EnsureExists().Write(data.Text);

            return this;
        }

        public TestFileSet AddFiles(params (string, TestFileData)[] files)
        {
            foreach (var (path, data) in files)
            {
                this.AddFile(path, data);
            }

            return this;
        }

        public abstract IOUri GetUri(string path);
    }
}
