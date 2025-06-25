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

        public TestFileSet AddDirectory(IOUri uri)
        {
            this.FileExplorer.GetDirectory(uri).EnsureExists();

            return this;
        }

        public TestFileSet AddDirectory(string path) => this.AddDirectory(this.GetUri(path));

        public TestFileSet AddFile(IOUri uri, TestFileData data)
        {
            if (data.IsDirectory)
            {
                return this.AddDirectory(uri);
            }

            this.FileExplorer.GetFile(uri).EnsureExists().Write(data.AsBinaryData());
            this.fileUris.Add(uri);

            return this;
        }

        public TestFileSet AddFile(string path, TestFileData data) => this.AddFile(this.GetUri(path), data);

        public TestFileSet AddFiles(params (IOUri, TestFileData)[] files)
        {
            foreach (var (uri, data) in files)
            {
                this.AddFile(uri, data);
            }

            return this;
        }

        public TestFileSet AddFiles(params (string, TestFileData)[] files) => this.AddFiles(files.Select(x => (this.GetUri(x.Item1), x.Item2)).ToArray());

        public TestFileSet RemoveFile(IOUri uri)
        {
            if (this.fileUris.Contains(uri))
            {
                this.FileExplorer.GetFile(uri).Delete();
                this.fileUris.Remove(uri);
            }

            return this;
        }

        public TestFileSet RemoveFile(string path) => this.RemoveFile(this.GetUri(path));

        public TestFileSet RemoveFiles(params IOUri[] uris)
        {
            foreach (var uri in uris)
            {
                this.RemoveFile(uri);
            }

            return this;
        }

        public TestFileSet RemoveFiles(params string[] paths) => this.RemoveFiles(paths.Select(this.GetUri).ToArray());

        public BinaryData GetFileData(IOUri uri) => this.FileExplorer.GetFile(uri).TryReadBinaryData().Unwrap();

        public BinaryData GetFileData(string path) => this.GetFileData(this.GetUri(path));

        public string GetFileText(IOUri uri) => this.GetFileData(uri).ToString();

        public string GetFileText(string path) => this.GetFileText(this.GetUri(path));

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
