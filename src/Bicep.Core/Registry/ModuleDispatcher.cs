// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        private readonly ConcurrentDictionary<ModuleReference, RestoreFailureInfo> restoreFailures = new();
        
        public ModuleDispatcher(IModuleRegistryProvider registryProvider)
        {
            this.registries = registryProvider.Registries.ToImmutableDictionary(registry => registry.Scheme);
            this.AvailableSchemes = this.registries.Keys.OrderBy(s => s).ToImmutableArray();
        }

        public ImmutableArray<string> AvailableSchemes { get; }

        public ModuleReference? TryGetModuleReference(string reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var parts = reference.Split(':', 2, System.StringSplitOptions.None);
            switch (parts.Length)
            {
                case 1:
                    // local path reference
                    if (registries.TryGetValue(ModuleReferenceSchemes.Local, out var localRegistry))
                    {
                        return localRegistry.TryParseModuleReference(parts[0], out failureBuilder);
                    }

                    failureBuilder = x => x.UnknownModuleReferenceScheme(ModuleReferenceSchemes.Local, this.AvailableSchemes);
                    return null;

                case 2:
                    var scheme = parts[0];

                    if (!string.IsNullOrEmpty(scheme) && registries.TryGetValue(scheme, out var registry))
                    {
                        // the scheme is recognized
                        var rawValue = parts[1];
                        return registry.TryParseModuleReference(rawValue, out failureBuilder);
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

        public ModuleReference? TryGetModuleReference(ModuleDeclarationSyntax module, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var moduleReferenceString = SyntaxHelper.TryGetModulePath(module, out var getModulePathFailureBuilder);
            if (moduleReferenceString is null)
            {
                failureBuilder = getModulePathFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(SyntaxHelper.TryGetModulePath)} to provide failure diagnostics.");
                return null;
            }

            return this.TryGetModuleReference(moduleReferenceString, out failureBuilder);
        }

        public RegistryCapabilities GetRegistryCapabilities(ModuleReference moduleReference)
        {
            var registry = this.GetRegistry(moduleReference);
            return registry.Capabilities;
        }

        public ModuleRestoreStatus GetModuleRestoreStatus(ModuleReference moduleReference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var registry = this.GetRegistry(moduleReference);
            
            // have we already failed to restore this module?
            if (this.HasRestoreFailed(moduleReference, out var restoreFailureBuilder))
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

        public Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, ModuleReference moduleReference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            // has restore already failed for this module?
            if(this.HasRestoreFailed(moduleReference, out var restoreFailureBuilder))
            {
                failureBuilder = restoreFailureBuilder;
                return null;
            }

            var registry = this.GetRegistry(moduleReference);
            return registry.TryGetLocalModuleEntryPointUri(parentModuleUri, moduleReference, out failureBuilder);
        }

        public async Task<bool> RestoreModules(IEnumerable<ModuleReference> moduleReferences)
        {
            // WARNING: The various operations on ModuleReference objects here rely on the custom Equals() implementation and NOT on object identity

            if (moduleReferences.All(module => this.GetModuleRestoreStatus(module, out _) == ModuleRestoreStatus.Succeeded))
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
                var restoreStatuses = await this.registries[scheme].RestoreModules(referencesByScheme[scheme]);

                // update restore status for each failed module restore
                foreach(var (failedReference, failureBuilder) in restoreStatuses)
                {
                    this.SetRestoreFailure(failedReference, failureBuilder);
                }
            }

            return true;
        }

        public async Task PublishModule(ModuleReference moduleReference, Stream compiled)
        {
            var registry = this.GetRegistry(moduleReference);
            await registry.PublishModule(moduleReference, compiled);
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

        private bool HasRestoreFailed(ModuleReference moduleReference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (this.restoreFailures.TryGetValue(moduleReference, out var failureInfo) && !IsFailureInfoExpired(failureInfo, DateTime.UtcNow))
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

        private void SetRestoreFailure(ModuleReference moduleReference, DiagnosticBuilder.ErrorBuilderDelegate failureBuilder)
        {
            // as the user is typing, the modules will keep getting recompiled
            // we can't keep retrying syntactically correct references to non-existent modules on every key press
            // absolute expiration here will ensure that the next retry is delayed until the specified interval passes
            // we're not not doing sliding expiration because we want a retry to happen eventually
            // (we may consider adding an ability to immediately retry to the UX in the future as well)
            var expiration = DateTime.UtcNow.Add(FailureExpirationInterval);
            this.restoreFailures.TryAdd(moduleReference, new RestoreFailureInfo(moduleReference, failureBuilder, expiration));
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
