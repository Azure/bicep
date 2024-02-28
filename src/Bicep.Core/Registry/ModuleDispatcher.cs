// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.SourceCode;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

namespace Bicep.Core.Registry
{
    public class ModuleDispatcher : IModuleDispatcher
    {
        private static readonly TimeSpan FailureExpirationInterval = TimeSpan.FromMinutes(30);

        private readonly ConcurrentDictionary<RestoreFailureKey, RestoreFailureInfo> restoreFailures = new();

        private readonly IArtifactRegistryProvider registryProvider;

        private readonly IConfigurationManager configurationManager;

        public ModuleDispatcher(IArtifactRegistryProvider registryProvider, IConfigurationManager configurationManager)
        {
            this.registryProvider = registryProvider;
            this.configurationManager = configurationManager;
        }

        private ImmutableDictionary<string, IArtifactRegistry> Registries(Uri parentModuleUri)
            => registryProvider.Registries(parentModuleUri).ToImmutableDictionary(r => r.Scheme);

        public ImmutableArray<string> AvailableSchemes(Uri parentModuleUri)
            => Registries(parentModuleUri).Keys.OrderBy(s => s).ToImmutableArray();

        public ResultWithDiagnostic<ArtifactReference> TryGetArtifactReference(ArtifactType artifactType, string reference, Uri parentModuleUri)
        {
            var registries = Registries(parentModuleUri);
            var parts = reference.Split(':', 2, StringSplitOptions.None);
            switch (parts.Length)
            {
                case 1:
                    // local path reference
                    if (registries.TryGetValue(ModuleReferenceSchemes.Local, out var localRegistry))
                    {
                        return localRegistry.TryParseArtifactReference(artifactType, null, parts[0]);
                    }

                    return new(x => x.UnknownModuleReferenceScheme(ModuleReferenceSchemes.Local, this.AvailableSchemes(parentModuleUri)));

                case 2:
                    string scheme = parts[0];
                    string? aliasName = null;

                    if (parts[0].Contains('/'))
                    {
                        // The scheme contains an alias.
                        var schemeParts = parts[0].Split('/', 2, StringSplitOptions.None);
                        scheme = schemeParts[0];
                        aliasName = schemeParts[1];
                    }

                    if (!string.IsNullOrEmpty(scheme) && registries.TryGetValue(scheme, out var registry))
                    {
                        // the scheme is recognized
                        var rawValue = parts[1];

                        return registry.TryParseArtifactReference(artifactType, aliasName, rawValue);
                    }

                    // unknown scheme
                    return new(x => x.UnknownModuleReferenceScheme(scheme, this.AvailableSchemes(parentModuleUri)));

                default:
                    // empty string
                    return new(x => x.ModulePathHasNotBeenSpecified());
            }
        }

        public ResultWithDiagnostic<ArtifactReference> TryGetArtifactReference(IArtifactReferenceSyntax artifactDeclaration, Uri parentModuleUri)
        {
            if (!SyntaxHelper.TryGetForeignTemplatePath(artifactDeclaration).IsSuccess(out var artifactReferenceString, out var failureBuilder))
            {
                return new(failureBuilder);
            }

            return this.TryGetArtifactReference(artifactDeclaration.GetArtifactType(), artifactReferenceString, parentModuleUri);
        }

        public RegistryCapabilities GetRegistryCapabilities(ArtifactReference artifactReference)
        {
            var registry = this.GetRegistry(artifactReference);
            return registry.GetCapabilities(artifactReference);
        }

        public ArtifactRestoreStatus GetArtifactRestoreStatus(
            ArtifactReference artifactReference,
            out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var registry = this.GetRegistry(artifactReference);
            var configuration = configurationManager.GetConfiguration(artifactReference.ParentModuleUri);

            // have we already failed to restore this artifact?
            if (this.HasRestoreFailed(artifactReference, configuration, out var restoreFailureBuilder))
            {
                failureBuilder = restoreFailureBuilder;
                return ArtifactRestoreStatus.Failed;
            }

            if (registry.IsArtifactRestoreRequired(artifactReference))
            {
                // module is not present on the local file system
                failureBuilder = x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference);
                return ArtifactRestoreStatus.Unknown;
            }

            failureBuilder = null;
            return ArtifactRestoreStatus.Succeeded;
        }

        public ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(ArtifactReference artifactReference)
        {
            var configuration = configurationManager.GetConfiguration(artifactReference.ParentModuleUri);
            // has restore already failed for this artifact?
            if (this.HasRestoreFailed(artifactReference, configuration, out var restoreFailureBuilder))
            {
                return new(restoreFailureBuilder);
            }

            var registry = this.GetRegistry(artifactReference);
            return registry.TryGetLocalArtifactEntryPointUri(artifactReference);
        }

