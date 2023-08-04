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
using static Bicep.Core.Registry.SourceArchive;

namespace Bicep.Core.Registry;

public class SourceArchive : IDisposable
{
    private ZipArchive? zipArchive;

    public const string SourceKind_Bicep = "bicep";
    public const string SourceKind_ArmTemplate = "armTemplate";
    public const string SourceKind_TemplateSpec = "templateSpec";

    public const string MetadataArchivedFileName = "__metadata.json";

    // WARNING: Only change this value if there is a breaking change such that old versions of Bicep should fail on reading this source archive
    public const int CurrentMetadataVersion = 0; // TODO: Change to 1 when remove experimental flag

    public record FileMetadata(
        // IF ADDING TO THIS: Remember both forwards and backwards compatibility.
        // E.g., previous versions must be able to deal with unrecognized source kinds.
        string Path,         // the location, relative to the main.bicep file's folder, for the file that will be shown to the end user (required in all Bicep versions)
        string ArchivedPath, // the location (relative to root) of where the file is stored in the archive
        string Kind          // kind of source
    );

    public record Metadata(
        int MetadataVersion,
        string EntryPoint, // Path of the entrypoint file
        IEnumerable<FileMetadata> SourceFiles
    );

    public SourceArchive(Stream stream)
    {
        this.zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
    }

    public string GetEntrypointPath()
    {
        return GetMetadata().EntryPoint;
    }

    public static Stream PackSources(SourceFileGrouping sourceFileGrouping)
    {
        return PackSources(sourceFileGrouping.EntryFileUri, sourceFileGrouping.SourceFiles.ToArray());
    }

    // TODO: Toughen this up to handle conflicting paths, ".." paths, etc.
    public static Stream PackSources(Uri entrypointFileUri, params ISourceFile[] sourceFiles)
    {
        string? entryPointPath = null;

        var baseFolderBuilder = new UriBuilder(entrypointFileUri)
        {
            Path = string.Join("", entrypointFileUri.Segments.SkipLast(1))
        };
        var baseFolderUri = baseFolderBuilder.Uri;

        var stream = new MemoryStream();
        using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            var filesMetadata = new List<FileMetadata>();

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

                var (location, archivedPath) = CalculateFilePathsFromUri(file.FileUri);
                WriteNewFileEntry(zipArchive, archivedPath, source);
                filesMetadata.Add(new FileMetadata(location, archivedPath, kind));

                if (file.FileUri == entrypointFileUri)
                {
                    if (entryPointPath is not null)
                    {
                        throw new ArgumentException($"{nameof(SourceArchive)}.{nameof(PackSources)}: Multiple source files with the entrypoint \"{entrypointFileUri.AbsoluteUri}\" were passed in.");
                    }

                    entryPointPath = location;
                }
            }

            if (entryPointPath is null)
            {
                throw new ArgumentException($"{nameof(SourceArchive)}.{nameof(PackSources)}: No source file with entrypoint \"{entrypointFileUri.AbsoluteUri}\" was passed in.");
            }

            // Add the metadata file
            var metadataContents = CreateMetadataFileContents(entryPointPath, filesMetadata);
            WriteNewFileEntry(zipArchive, MetadataArchivedFileName, metadataContents);
        }

        stream.Seek(0, SeekOrigin.Begin);
        return stream;

        (string location, string archivedPath) CalculateFilePathsFromUri(Uri uri)
        {
            Uri relativeUri = baseFolderUri.MakeRelativeUri(uri);
            var relativeLocation = Uri.UnescapeDataString(relativeUri.OriginalString);
            return (relativeLocation, relativeLocation);
        }
    }

    public string GetMetadataFileContents()
    {
        return GetFileEntryContents(MetadataArchivedFileName);
    }

    public IEnumerable<(FileMetadata Metadata, string Contents)> GetSourceFiles()
    {
        if (zipArchive is null)
        {
            throw new ObjectDisposedException(nameof(SourceArchive));
        }

        var metadata = GetMetadata();
        foreach (var entry in metadata.SourceFiles)
        {
            yield return (entry, GetFileEntryContents(entry.ArchivedPath));
        }
    }

    private string GetFileEntryContents(string archivedPath)
    {
        if (zipArchive is null)
        {
            throw new ObjectDisposedException(nameof(SourceArchive));
        }

        if (zipArchive.GetEntry(archivedPath) is not ZipArchiveEntry entry)
        {
            throw new Exception($"Could not find expected entry in archived module sources \"{archivedPath}\"");
        }

        using var entryStream = entry.Open();
        using var sr = new StreamReader(entryStream);
        return sr.ReadToEnd();
    }

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    private static string CreateMetadataFileContents(string entrypointPath, IEnumerable<FileMetadata> files)
    {
        // Add the __metadata.json file
        var metadata = new Metadata(CurrentMetadataVersion, entrypointPath, files);
        return JsonSerializer.Serialize(metadata, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    private Metadata GetMetadata()
    {
        var metadataJson = GetFileEntryContents(MetadataArchivedFileName);
        var metadata = JsonSerializer.Deserialize<Metadata>(metadataJson, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            ?? throw new ArgumentException($"Unable to deserialize metadata from archived file \"{MetadataArchivedFileName}\"");
        if (metadata.MetadataVersion > CurrentMetadataVersion)
        {
            throw new Exception($"Source archive contains a metadata file with metadata version {metadata.MetadataVersion}, which this version of Bicep cannot handle. Please upgrade Bicep.");
        }

        return metadata;
    }

    private static void WriteNewFileEntry(ZipArchive archive, string path, string contents)
    {
        var metadataEntry = archive.CreateEntry(path);
        using var entryStream = metadataEntry.Open();
        using var writer = new StreamWriter(entryStream);
        writer.Write(contents);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && zipArchive is not null)
        {
            var archive = this.zipArchive;
            this.zipArchive = null;
            archive.Dispose();
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
