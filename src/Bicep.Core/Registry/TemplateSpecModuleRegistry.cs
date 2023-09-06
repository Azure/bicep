// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.Modules;
using Bicep.Core.Semantics;
using Bicep.Core.Tracing;

namespace Bicep.Core.Registry
{
    public readonly record struct TemplateSpecEntity(string Content);

    public class TemplateSpecModuleRegistry : ExternalArtifactRegistry<TemplateSpecModuleReference, TemplateSpecEntity>
    {
        private readonly ITemplateSpecRepositoryFactory repositoryFactory;

        private readonly IFeatureProvider featureProvider;

        private readonly RootConfiguration configuration;

        private readonly Uri parentModuleUri;

        public TemplateSpecModuleRegistry(IFileResolver fileResolver, ITemplateSpecRepositoryFactory repositoryFactory, IFeatureProvider featureProvider, RootConfiguration configuration, Uri parentModuleUri)
            : base(fileResolver)
        {
            this.repositoryFactory = repositoryFactory;
            this.featureProvider = featureProvider;
            this.configuration = configuration;
            this.parentModuleUri = parentModuleUri;
        }

        public override string Scheme => ModuleReferenceSchemes.TemplateSpecs;

        public override RegistryCapabilities GetCapabilities(TemplateSpecModuleReference reference) => RegistryCapabilities.Default;

        public override bool TryParseArtifactReference(string? aliasName, string reference, [NotNullWhen(true)] out ArtifactReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (TemplateSpecModuleReference.TryParse(aliasName, reference, configuration, parentModuleUri, out var @ref, out failureBuilder))
            {
                moduleReference = @ref;
                return true;
            }

            moduleReference = null;
            return false;
        }

        public override bool IsArtifactRestoreRequired(TemplateSpecModuleReference reference) =>
            !this.FileResolver.FileExists(this.GetModuleEntryPointUri(reference));

        public override Task PublishArtifact(TemplateSpecModuleReference reference, Stream compiled, string? documentationUri, string? description) => throw new NotSupportedException("Template Spec modules cannot be published.");

        public override Task<bool> CheckArtifactExists(TemplateSpecModuleReference reference) => throw new NotSupportedException("Template Spec modules cannot be published.");

        public override bool TryGetLocalArtifactEntryPointUri(TemplateSpecModuleReference reference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            failureBuilder = null;
            localUri = this.GetModuleEntryPointUri(reference);
            return true;
        }

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<TemplateSpecModuleReference> references)
        {
            var statuses = new Dictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference}");
                try
                {
                    var repository = this.repositoryFactory.CreateRepository(configuration, reference.SubscriptionId);
                    var templateSpecEntity = await repository.FindTemplateSpecByIdAsync(reference.TemplateSpecResourceId);

                    await this.TryWriteArtifactContentAsync(reference, templateSpecEntity);
                }
                catch (ExternalArtifactException templateSpecException)
                {
                    statuses.Add(reference, x => x.ModuleRestoreFailedWithMessage(reference.FullyQualifiedReference, templateSpecException.Message));
                    timer.OnFail(templateSpecException.Message);
                }
                catch (Exception exception)
                {
                    if (exception.Message is { } message)
                    {
                        statuses.Add(reference, x => x.ModuleRestoreFailedWithMessage(reference.FullyQualifiedReference, message));
                        timer.OnFail($"Unexpected exception {exception}: {message}");

                        return statuses;
                    }

                    statuses.Add(reference, x => x.ModuleRestoreFailed(reference.FullyQualifiedReference));
                    timer.OnFail($"Unexpected exception {exception}.");
                }
            }

            return statuses;
        }

        protected override void WriteArtifactContentToCache(TemplateSpecModuleReference reference, TemplateSpecEntity entity) =>
            File.WriteAllText(this.GetModuleEntryPointPath(reference), entity.Content);

        protected override string GetArtifactDirectoryPath(TemplateSpecModuleReference reference) => Path.Combine(
            this.featureProvider.CacheRootDirectory,
            this.Scheme,
            reference.SubscriptionId.ToLowerInvariant(),
            reference.ResourceGroupName.ToLowerInvariant(),
            reference.TemplateSpecName.ToLowerInvariant(),
            reference.Version.ToLowerInvariant());

        protected override Uri GetArtifactLockFileUri(TemplateSpecModuleReference reference) => new(Path.Combine(this.GetArtifactDirectoryPath(reference), "lock"), UriKind.Absolute);

        private string GetModuleEntryPointPath(TemplateSpecModuleReference reference) => Path.Combine(this.GetArtifactDirectoryPath(reference), "main.json");

        private Uri GetModuleEntryPointUri(TemplateSpecModuleReference reference) => new(this.GetModuleEntryPointPath(reference), UriKind.Absolute);

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<TemplateSpecModuleReference> references)
        {
            return await base.InvalidateArtifactsCacheInternal(references);
        }

        public override string? TryGetDocumentationUri(TemplateSpecModuleReference moduleReference) => null;

        public override Task<string?> TryGetDescription(TemplateSpecModuleReference moduleReference)
        {
            try
            {
                string entrypointPath = this.GetModuleEntryPointPath(moduleReference);
                if (File.Exists(entrypointPath))
                {
                    using var stream = File.OpenRead(entrypointPath);
                    return Task.FromResult(DescriptionHelper.TryGetFromTemplateSpec(stream));
                }
            }
            catch
            {
                // ignore
            }

            return Task.FromResult<string?>(null);
        }
    }
}
