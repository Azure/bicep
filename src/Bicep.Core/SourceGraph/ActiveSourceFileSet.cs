// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.FileSystem;

namespace Bicep.Core.SourceGraph
{
    /// <summary>
    /// Represents the active set of files and shared data that can be utilized to compile one or more bicep files.
    /// </summary>
    public class ActiveSourceFileSet : IActiveSourceFileSet
    {
        private readonly IDictionary<Uri, ISourceFile> activeFiles = new Dictionary<Uri, ISourceFile>();

        public bool TryGetSourceFile(Uri fileUri, [NotNullWhen(true)] out ISourceFile? file)
            => activeFiles.TryGetValue(fileUri, out file);

        public IEnumerable<ISourceFile> GetSourceFilesForDirectory(Uri fileUri)
            => activeFiles
                .Where(kvp => PathHelper.IsSubPathOf(fileUri, kvp.Key))
                .Select(kvp => kvp.Value);

        public ImmutableDictionary<Uri, ISourceFile> GetActiveSourceFilesByUri()
            => activeFiles.ToImmutableDictionary();

        public (ImmutableArray<ISourceFile> added, ImmutableArray<ISourceFile> removed) UpsertSourceFiles(IEnumerable<ISourceFile> files)
        {
            var added = new List<ISourceFile>();
            var removed = new List<ISourceFile>();

            foreach (var newFile in files)
            {
                if (activeFiles.TryGetValue(newFile.Uri, out var oldFile))
                {
                    if (oldFile == newFile)
                    {
                        continue;
                    }

                    removed.Add(oldFile);
                }

                added.Add(newFile);

                activeFiles[newFile.Uri] = newFile;
            }

            return (added.ToImmutableArray(), removed.ToImmutableArray());
        }

        public void RemoveSourceFiles(IEnumerable<ISourceFile> files)
        {
            foreach (var file in files)
            {
                if (activeFiles.TryGetValue(file.Uri, out var treeToRemove) && treeToRemove == file)
                {
                    activeFiles.Remove(file.Uri);
                }
            }
        }
    }
}
