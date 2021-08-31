// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Configuration;

namespace Bicep.Core.Workspaces
{
    /// <summary>
    /// Represents the active set of files and shared data that can be utilized to compile one or more bicep files.
    /// </summary>
    public class Workspace : IWorkspace
    {
        private readonly IDictionary<Uri, ISourceFile> activeFiles = new Dictionary<Uri, ISourceFile>();
        private readonly ConcurrentDictionary<Uri, BicepConfig> activeBicepConfigs = new ConcurrentDictionary<Uri, BicepConfig>();

        public bool TryGetSourceFile(Uri fileUri, [NotNullWhen(true)] out ISourceFile? file)
            => activeFiles.TryGetValue(fileUri, out file);

        public IEnumerable<ISourceFile> GetSourceFilesForDirectory(Uri fileUri)
            => activeFiles
                .Where(kvp => fileUri.IsBaseOf(kvp.Key))
                .Select(kvp => kvp.Value);

        public ImmutableDictionary<Uri, ISourceFile> GetActiveSourceFilesByUri()
            => activeFiles.ToImmutableDictionary();

        public void UpsertBicepConfig(Uri uri, BicepConfig bicepConfig)
        {
            activeBicepConfigs.AddOrUpdate(
                uri,
                (uri) => bicepConfig,
                (uri, prevConfig) => bicepConfig);
        }

        public BicepConfig? GetBicepConfig(Uri uri)
        {
            activeBicepConfigs.TryGetValue(uri, out BicepConfig? bicepConfig);
            return bicepConfig;
        }

        public void RemoveBicepConfig(Uri uri)
        {
            if (activeBicepConfigs.ContainsKey(uri))
            {
                activeBicepConfigs.TryRemove(uri, out _);
            }
        }

        public (ImmutableArray<ISourceFile> added, ImmutableArray<ISourceFile> removed) UpsertSourceFiles(IEnumerable<ISourceFile> files)
        {
            var added = new List<ISourceFile>();
            var removed = new List<ISourceFile>();

            foreach (var newFile in files)
            {
                if (activeFiles.TryGetValue(newFile.FileUri, out var oldFile))
                {
                    if (oldFile == newFile)
                    {
                        continue;
                    }

                    removed.Add(oldFile);
                }

                added.Add(newFile);

                activeFiles[newFile.FileUri] = newFile;
            }

            return (added.ToImmutableArray(), removed.ToImmutableArray());
        }

        public void RemoveSourceFiles(IEnumerable<ISourceFile> files)
        {
            foreach (var file in files)
            {
                if (activeFiles.TryGetValue(file.FileUri, out var treeToRemove) && treeToRemove == file)
                {
                    activeFiles.Remove(file.FileUri);
                }
            }
        }
    }
}
