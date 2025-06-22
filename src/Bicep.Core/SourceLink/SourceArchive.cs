// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using static Bicep.Core.SourceLink.SourceArchiveConstants;

namespace Bicep.Core.SourceLink
{
    /// <summary>
    /// Contains the individual source code files for a Bicep file and all of its dependencies.
    /// </summary>
    public class SourceArchive
    {
        private readonly SourceArchiveMetadata metadata;

        private readonly FrozenDictionary<string, string> fileEntries;

        public record SourceFileWithArtifactReference(
            ISourceFile SourceFile,
            ArtifactReference? SourceArtifact);

        private SourceArchive(SourceArchiveMetadata metadata, FrozenDictionary<string, string> fileEntries)
        {
            this.metadata = metadata;
            this.fileEntries = fileEntries;
        }

        public string EntrypointRelativePath => metadata.EntryPoint;

        public int SourceFileCount => metadata.SourceFiles.Length;

        public static ResultWithException<SourceArchive> TryUnpackFromFile(IFileHandle sourceTgzFile)
        {
            if (!sourceTgzFile.Exists())
            {
                return new(new SourceNotAvailableException());
            }

            try
            {
                var fileEntries = TgzFileExtractor.ExtractFromFileHandle(sourceTgzFile).ToFrozenDictionary(x => x.Key, x => x.Value.ToString());
                var metadataJson = fileEntries.TryGetValue(MetadataFileName)
                    ?? throw new InvalidOperationException($"Incorrectly formatted source file: No {MetadataFileName} entry");
                var metadata = JsonSerializer.Deserialize(metadataJson, SourceArchiveMetadataSerializationContext.Default.SourceArchiveMetadata)
                    ?? throw new InvalidOperationException("Source archive has invalid metadata entry");

                if (metadata.MetadataVersion < CurrentMetadataVersion)
                {
                    throw new InvalidOperationException($"This source code was published with an older, incompatible version of Bicep ({metadata.BicepVersion}). You are using version {ThisAssembly.AssemblyVersion}.");
                }

                if (metadata.MetadataVersion > CurrentMetadataVersion)
                {
                    throw new InvalidOperationException($"This source code was published with a newer, incompatible version of Bicep ({metadata.BicepVersion}). You are using version {ThisAssembly.AssemblyVersion}. You need a newer version in order to view the module source.");
                }

                return new(new SourceArchive(metadata, fileEntries));
            }
            catch (Exception exception)
            {
                return new(exception);
            }
        }

        public BinaryData PackIntoBinaryData()
        {
            var stream = new MemoryStream();
            using (var tgzWriter = new TgzWriter(stream, leaveOpen: true))
            {
                var metadataJson = JsonSerializer.Serialize(this.metadata, SourceArchiveMetadataSerializationContext.Default.SourceArchiveMetadata);
                tgzWriter.WriteEntry(MetadataFileName, metadataJson);

                foreach (var sourceFileMetadata in this.metadata.SourceFiles)
                {
                    var contents = this.fileEntries[sourceFileMetadata.ArchivePath];
                    tgzWriter.WriteEntry(sourceFileMetadata.ArchivePath, contents);
                }
            }

            stream.Seek(0, SeekOrigin.Begin);

            return BinaryData.FromStream(stream);
        }

        public static SourceArchive CreateFrom(SourceFileGrouping sourceFileGrouping)
        {
            if (sourceFileGrouping.EntryPoint is not BicepFile)
            {
                throw new InvalidOperationException("The entry point must be a Bicep file.");
            }

            var metadataBySourceFile = CreateMetadataBySourceFile(sourceFileGrouping);
            var sourceDocumentLink = CreateSourceFileLinks(sourceFileGrouping, metadataBySourceFile);
            var entryPointPath = metadataBySourceFile[sourceFileGrouping.EntryPoint].Path;
            var bicepVersion = sourceFileGrouping.EntryPoint.Features.AssemblyVersion;
            var metadata = new SourceArchiveMetadata(CurrentMetadataVersion, bicepVersion, entryPointPath, [.. metadataBySourceFile.Values], sourceDocumentLink);
            var fileEntries = metadataBySourceFile.ToFrozenDictionary(x => x.Value.ArchivePath, x => x.Key.Text);

            return new SourceArchive(metadata, fileEntries);
        }

