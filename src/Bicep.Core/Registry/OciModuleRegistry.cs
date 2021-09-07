// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Tracing;

namespace Bicep.Core.Registry
{
    public class OciModuleRegistry : ModuleRegistry<OciArtifactModuleReference>
    {
        private readonly IFileResolver fileResolver;

        private readonly AzureContainerRegistryManager client;

        public OciModuleRegistry(IFileResolver fileResolver, IContainerRegistryClientFactory clientFactory, IFeatureProvider features)
        {
            this.fileResolver = fileResolver;
            this.client = new AzureContainerRegistryManager(features.CacheRootDirectory, new DefaultAzureCredential(), clientFactory);
        }

        public override string Scheme => ModuleReferenceSchemes.Oci;

        public override RegistryCapabilities Capabilities => RegistryCapabilities.Publish;

        public override ModuleReference? TryParseModuleReference(string reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder) => OciArtifactModuleReference.TryParse(reference, out failureBuilder);

        public override bool IsModuleRestoreRequired(OciArtifactModuleReference reference)
        {
            // TODO: This may need to be updated to account for concurrent processes updating the local cache

            // if module is missing, it requires init
            return !this.fileResolver.FileExists(this.GetEntryPointUri(reference));
        }

        public override Uri? TryGetLocalModuleEntryPointUri(Uri parentModuleUri, OciArtifactModuleReference reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            failureBuilder = null;
            return this.GetEntryPointUri(reference);
        }

        public override async Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<OciArtifactModuleReference> references)
        {
            var statuses = new Dictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>();

            foreach(var reference in references)
            {
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference}");
                var result = await this.client.PullArtifactsync(reference);

                if (!result.Success)
                {
                    if (result.ErrorMessage is not null)
                    {
                        statuses.Add(reference, x => x.ModuleRestoreFailedWithMessage(reference.FullyQualifiedReference, result.ErrorMessage));
                        timer.OnFail(result.ErrorMessage);
                    }
                    else
                    {
                        statuses.Add(reference, x => x.ModuleRestoreFailed(reference.FullyQualifiedReference));
                        timer.OnFail();
                    }
                }
            }
            
            return statuses;
        }

        public override async Task PublishModule(OciArtifactModuleReference moduleReference, Stream compiled)
        {
            var config = new StreamDescriptor(Stream.Null, BicepMediaTypes.BicepModuleConfigV1);
            var layer = new StreamDescriptor(compiled, BicepMediaTypes.BicepModuleLayerV1Json);

            await this.client.PushArtifactAsync(moduleReference, config, layer);
        }

        
        private Uri GetEntryPointUri(OciArtifactModuleReference reference)
        {
            string localArtifactPath = this.client.GetLocalPackageEntryPointPath(reference);
            if (Uri.TryCreate(localArtifactPath, UriKind.Absolute, out var uri))
            {
                return uri;
            }

            throw new NotImplementedException($"Local OCI artifact path is malformed: \"{localArtifactPath}\"");
        }
    }
}
