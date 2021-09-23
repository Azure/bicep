// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        // if we're unable to acquire a lock on the module directory in the cache, we will retry until this timeout is reached
        private static readonly TimeSpan ModuleDirectoryContentionTimeout = TimeSpan.FromSeconds(5);

        // interval at which we will retry acquiring the lock on the module directory in the cache
        private static readonly TimeSpan ModuleDirectoryContentionRetryInterval = TimeSpan.FromMilliseconds(300);

        private readonly IFileResolver fileResolver;
        private readonly AzureContainerRegistryManager client;
        private readonly string cachePath;

        public OciModuleRegistry(IFileResolver fileResolver, IContainerRegistryClientFactory clientFactory, IFeatureProvider features)
        {
            this.fileResolver = fileResolver;
            this.cachePath = Path.Combine(features.CacheRootDirectory, ModuleReferenceSchemes.Oci);
            this.client = new AzureContainerRegistryManager(new DefaultAzureCredential(), clientFactory);
        }

        public override string Scheme => ModuleReferenceSchemes.Oci;

        public override RegistryCapabilities Capabilities => RegistryCapabilities.Publish;

        public override ModuleReference? TryParseModuleReference(string reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder) => OciArtifactModuleReference.TryParse(reference, out failureBuilder);

        public override bool IsModuleRestoreRequired(OciArtifactModuleReference reference)
        {
            /*
             * this should be kept in sync with the WriteModuleContent() implementation
             * 
             * when we write content to the module cache, we attempt to get a lock so that no other writes happen in the directory
             * the code here appears to implement a lock-free read by checking existence of several files that are expected in a fully restored module
             * 
             * this relies on the assumption that modules are never updated in-place in the cache
             * when we need to invalidate the cache, the module directory (or even a single file) should be deleted from the cache
             */
            return
                !this.fileResolver.FileExists(this.GetModuleFileUri(reference, ModuleFileType.ModuleMain)) ||
                !this.fileResolver.FileExists(this.GetModuleFileUri(reference, ModuleFileType.Manifest)) ||
                !this.fileResolver.FileExists(this.GetModuleFileUri(reference, ModuleFileType.Metadata));
        }

        public override Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, OciArtifactModuleReference reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            failureBuilder = null;
            return this.GetModuleFileUri(reference, ModuleFileType.ModuleMain);
        }

        public override async Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<OciArtifactModuleReference> references)
        {
            var statuses = new Dictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>();

            foreach(var reference in references)
            {
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference}");
                var (result, errorMessage) = await this.TryPullArtifactAsync(reference);

                if(result is null)
                {
                    if (errorMessage is not null)
                    {
                        statuses.Add(reference, x => x.ModuleRestoreFailedWithMessage(reference.FullyQualifiedReference, errorMessage));
                        timer.OnFail(errorMessage);
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

        private async Task<(OciArtifactResult?, string? errorMessage)> TryPullArtifactAsync(OciArtifactModuleReference reference)
        {
            try
            {
                var result = await this.client.PullArtifactAsync(reference);

                await this.TryWriteModuleContentAsync(reference, result);

                return (result, null);
            }
            catch (OciModuleRegistryException exception)
            {
                // we can trust the message in this exception
                return (null, exception.Message);
            }
            catch (Exception exception)
            {
                return (null, $"Unhandled exception: {exception}");
            }
        }

        private async Task TryWriteModuleContentAsync(OciArtifactModuleReference reference, OciArtifactResult result)
        {
            // this has to be after downloading the manifest so we don't create directories for non-existent modules
            string modulePath = GetModuleDirectoryPath(reference);

            // creating the directory doesn't require locking
            CreateModuleDirectory(modulePath);

            /*
             * We have already downloaded the module content from the registry.
             * The following sections will attempt to synchronize the module write with other
             * instances of the language server running on the same machine.
             * 
             * We are not trying to prevent tampering with the module cache by the user.
             */

            Uri lockFileUri = GetModuleFileUri(reference, ModuleFileType.Lock);
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < ModuleDirectoryContentionTimeout)
            {                
                var @lock = this.fileResolver.TryAcquireFileLock(lockFileUri);
                using(@lock)
                {
                    // the placement of "if" inside "using" guarantees that even an exception thrown by the condition results in the lock being released
                    // (current condition can't throw, but this potentially avoids future regression)
                    if(@lock is not null)
                    {
                        // we have acquired the lock
                        if (!this.IsModuleRestoreRequired(reference))
                        {
                            // the other instance has already written out the content to disk - we can discard the content we downloaded
                            return;
                        }

                        // write the contents to disk
                        this.WriteModuleContent(reference, result);
                        return;
                    }
                }

                // we have not acquired the lock - let's give the instance that has the lock some time to finish writing the content to the directory
                // (the operation involves only writing the already downloaded content to disk, so it "should" complete fairly quickly)
                await Task.Delay(ModuleDirectoryContentionRetryInterval);
            }

            // we have exceeded the timeout
            throw new OciModuleRegistryException($"Exceeded the timeout of \"{ModuleDirectoryContentionTimeout}\" to acquire the lock on file \"{lockFileUri}\".");
        }

        private void WriteModuleContent(OciArtifactModuleReference reference, OciArtifactResult result)
        {
            /*
             * this should be kept in sync with the IsModuleRestoreRequired() implementation
             */

            // write main.bicep
            this.fileResolver.Write(this.GetModuleFileUri(reference, ModuleFileType.ModuleMain), result.ModuleStream);

            // write manifest
            // it's important to write the original stream here rather than serialize the manifest object
            // this way we guarantee the manifest hash will match
            this.fileResolver.Write(this.GetModuleFileUri(reference, ModuleFileType.Manifest), result.ManifestStream);

            // write metadata
            var metadata = new ModuleMetadata(result.ManifestDigest);
            using var metadataStream = new MemoryStream();
            OciSerialization.Serialize(metadataStream, metadata);
            metadataStream.Position = 0;
            this.fileResolver.Write(this.GetModuleFileUri(reference, ModuleFileType.Metadata), metadataStream);
        }

        private Uri GetModuleFileUri(OciArtifactModuleReference reference, ModuleFileType fileType)
        {
            string localFilePath = this.GetModuleFilePath(reference, fileType);
            if (Uri.TryCreate(localFilePath, UriKind.Absolute, out var uri))
            {
                return uri;
            }

            throw new NotImplementedException($"Local module file path is malformed: \"{localFilePath}\"");
        }

        private static void CreateModuleDirectory(string modulePath)
        {
            try
            {
                // ensure that the directory exists
                Directory.CreateDirectory(modulePath);
            }
            catch (Exception exception)
            {
                throw new OciModuleRegistryException($"Unable to create the local module directory \"{modulePath}\". {exception.Message}", exception);
            }
        }

        private string GetModuleFilePath(OciArtifactModuleReference reference, ModuleFileType fileType)
        {
            var fileName = fileType switch
            {
                ModuleFileType.ModuleMain => "main.json",
                ModuleFileType.Lock => "lock",
                ModuleFileType.Manifest => "manifest",
                ModuleFileType.Metadata => "metadata",
                _ => throw new NotImplementedException($"Unexpected module file type '{fileType}'.")
            };
               
            return Path.Combine(this.GetModuleDirectoryPath(reference), fileName);
        }

        private string GetModuleDirectoryPath(OciArtifactModuleReference reference)
        {
            // cachePath is already set to %userprofile%\.bicep\br or ~/.bicep/br by default depending on OS
            // we need to split each component of the reference into a sub directory to fit within the max file name length limit on linux and mac

            // TODO: Need to deal with IDNs (internationalized domain names)
            // domain names can only contain alphanumeric chars, _, -, and numbers (example.azurecr.io or example.azurecr.io:443)
            // IPV4 addresses only contain dots and numbers (127.0.0.1 or 127.0.0.1:5000)
            // IPV6 addresses are hex digits with colons (2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF or [2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF]:5000)
            // the only problematic character is the colon, which we will replace with $ which is not allowed in any of the possible registry values
            // we will also normalize casing for registries since they are case-insensitive
            var registry = reference.Registry.Replace(':', '$').ToLowerInvariant();

            // modules can have mixed hierarchy depths so we will flatten a repository into a single directory name
            // however to do this we must get rid of slashes (not a valid file system char on windows and a directory separator on linux/mac)
            // the replacement char must one that is not valid in a repository string but is valid in common type systems
            var repository = reference.Repository.Replace('/', '$');

            // tags are case-sensitive with length up to 128
            var tag = TagEncoder.Encode(reference.Tag);

            //var packageDir = WebUtility.UrlEncode(reference.UnqualifiedReference);
            return Path.Combine(this.cachePath, registry, repository, tag);
        }

        private enum ModuleFileType
        {
            ModuleMain,
            Manifest,
            Lock,
            Metadata
        };
    }
}
