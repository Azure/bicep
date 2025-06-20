// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.IO.Abstraction;

namespace Bicep.TextFixtures.IO
{
    public abstract class TestFileSet
    {
        private readonly HashSet<IOUri> fileUris;

        protected TestFileSet(IFileExplorer fileExplorer)
        {
            this.fileUris = [];
            this.FileExplorer = fileExplorer;
            this.FileExplorer.GetDirectory(this.GetUri("")).EnsureExists();
        }

        public IFileExplorer FileExplorer { get; }

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

            var uri = this.GetUri(path);

            this.FileExplorer.GetFile(uri).EnsureExists().Write(data.AsBinaryData());
            this.fileUris.Add(uri);

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

        public TestFileSet RemoveFile(string path)
        {
            var uri = this.GetUri(path);

            if (this.fileUris.Contains(uri))
            {
                this.FileExplorer.GetFile(uri).Delete();
            }

            return this;
        }

        public TestFileSet RemoveFiles(params string[] paths)
        {
            foreach (var path in paths)
            {
                this.RemoveFile(path);
            }

            return this;
        }

        public TestFileSet Clear()
        {
            foreach (var uri in this.fileUris)
            {
                this.FileExplorer.GetFile(uri).Delete();
            }

            this.fileUris.Clear();

            return this;
        }

        public abstract IOUri GetUri(string path);
    }
}
