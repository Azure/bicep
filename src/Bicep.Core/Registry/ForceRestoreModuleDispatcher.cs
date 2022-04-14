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
    // The purpose of this class is to handle the cache invalidation process, all the rest is passed down through ModuleDispatcher.
    public sealed class ForceRestoreModuleDispatcher : IModuleDispatcher
    {        
        private readonly IModuleDispatcher moduleDispatcher;

        private bool cacheInvalidationInvoked = false;

        public ForceRestoreModuleDispatcher(IModuleDispatcher moduleDispatcher)
        {
            this.moduleDispatcher = moduleDispatcher;
        }

        public ImmutableArray<string> AvailableSchemes => this.moduleDispatcher.AvailableSchemes;

        public ImmutableDictionary<string, IModuleRegistry> AvailableRegistries => this.moduleDispatcher.AvailableRegistries;

        public ModuleRestoreStatus GetModuleRestoreStatus(ModuleReference moduleReference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? errorDetailBuilder)
        {
            // return Unknown until we actually invalidated the cache and perform a normal restore
            if (! this.cacheInvalidationInvoked) {
                // act like module is not present on the local file system
                errorDetailBuilder = x => x.ModuleRequiresRestore(moduleReference.FullyQualifiedReference);
                return ModuleRestoreStatus.Unknown;
            }
            
            return this.moduleDispatcher.GetModuleRestoreStatus(moduleReference, configuration, out errorDetailBuilder);
        }

        public RegistryCapabilities GetRegistryCapabilities(ModuleReference moduleReference)
        {
            return this.moduleDispatcher.GetRegistryCapabilities(moduleReference);
        }

        public void PruneRestoreStatuses()
        {
            this.moduleDispatcher.PruneRestoreStatuses();
        }

        public Task PublishModule(RootConfiguration configuration, ModuleReference moduleReference, Stream compiled)
        {
            return this.moduleDispatcher.PublishModule(configuration, moduleReference, compiled);
        }

        public async Task<bool> RestoreModules(RootConfiguration configuration, IEnumerable<ModuleReference> moduleReferences)
        {
            if(!this.cacheInvalidationInvoked) {
                try{
                    // WARNING: The various operations on ModuleReference objects here rely on the custom Equals() implementation and NOT on object identity

                    // many module declarations can point to the same module
                    var uniqueReferences = moduleReferences.Distinct();

                    // split module refs by scheme
                    var referencesByScheme = uniqueReferences.ToLookup(@ref => @ref.Scheme);

                    // send each set of refs to its own registry
                    foreach (var scheme in this.AvailableRegistries.Keys.Where(refType => referencesByScheme.Contains(refType)))
                    {
                        // we're asked to purge modules cache
                        var forceModulesRestoreStatuses = await this.AvailableRegistries[scheme].InvalidateModulesCache(configuration, referencesByScheme[scheme]);

                        // update cache invalidation status for each failed modules
                        foreach (var (failedReference, failureBuilder) in forceModulesRestoreStatuses)
                        {
                            this.moduleDispatcher.SetRestoreFailure(failedReference, configuration, failureBuilder);
                        }
                    }
                }
                finally {
                    this.cacheInvalidationInvoked = true;
                }
            }
            
            return await this.moduleDispatcher.RestoreModules(configuration, moduleReferences);
        }

        public Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, ModuleReference moduleReference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            return this.moduleDispatcher.TryGetLocalModuleEntryPointUri(parentModuleUri, moduleReference, configuration, out failureBuilder);
        }

        public void SetRestoreFailure(ModuleReference moduleReference, RootConfiguration configuration, DiagnosticBuilder.ErrorBuilderDelegate failureBuilder)
        {
             this.moduleDispatcher.SetRestoreFailure(moduleReference, configuration, failureBuilder);
        }

        public ModuleReference? TryGetModuleReference(string reference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            return this.moduleDispatcher.TryGetModuleReference(reference, configuration, out failureBuilder);
        }

        public ModuleReference? TryGetModuleReference(ModuleDeclarationSyntax module, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            return this.moduleDispatcher.TryGetModuleReference(module, configuration, out failureBuilder);
        }
    }
}
