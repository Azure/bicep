// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
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
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.SourceCode
{
    public class SourceNotAvailableException : Exception
    {
        public SourceNotAvailableException()
            : base("No source code is available for this module")
        { }
    }

    // Contains the individual source code files for a Bicep file and all of its dependencies.
    public partial class SourceArchive // Partial required for serialization
    {
        #region Attributes of this archive instance

        private ArchiveMetadataDto InstanceMetadata { get; init; }

        public ImmutableArray<SourceFileInfo> SourceFiles { get; init; }

        public string EntrypointRelativePath => InstanceMetadata.EntryPoint;

        // The version of Bicep which created this deserialized archive instance.
        public string? BicepVersion => InstanceMetadata.BicepVersion;
        public string FriendlyBicepVersion => InstanceMetadata.BicepVersion ?? "unknown";

        // The version of the metadata file format used by this archive instance.
        public int MetadataVersion => InstanceMetadata.MetadataVersion;

        public IReadOnlyDictionary<string, SourceCodeDocumentPathLink[]> DocumentLinks => InstanceMetadata.DocumentLinks ?? new Dictionary<string, SourceCodeDocumentPathLink[]>();

        #endregion

        public static class SourceKind
        {
            public const string Bicep = "bicep";
            public const string ArmTemplate = "armTemplate";
            public const string TemplateSpec = "templateSpec";
        }

        #region Constants

        private const string MetadataFileName = "__metadata.json";
        private const string FilesFolderName = "files";

        private static readonly ImmutableHashSet<char> PathCharsToAvoid = Path.GetInvalidFileNameChars()
            .Union(Path.GetInvalidPathChars())
            .Union(new char[] { '"', '*', ':', '&', '<', '>', '?', '\\', '/', '|', '+', '[', ']', '#' })
            .Where(ch => ch != '/')
            .ToImmutableHashSet();

        private const int MaxLegalPathLength = 260; // Limit for Windows
        private const int MaxArchivePathLength = MaxLegalPathLength - 10; // ... this gives us some extra room to deduplicate paths

        // NOTE: Only change this value if there is a breaking change such that old versions of Bicep should fail on reading new source archives
        public const int CurrentMetadataVersion = 1;
        private static readonly string CurrentBicepVersion = ThisAssembly.AssemblyVersion;

        #endregion

        // This is the info we expose via SourceFiles
        public record SourceFileInfo(
            // Note: Path is also used as the key for source file retrieval
            string Path,        // The location, relative to the main.bicep file's folder or one of the other roots.
            string ArchivePath, // The location (relative to root) of where the file is stored in the archive (munged from Path, e.g. in case Path starts with "../")
            string Kind,        // Kind of source (SourceKind)
            string Contents,    // File contents
            IOciArtifactReference? SourceArtifact // Points to an external artifact that contains the source for this module (e.g. "br:contoso.io/test/module1:v1"), appears in v0.26 and higher
        );

        public record SourceFileWithArtifactReference(
            ISourceFile SourceFile,
            ArtifactReference? SourceArtifact);

        #region Serialization

        [JsonSerializable(typeof(ArchiveMetadataDto))]
        [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
        private partial class MetadataSerializationContext : JsonSerializerContext { }

        // Metadata for the entire archive (stored in __metadata.json file in archive)
        private record ArchiveMetadataDto(
            int MetadataVersion,
            string? BicepVersion,
            string EntryPoint, // Path of the entrypoint file
            IEnumerable<SourceFileInfoDto> SourceFiles,
            IReadOnlyDictionary<string, SourceCodeDocumentPathLink[]>? DocumentLinks = null // Maps source file path -> array of document links inside that file
        );

        // A single SourceFiles entry in the metadata.json file
        private partial record SourceFileInfoDto(
            // IF ADDING TO THIS: Remember both forwards and backwards compatibility.
            // E.g., previous versions must be able to deal with unrecognized source kinds.
            // (but see CurrentMetadataVersion for breaking changes)
            string Path,        // the location, relative to the main.bicep file's folder, for the file that will be shown to the end user (required in all Bicep versions)
            string ArchivePath, // the location (relative to root) of where the file is stored in the archive
            string Kind,        // kind of source (SourceKind)
            string? SourceArtifactId = null // Points to an external artifact that contains the source for this module (e.g. "br:contoso.io/test/module1:v1"), appears in v0.26 and higher
        );

        /* Example __metadata.json (ArchiveMetadataDto):
        {
            "metadataVersion": 0,
            "entryPoint": "my entrypoint.bicep",
            "bicepVersion": "0.18.19",
            "sourceFiles": [
                {
                    "sourcePath": "my entrypoint.bicep",
                    "archivePath": "files/my entrypoint.bicep",
                    "kind": "bicep"
                },
                {
                    "sourcePath": "modules/main.bicep",
                    "archivePath": "files/modules/main.bicep",
                    "kind": "bicep"
                },
                {
                  "path": "\u003Ccache\u003E/br/mcr.microsoft.com/bicep$app$app-configuration/1.0.1$/main.json",
                  "archivePath": "files/_cache_/br/mcr.microsoft.com/bicep$app$app-configuration/1.0.1$/main.json",
                  "kind": "armTemplate",
                  "sourceArtifactId": "br:contoso.io/test/module1:v1"
                },
            ],
            "DocumentLinks": { ... }
        }
        */

        #endregion

        public static ResultWithException<SourceArchive> UnpackFromStream(Stream stream)
        {
            try
            {
                var archive = new SourceArchive(stream);
                if (archive.GetRequiredBicepVersionMessage() is string message)
                {
                    return new(new Exception(message));
                }
                else
                {
                    return new(archive);
                }
            }
            catch (Exception ex)
            {
                return new(ex);
            }
        }

        /// <summary>
        /// Bundles all the sources from a compilation group (thus source for a bicep file and all its dependencies
        /// in JSON form) into an archive (as a stream)
        /// </summary>
        /// <returns>A .tgz file as a binary stream</returns>
        public static Stream PackSourcesIntoStream(IModuleDispatcher moduleDispatcher, SourceFileGrouping sourceFileGrouping, string? cacheRoot)
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
                sourceFileGrouping.SourceFiles.Select(x => new SourceFileWithArtifactReference(x, uriToArtifactReference.TryGetValue(x.FileUri, out var reference) ? reference : null));

            var documentLinks = SourceCodeDocumentLinkHelper.GetAllModuleDocumentLinks(sourceFileGrouping);
            return PackSourcesIntoStream(sourceFileGrouping.EntryPoint.FileUri, cacheRoot, documentLinks, sourceFilesWithArtifactReference.ToArray());
        }

        public static Stream PackSourcesIntoStream(Uri entrypointFileUri, string? cacheRoot, params SourceFileWithArtifactReference[] sourceFiles)
        {
            return PackSourcesIntoStream(entrypointFileUri, cacheRoot, documentLinks: null, sourceFiles);
        }

        public static Stream PackSourcesIntoStream(Uri entrypointFileUri, string? cacheRoot, IReadOnlyDictionary<Uri, SourceCodeDocumentUriLink[]>? documentLinks, params SourceFileWithArtifactReference[] sourceFiles)
        {
            // Don't package template spec files - they don't appear in the compiled JSON so we shouldn't expose them
            sourceFiles = sourceFiles.Where(sf => sf.SourceFile is not TemplateSpecFile).ToArray();

            // Filter out any links where the source or target is not in our list of files to package
            var sourceFileUris = sourceFiles.Select(sf => sf.SourceFile.FileUri).ToArray();
            documentLinks = documentLinks?
                .Where(kvp => sourceFileUris.Contains(kvp.Key))
                .Select(uriAndLink => (uriAndLink.Key, uriAndLink.Value.Where(link => sourceFileUris.Contains(link.Target)).ToArray()))
                .ToDictionary();

            var stream = new MemoryStream();
            using (var gz = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
            {
                using (var tarWriter = new TarWriter(gz, leaveOpen: true))
                {
                    var filesMetadata = new List<SourceFileInfoDto>();
                    string? entryPointPath = null;

                    var paths = sourceFiles.Select(f => GetPath(f.SourceFile.FileUri)).ToArray();
                    var mapPathToRootPath = SourceCodePathHelper.MapPathsToDistinctRoots(cacheRoot, paths);
                    var entrypointRootPath = mapPathToRootPath[GetPath(entrypointFileUri)];
                    var mapRootPathToRootNewName = NameRoots(mapPathToRootPath, entrypointRootPath, cacheRoot);

                    var sourceUriToRelativePathMap = new Dictionary<Uri, string>();

                    foreach (var (file, artifactReference) in sourceFiles)
                    {
                        string source = file.GetOriginalSource();
                        string kind = file switch
                        {
                            BicepFile bicepFile => SourceKind.Bicep,
                            ArmTemplateFile armTemplateFile => SourceKind.ArmTemplate,
                            TemplateSpecFile => SourceKind.TemplateSpec,
                            _ => throw new ArgumentException($"Unexpected input source file type {file.GetType().Name}"),
                        };

                        Debug.Assert(artifactReference is null || artifactReference is OciArtifactReference ociArtifactReference && ociArtifactReference.Type == ArtifactType.Module,
                            "Artifact reference must be null or an OCI module reference");

                        var path = GetPath(file.FileUri);
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
                            new SourceFileInfoDto(
                                relativePath,
                                archivePath,
                                kind,
                                artifactReference?.FullyQualifiedReference));

                        if (PathHelper.PathComparer.Equals(file.FileUri, entrypointFileUri))
                        {
                            if (entryPointPath is not null)
                            {
                                throw new ArgumentException($"{nameof(SourceArchive)}.{nameof(PackSourcesIntoStream)}: Multiple source files with the entrypoint \"{entrypointFileUri.AbsoluteUri}\" were passed in.");
                            }

                            entryPointPath = relativePath;
                        }

                        sourceUriToRelativePathMap.Add(file.FileUri, relativePath);
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

        private static (string relativePath, string archivePath) CalculateRelativeAndArchivePaths(string path, string root, string rootName)
        {
            // Replace root of the path with root's "friendly" name to help avoid unintended
            //   disclosure of user paths
            var relativePath = Path.GetRelativePath(root, path);
            Debug.Assert(!relativePath.StartsWith("../"), $"All paths should be under one of the roots");
            relativePath = SourceCodePathHelper.NormalizeSlashes($"{rootName}{relativePath}");

            // Handle illegal/problematic file characters in the path we use inside the archive
            var archivePath = new string(relativePath.Select(ch => PathCharsToAvoid.Contains(ch) ? '_' : ch).ToArray());

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

        private static string UniquifyArchivePath(IList<SourceFileInfoDto> filesMetadata, string archivePath)
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

        public SourceFileInfo FindExpectedSourceFile(string path)
        {
            // Note: We can use ordinal compare here because even if the module was published from Windows, the paths written into the
            //   source archive will be consistent in casing
            if (this.SourceFiles.FirstOrDefault(f => 0 == StringComparer.Ordinal.Compare(f.Path, path)) is { } sourceFile)
            {
                return sourceFile;
            }

            throw new Exception($"Unable to find source file \"{path}\" in the source archive");
        }

        private string? GetRequiredBicepVersionMessage()
        {
            if (MetadataVersion < CurrentMetadataVersion)
            {
                return $"This source code was published with an older, incompatible version of Bicep ({FriendlyBicepVersion}). You are using version {ThisAssembly.AssemblyVersion}.";
            }

            if (MetadataVersion > CurrentMetadataVersion)
            {
                return $"This source code was published with a newer, incompatible version of Bicep ({FriendlyBicepVersion}). You are using version {ThisAssembly.AssemblyVersion}. You need a newer version in order to view the module source.";
            }

            return null;
        }

        private string FindRootForFile(string[] roots, string path)
        {
            foreach (var root in roots)
            {
                if (path.StartsWith(root))
                {
                    return root;
                }
            }

            throw new ArgumentException($"Could not find a root for path \"{path}\"");
        }

        private SourceArchive(Stream stream)
        {
            var filesBuilder = ImmutableDictionary.CreateBuilder<string, string>();

            var gz = new GZipStream(stream, CompressionMode.Decompress);
            using var tarReader = new TarReader(gz);

            while (tarReader.GetNextEntry() is { } entry)
            {
                string contents = entry.DataStream is null ? string.Empty : new StreamReader(entry.DataStream, Encoding.UTF8).ReadToEnd();
                filesBuilder.Add(entry.Name, contents);
            }

            var dictionary = filesBuilder.ToImmutableDictionary();

            var metadataJson = dictionary[MetadataFileName]
                ?? throw new BicepException("Incorrectly formatted source file: No {MetadataArchivedFileName} entry");
            var metadata = JsonSerializer.Deserialize(metadataJson, MetadataSerializationContext.Default.ArchiveMetadataDto)
                ?? throw new BicepException("Source archive has invalid metadata entry");

            var infos = new List<SourceFileInfo>();
            foreach (var info in metadata.SourceFiles.OrderBy(e => e.Path).ThenBy(e => e.ArchivePath))
            {
                var contents = dictionary[info.ArchivePath]
                    ?? throw new BicepException("Incorrectly formatted source file: File entry not found: \"{info.ArchivePath}\"");
                var artifactId = TrimScheme(info.SourceArtifactId ?? string.Empty);
                var artifactReference = artifactId is { } ? OciArtifactReference.TryParseModule(artifactId).TryUnwrap() : null;
                infos.Add(new SourceFileInfo(info.Path, info.ArchivePath, info.Kind, contents, artifactReference));
            }

            this.InstanceMetadata = metadata;
            this.SourceFiles = [.. infos];
        }

        private string? TrimScheme(string artifactId)
        {
            if (artifactId.StartsWith(OciArtifactReferenceFacts.SchemeWithColon))
            {
                return artifactId[OciArtifactReferenceFacts.SchemeWithColon.Length..];
            }

            // Skip unknown schemes for possible future compatibility
            return null;
        }

        private static string CreateMetadataFileContents(
            string entrypointPath,
            IEnumerable<SourceFileInfoDto> files,
            IReadOnlyDictionary<string, SourceCodeDocumentPathLink[]>? documentLinks
        )
        {
            var metadata = new ArchiveMetadataDto(CurrentMetadataVersion, CurrentBicepVersion, entrypointPath, files, documentLinks);
            return JsonSerializer.Serialize(metadata, MetadataSerializationContext.Default.ArchiveMetadataDto);
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
    }
}
