// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Bicep.Core.Semantics;
using Bicep.Core.Workspaces;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;
using System.Text;
using Bicep.Core.Navigation;
using System.IO.Abstractions;
using System.Linq;
using System.Formats.Tar;
using System.Threading.Tasks;
using System.Collections.Immutable;
using static Bicep.Core.SourceCode.SourceArchive;
using System.Text.Json.Serialization;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Exceptions;

namespace Bicep.Core.SourceCode
{
    // Contains the individual source code files for a Bicep file and all of its dependencies.
    public partial class SourceArchive
    {
        public ImmutableArray<SourceFileInfo> SourceFiles { get; init; }
        public string EntrypointPath { get; init; }

        public const string SourceKind_Bicep = "bicep";
        public const string SourceKind_ArmTemplate = "armTemplate";
        public const string SourceKind_TemplateSpec = "templateSpec";

        private const string MetadataArchivedFileName = "__metadata.json";

        private bool isDisposed = false;//asfdg remove

        // WARNING: Only change this value if there is a breaking change such that old versions of Bicep should fail on reading this source archive
        private const int CurrentMetadataVersion = 0; // TODO: Change to 1 when remove experimental flag

        public partial record SourceFileInfo(
            string Path,        // the location, relative to the main.bicep file's folder, for the file that will be shown to the end user (required in all Bicep versions)
            string ArchivePath, // the location (relative to root) of where the file is stored in the archive
            string Kind,         // kind of source
            string Contents
        );

