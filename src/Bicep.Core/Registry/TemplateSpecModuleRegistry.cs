// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Tracing;

namespace Bicep.Core.Registry
{
    public class TemplateSpecModuleRegistry : ModuleRegistry<TemplateSpecModuleReference>
    {
        private readonly IFileResolver fileResolver;
        private readonly ITemplateSpecRepositoryFactory repositoryFactory;
        private readonly IFeatureProvider featureProvider;

        public TemplateSpecModuleRegistry(IFileResolver fileResolver, ITemplateSpecRepositoryFactory repositoryFactory, IFeatureProvider featureProvider)
        {
            this.fileResolver = fileResolver;
            this.repositoryFactory = repositoryFactory;
            this.featureProvider = featureProvider;
        }

        public override string Scheme => ModuleReferenceSchemes.TemplateSpecs;

        public override RegistryCapabilities Capabilities => RegistryCapabilities.Default;

        public override ModuleReference? TryParseModuleReference(string reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder) =>
            TemplateSpecModuleReference.TryParse(reference, out failureBuilder);

        public override bool IsModuleRestoreRequired(TemplateSpecModuleReference reference) =>
            !this.fileResolver.FileExists(this.GetModuleEntryPointUri(reference));

        public override Task PublishModule(TemplateSpecModuleReference reference, Stream compiled) => throw new NotSupportedException("Template Spec modules cannot be published.");

        public override Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, TemplateSpecModuleReference reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            failureBuilder = null;
            return this.GetModuleEntryPointUri(reference);
        }

        public override async Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<TemplateSpecModuleReference> references)
        {
            var statuses = new Dictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference}");
                try
                {
                    var repository = this.repositoryFactory.CreateRepository(reference.EndpointUri, reference.SubscriptionId);
                    var templateSpecEntity = await repository.FindTemplateSpecByIdAsync(reference.TemplateSpecResourceId);

                    await this.SaveModuleToDisk(reference, templateSpecEntity);
                }
                catch (TemplateSpecException templateSpecException)
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

        private string GetModuleDirectoryPath(TemplateSpecModuleReference reference) => Path.Combine(
            this.featureProvider.CacheRootDirectory,
            this.Scheme,
            reference.SubscriptionId.ToLowerInvariant(),
            reference.ResourceGroupName.ToLowerInvariant(),
            reference.TemplateSpecName.ToLowerInvariant(),
            reference.Version.ToLowerInvariant());

        private string GetModuleEntryPointPath(TemplateSpecModuleReference reference) => Path.Combine(this.GetModuleDirectoryPath(reference), "main.json");

        private Uri GetModuleEntryPointUri(TemplateSpecModuleReference reference) => new(this.GetModuleEntryPointPath(reference), UriKind.Absolute);

        private async Task SaveModuleToDisk(TemplateSpecModuleReference reference, TemplateSpecEntity templateSpecEntity)
        {
            var moduleDirectoryPath = this.GetModuleDirectoryPath(reference);

            try
            {
                Directory.CreateDirectory(moduleDirectoryPath);
            }
            catch (Exception exception)
            {
                throw new TemplateSpecException($"Unable to create the local module directory \"{moduleDirectoryPath}\".", exception);
            }

            await File.WriteAllTextAsync(this.GetModuleEntryPointPath(reference), templateSpecEntity.ToUtf8Json());
        }
    }
}
