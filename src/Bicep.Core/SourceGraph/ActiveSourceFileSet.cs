// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.FileSystem;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    /// <summary>
    /// Represents the active set of files and shared data that can be utilized to compile one or more bicep files.
    /// </summary>
    public class ActiveSourceFileSet : IActiveSourceFileSet
    {
        private readonly Dictionary<IOUri, ISourceFile> activeFiles = [];

        public ISourceFile? TryGetSourceFile(IOUri fileUri) => activeFiles.TryGetValue(fileUri, out var file) ? file : null;

        public bool HasSourceFile(IOUri fileUri) => activeFiles.ContainsKey(fileUri);

        public IEnumerator<ISourceFile> GetEnumerator() => activeFiles.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public (ImmutableArray<ISourceFile> added, ImmutableArray<ISourceFile> removed) UpsertSourceFiles(IEnumerable<ISourceFile> files)
        {
            var added = new List<ISourceFile>();
            var removed = new List<ISourceFile>();

            foreach (var newFile in files)
            {
                if (activeFiles.TryGetValue(newFile.FileHandle.Uri, out var oldFile))
                {
                    if (oldFile == newFile)
                    {
                        continue;
                    }

                    removed.Add(oldFile);
                }

                added.Add(newFile);

                activeFiles[newFile.FileHandle.Uri] = newFile;
            }

            return (added.ToImmutableArray(), removed.ToImmutableArray());
        }

        public void RemoveSourceFiles(IEnumerable<ISourceFile> files)
        {
            foreach (var file in files)
            {
                if (activeFiles.TryGetValue(file.FileHandle.Uri, out var treeToRemove) && treeToRemove == file)
                {
                    activeFiles.Remove(file.FileHandle.Uri);
                }
            }
        }
    }
}
