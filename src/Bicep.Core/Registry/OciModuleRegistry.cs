// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Identity;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public class OciModuleRegistry : IModuleRegistry
    {
        private readonly IFileResolver fileResolver;

        private readonly AzureContainerRegistryManager client;

        public OciModuleRegistry(IFileResolver fileResolver, IContainerRegistryClientFactory clientFactory, IFeatureProvider features)
        {
            this.fileResolver = fileResolver;
            this.client = new AzureContainerRegistryManager(features.CacheRootPath, new DefaultAzureCredential(), clientFactory);
        }

        public string Scheme => ModuleReferenceSchemes.Oci;

        public RegistryCapabilities Capabilities => RegistryCapabilities.Publish;

        public ModuleReference? TryParseModuleReference(string reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder) => OciArtifactModuleReference.TryParse(reference, out failureBuilder);

        public bool IsModuleRestoreRequired(ModuleReference reference)
        {
            // TODO: This may need to be updated to account for concurrent processes updating the local cache
            var typed = ConvertReference(reference);

            // if module is missing, it requires init
            return !this.fileResolver.FileExists(GetEntryPointUri(typed));
        }

        public Uri? TryGetLocalModuleEntryPointPath(Uri parentModuleUri, ModuleReference reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var typed = ConvertReference(reference);
            failureBuilder = null;
            return GetEntryPointUri(typed);
        }

        public async Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<ModuleReference> references)
        {
            var statuses = new Dictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>();
            foreach(var reference in references.OfType<OciArtifactModuleReference>())
            {
                using (var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference}"))
                {
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
            }
            
            return statuses;
        }

        public async Task PublishModule(ModuleReference moduleReference, Stream compiled)
        {
            var typed = ConvertReference(moduleReference);

            var config = new StreamDescriptor(Stream.Null, BicepMediaTypes.BicepModuleConfigV1);
            var layer = new StreamDescriptor(compiled, BicepMediaTypes.BicepModuleLayerV1Json);

            await this.client.PushArtifactAsync(typed, config, layer);
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

        private static OciArtifactModuleReference ConvertReference(ModuleReference reference)
        {
            if(reference is OciArtifactModuleReference typed)
            {
                return typed;
            }

            throw new ArgumentException($"Reference type '{reference.GetType().Name}' is not supported.");
        }
    }
}
