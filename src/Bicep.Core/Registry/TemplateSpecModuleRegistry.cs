// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Tracing;

namespace Bicep.Core.Registry
{
    public readonly record struct TemplateSpecEntity(string Content);

    public class TemplateSpecModuleRegistry : ExternalModuleRegistry<TemplateSpecModuleReference, TemplateSpecEntity>
    {
        private readonly ITemplateSpecRepositoryFactory repositoryFactory;

        private readonly IFeatureProvider featureProvider;

        public TemplateSpecModuleRegistry(IFileResolver fileResolver, ITemplateSpecRepositoryFactory repositoryFactory, IFeatureProvider featureProvider)
            : base(fileResolver)
        {
            this.repositoryFactory = repositoryFactory;
            this.featureProvider = featureProvider;
        }

        public override string Scheme => ModuleReferenceSchemes.TemplateSpecs;

        public override RegistryCapabilities GetCapabilities(TemplateSpecModuleReference reference) => RegistryCapabilities.Default;

        public override ModuleReference? TryParseModuleReference(string? aliasName, string reference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder) =>
            TemplateSpecModuleReference.TryParse(aliasName, reference, configuration, out failureBuilder);

        public override bool IsModuleRestoreRequired(TemplateSpecModuleReference reference) =>
            !this.FileResolver.FileExists(this.GetModuleEntryPointUri(reference));

        public override Task PublishModule(RootConfiguration configuration, TemplateSpecModuleReference reference, Stream compiled) => throw new NotSupportedException("Template Spec modules cannot be published.");

        public override Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, TemplateSpecModuleReference reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            failureBuilder = null;
            return this.GetModuleEntryPointUri(reference);
        }

        public override async Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(RootConfiguration configuration, IEnumerable<TemplateSpecModuleReference> references)
        {
            var statuses = new Dictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference}");
                try
                {
                    var repository = this.repositoryFactory.CreateRepository(configuration, reference.SubscriptionId);
                    var templateSpecEntity = await repository.FindTemplateSpecByIdAsync(reference.TemplateSpecResourceId);

                    await this.TryWriteModuleContentAsync(reference, templateSpecEntity);
                }
                catch (ExternalModuleException templateSpecException)
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

        protected override void WriteModuleContent(TemplateSpecModuleReference reference, TemplateSpecEntity entity) =>
            File.WriteAllText(this.GetModuleEntryPointPath(reference), entity.Content);

        protected override void WriteTypesContent(TemplateSpecModuleReference reference, TemplateSpecEntity entity)
            => throw new NotImplementedException();

        protected override string GetModuleDirectoryPath(TemplateSpecModuleReference reference) => Path.Combine(
            this.featureProvider.CacheRootDirectory,
            this.Scheme,
            reference.SubscriptionId.ToLowerInvariant(),
            reference.ResourceGroupName.ToLowerInvariant(),
            reference.TemplateSpecName.ToLowerInvariant(),
            reference.Version.ToLowerInvariant());

        protected override Uri GetModuleLockFileUri(TemplateSpecModuleReference reference) => new(Path.Combine(this.GetModuleDirectoryPath(reference), "lock"), UriKind.Absolute);

        private string GetModuleEntryPointPath(TemplateSpecModuleReference reference) => Path.Combine(this.GetModuleDirectoryPath(reference), "main.json");

        private Uri GetModuleEntryPointUri(TemplateSpecModuleReference reference) => new(this.GetModuleEntryPointPath(reference), UriKind.Absolute);

        public override async Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateModulesCache(RootConfiguration configuration, IEnumerable<TemplateSpecModuleReference> references)
        {
            return await base.InvalidateModulesCacheInternal(configuration, references);
        }
    }
}