        private static ImmutableDictionary<string, ImmutableArray<ArchivedSourceFileLink>> CreateSourceFileLinks(SourceFileGrouping sourceFileGrouping, Dictionary<ISourceFile, ArchivedSourceFileMetadata> metadataBySourceFile)
        {
            var sourceFileLinks = new Dictionary<string, List<ArchivedSourceFileLink>>();

            foreach (var grouping in sourceFileGrouping.ArtifactLookup.Values.GroupBy(x => x.ReferencingFile))
            {
                if (grouping.Key is not BicepFile referencingFile)
                {
                    continue;
                }

                foreach (var artifactResolutionInfo in grouping)
                {
                    if (artifactResolutionInfo.Syntax is { Path: { } path } &&
                        artifactResolutionInfo.Result.TryUnwrap() is { } fileHandle &&
                        sourceFileGrouping.SourceFileLookup[fileHandle.Uri.ToUri()].IsSuccess(out var referencedFile) &&
                        ShouldArchiveSourceFile(referencedFile))
                    {
                        var start = new TextPosition(TextCoordinateConverter.GetPosition(referencingFile.LineStarts, path.Span.Position));
                        var end = new TextPosition(TextCoordinateConverter.GetPosition(referencingFile.LineStarts, path.Span.Position + path.Span.Length));
                        var textRange = new TextRange(start, end);

                        var sourcePath = metadataBySourceFile[referencingFile].Path;
                        var targetPath = metadataBySourceFile[referencedFile].Path;

                        if (!sourceFileLinks.TryGetValue(sourcePath, out var links))
                        {
                            links = [];
                            sourceFileLinks[sourcePath] = links;
                        }

                        links.Add(new ArchivedSourceFileLink(textRange, targetPath));
                    }
                }
            }

            return sourceFileLinks.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableArray());
        }

        public ImmutableArray<ArchivedSourceFileLink> FindDocumentLinks(string path) =>
            this.metadata.DocumentLinks.TryGetValue(path, out var documentLinks) ? documentLinks : [];

        public ArchivedSourceFile FindSourceFile(string path)
        {
            // TODO(shenglol): Binary search.
            var metadata = this.metadata.SourceFiles.Single(x => x.Path.Equals(path, StringComparison.Ordinal));
            var contents = this.fileEntries[metadata.ArchivePath];

            return new(metadata, contents);
        }

        private static bool ShouldArchiveSourceFile(ISourceFile sourceFile) => sourceFile is BicepFile or ArmTemplateFile;

        private static void UniquifyArchivePaths(Dictionary<ISourceFile, ArchivedSourceFileMetadata> metadataBySourceFile)
        {
            var pathComparer = metadataBySourceFile.Keys.First().FileHandle.Uri.PathComparer;
            var uniqueArchivePaths = new HashSet<string>(pathComparer);
            var uniqueArchivePathSuffix = 0;

            foreach (var (sourceFile, metadata) in metadataBySourceFile)
            {
                var uniqueArchivePath = metadata.ArchivePath;
                var uniquified = false;

                while (uniqueArchivePaths.Contains(uniqueArchivePath))
                {
                    uniqueArchivePathSuffix++;
                    uniqueArchivePath = $"{metadata.ArchivePath}({uniqueArchivePathSuffix})";
                    uniquified = true;
                }

                uniqueArchivePaths.Add(uniqueArchivePath);

                if (uniquified)
                {
                    metadataBySourceFile[sourceFile] = metadata with
                    {
                        ArchivePath = uniqueArchivePath,
                    };
                }
            }
        }

