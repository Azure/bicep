// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Security.Cryptography;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.IO.InMemory;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Features.Language.DocumentLink;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// A Bicep file from a module's published source, compiled on its own. Its module references are resolved
    /// through the archive's document links rather than the file system, so the compilation is never asked to
    /// find the files beside it.
    /// </summary>
    public sealed record ArchivedBicepFile(
        string Path,
        SemanticModel Model,
        ImmutableArray<ArchivedSourceFileLink> Links);

    /// <summary>
    /// A registry module whose published Bicep source is in the local module cache.
    /// </summary>
    public sealed class ArchivedModuleSource
    {
        private readonly Func<string, ArchivedBicepFile?> compile;

        private readonly ConcurrentDictionary<string, Lazy<ArchivedBicepFile?>> files = new(StringComparer.Ordinal);

        public ArchivedModuleSource(OciArtifactReference reference, SourceArchive archive, Func<SourceArchive, string, ArchivedBicepFile?> compile)
        {
            this.Reference = reference;
            this.Archive = archive;
            this.compile = path => compile(archive, path);
        }

        public OciArtifactReference Reference { get; }

        public SourceArchive Archive { get; }

        public ArchivedBicepFile? EntryPoint => this.GetBicepFile(this.Archive.EntrypointRelativePath);

        /// <summary>Compiles a Bicep file from the archive on first use. Null if the path is not an archived Bicep file.</summary>
        public ArchivedBicepFile? GetBicepFile(string path) =>
            this.files.GetOrAdd(path, key => new Lazy<ArchivedBicepFile?>(() => this.compile(key))).Value;

        /// <summary>The <c>bicep-extsrc:</c> document URI that shows an archived file in the editor.</summary>
        public string GetDocumentUri(string path) =>
            new ExternalSourceReference(this.Reference.AddressComponents, this.Archive).WithRequestForSourceFile(path).ToUri().ToString();
    }

    /// <summary>
    /// Whether a registry module's published source is in the local module cache. When it is not,
    /// <see cref="RequiresRestore"/> says whether restoring the module could supply it: false when the module is
    /// cached but was published without source, or its source archive cannot be read.
    /// </summary>
    public sealed record VisualModuleSourceResult(ArchivedModuleSource? Source, bool RequiresRestore)
    {
        public static readonly VisualModuleSourceResult Unavailable = new(null, false);

        public static readonly VisualModuleSourceResult NotRestored = new(null, true);
    }

    /// <summary>
    /// Supplies the published Bicep source of registry modules from the local module cache. It never restores:
    /// a module that is not cached is reported as such, and the caller decides whether to restore it.
    /// </summary>
    public interface IVisualModuleSourceProvider
    {
        VisualModuleSourceResult GetModuleSource(OciArtifactReference reference);
    }

    public sealed class VisualModuleSourceProvider : IVisualModuleSourceProvider
    {
        private readonly ISourceFileFactory sourceFileFactory;

        private readonly BicepCompiler bicepCompiler;

        /// <summary>
        /// Compiled sources by module reference. Each entry is keyed by the hash of its archive, so a forced
        /// restore that replaces the archive is picked up rather than served stale.
        /// </summary>
        private readonly ConcurrentDictionary<string, (byte[] Hash, ArchivedModuleSource Source)> cache = new(StringComparer.Ordinal);

        public VisualModuleSourceProvider(ISourceFileFactory sourceFileFactory, BicepCompiler bicepCompiler)
        {
            this.sourceFileFactory = sourceFileFactory;
            this.bicepCompiler = bicepCompiler;
        }

        public VisualModuleSourceResult GetModuleSource(OciArtifactReference reference)
        {
            var sourceFile = reference.ModuleSourceTgzFile;

            if (!sourceFile.Exists())
            {
                // A restored module without a source archive was published without its source.
                return reference.ModuleMainTemplateFile.Exists() ? VisualModuleSourceResult.Unavailable : VisualModuleSourceResult.NotRestored;
            }

            if (!sourceFile.TryReadBinaryData().IsSuccess(out var data, out _))
            {
                return VisualModuleSourceResult.Unavailable;
            }

            var key = reference.FullyQualifiedReference;
            var hash = SHA256.HashData(data.ToMemory().Span);

            if (this.cache.TryGetValue(key, out var cached) && cached.Hash.AsSpan().SequenceEqual(hash))
            {
                return new(cached.Source, RequiresRestore: false);
            }

            if (!SourceArchive.TryUnpackFromFile(sourceFile).IsSuccess(out var archive))
            {
                return VisualModuleSourceResult.Unavailable;
            }

            var source = new ArchivedModuleSource(reference, archive, this.Compile);
            this.cache[key] = (hash, source);

            return new(source, RequiresRestore: false);
        }

        private ArchivedBicepFile? Compile(SourceArchive archive, string path)
        {
            if (archive.TryFindSourceFile(path) is not { Metadata.Kind: ArchivedSourceFileKind.Bicep } file)
            {
                return null;
            }

            // The file is compiled alone, like the definition handler does for imported symbols: references to the
            // files beside it fail to resolve harmlessly, and the graph follows the archive's links instead.
            var bicepFile = this.sourceFileFactory.CreateBicepFile(DummyFileHandle.Default, file.Contents);
            var workspace = new ActiveSourceFileSet();
            workspace.UpsertSourceFile(bicepFile);
            var compilation = this.bicepCompiler.CreateCompilationWithoutRestore(bicepFile.FileHandle.Uri, workspace);

            return new ArchivedBicepFile(path, compilation.GetEntrypointSemanticModel(), archive.FindDocumentLinks(path));
        }
    }
}
