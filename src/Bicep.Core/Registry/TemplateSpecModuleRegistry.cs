// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public override bool TryParseModuleReference(string? aliasName, string reference, [NotNullWhen(true)] out ModuleReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (TemplateSpecModuleReference.TryParse(aliasName, reference, configuration, parentModuleUri, out var @ref, out failureBuilder))
            {
                moduleReference = @ref;
                return true;
            }

            moduleReference = null;
            return false;
        }

        public override bool IsModuleRestoreRequired(TemplateSpecModuleReference reference) =>
            !this.FileResolver.FileExists(this.GetModuleEntryPointUri(reference));

        public override Task PublishModule(TemplateSpecModuleReference reference, Stream compiled) => throw new NotSupportedException("Template Spec modules cannot be published.");

        public override bool TryGetLocalModuleEntryPointUri(TemplateSpecModuleReference reference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            failureBuilder = null;
            localUri = this.GetModuleEntryPointUri(reference);
            return true;
        }

        public override async Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<TemplateSpecModuleReference> references)
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

        public override async Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateModulesCache(IEnumerable<TemplateSpecModuleReference> references)
        {
            return await base.InvalidateModulesCacheInternal(configuration, references);
        }
    }
}