        private static Dictionary<ISourceFile, ArchivedSourceFileMetadata> CreateMetadataBySourceFile(SourceFileGrouping sourceFileGrouping)
        {
            var result = new Dictionary<ISourceFile, ArchivedSourceFileMetadata>()
            {
                [sourceFileGrouping.EntryPoint] = CreateSourceFileMetadata(sourceFileGrouping.EntryPoint.GetFileName(), sourceFileGrouping.EntryPoint),
            };

            var sourceArtifactIdBySourceFileUri = sourceFileGrouping.ArtifactLookup.Values
                .Where(x => x.Result.IsSuccess() && x.Reference is OciArtifactReference { Type: ArtifactType.Module })
                .DistinctBy(x => x.Result.Unwrap().Uri)
                .ToDictionary(x => x.Result.Unwrap().Uri, x =>
                {
                    // Only include sourceArtifactId for those that were published with source.
                    if (x.Reference is OciArtifactReference moduleArtifactReference &&
                        moduleArtifactReference.TryLoadSourceArchive().IsSuccess())
                    {
                        return moduleArtifactReference.FullyQualifiedReference;
                    }

                    return null;
                });

            var entryPointDirectoryUri = sourceFileGrouping.EntryPoint.FileHandle.Uri.Resolve(".");
            var cacheRootDirectoryUri = sourceFileGrouping.EntryPoint.Features.CacheRootDirectory.Uri;
            var otherDirectoryFiles = new List<ISourceFile>();

            foreach (var file in sourceFileGrouping.SourceFiles.Where(x => x != sourceFileGrouping.EntryPoint && ShouldArchiveSourceFile(x)))
            {
                var sourceFileUri = file.FileHandle.Uri;

                if (entryPointDirectoryUri.IsBaseOf(sourceFileUri))
                {
                    var path = sourceFileUri.GetPathRelativeTo(entryPointDirectoryUri);
                    result[file] = CreateSourceFileMetadata(path, file);
                }
                else if (cacheRootDirectoryUri.IsBaseOf(sourceFileUri) && sourceArtifactIdBySourceFileUri.TryGetValue(sourceFileUri, out var sourceArtifactId))
                {
                    var path = $"<cache>/{sourceFileUri.GetPathRelativeTo(cacheRootDirectoryUri)}";
                    result[file] = CreateSourceFileMetadata(path, file, sourceArtifactId);
                }
                else
                {
                    // The source file is not under the same directory as the entrypoint file,
                    // and is not a restored module source file.
                    otherDirectoryFiles.Add(file);
                }
            }

            if (otherDirectoryFiles.Count > 0)
            {
                var rootCounter = 1;
                var distinctRootUris = FindDistinctRootUris(otherDirectoryFiles);

                foreach (var rootUri in distinctRootUris)
                {
                    var rootName = $"<root{rootCounter++}>";

                    foreach (var file in otherDirectoryFiles.Where(x => rootUri.IsBaseOf(x.FileHandle.Uri)))
                    {
                        var relativePath = file.FileHandle.Uri.GetPathRelativeTo(rootUri);
                        var path = $"{rootName}/{relativePath}";
                        result[file] = CreateSourceFileMetadata(path, file);
                    }
                }
            }

            UniquifyArchivePaths(result);

            return result;
        }

        private static ArchivedSourceFileMetadata CreateSourceFileMetadata(string path, ISourceFile sourceFile, string? sourceArtifactId = null) =>
            new(path, GetArchivedPath(path), sourceFile is BicepFile ? ArchivedSourceFileKind.Bicep : ArchivedSourceFileKind.ArmTemplate, sourceArtifactId);

        private static List<IOUri> FindDistinctRootUris(List<ISourceFile> sourceFiles)
        {
            var pathComparer = sourceFiles[0].FileHandle.Uri.PathComparer;
            var distinctRootUris = new List<IOUri>();
            var sortedDirectoryUris = sourceFiles
                .Select(x => x.FileHandle.Uri.Resolve("."))
                .DistinctBy(x => x.ToString(), pathComparer)
                .OrderBy(x => x.ToString(), pathComparer) // Sort by source file directory URI.
                .ToList();

            foreach (var directoryUri in sortedDirectoryUris)
            {
                if (distinctRootUris.Count == 0 || !distinctRootUris[^1].IsBaseOf(directoryUri))
                {
                    distinctRootUris.Add(directoryUri);
                }
            }

            return distinctRootUris;
        }

        private static string GetArchivedPath(string path)
        {
            path = $"files/{ReplaceForbiddenPathCharacters(path)}";

            if (path.Length <= MaxArchivePathLength)
            {
                return path;
            }

            var extension = path.LastIndexOf('.') is var lastDotIndex && lastDotIndex > 0 ? path[lastDotIndex..] : "";
            var tail = string.Concat("__path_too_long__", extension.AsSpan(0, Math.Min(10, extension.Length)));
            var shortPath = string.Concat(path.AsSpan(0, MaxArchivePathLength - tail.Length), tail);

            Debug.Assert(shortPath.Length == MaxArchivePathLength);

            return shortPath;
        }

        private static string ReplaceForbiddenPathCharacters(string path)
        {
            var buffer = new StringBuilder();

            foreach (var character in path)
            {
                buffer.Append(ForbiddenPathCharacters.Contains(character) ? '_' : character);
            }

            return buffer.ToString();
        }
    }
}
