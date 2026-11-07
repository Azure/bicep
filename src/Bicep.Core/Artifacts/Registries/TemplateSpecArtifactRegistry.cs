// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Cache;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.RegistryClients.TemplateSpecs;
using Bicep.Core.Tracing;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Registries
{
    public class TemplateSpecArtifactRegistry : RemoteArtifactRegistry<TemplateSpecArtifactReference>
    {
        private readonly ITemplateSpecRepositoryFactory repositoryFactory;

        public TemplateSpecArtifactRegistry(ITemplateSpecRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }

        public override string Scheme => ArtifactSchemes.TemplateSpecs;

        protected override ArtifactCacheEntry GetCacheEntry(TemplateSpecArtifactReference reference) => reference.CacheEntry;

        protected override async Task<IDictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<TemplateSpecArtifactReference> references)
        {
            var statuses = new Dictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference} to {reference.CacheEntry.CacheDirectory.Uri.GetFilePath()}");
                try
                {
                    var repository = this.repositoryFactory.CreateRepository(reference.Context.Cloud, reference.SubscriptionId);
                    var templateSpecEntity = await repository.FindTemplateSpecByIdAsync(reference.TemplateSpecResourceId);

                    await reference.CacheEntry.WriteAsync(templateSpecEntity.Content);
                }
                catch (ArtifactException templateSpecException)
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
    }
}
