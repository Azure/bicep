// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Registry
{
    public class ModuleDispatcher : IModuleDispatcher
    {
        private readonly ImmutableDictionary<string, IModuleRegistry> registries;

        /*
         * This data structure behaves like a dictionary but creates a weak reference to the keys stored within.
         * When the keys stop being reachable and are cleaned up by the GC, they will disappear from this table.
         */ 
        private readonly ConditionalWeakTable<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate> restoreStatuses;

        public ModuleDispatcher(IModuleRegistryProvider registryProvider)
        {
            this.registries = registryProvider.Registries.ToImmutableDictionary(registry => registry.Scheme);
            this.AvailableSchemes = this.registries.Keys.OrderBy(s => s).ToImmutableArray();
            this.restoreStatuses = new ConditionalWeakTable<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate>();
        }

        public ImmutableArray<string> AvailableSchemes { get; }

        public bool ValidateModuleReference(ModuleDeclarationSyntax module, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder) =>
            this.TryGetModuleReference(module, out failureBuilder) is not null;

        public bool IsModuleAvailable(ModuleDeclarationSyntax module, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var reference = GetModuleReference(module);
            if (!this.registries.TryGetValue(reference.Scheme, out var registry))
            {
                throw new NotImplementedException($"Unexpected module reference scheme '{reference.Scheme}'");
            }

            // have we already failed to restore this module?
            // TODO: This needs to reset after some time
            if (this.HasRestoreFailed(module, out var restoreFailureBuilder))
            {
                failureBuilder = restoreFailureBuilder;
                return false;
            }

            if (registry.IsModuleRestoreRequired(reference))
            {
                // module is not present on the local file system
                // TODO: This error needs to have different text in CLI vs the language server
                failureBuilder = x => x.ModuleRequiresRestore(reference.FullyQualifiedReference);
                return false;
            }

            failureBuilder = null;
            return true;
        }

        public Uri? TryGetLocalModuleEntryPointUri(Uri parentModuleUri, ModuleDeclarationSyntax module, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            // has restore already failed for this module?
            if(this.HasRestoreFailed(module, out var restoreFailureBuilder))
            {
                failureBuilder = restoreFailureBuilder;
                return null;
            }

            var reference = GetModuleReference(module);
            Type refType = reference.GetType();
            if (this.registries.TryGetValue(reference.Scheme, out var registry))
            {
                return registry.TryGetLocalModuleEntryPointPath(parentModuleUri, reference, out failureBuilder);
            }

            throw new NotImplementedException($"Unexpected module reference type '{refType.Name}'");
        }

        public bool RestoreModules(IEnumerable<ModuleDeclarationSyntax> modules)
        {
            // WARNING: The various operations on ModuleReference objects here rely on the custom Equals() implementation and NOT on object identity

            if (modules.All(module => this.IsModuleAvailable(module, out _)))
            {
                // all the modules have already been restored - no need to do anything
                return false;
            }

            // one module ref can be associated with multiple module declarations
            var referenceToModules = modules.ToLookup(module => GetModuleReference(module));

            var references = modules.Select(module => GetModuleReference(module)).Distinct();

            // split module refs by scheme
            var referencesByScheme = references.ToLookup(@ref => @ref.Scheme);

            // send each set of refs to its own registry
            foreach (var scheme in this.registries.Keys.Where(refType => referencesByScheme.Contains(refType)))
            {
                var restoreStatuses = this.registries[scheme].RestoreModules(referencesByScheme[scheme]);

                // update restore status for each failed module restore
                foreach(var (failedReference, failureBuilder) in restoreStatuses)
                {
                    foreach(var failedModule in referenceToModules[failedReference])
                    {
                        this.SetRestoreFailure(failedModule, failureBuilder);
                    }
                }
            }

            return true;
        }

        private ModuleReference GetModuleReference(ModuleDeclarationSyntax module) =>
            TryGetModuleReference(module, out _) ?? throw new InvalidOperationException($"The specified module is not valid. Call {nameof(ValidateModuleReference)}() first.");

        private ModuleReference? TryGetModuleReference(ModuleDeclarationSyntax module, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var moduleReferenceString = SyntaxHelper.TryGetModulePath(module, out var getModulePathFailureBuilder);
            if (moduleReferenceString is null)
            {
                failureBuilder = getModulePathFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(SyntaxHelper.TryGetModulePath)} to provide failure diagnostics.");
                return null;
            }

            return this.TryParseModuleReference(moduleReferenceString, out failureBuilder);
        }

        private ModuleReference? TryParseModuleReference(string reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
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

        private bool HasRestoreFailed(ModuleDeclarationSyntax module, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            // TODO: In cases the user publishes a module after authoring an invalid reference to it, the current logic will permanently block it
            // until the language server is restarted. We need to reset the failure status after some time or create an alternative mechanism.
            return this.restoreStatuses.TryGetValue(module, out failureBuilder);
        }

        private void SetRestoreFailure(ModuleDeclarationSyntax module, DiagnosticBuilder.ErrorBuilderDelegate failureBuilder)
        {
            this.restoreStatuses.AddOrUpdate(module, failureBuilder);
        }
    }
}
