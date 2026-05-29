// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceGraph.Artifacts;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry
{
    public class ExecExtensionRegistry : ExternalArtifactRegistry<ExecExtensionReference, BinaryExtensionRestoreEntity>
    {
        private readonly IFileExplorer fileExplorer;
        private readonly IBinaryExtensionTypesFetcher? typesFetcher;

        public ExecExtensionRegistry(IFileExplorer fileExplorer, IBinaryExtensionTypesFetcher? typesFetcher)
        {
            this.fileExplorer = fileExplorer;
            this.typesFetcher = typesFetcher;
        }

        public override string Scheme => ArtifactReferenceSchemes.Exec;

        public override RegistryCapabilities GetCapabilities(ArtifactType artifactType, ExecExtensionReference reference)
            => RegistryCapabilities.Default;

        public override ResultWithDiagnosticBuilder<ArtifactReference> TryParseArtifactReference(
            BicepSourceFile referencingFile,
            ArtifactType artifactType,
            string? aliasName,
            string reference)
        {
            if (artifactType != ArtifactType.Extension)
            {
                return new(x => x.UnsupportedArtifactType(artifactType));
            }

            if (!ExecExtensionReference.TryParse(referencingFile, reference, fileExplorer).IsSuccess(out var execRef, out var failureBuilder))
            {
                return new(failureBuilder);
            }

            return new(execRef);
        }

        public override bool IsArtifactRestoreRequired(ExecExtensionReference reference) =>
            !GetTypesTgzFile(reference).Exists();

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(
            IEnumerable<ExecExtensionReference> references)
        {
            var statuses = new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>();

            foreach (var reference in references)
            {
                if (typesFetcher is null)
                {
                    statuses.Add(reference, x => x.ArtifactRestoreFailedWithMessage(
                        reference.FullyQualifiedReference,
                        "Restoring exec: extensions requires the Bicep local deploy runtime. " +
                        "Ensure the application is configured with AddBicepLocalDeploy()."));
                    continue;
                }

                try
                {
                    var result = await typesFetcher.FetchTypesTgzAndResolvePath(reference.RawValue, CancellationToken.None);
                    await WriteArtifactContentToCacheAsync(reference, result);
                }
                catch (Exception ex)
                {
                    statuses.Add(reference, x => x.ArtifactRestoreFailedWithMessage(
                        reference.FullyQualifiedReference,
                        ex.Message));
                }
            }

            return statuses;
        }

        public override Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(
            IEnumerable<ExecExtensionReference> references) =>
            base.InvalidateArtifactsCacheInternal(references);

        public override Task PublishModule(ExecExtensionReference reference, BinaryData compiled, BinaryData? bicepSources, string? documentationUri, string? description) =>
            throw new NotSupportedException("exec: extensions cannot be published.");

        public override Task PublishExtension(ExecExtensionReference reference, ExtensionPackage package) =>
            throw new NotSupportedException("exec: extensions cannot be published.");

        public override Task<bool> CheckArtifactExists(ArtifactType artifactType, ExecExtensionReference reference) =>
            Task.FromResult(false);

        public override string? TryGetDocumentationUri(ExecExtensionReference reference) => null;

        public override Task<string?> TryGetModuleDescription(ModuleSymbol module, ExecExtensionReference reference) =>
            Task.FromResult<string?>(null);

        protected override void WriteArtifactContentToCache(ExecExtensionReference reference, BinaryExtensionRestoreEntity entity)
        {
            GetTypesTgzFile(reference).Write(entity.TypesTgz);
            GetBinaryPathFile(reference).WriteAllText(entity.ResolvedBinaryPath);
        }

        protected override IDirectoryHandle GetArtifactDirectory(ExecExtensionReference reference) =>
            ExecExtensionArtifact.ResolveCacheDirectory(
                reference.RawValue,
                reference.ReferencingFile.Features.CacheRootDirectory);

        protected override IFileHandle GetArtifactLockFile(ExecExtensionReference reference) =>
            GetArtifactDirectory(reference).GetFile("lock");

        private IFileHandle GetTypesTgzFile(ExecExtensionReference reference) =>
            GetArtifactDirectory(reference).GetFile("types.tgz");

        private IFileHandle GetBinaryPathFile(ExecExtensionReference reference) =>
            GetArtifactDirectory(reference).GetFile("binary-path.txt");
    }
}

