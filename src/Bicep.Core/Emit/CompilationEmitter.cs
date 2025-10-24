// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit;

public record ParametersResult(
    bool Success,
    ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> Diagnostics,
    string? Parameters,
    string? TemplateSpecId,
    TemplateResult? Template);

public record TemplateResult(
    bool Success,
    ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> Diagnostics,
    string? Template,
    string? SourceMap);

public record TemplateArchiveResult(
    bool Success,
    ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> Diagnostics,
    string? EntryPointKey,
    ImmutableDictionary<string, (string Template, string? SourceMap)>? Templates);

public record TemplateOciArchiveResult(
    bool Success,
    ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> Diagnostics,
    string? Index,
    ImmutableSortedDictionary<string, byte[]>? Blobs);

public interface ICompilationEmitter
{
    TemplateResult Template();

    TemplateArchiveResult TemplateArchive();

    ParametersResult Parameters();
}

public class CompilationEmitter : ICompilationEmitter
{
    private readonly Compilation compilation;

    public CompilationEmitter(Compilation compilation)
    {
        this.compilation = compilation;
    }

    public ParametersResult Parameters()
    {
        var model = compilation.GetEntrypointSemanticModel();
        if (model.SourceFileKind != BicepSourceFileKind.ParamsFile)
        {
            throw new InvalidOperationException($"Entry-point {model.SourceFile.FileHandle.Uri} is not a parameters file");
        }

        var diagnostics = compilation.GetAllDiagnosticsByBicepFile();

        using var writer = new StringWriter { NewLine = "\n" };
        var result = new ParametersEmitter(model).Emit(writer);
        if (result.Status != EmitStatus.Succeeded)
        {
            return new(false, diagnostics, null, null, null);
        }

        var parametersData = writer.ToString();
        if (!model.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
        {
            throw new InvalidOperationException($"Failed to find linked bicep file for parameters file {model.SourceFile.FileHandle.Uri}");
        }

        switch (usingModel)
        {
            case SemanticModel bicepModel:
                {
                    var templateResult = Template(bicepModel);
                    return new ParametersResult(true, diagnostics, parametersData, null, templateResult);
                }
            case ArmTemplateSemanticModel armTemplateModel:
                {
                    var template = armTemplateModel.SourceFile.Text;
                    var templateResult = new TemplateResult(true, ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>>.Empty, template, null);

                    return new ParametersResult(true, diagnostics, parametersData, null, templateResult);
                }
            case TemplateSpecSemanticModel templateSpecModel:
                {
                    return new ParametersResult(true, diagnostics, parametersData, templateSpecModel.SourceFile.TemplateSpecId, null);
                }
            case EmptySemanticModel _:
                {
                    return new ParametersResult(true, diagnostics, parametersData, null, null);
                }
        }

        throw new InvalidOperationException($"Invalid semantic model of type {usingModel.GetType()}");
    }

    public TemplateResult Template() => Template(GetEntrypointTemplateModel());

    private SemanticModel GetEntrypointTemplateModel()
    {
        var model = this.compilation.GetEntrypointSemanticModel();
        if (model.SourceFileKind != SourceGraph.BicepSourceFileKind.BicepFile)
        {
            throw new InvalidOperationException($"Entry-point {model.SourceFile.FileHandle.Uri} is not a bicep file");
        }

        return model;
    }

    private TemplateResult Template(SemanticModel model, IModuleWriterFactory? moduleWriterFactory = null)
    {
        var diagnostics = compilation.GetAllDiagnosticsByBicepFile();

        using var writer = new StringWriter { NewLine = "\n" };
        var result = new TemplateEmitter(model, moduleWriterFactory).Emit(writer);
        if (result.Status != EmitStatus.Succeeded)
        {
            return new(false, diagnostics, null, null);
        }

        var template = writer.ToString();
        var sourceMap = result.SourceMap is { } ? JsonConvert.SerializeObject(result.SourceMap) : null;

        return new(true, diagnostics, template, sourceMap);
    }

    public TemplateArchiveResult TemplateArchive()
    {
        var diagnostics = compilation.GetAllDiagnosticsByBicepFile();
        var keyedSources = GetKeyedArchiveSources();

        var templates = ImmutableDictionary.CreateBuilder<string, (string, string?)>();

        foreach (var (model, pathInArchive) in keyedSources)
        {
            switch (model)
            {
                case SemanticModel bicepModel:
                    RelativeLinkModuleWriterFactory moduleWriterFactory = new(keyedSources[bicepModel], keyedSources);
                    var result = Template(bicepModel, moduleWriterFactory);
                    if (!result.Success)
                    {
                        return new(false, diagnostics, null, null);
                    }
                    templates.Add(pathInArchive.GetFilePath(), (result.Template!, result.SourceMap));
                    break;
                case ArmTemplateSemanticModel armModel:
                    templates.Add(pathInArchive.GetFilePath(), (armModel.SourceFile.Text, null));
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Expected only ARM or Bicep models but encountered one of type {model.GetType()}");
            }
        }

        return new(
            true,
            compilation.GetAllDiagnosticsByBicepFile(),
            keyedSources[compilation.GetEntrypointSemanticModel()].GetFilePath(),
            templates.ToImmutable());
    }

    private IReadOnlyDictionary<ISemanticModel, IOUri> GetKeyedArchiveSources()
    {
        Dictionary<ISemanticModel, IOUri> sources = new();
        var restoredFilesRoot = IOUri.FromFilePath($"/{EmitConstants.RestoredFilesArchivePrefix}/");
        var localFilesRoot = IOUri.FromFilePath($"/{EmitConstants.LocalFilesArchivePrefix}/");
        var cacheRoot = compilation.GetEntrypointSemanticModel().Features.CacheRootDirectory;
        HashSet<ISourceFile> localFiles = new();
        IDirectoryHandle? archiveRoot = null;

        foreach (var file in compilation.SourceFileGrouping.SourceFiles)
        {
            if (file is TemplateSpecFile)
            {
                // we link directly to template specs rather than bundling them into the archive
                continue;
            }

            if (cacheRoot.Uri.IsBaseOf(file.FileHandle.Uri))
            {
                // if a file is within the cache root, assume it is a restored artifact
                sources.Add(
                    compilation.GetSemanticModel(file),
                    restoredFilesRoot.Resolve(file.FileHandle.Uri.GetPathRelativeTo(cacheRoot.Uri)));
                continue;
            }

            localFiles.Add(file);
            archiveRoot ??= file.FileHandle.GetParent();
            while (!archiveRoot!.Uri.IsBaseOf(file.FileHandle.Uri))
            {
                archiveRoot = archiveRoot.GetParent();
            }
        }

        foreach (var localFile in localFiles)
        {
            var uri = localFile.FileHandle.Uri;

            sources.Add(
                compilation.GetSemanticModel(localFile),
                // if `localFiles` is not empty, then `archiveRoot` cannot be null
                localFilesRoot.Resolve(uri.GetPathRelativeTo(archiveRoot!.Uri)));
        }

        return sources;
    }

    private record OciBlob(string DigestType, string Digest, byte[] Content)
    {
        public OciBlob(byte[] content) : this("sha256", Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant(), content) { }

        public string OciImageLayoutPath => $"/blobs/{DigestType}/{Digest}";

        public string OciDigestString => $"{DigestType}:{Digest}";
    }

    public TemplateOciArchiveResult TemplateOciArchive()
    {
        var keyedSources = GetKeyedArchiveSources();
        SortedDictionary<string, OciBlob> blobs = new();
        OciBlob? entryPointBlob = null;
        Dictionary<OciBlob, List<string>> blobSources = new();
        Dictionary<ISemanticModel, IOUri> hashes = new();

        foreach (var file in GetTopoSortedArchiveSources(compilation.SourceFileGrouping))
        {
            if (file is TemplateSpecFile)
            {
                continue;
            }

            var content = file.Text;
            var model = compilation.GetSemanticModel(file);
            if (model is SemanticModel bicepModel)
            {
                RelativeLinkModuleWriterFactory moduleWriterFactory = new(
                    IOUri.FromFilePath(new OciBlob([]).OciImageLayoutPath),
                    hashes);
                var result = Template(bicepModel, moduleWriterFactory);
                if (!result.Success)
                {
                    return new(false, compilation.GetAllDiagnosticsByBicepFile(), null, null);
                }
                content = result.Template!;
            }

            OciBlob blob = new(Encoding.UTF8.GetBytes(content.ReplaceLineEndings("\n")));
            hashes.Add(model, IOUri.FromFilePath(blob.OciImageLayoutPath));
            string title = keyedSources[model].GetFilePath().TrimStart('/');

            if (blobs.TryGetValue(blob.OciDigestString, out var alreadyCompiled))
            {
                blobSources[alreadyCompiled].Add(title);
            }
            else
            {
                blobs.Add(blob.OciDigestString, blob);
                blobSources[blob] = new() { title };
            }

            if (file == compilation.SourceFileGrouping.EntryPoint)
            {
                entryPointBlob = blob;
            }
        }

        JArray layers = new();
        foreach (var blob in blobs.Values)
        {
            JObject annotations = new()
            {
                { OciAnnotationKeys.OciOpenContainerImageTitleAnnotation, blobSources[blob].Order().ConcatString(",") },
            };
            if (blob == entryPointBlob)
            {
                annotations[OciAnnotationKeys.DeploymentsEntryPointAnnotation] = "true";
            }

            layers.Add(new JObject
            {
                { "mediaType", BicepMediaTypes.BicepModuleLayerV1Json },
                { "digest", blob.OciDigestString },
                { "size", blob.Content.Length },
                { "annotations", annotations },
            });
        }

        OciBlob configBlob = new(Encoding.UTF8.GetBytes("{}"));
        blobs.TryAdd(configBlob.OciDigestString, configBlob);

        JObject manifest = new()
        {
            { "schemaVersion", "2.0" },
            { "mediaType", ManifestMediaType.OciImageManifest.ToString() },
            { "artifactType", BicepMediaTypes.BicepModuleArtifactType },
            {
                "config",
                new JObject
                {
                    { "mediaType", BicepMediaTypes.BicepModuleConfigV1 },
                    { "digest", configBlob.OciDigestString },
                    { "size", configBlob.Content.Length },
                    { "annotations", new JObject() },
                }
            },
            { "layers", layers },
        };

        // Use a LF line ending to ensure reproducibility across platforms
        OciBlob manifestBlob = new(Encoding.UTF8.GetBytes(manifest.ToString().ReplaceLineEndings("\n")));
        blobs.Add(manifestBlob.OciDigestString, manifestBlob);

        JObject index = new()
        {
            { "schemaVersion", "2.0" },
            { "mediaType", "application/vnd.oci.image.index.v1+json" },
            {
                "manifests",
                new JArray
                {
                    new JObject
                    {
                        { "mediaType", ManifestMediaType.OciImageManifest.ToString() },
                        { "digest", manifestBlob.OciDigestString },
                        { "size", manifestBlob.Content.Length },
                    },
                }
            },
        };

        return new(
            true,
            compilation.GetAllDiagnosticsByBicepFile(),
            index.ToString(),
            blobs.ToImmutableSortedDictionary(kvp => kvp.Value.OciImageLayoutPath, kvp => kvp.Value.Content));
    }

    private static IEnumerable<ISourceFile> GetTopoSortedArchiveSources(SourceFileGrouping files)
    {
        var dependenciesByFile = files.SourceFiles.ToDictionary(f => f, _ => new HashSet<ISourceFile>());

        foreach (var (child, parents) in files.SourceFileParentLookup)
        {
            foreach (var parent in parents)
            {
                dependenciesByFile[parent].Add(child);
            }
        }

        HashSet<ISourceFile> processed = new();
        IEnumerable<ISourceFile> YieldDependenciesAndModel(ISourceFile file)
        {
            if (!processed.Add(file))
            {
                yield break;
            }

            foreach (var dependency in dependenciesByFile[file].SelectMany(YieldDependenciesAndModel))
            {
                yield return dependency;
            }

            yield return file;
        }

        return files.SourceFiles.SelectMany(YieldDependenciesAndModel);
    }
}
