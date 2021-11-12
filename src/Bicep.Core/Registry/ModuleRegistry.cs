// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;

namespace Bicep.Core.Registry
{
    public abstract class ModuleRegistry<T> : IModuleRegistry where T : ModuleReference
    {
        public abstract string Scheme { get; }

        public RegistryCapabilities GetCapabilities(ModuleReference reference) => this.GetCapabilities(ConvertReference(reference));

        public abstract bool IsModuleRestoreRequired(T reference);

        public abstract Task PublishModule(RootConfiguration configuration, T reference, Stream compiled);

        public abstract Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(RootConfiguration configuration, IEnumerable<T> references);

        public abstract Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, T reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        public abstract ModuleReference? TryParseModuleReference(string? aliasName, string reference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        public bool IsModuleRestoreRequired(ModuleReference reference) => this.IsModuleRestoreRequired(ConvertReference(reference));

        public Task PublishModule(RootConfiguration configuration, ModuleReference moduleReference, Stream compiled) => this.PublishModule(configuration, ConvertReference(moduleReference), compiled);

        public Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(RootConfiguration configuration, IEnumerable<ModuleReference> references) =>
            this.RestoreModules(configuration, references.Select(ConvertReference));

        public Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, ModuleReference reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder) =>
            this.TryGetLocalModuleEntryPointUri(parentModuleUri, ConvertReference(reference), out failureBuilder);

        public abstract RegistryCapabilities GetCapabilities(T reference);

        private static T ConvertReference(ModuleReference reference) => reference switch
        {
            T typed => typed,
            _ => throw new ArgumentException($"Reference type '{reference.GetType().Name}' is not supported."),
        };
    }
}
