// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Semantics;
using Bicep.Core.SourceCode;
using Bicep.Core.Tracing;
using Bicep.Core.Utils;

namespace Bicep.Core.Registry
{
    public readonly record struct TemplateSpecEntity(string Content);

    public class TemplateSpecModuleRegistry : ExternalArtifactRegistry<TemplateSpecModuleReference, TemplateSpecEntity>
    {
        private readonly ITemplateSpecRepositoryFactory repositoryFactory;

        private readonly IFeatureProvider featureProvider;

        private readonly RootConfiguration configuration;

        private readonly Uri parentModuleUri;

        public TemplateSpecModuleRegistry(IFileResolver fileResolver, IFileSystem fileSystem, ITemplateSpecRepositoryFactory repositoryFactory, IFeatureProvider featureProvider, RootConfiguration configuration, Uri parentModuleUri)
            : base(fileResolver, fileSystem)
        {
            this.repositoryFactory = repositoryFactory;
            this.featureProvider = featureProvider;
            this.configuration = configuration;
            this.parentModuleUri = parentModuleUri;
        }

        public override string Scheme => ArtifactReferenceSchemes.TemplateSpecs;

        public override RegistryCapabilities GetCapabilities(ArtifactType artifactType, TemplateSpecModuleReference reference) => RegistryCapabilities.Default;

        public override ResultWithDiagnostic<ArtifactReference> TryParseArtifactReference(ArtifactType artifactType, string? aliasName, string reference)
        {
            if (artifactType != ArtifactType.Module)
            {
                return new(x => x.UnsupportedArtifactType(artifactType));
            }
            if (!TemplateSpecModuleReference.TryParse(aliasName, reference, configuration, parentModuleUri).IsSuccess(out var @ref, out var failureBuilder))
            {
                return new(failureBuilder);
            }

            return new(@ref);
        }

        public override bool IsArtifactRestoreRequired(TemplateSpecModuleReference reference) =>
            !this.FileResolver.FileExists(this.GetModuleEntryPointUri(reference));

        public override Task PublishModule(TemplateSpecModuleReference reference, BinaryData compiled, BinaryData? bicepSources, string? documentationUri, string? description)
            => throw new NotSupportedException("Template Spec modules cannot be published.");

        public override Task PublishProvider(TemplateSpecModuleReference reference, ProviderPackage provider)
            => throw new NotSupportedException("Template Spec providers cannot be published.");

        public override Task<bool> CheckArtifactExists(ArtifactType artifactType, TemplateSpecModuleReference reference)
            => throw new NotSupportedException("Template Spec modules cannot be published.");

        public override ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(TemplateSpecModuleReference reference)
        {
            var localUri = this.GetModuleEntryPointUri(reference);
            return new(localUri);
        }

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<TemplateSpecModuleReference> references)
        {
            var statuses = new Dictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference} to {GetArtifactDirectoryPath(reference)}");
                try
                {
                    var repository = this.repositoryFactory.CreateRepository(configuration, reference.SubscriptionId);
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

        protected override void WriteArtifactContentToCache(TemplateSpecModuleReference reference, TemplateSpecEntity entity) =>
            FileSystem.File.WriteAllText(this.GetModuleEntryPointPath(reference), entity.Content);

        protected override string GetArtifactDirectoryPath(TemplateSpecModuleReference reference) => FileSystem.Path.Combine(
            this.featureProvider.CacheRootDirectory,
            this.Scheme,
            reference.SubscriptionId.ToLowerInvariant(),
            reference.ResourceGroupName.ToLowerInvariant(),
            reference.TemplateSpecName.ToLowerInvariant(),
            reference.Version.ToLowerInvariant());

        protected override Uri GetArtifactLockFileUri(TemplateSpecModuleReference reference) => new(FileSystem.Path.Combine(this.GetArtifactDirectoryPath(reference), "lock"), UriKind.Absolute);

        private string GetModuleEntryPointPath(TemplateSpecModuleReference reference) => FileSystem.Path.Combine(this.GetArtifactDirectoryPath(reference), "main.json");

        private Uri GetModuleEntryPointUri(TemplateSpecModuleReference reference) => new(this.GetModuleEntryPointPath(reference), UriKind.Absolute);

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<TemplateSpecModuleReference> references)
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

        public override ResultWithException<SourceArchive> TryGetSource(TemplateSpecModuleReference reference)
        {
            return new(new SourceNotAvailableException());
        }

        public override Uri? TryGetProviderBinary(TemplateSpecModuleReference reference)
            => null;
    }
}