        [JsonSerializable(typeof(MetadataEntry))]
        [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
        private partial class MetadataSerializationContext : JsonSerializerContext { }

        [JsonSerializable(typeof(MetadataEntry))]
        private record MetadataEntry(
            int MetadataVersion,
            string EntryPoint, // Path of the entrypoint file
            IEnumerable<SourceFileInfoEntry> SourceFiles
        );

        [JsonSerializable(typeof(SourceFileInfoEntry))]
        private partial record SourceFileInfoEntry(
            // IF ADDING TO THIS: Remember both forwards and backwards compatibility.
            // E.g., previous versions must be able to deal with unrecognized source kinds.
            // (but see CurrentMetadataVersion for breaking changes)
            string Path,        // the location, relative to the main.bicep file's folder, for the file that will be shown to the end user (required in all Bicep versions)
            string ArchivePath, // the location (relative to root) of where the file is stored in the archive
            string Kind         // kind of source
        );

        public static SourceArchive FromStream(Stream stream)
        {
            return new SourceArchive(stream);
        }

        /// <summary>
        /// Bundles all the sources from a compilation group (thus source for a bicep file and all its dependencies
        /// in JSON form)
        /// </summary>
        /// <returns></returns>
        public static Stream PackSourcesIntoStream(SourceFileGrouping sourceFileGrouping)
        {
            return PackSourcesIntoStream(sourceFileGrouping.EntryFileUri, sourceFileGrouping.SourceFiles.ToArray());
        }

        // TODO: Toughen this up to handle conflicting paths, ".." paths, etc.
        public static Stream PackSourcesIntoStream(Uri entrypointFileUri, params ISourceFile[] sourceFiles)
        {
            var baseFolderBuilder = new UriBuilder(entrypointFileUri)
            {
                Path = string.Join("", entrypointFileUri.Segments.SkipLast(1))
            };
            var baseFolderUri = baseFolderBuilder.Uri;

            var stream = new MemoryStream();
            using (var gz = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
            {
                using (var tarWriter = new TarWriter(gz, leaveOpen: true))
                {
                    var filesMetadata = new List<SourceFileInfoEntry>();
                    string? entryPointPath = null;

                    foreach (var file in sourceFiles)
                    {
                        string source = file.GetOriginalSource();
                        string kind = file switch
                        {
                            BicepFile bicepFile => SourceKind_Bicep,
                            ArmTemplateFile armTemplateFile => SourceKind_ArmTemplate,
                            TemplateSpecFile => SourceKind_TemplateSpec,
                            _ => throw new ArgumentException($"Unexpected source file type {file.GetType().Name}"),
                        };

                        var (location, archivePath) = CalculateFilePathsFromUri(file.FileUri);
                        WriteNewFileEntry(tarWriter, archivePath, source);
                        filesMetadata.Add(new SourceFileInfoEntry(location, archivePath, kind));

                        if (file.FileUri == entrypointFileUri)
                        {
                            if (entryPointPath is not null)
                            {
                                throw new ArgumentException($"{nameof(SourceArchive)}.{nameof(PackSourcesIntoStream)}: Multiple source files with the entrypoint \"{entrypointFileUri.AbsoluteUri}\" were passed in.");
                            }

                            entryPointPath = location;
                        }
                    }

                    if (entryPointPath is null)
                    {
                        throw new ArgumentException($"{nameof(SourceArchive)}.{nameof(PackSourcesIntoStream)}: No source file with entrypoint \"{entrypointFileUri.AbsoluteUri}\" was passed in.");
                    }

                    // Add the metadata file
                    var metadataContents = CreateMetadataFileContents(entryPointPath, filesMetadata);
                    WriteNewFileEntry(tarWriter, MetadataArchivedFileName, metadataContents);
                }
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;

            (string location, string archivePath) CalculateFilePathsFromUri(Uri uri)
            {
                Uri relativeUri = baseFolderUri.MakeRelativeUri(uri);
                var relativeLocation = Uri.UnescapeDataString(relativeUri.OriginalString);
                return (relativeLocation, relativeLocation);
            }
        }

        private SourceArchive(Stream stream)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(SourceArchive));
            }

            var filesBuilder = ImmutableDictionary.CreateBuilder<string, string>();

            stream.Position = 0;
            var gz = new GZipStream(stream, CompressionMode.Decompress);
            using var tarReader = new TarReader(gz);

            while (tarReader.GetNextEntry() is { } entry)
            {
                string contents = entry.DataStream is null ? string.Empty : new StreamReader(entry.DataStream, Encoding.UTF8).ReadToEnd();
                filesBuilder.Add(entry.Name, contents);
            }

            var dictionary = filesBuilder.ToImmutableDictionary();

            var metadataJson = dictionary[MetadataArchivedFileName]
                ?? throw new BicepException("Incorrectly formatted source file: No {MetadataArchivedFileName} entry");
            var metadata = JsonSerializer.Deserialize<MetadataEntry>(metadataJson, MetadataSerializationContext.Default.MetadataEntry)
                ?? throw new BicepException("Source archive has invalid metadata entry");

            var infos = new List<SourceFileInfo>();
            foreach (var info in metadata.SourceFiles.OrderBy(e => e.Path).ThenBy(e => e.ArchivePath))
            {
                var contents = dictionary[info.ArchivePath]
                    ?? throw new BicepException("Incorrectly formatted source file: File entry not found: \"{info.ArchivePath}\"");
                infos.Add(new SourceFileInfo(info.Path, info.ArchivePath, info.Kind, contents));
            }

            this.EntrypointPath = metadata.EntryPoint;
            this.SourceFiles = infos.ToImmutableArray();
        }

        private static string CreateMetadataFileContents(string entrypointPath, IEnumerable<SourceFileInfoEntry> files)
        {
            // Add the __metadata.json file
            var metadata = new MetadataEntry(CurrentMetadataVersion, entrypointPath, files);
            return JsonSerializer.Serialize(metadata, MetadataSerializationContext.Default.MetadataEntry);
        }

        private static void WriteNewFileEntry(TarWriter tarWriter, string archivePath, string contents)
        {
            var tarEntry = new PaxTarEntry(TarEntryType.RegularFile, archivePath);
            tarEntry.DataStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
            tarWriter.WriteEntry(tarEntry);
        }
    }
}
