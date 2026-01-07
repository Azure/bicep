// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;

namespace Bicep.Core.SourceGraph
{
    public abstract class BicepSourceFile : ISourceFile
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IAuxiliaryFileCache auxiliaryFileCache;
        private readonly ConcurrentBag<IOUri> referencedAuxiliaryFileUris;

        protected BicepSourceFile(
            Uri fileUri,
            IFileHandle fileHandle,
            ImmutableArray<int> lineStarts,
            ProgramSyntax programSyntax,
            IConfigurationManager configurationManager,
            IFeatureProviderFactory featureProviderFactory,
            IAuxiliaryFileCache auxiliaryFileCache,
            IDiagnosticLookup lexingErrorLookup,
            IDiagnosticLookup parsingErrorLookup)
        {
            this.Uri = fileUri;
            this.FileHandle = fileHandle;
            this.LineStarts = lineStarts;
            this.ProgramSyntax = programSyntax;
            this.Hierarchy = SyntaxHierarchy.Build(ProgramSyntax);
            this.configurationManager = configurationManager;
            this.featureProviderFactory = featureProviderFactory;
            this.auxiliaryFileCache = auxiliaryFileCache;
            this.referencedAuxiliaryFileUris = [];
            this.LexingErrorLookup = lexingErrorLookup;
            this.ParsingErrorLookup = parsingErrorLookup;
            this.DisabledDiagnosticsCache = new DisabledDiagnosticsCache(ProgramSyntax, lineStarts);
        }

        protected BicepSourceFile(BicepSourceFile original)
        {
            this.Uri = original.Uri;
            this.FileHandle = original.FileHandle;
            this.LineStarts = original.LineStarts;
            this.ProgramSyntax = original.ProgramSyntax;
            this.Hierarchy = original.Hierarchy;
            this.configurationManager = original.configurationManager;
            this.featureProviderFactory = original.featureProviderFactory;
            this.auxiliaryFileCache = original.auxiliaryFileCache;
            this.referencedAuxiliaryFileUris = [.. original.referencedAuxiliaryFileUris];
            this.LexingErrorLookup = original.LexingErrorLookup;
            this.ParsingErrorLookup = original.ParsingErrorLookup;
            this.DisabledDiagnosticsCache = original.DisabledDiagnosticsCache;
        }

        public Uri Uri { get; }

        public IFileHandle FileHandle { get; }

        public ImmutableArray<int> LineStarts { get; }

        public ProgramSyntax ProgramSyntax { get; }

        public string Text => ProgramSyntax.ToString();

        public abstract BicepSourceFileKind FileKind { get; }

        public ISyntaxHierarchy Hierarchy { get; }

        public RootConfiguration Configuration => this.configurationManager.GetConfiguration(this.FileHandle.Uri);

        public IFeatureProvider Features => this.featureProviderFactory.GetFeatureProvider(this.FileHandle.Uri);

        public IDiagnosticLookup LexingErrorLookup { get; }

        public IDiagnosticLookup ParsingErrorLookup { get; }

        public DisabledDiagnosticsCache DisabledDiagnosticsCache { get; }

        public abstract BicepSourceFile ShallowClone();

        public ResultWithDiagnosticBuilder<AuxiliaryFile> TryLoadAuxiliaryFile(RelativePath relativePath)
        {
            if (this.FileHandle is DummyFileHandle)
            {
                // This is only invoked when building SnippetCache. The error is swallowed.
                return new(x => x.ErrorOccurredReadingFile("Cannot load auxiliary file from dummy file handle"));
            }

            return this.GetDirectoryHandle()
                .TryGetRelativeFile(relativePath)
                .Transform(fileHandle =>
                {
                    this.referencedAuxiliaryFileUris.Add(fileHandle.Uri);

                    return this.auxiliaryFileCache.GetOrAdd(fileHandle.Uri, () => fileHandle
                        .TryReadBinaryData()
                        .Transform(data => new AuxiliaryFile(fileHandle.Uri, data)));
                });
        }

        public ResultWithDiagnosticBuilder<IEnumerable<IOUri>> TryListFilesInDirectory(RelativePath relativePath, string searchPattern = "")
        {
            if (this.FileHandle is DummyFileHandle)
            {
                // This is only invoked when building SnippetCache. The error is swallowed.
                return new(x => x.ErrorOccurredReadingFile("Cannot load auxiliary file from dummy file handle"));
            }

            var directoryHandle = this.GetDirectoryHandle().GetDirectory(relativePath);
            if (!directoryHandle.Exists())
            {
                if (this.GetDirectoryHandle().GetFile(relativePath).Exists())
                {
                    return new(x => x.FoundFileInsteadOfDirectory(relativePath));
                }

                return new(x => x.DirectoryDoesNotExist(relativePath));
            }

            try
            {
                var handles = directoryHandle.EnumerateFiles(searchPattern);
                return new(handles.Select(x => x.Uri));
            }
            catch (Exception ex)
            {
                return new(x => x.ErrorOccuredBrowsingDirectory(ex.Message));
            }
        }

        public FrozenSet<IOUri> GetReferencedAuxiliaryFileUris() => this.referencedAuxiliaryFileUris.ToFrozenSet();

        public bool IsReferencingAuxiliaryFile(IOUri uri) => this.referencedAuxiliaryFileUris.Contains(uri);

        protected virtual IDirectoryHandle GetDirectoryHandle() => this.FileHandle.GetParent();
    }
}
