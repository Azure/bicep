// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Tracing;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry
{
    public readonly record struct TemplateSpecEntity(string Content);

    public class TemplateSpecModuleRegistry : ExternalArtifactRegistry<TemplateSpecModuleReference, TemplateSpecEntity>
    {
        private readonly ITemplateSpecRepositoryFactory repositoryFactory;

        public TemplateSpecModuleRegistry(ITemplateSpecRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }

        public override string Scheme => ArtifactReferenceSchemes.TemplateSpecs;

        public override RegistryCapabilities GetCapabilities(ArtifactType artifactType, TemplateSpecModuleReference reference) => RegistryCapabilities.Default;

        public override ResultWithDiagnosticBuilder<ArtifactReference> TryParseArtifactReference(BicepSourceFile referencingFile, ArtifactType artifactType, string? aliasName, string reference)
        {
            if (artifactType != ArtifactType.Module)
            {
                return new(x => x.UnsupportedArtifactType(artifactType));
            }
            if (!TemplateSpecModuleReference.TryParse(referencingFile, aliasName, reference).IsSuccess(out var @ref, out var failureBuilder))
            {
                return new(failureBuilder);
            }

            return new(@ref);
        }

        public override bool IsArtifactRestoreRequired(TemplateSpecModuleReference reference) => !reference.MainTemplateSpecFile.Exists();

        public override Task PublishModule(TemplateSpecModuleReference reference, BinaryData compiled, BinaryData? bicepSources, string? documentationUri, string? description)
            => throw new NotSupportedException("Template Spec modules cannot be published.");

        public override Task PublishExtension(TemplateSpecModuleReference reference, ExtensionPackage package)
            => throw new NotSupportedException("Template Spec extensions cannot be published.");

        public override Task<bool> CheckArtifactExists(ArtifactType artifactType, TemplateSpecModuleReference reference)
            => throw new NotSupportedException("Template Spec modules cannot be published.");

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<TemplateSpecModuleReference> references)
        {
            var statuses = new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference} to {GetArtifactDirectory(reference).Uri.GetFilePath()}");
                try
                {
                    var repository = this.repositoryFactory.CreateRepository(reference.ReferencingFile.Configuration, reference.SubscriptionId);
                    var templateSpecEntity = await repository.FindTemplateSpecByIdAsync(reference.TemplateSpecResourceId);

                    await this.WriteArtifactContentToCacheAsync(reference, templateSpecEntity);
                }
                catch (ExternalArtifactException templateSpecException)
                {
                    statuses.Add(reference, x => x.ArtifactRestoreFailedWithMessage(reference.FullyQualifiedReference, templateSpecException.Message));
                    timer.OnFail(templateSpecException.Message);
                }
                catch (Exception exception)
                {
                    if (exception.Message is { } message)
                    {
                        statuses.Add(reference, x => x.ArtifactRestoreFailedWithMessage(reference.FullyQualifiedReference, message));
                        timer.OnFail($"Unexpected exception {exception}: {message}");

                        return statuses;
                    }

                    statuses.Add(reference, x => x.ArtifactRestoreFailed(reference.FullyQualifiedReference));
                    timer.OnFail($"Unexpected exception {exception}.");
                }
            }

            return statuses;
        }

        protected override void WriteArtifactContentToCache(TemplateSpecModuleReference reference, TemplateSpecEntity entity) => reference.MainTemplateSpecFile.Write(entity.Content);

        protected override IDirectoryHandle GetArtifactDirectory(TemplateSpecModuleReference reference) => reference.ReferencingFile.Features.CacheRootDirectory.GetDirectory(
            $"{this.Scheme}/{reference.SubscriptionId}/{reference.ResourceGroupName}/{reference.TemplateSpecName}/{reference.Version}".ToLowerInvariant());

        protected override IFileHandle GetArtifactLockFile(TemplateSpecModuleReference reference) => this.GetArtifactDirectory(reference).GetFile("lock");

        //private IFileHandle GetModuleEntryPointFile(TemplateSpecModuleReference reference) => this.GetArtifactDirectory(reference).GetFile("main.json");

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<TemplateSpecModuleReference> references)
        {
            return await base.InvalidateArtifactsCacheInternal(references);
        }

        public override string? TryGetDocumentationUri(TemplateSpecModuleReference moduleReference) => null;

        public override Task<string?> TryGetModuleDescription(ModuleSymbol module, TemplateSpecModuleReference moduleReference)
        {
            if (module.TryGetSemanticModel().TryUnwrap() is { } model)
            {
                return Task.FromResult(DescriptionHelper.TryGetFromSemanticModel(model));
            }

            return Task.FromResult<string?>(null);
        }
    }
}
