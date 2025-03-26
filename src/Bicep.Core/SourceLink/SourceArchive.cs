// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Formats.Tar;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;
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

        private readonly ImmutableDictionary<string, string> fileEntries;

        public record SourceFileWithArtifactReference(
            ISourceFile SourceFile,
            ArtifactReference? SourceArtifact);

        private SourceArchive(SourceArchiveMetadata metadata, ImmutableDictionary<string, string> fileEntries)
        {
            this.metadata = metadata;
            this.fileEntries = fileEntries;
        }

        public string EntrypointRelativePath => metadata.EntryPoint;

        public int SourceFileCount => metadata.SourceFiles.Length;

        public static ResultWithException<SourceArchive> UnpackFromStream(Stream stream)
        {
            try
            {
                var fileEntries = new Dictionary<string, string>();
                var gz = new GZipStream(stream, CompressionMode.Decompress);
                using var tarReader = new TarReader(gz);

                while (tarReader.GetNextEntry() is { } entry)
                {
                    string contents = entry.DataStream is null ? string.Empty : new StreamReader(entry.DataStream).ReadToEnd();
                    fileEntries.Add(entry.Name, contents);
                }

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

                return new(new SourceArchive(metadata, fileEntries.ToImmutableDictionary()));
            }
            catch (Exception ex)
            {
                return new(ex);
            }
        }

        public BinaryData ToBinaryData()
        {
            var stream = new MemoryStream();
            using (var gz = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
            using (var tarWriter = new TarWriter(gz, leaveOpen: true))
            {
                var metadataJson = JsonSerializer.Serialize(this.metadata, SourceArchiveMetadataSerializationContext.Default.SourceArchiveMetadata);
                WriteNewFileEntry(tarWriter, MetadataFileName, metadataJson);

                foreach (var sourceFileMetadata in this.metadata.SourceFiles)
                {
                    var contents = this.fileEntries[sourceFileMetadata.ArchivePath];
                    WriteNewFileEntry(tarWriter, sourceFileMetadata.ArchivePath, contents);
                }
            }

            stream.Seek(0, SeekOrigin.Begin);

            return BinaryData.FromStream(stream);
        }

        /// <summary>
        /// Bundles all the sources from a compilation group (thus source for a bicep file and all its dependencies
        /// in JSON form) into an archive (as a stream)
        /// </summary>
        /// <returns>A .tgz file as a binary stream</returns>
        public static Stream PackSourcesIntoStream(IModuleDispatcher moduleDispatcher, SourceFileGrouping sourceFileGrouping, IDirectoryHandle? cacheRoot)
        {
            // Find the artifact reference for each source file of an external module that was published with sources
            Dictionary<Uri, OciArtifactReference> uriToArtifactReference = new();
            foreach (var artifact in sourceFileGrouping.ArtifactLookup.Values)
            {
                if (artifact.Syntax is not ModuleDeclarationSyntax module ||
                    artifact.Reference is not OciArtifactReference artifactReference ||
                    !artifact.Result.IsSuccess(out var uri) ||
                    uriToArtifactReference.ContainsKey(uri) ||
                    // Only those that were published with source
                    !moduleDispatcher.TryGetModuleSources(artifact.Reference).IsSuccess(out var archive))
                {
                    continue;
                }

                uriToArtifactReference[uri] = artifactReference;
            }

            var sourceFilesWithArtifactReference =
                sourceFileGrouping.SourceFiles.Select(x => new SourceFileWithArtifactReference(x, uriToArtifactReference.TryGetValue(x.Uri, out var reference) ? reference : null));

            var documentLinks = SourceCodeDocumentLinkHelper.GetAllModuleDocumentLinks(sourceFileGrouping);
            return PackSourcesIntoStream(sourceFileGrouping.EntryPoint.Uri, cacheRoot, documentLinks, sourceFilesWithArtifactReference.ToArray());
        }

        public static Stream PackSourcesIntoStream(Uri entrypointFileUri, IDirectoryHandle? cacheRoot, params SourceFileWithArtifactReference[] sourceFiles)
        {
            return PackSourcesIntoStream(entrypointFileUri, cacheRoot, documentLinks: null, sourceFiles);
        }

        public static Stream PackSourcesIntoStream(Uri entrypointFileUri, IDirectoryHandle? cacheRoot, IReadOnlyDictionary<Uri, SourceCodeDocumentUriLink[]>? documentLinks, params SourceFileWithArtifactReference[] sourceFiles)
        {
            // Don't package template spec files - they don't appear in the compiled JSON so we shouldn't expose them
            sourceFiles = sourceFiles.Where(sf => sf.SourceFile is not TemplateSpecFile).ToArray();

            // Filter out any links where the source or target is not in our list of files to package
            var sourceFileUris = sourceFiles.Select(sf => sf.SourceFile.Uri).ToArray();
            documentLinks = documentLinks?
                .Where(kvp => sourceFileUris.Contains(kvp.Key))
                .Select(uriAndLink => (uriAndLink.Key, uriAndLink.Value.Where(link => sourceFileUris.Contains(link.Target)).ToArray()))
                .ToDictionary();

            var stream = new MemoryStream();
            using (var gz = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
            {
                using (var tarWriter = new TarWriter(gz, leaveOpen: true))
                {
                    var filesMetadata = new List<LinkedSourceFileMetadata>();
                    string? entryPointPath = null;

                    var paths = sourceFiles.Select(f => GetPath(f.SourceFile.Uri)).ToArray();
                    var mapPathToRootPath = SourceCodePathHelper.MapPathsToDistinctRoots(cacheRoot?.Uri.TryGetLocalFilePath(), paths);
                    var entrypointRootPath = mapPathToRootPath[GetPath(entrypointFileUri)];
                    var mapRootPathToRootNewName = NameRoots(mapPathToRootPath, entrypointRootPath, cacheRoot?.Uri.TryGetLocalFilePath());

                    var sourceUriToRelativePathMap = new Dictionary<Uri, string>();

                    foreach (var (file, artifactReference) in sourceFiles)
                    {
                        string source = file.Text;
                        LinkedSourceFileKind kind = file switch
                        {
                            BicepFile bicepFile => LinkedSourceFileKind.Bicep,
                            ArmTemplateFile armTemplateFile => LinkedSourceFileKind.ArmTemplate,
                            TemplateSpecFile => LinkedSourceFileKind.TemplateSpec,
                            _ => throw new ArgumentException($"Unexpected input source file type {file.GetType().Name}"),
                        };

                        Debug.Assert(artifactReference is null || artifactReference is OciArtifactReference ociArtifactReference && ociArtifactReference.Type == ArtifactType.Module,
                            "Artifact reference must be null or an OCI module reference");

                        var path = GetPath(file.Uri);
                        var root = mapPathToRootPath[path];
                        var rootName = mapRootPathToRootNewName[root];
                        var (relativePath, archivePath) = CalculateRelativeAndArchivePaths(path, root, rootName);

                        Trace.WriteLine($"Packing source file: {path} -> {relativePath}");

                        if (filesMetadata.Any(f => PathHelper.PathComparer.Equals(f.Path, path)))
                        {
                            throw new ArgumentException("Cannot have multiple files in source archive with the same path");
                        }

                        // Duplicate archive paths are possible after names have been munged
                        archivePath = UniquifyArchivePath(filesMetadata, archivePath);

                        WriteNewFileEntry(tarWriter, archivePath, source);
                        filesMetadata.Add(
                            new LinkedSourceFileMetadata(
                                relativePath,
                                archivePath,
                                kind,
                                artifactReference?.FullyQualifiedReference));

                        if (PathHelper.PathComparer.Equals(file.Uri, entrypointFileUri))
                        {
                            if (entryPointPath is not null)
                            {
                                throw new ArgumentException($"{nameof(SourceArchive)}.{nameof(PackSourcesIntoStream)}: Multiple source files with the entrypoint \"{entrypointFileUri.AbsoluteUri}\" were passed in.");
                            }

                            entryPointPath = relativePath;
                        }

                        sourceUriToRelativePathMap.Add(file.Uri, relativePath);
                    }

                    if (entryPointPath is null)
                    {
                        throw new ArgumentException($"{nameof(SourceArchive)}.{nameof(PackSourcesIntoStream)}: No source file with entrypoint \"{entrypointFileUri.AbsoluteUri}\" was passed in.");
                    }

                    // Convert links
                    var pathBasedLinks = UriDocumentLinksToPathBasedLinks(sourceUriToRelativePathMap, documentLinks);

                    // Create and add the metadata file
                    var metadataContents = CreateMetadataFileContents(entryPointPath, filesMetadata, pathBasedLinks);
                    WriteNewFileEntry(tarWriter, MetadataFileName, metadataContents);
                }
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public ImmutableArray<SourceCodeDocumentPathLink> FindDocumentLinks(string path) => this.metadata.DocumentLinks[path];

        public LinkedSourceFile FindSourceFile(string path)
        {
            // TODO(shenglol): Binary search.
            var metadata = this.metadata.SourceFiles.Single(x => x.Path.Equals(path, StringComparison.Ordinal));
            var contents = this.fileEntries[metadata.ArchivePath];

            return new(metadata, contents);
        }

        private static (string relativePath, string archivePath) CalculateRelativeAndArchivePaths(string path, string root, string rootName)
        {
            // Replace root of the path with root's "friendly" name to help avoid unintended
            //   disclosure of user paths
            var relativePath = Path.GetRelativePath(root, path);
            Debug.Assert(!relativePath.StartsWith("../"), $"All paths should be under one of the roots");
            relativePath = SourceCodePathHelper.NormalizeSlashes($"{rootName}{relativePath}");

            // Handle illegal/problematic file characters in the path we use inside the archive
            var archivePath = ReplaceForbiddenPathCharacters(relativePath);

            // Place all sources files under "files/" in the archive
            archivePath = Path.Join(FilesFolderName, archivePath);

            // Shorten if needed
            archivePath = SourceCodePathHelper.Shorten(archivePath, MaxArchivePathLength);

            return (SourceCodePathHelper.NormalizeSlashes(relativePath), SourceCodePathHelper.NormalizeSlashes(archivePath));
        }

        private static string GetPath(Uri uri)
        {
            return SourceCodePathHelper.NormalizeSlashes(uri.LocalPath);
        }

        private static string UniquifyArchivePath(IList<LinkedSourceFileMetadata> filesMetadata, string archivePath)
        {
            int suffix = 1;
            string tryPath = archivePath;

            while (filesMetadata.Any(f => PathHelper.PathComparer.Equals(f.ArchivePath, tryPath)))
            {
                suffix += 1;
                tryPath = $"{archivePath}({suffix})";
            }

            return tryPath;
        }

        private static Dictionary<string, string> NameRoots(IDictionary<string, string> rootsMap, string entrypointRoot, string? cacheRoot)
        {
            var rootNamesMap = new Dictionary<string, string>();

            // Entrypoint path is always as the top of the archive path hierarchy
            rootNamesMap[entrypointRoot] = "";

            // Remaining roots are "<root2>", "<root3>", etc., or <cache> for the module cache root
            int i = 2;
            foreach (var root in rootsMap.Values.Distinct().Where(r => r != entrypointRoot))
            {
                if (PathHelper.PathComparer.Equals(root, cacheRoot))
                {
                    rootNamesMap[root] = "<cache>/";
                    continue;
                }

                var rootName = $"<root{i}>/";
                rootNamesMap.Add(root, rootName);
                ++i;
            }

            return rootNamesMap;
        }

        private static string CreateMetadataFileContents(
            string entrypointPath,
            IEnumerable<LinkedSourceFileMetadata> files,
            IReadOnlyDictionary<string, SourceCodeDocumentPathLink[]>? documentLinks
        )
        {
            var metadata = new SourceArchiveMetadata(CurrentMetadataVersion, CurrentBicepVersion, entrypointPath, files.ToImmutableArray(), documentLinks?.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableArray()));
            return JsonSerializer.Serialize(metadata, SourceArchiveMetadataSerializationContext.Default.SourceArchiveMetadata);
        }

        private static IReadOnlyDictionary<string, SourceCodeDocumentPathLink[]>? UriDocumentLinksToPathBasedLinks(
            IReadOnlyDictionary<Uri, string> sourceUriToRelativePathMap,
            IReadOnlyDictionary<Uri, SourceCodeDocumentUriLink[]>? uriBasedDocumentLinks
        )
        {
            return uriBasedDocumentLinks?.Select(
                x => new KeyValuePair<string, SourceCodeDocumentPathLink[]>(
                    sourceUriToRelativePathMap[x.Key],
                    x.Value.Select(link => DocumentPathLinkFromUriLink(sourceUriToRelativePathMap, link)).ToArray()
                )).ToImmutableDictionary();
        }

        private static SourceCodeDocumentPathLink DocumentPathLinkFromUriLink(
            IReadOnlyDictionary<Uri, string> sourceUriToRelativePathMap,
            SourceCodeDocumentUriLink uriBasedLink)
        {
            return new SourceCodeDocumentPathLink(
                uriBasedLink.Range,
                sourceUriToRelativePathMap[uriBasedLink.Target]);
        }

        private static void WriteNewFileEntry(TarWriter tarWriter, string archivePath, string contents)
        {
            Debug.Assert(!archivePath.Contains('\\'), $"Source archive paths should not contain backslashes, only forward slashes: \"{archivePath}\"");
            Debug.Assert(!archivePath.Contains(':'), $"Source archive paths should not contain drive letters or colons: \"{archivePath}\"");
            Debug.Assert(!archivePath.Contains("/../") && !archivePath.StartsWith("../"), $"Source archive paths should not contain \"..\" folders: \"{archivePath}\"");
            Debug.Assert(!archivePath.Contains("/./") && !archivePath.StartsWith("./"), $"Source archive paths should not contain \"/./\" folders: \"{archivePath}\"");
            Debug.Assert(!Path.IsPathFullyQualified(archivePath)
                && !Path.IsPathRooted(archivePath), $"Source archive paths must be relative: \"{archivePath}\"");
            Debug.Assert(archivePath.Length <= MaxLegalPathLength, $"Source archive paths must have length at most {MaxLegalPathLength}");
            if (archivePath != MetadataFileName)
            {
                Debug.Assert(archivePath.StartsWith($"{FilesFolderName}/"), $"Source archive paths should be relative to the \"{FilesFolderName}\" folder");
            }

            var tarEntry = new PaxTarEntry(TarEntryType.RegularFile, archivePath);
            tarEntry.DataStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
            tarWriter.WriteEntry(tarEntry);
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