        public async Task<bool> RestoreModules(IEnumerable<ArtifactReference> references, bool forceRestore)
        {
            // WARNING: The various operations on ModuleReference objects here rely on the custom Equals() implementation and NOT on object identity

            if (!forceRestore &&
                references.All(module => this.GetArtifactRestoreStatus(module, out _) == ArtifactRestoreStatus.Succeeded))
            {
                // all the modules have already been restored - no need to do anything
                return false;
            }

            // many module declarations can point to the same module
            var uniqueReferences = references.Distinct();

            // split modules refs by registry
            var referencesByRegistry = uniqueReferences.ToLookup(@ref => Registries(@ref.ParentModuleUri)[@ref.Scheme]);

            // send each set of refs to its own registry
            foreach (var registry in referencesByRegistry.Select(byRegistry => byRegistry.Key))
            {
                // if we're asked to purge modules cache
                if (forceRestore)
                {
                    var invalidateFailures = await registry.InvalidateArtifactsCache(referencesByRegistry[registry]);

                    // update cache invalidation status for each failed module
                    foreach (var (failedReference, failureBuilder) in invalidateFailures)
                    {
                        this.SetRestoreFailure(failedReference, configurationManager.GetConfiguration(failedReference.ParentModuleUri), failureBuilder);
                    }
                }

                var restoreFailures = await registry.RestoreArtifacts(referencesByRegistry[registry]);

                // update restore status for each failed module restore
                foreach (var (failedReference, failureBuilder) in restoreFailures)
                {
                    this.SetRestoreFailure(failedReference, configurationManager.GetConfiguration(failedReference.ParentModuleUri), failureBuilder);
                }
            }

            return true;
        }

        public async Task PublishModule(ArtifactReference reference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri)
        {
            var registry = this.GetRegistry(reference);

            var description = DescriptionHelper.TryGetFromArmTemplate(compiledArmTemplate);
            await registry.PublishModule(reference, compiledArmTemplate, bicepSources, documentationUri, description);
        }

        public async Task PublishProvider(ArtifactReference reference, BinaryData typesTgz)
        {
            var registry = this.GetRegistry(reference);

            await registry.PublishProvider(reference, typesTgz);
        }

        public async Task<bool> CheckModuleExists(ArtifactReference reference)
        {
            var registry = this.GetRegistry(reference);
            return await registry.CheckArtifactExists(reference);
        }

        public async Task<bool> CheckProviderExists(ArtifactReference reference)
        {
            var registry = this.GetRegistry(reference);
            return await registry.CheckArtifactExists(reference);
        }

        public void PruneRestoreStatuses()
        {
            // concurrent dictionary enumeration does NOT provide a point-in-time snapshot of values in the dictionary
            // however it also does not take a lock on all the items
            var dateTime = DateTime.UtcNow;
            foreach (var (key, value) in this.restoreFailures)
            {
                if (IsFailureInfoExpired(value, dateTime))
                {
                    // value is expired - remove it
                    this.restoreFailures.TryRemove(key, out _);
                }
            }
        }

        private IArtifactRegistry GetRegistry(ArtifactReference reference) =>
            Registries(reference.ParentModuleUri).TryGetValue(reference.Scheme, out var registry) ? registry : throw new InvalidOperationException($"Unexpected artifactDeclaration reference scheme '{reference.Scheme}'.");

        public ResultWithException<SourceArchive> TryGetModuleSources(ArtifactReference reference)
        {
            var registry = this.GetRegistry(reference);
            return registry.TryGetSource(reference);
        }

        private bool HasRestoreFailed(ArtifactReference reference, RootConfiguration configuration, [NotNullWhen(true)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (this.restoreFailures.TryGetValue(new(configuration.Cloud, reference), out var failureInfo) && !IsFailureInfoExpired(failureInfo, DateTime.UtcNow))
            {
                // the restore operation failed on the module previously
                // and the record of the failure has not yet expired
                failureBuilder = failureInfo.FailureBuilder;
                return true;
            }

            failureBuilder = null;
            return false;
        }

        private static bool IsFailureInfoExpired(RestoreFailureInfo failureInfo, DateTime dateTime) => dateTime >= failureInfo.Expiration;

        private void SetRestoreFailure(ArtifactReference reference, RootConfiguration configuration, DiagnosticBuilder.ErrorBuilderDelegate failureBuilder)
        {
            // as the user is typing, the modules will keep getting recompiled
            // we can't keep retrying syntactically correct references to non-existent modules on every key press
            // absolute expiration here will ensure that the next retry is delayed until the specified interval passes
            // we're not doing sliding expiration because we want a retry to happen eventually
            // (we may consider adding an ability to immediately retry to the UX in the future as well)
            var expiration = DateTime.UtcNow.Add(FailureExpirationInterval);
            this.restoreFailures.TryAdd(new(configuration.Cloud, reference), new RestoreFailureInfo(reference, failureBuilder, expiration));
        }

        private class RestoreFailureKey
        {
            private readonly CloudConfiguration configuration;

            private readonly ArtifactReference reference;

            public RestoreFailureKey(CloudConfiguration configuration, ArtifactReference reference)
            {
                this.configuration = configuration;
                this.reference = reference;
            }

            public override bool Equals(object? obj) =>
                obj is RestoreFailureKey other &&
                this.configuration.Equals(other.configuration) &&
                this.reference.Equals(other.reference);

            public override int GetHashCode() => HashCode.Combine(this.configuration, this.reference);
        }

        private class RestoreFailureInfo
        {
            public RestoreFailureInfo(ArtifactReference reference, DiagnosticBuilder.ErrorBuilderDelegate failureBuilder, DateTime expiration)
            {
                this.Reference = reference;
                this.FailureBuilder = failureBuilder;
                this.Expiration = expiration;
            }

            public ArtifactReference Reference { get; }

            public DiagnosticBuilder.ErrorBuilderDelegate FailureBuilder { get; }

            public DateTime Expiration { get; }
        }
    }
}
