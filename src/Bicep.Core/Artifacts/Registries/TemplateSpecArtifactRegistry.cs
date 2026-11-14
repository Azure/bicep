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
        private readonly ITemplateSpecClientFactory clientFactory;

        public TemplateSpecArtifactRegistry(ITemplateSpecClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public override string Scheme => ArtifactSchemes.TemplateSpecs;

        protected override TemplateSpecArtifactCacheEntry GetCacheEntry(TemplateSpecArtifactReference reference) =>
            new(reference.SubscriptionId, reference.ResourceGroupName, reference.TemplateSpecName, reference.Version, reference.Context.CacheRootDirectory);

        protected override async Task<IDictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<TemplateSpecArtifactReference> references)
        {
            var statuses = new Dictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>();

            foreach (var reference in references)
            {
                var cacheEntry = this.GetCacheEntry(reference);
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference} to {cacheEntry.CacheDirectory.Uri.GetFilePath()}");
                try
                {
                    var client = this.clientFactory.CreateClient(reference.Context.Cloud, reference.SubscriptionId);
                    var templateSpecVersion = await client.GetTemplateSpecVersionAsync(reference.TemplateSpecResourceId);

                    await cacheEntry.WriteAsync(templateSpecVersion.Content);
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
