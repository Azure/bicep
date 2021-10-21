// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Syntax;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public class ModuleDispatcher : IModuleDispatcher
    {
        private static readonly TimeSpan FailureExpirationInterval = TimeSpan.FromMinutes(30);

        private readonly ImmutableDictionary<string, IModuleRegistry> registries;

        private readonly ConcurrentDictionary<RestoreFailureKey, RestoreFailureInfo> restoreFailures = new();
        
        public ModuleDispatcher(IModuleRegistryProvider registryProvider)
        {
            this.registries = registryProvider.Registries.ToImmutableDictionary(registry => registry.Scheme);
            this.AvailableSchemes = this.registries.Keys.OrderBy(s => s).ToImmutableArray();
        }

        public ImmutableArray<string> AvailableSchemes { get; }

        public ModuleReference? TryGetModuleReference(string reference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var parts = reference.Split(':', 2, StringSplitOptions.None);
            switch (parts.Length)
            {
                case 1:
                    // local path reference
                    if (registries.TryGetValue(ModuleReferenceSchemes.Local, out var localRegistry))
                    {
                        return localRegistry.TryParseModuleReference(null, parts[0], configuration, out failureBuilder);
                    }

                    failureBuilder = x => x.UnknownModuleReferenceScheme(ModuleReferenceSchemes.Local, this.AvailableSchemes);
                    return null;

                case 2:
                    string scheme = parts[0];
                    string? aliasName = null;

                    if (parts[0].Contains("/"))
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

                        return registry.TryParseModuleReference(aliasName, rawValue, configuration, out failureBuilder);
                    }

                    // unknown scheme
                    failureBuilder = x => x.UnknownModuleReferenceScheme(scheme, this.AvailableSchemes);
                    return null;

                default:
                    // empty string
                    failureBuilder = x => x.ModulePathHasNotBeenSpecified();
                    return null;
            }
        }

        public ModuleReference? TryGetModuleReference(ModuleDeclarationSyntax module, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var moduleReferenceString = SyntaxHelper.TryGetModulePath(module, out var getModulePathFailureBuilder);
            if (moduleReferenceString is null)
            {
                failureBuilder = getModulePathFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(SyntaxHelper.TryGetModulePath)} to provide failure diagnostics.");
                return null;
            }

            return this.TryGetModuleReference(moduleReferenceString, configuration, out failureBuilder);
        }

        public RegistryCapabilities GetRegistryCapabilities(ModuleReference moduleReference)
        {
            var registry = this.GetRegistry(moduleReference);
            return registry.Capabilities;
        }

        public ModuleRestoreStatus GetModuleRestoreStatus(ModuleReference moduleReference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var registry = this.GetRegistry(moduleReference);
            
            // have we already failed to restore this module?
            if (this.HasRestoreFailed(moduleReference, configuration, out var restoreFailureBuilder))
            {
                failureBuilder = restoreFailureBuilder;
                return ModuleRestoreStatus.Failed;
            }

            if (registry.IsModuleRestoreRequired(moduleReference))
            {
                // module is not present on the local file system
                failureBuilder = x => x.ModuleRequiresRestore(moduleReference.FullyQualifiedReference);
                return ModuleRestoreStatus.Unknown;
            }

            failureBuilder = null;
            return ModuleRestoreStatus.Succeeded;
        }

        public Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, ModuleReference moduleReference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            // has restore already failed for this module?
            if(this.HasRestoreFailed(moduleReference, configuration, out var restoreFailureBuilder))
            {
                failureBuilder = restoreFailureBuilder;
                return null;
            }

            var registry = this.GetRegistry(moduleReference);
            return registry.TryGetLocalModuleEntryPointUri(parentModuleUri, moduleReference, out failureBuilder);
        }

        public async Task<bool> RestoreModules(RootConfiguration configuration, IEnumerable<ModuleReference> moduleReferences)
        {
            // WARNING: The various operations on ModuleReference objects here rely on the custom Equals() implementation and NOT on object identity

            if (moduleReferences.All(module => this.GetModuleRestoreStatus(module, configuration, out _) == ModuleRestoreStatus.Succeeded))
            {
                // all the modules have already been restored - no need to do anything
                return false;
            }

            // many module declarations can point to the same module
            var uniqueReferences = moduleReferences.Distinct();

            // split module refs by scheme
            var referencesByScheme = uniqueReferences.ToLookup(@ref => @ref.Scheme);

            // send each set of refs to its own registry
            foreach (var scheme in this.registries.Keys.Where(refType => referencesByScheme.Contains(refType)))
            {
                var restoreStatuses = await this.registries[scheme].RestoreModules(configuration, referencesByScheme[scheme]);

                // update restore status for each failed module restore
                foreach(var (failedReference, failureBuilder) in restoreStatuses)
                {
                    this.SetRestoreFailure(failedReference, configuration, failureBuilder);
                }
            }

            return true;
        }

        public async Task PublishModule(RootConfiguration configuration, ModuleReference moduleReference, Stream compiled)
        {
            var registry = this.GetRegistry(moduleReference);
            await registry.PublishModule(configuration, moduleReference, compiled);
        }

        public void PruneRestoreStatuses()
        {
            // concurrent dictionary enumeration does NOT provide a point-in-time snapshot of values in the dictionary
            // however it also does not take a lock on all the items
            var dateTime = DateTime.UtcNow;
            foreach (var (key, value) in this.restoreFailures)
            {
                if(IsFailureInfoExpired(value, dateTime))
                {
                    // value is expired - remove it
                    this.restoreFailures.TryRemove(key, out _);
                }
            }
        }

        private IModuleRegistry GetRegistry(ModuleReference moduleReference) =>
            this.registries.TryGetValue(moduleReference.Scheme, out var registry) ? registry : throw new InvalidOperationException($"Unexpected module reference scheme '{moduleReference.Scheme}'.");

        private bool HasRestoreFailed(ModuleReference moduleReference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
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

        private void SetRestoreFailure(ModuleReference moduleReference, RootConfiguration configuration, DiagnosticBuilder.ErrorBuilderDelegate failureBuilder)
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

            private readonly ModuleReference moduleReference;

            public RestoreFailureKey(CloudConfiguration configuration, ModuleReference moduleReference)
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
            public RestoreFailureInfo(ModuleReference moduleReference, DiagnosticBuilder.ErrorBuilderDelegate failureBuilder, DateTime expiration)
            {
                this.ModuleReference = moduleReference;
                this.FailureBuilder = failureBuilder;
                this.Expiration = expiration;
            }

            public ModuleReference ModuleReference { get; }

            public DiagnosticBuilder.ErrorBuilderDelegate FailureBuilder { get; }

            public DateTime Expiration { get; }
        }
    }
}
