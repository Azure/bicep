// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public bool TryGetModuleReference(string reference, Uri parentModuleUri, [NotNullWhen(true)] out ArtifactReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var registries = Registries(parentModuleUri);
            var parts = reference.Split(':', 2, StringSplitOptions.None);
            switch (parts.Length)
            {
                case 1:
                    // local path reference
                    if (registries.TryGetValue(ModuleReferenceSchemes.Local, out var localRegistry))
                    {
                        return localRegistry.TryParseArtifactReference(null, parts[0], out moduleReference, out failureBuilder);
                    }

                    failureBuilder = x => x.UnknownModuleReferenceScheme(ModuleReferenceSchemes.Local, this.AvailableSchemes(parentModuleUri));
                    moduleReference = null;
                    return false;

                case 2:
                    string scheme = parts[0];
                    string? aliasName = null;

                    if (parts[0].Contains('/'))
                    {
                        // The sheme contains an alias.
                        var schemeParts = parts[0].Split('/', 2, StringSplitOptions.None);
                        scheme = schemeParts[0];
                        aliasName = schemeParts[1];
                    }

                    if (!string.IsNullOrEmpty(scheme) && registries.TryGetValue(scheme, out var registry))
                    {
                        // the scheme is recognized
                        var rawValue = parts[1];

                        return registry.TryParseArtifactReference(aliasName, rawValue, out moduleReference, out failureBuilder);
                    }

                    // unknown scheme
                    failureBuilder = x => x.UnknownModuleReferenceScheme(scheme, this.AvailableSchemes(parentModuleUri));
                    moduleReference = null;
                    return false;

                default:
                    // empty string
                    failureBuilder = x => x.ModulePathHasNotBeenSpecified();
                    moduleReference = null;
                    return false;
            }
        }

        public bool TryGetModuleReference(IForeignArtifactReference module, Uri parentModuleUri, [NotNullWhen(true)] out ArtifactReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (!SyntaxHelper.TryGetForeignTemplatePath(module, out var moduleReferenceString, out failureBuilder))
            {
                moduleReference = null;
                return false;
            }

            return this.TryGetModuleReference(moduleReferenceString, parentModuleUri, out moduleReference, out failureBuilder);
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

            // have we already failed to restore this module?
            if (this.HasRestoreFailed(artifactReference, configuration, out var restoreFailureBuilder))
            {
                failureBuilder = restoreFailureBuilder;
                return ArtifactRestoreStatus.Failed;
            }

            if (registry.IsArtifactRestoreRequired(artifactReference))
            {
                // module is not present on the local file system
                failureBuilder = x => x.ModuleRequiresRestore(artifactReference.FullyQualifiedReference);
                return ArtifactRestoreStatus.Unknown;
            }

            failureBuilder = null;
            return ArtifactRestoreStatus.Succeeded;
        }

        public bool TryGetLocalModuleEntryPointUri(ArtifactReference moduleReference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var configuration = configurationManager.GetConfiguration(moduleReference.ParentModuleUri);
            // has restore already failed for this module?
            if (this.HasRestoreFailed(moduleReference, configuration, out var restoreFailureBuilder))
            {
                failureBuilder = restoreFailureBuilder;
                localUri = null;
                return false;
            }

            var registry = this.GetRegistry(moduleReference);
            return registry.TryGetLocalArtifactEntryPointUri(moduleReference, out localUri, out failureBuilder);
        }

        public async Task<bool> RestoreModules(IEnumerable<ArtifactReference> moduleReferences, bool forceModulesRestore = false)
        {
            // WARNING: The various operations on ModuleReference objects here rely on the custom Equals() implementation and NOT on object identity

            if (!forceModulesRestore && 
                moduleReferences.All(module => this.GetArtifactRestoreStatus(module, out _) == ArtifactRestoreStatus.Succeeded))
            {
                // all the modules have already been restored - no need to do anything
                return false;
            }

            // many module declarations can point to the same module
            var uniqueReferences = moduleReferences.Distinct();

            // split modules refs by registry
            var referencesByRegistry = uniqueReferences.ToLookup(@ref => Registries(@ref.ParentModuleUri)[@ref.Scheme]);

            // send each set of refs to its own registry
            foreach (var registry in referencesByRegistry.Select(byRegistry => byRegistry.Key))
            {
                // if we're asked to purge modules cache
                if (forceModulesRestore)
                {
                    var forceModulesRestoreStatuses = await registry.InvalidateArtifactsCache(referencesByRegistry[registry]);

                    // update cache invalidation status for each failed module
                    foreach (var (failedReference, failureBuilder) in forceModulesRestoreStatuses)
                    {
                        this.SetRestoreFailure(failedReference, configurationManager.GetConfiguration(failedReference.ParentModuleUri), failureBuilder);
                    }
                }

                var restoreStatuses = await registry.RestoreArtifacts(referencesByRegistry[registry]);

                // update restore status for each failed module restore
                foreach (var (failedReference, failureBuilder) in restoreStatuses)
                {
                    this.SetRestoreFailure(failedReference, configurationManager.GetConfiguration(failedReference.ParentModuleUri), failureBuilder);
                }
            }

            return true;
        }

        public async Task PublishModule(ArtifactReference moduleReference, Stream compiled, string? documentationUri)
        {
            var registry = this.GetRegistry(moduleReference);

            var description = DescriptionHelper.TryGetFromArmTemplate(compiled);
            await registry.PublishArtifact(moduleReference, compiled, documentationUri, description);
        }

        public async Task<bool> CheckModuleExists(ArtifactReference moduleReference)
        {
            var registry = this.GetRegistry(moduleReference);
            return await registry.CheckArtifactExists(moduleReference);
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

        private IArtifactRegistry GetRegistry(ArtifactReference moduleReference) =>
            Registries(moduleReference.ParentModuleUri).TryGetValue(moduleReference.Scheme, out var registry) ? registry : throw new InvalidOperationException($"Unexpected module reference scheme '{moduleReference.Scheme}'.");

        private bool HasRestoreFailed(ArtifactReference moduleReference, RootConfiguration configuration, [NotNullWhen(true)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (this.restoreFailures.TryGetValue(new(configuration.Cloud, moduleReference), out var failureInfo) && !IsFailureInfoExpired(failureInfo, DateTime.UtcNow))
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

        private void SetRestoreFailure(ArtifactReference moduleReference, RootConfiguration configuration, DiagnosticBuilder.ErrorBuilderDelegate failureBuilder)
        {
            // as the user is typing, the modules will keep getting recompiled
            // we can't keep retrying syntactically correct references to non-existent modules on every key press
            // absolute expiration here will ensure that the next retry is delayed until the specified interval passes
            // we're not not doing sliding expiration because we want a retry to happen eventually
            // (we may consider adding an ability to immediately retry to the UX in the future as well)
            var expiration = DateTime.UtcNow.Add(FailureExpirationInterval);
            this.restoreFailures.TryAdd(new(configuration.Cloud, moduleReference), new RestoreFailureInfo(moduleReference, failureBuilder, expiration));
        }

        private class RestoreFailureKey
        {
            private readonly CloudConfiguration configuration;

            private readonly ArtifactReference moduleReference;

            public RestoreFailureKey(CloudConfiguration configuration, ArtifactReference moduleReference)
            {
                this.configuration = configuration;
                this.moduleReference = moduleReference;
            }

            public override bool Equals(object? obj) =>
                obj is RestoreFailureKey other &&
                this.configuration.Equals(other.configuration) &&
                this.moduleReference.Equals(other.moduleReference);

            public override int GetHashCode() => HashCode.Combine(this.configuration, this.moduleReference);
        }

        private class RestoreFailureInfo
        {
            public RestoreFailureInfo(ArtifactReference moduleReference, DiagnosticBuilder.ErrorBuilderDelegate failureBuilder, DateTime expiration)
            {
                this.ModuleReference = moduleReference;
                this.FailureBuilder = failureBuilder;
                this.Expiration = expiration;
            }

            public ArtifactReference ModuleReference { get; }

            public DiagnosticBuilder.ErrorBuilderDelegate FailureBuilder { get; }

            public DateTime Expiration { get; }
        }
    }
}
