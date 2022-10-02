// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;

namespace Bicep.Core.Registry
{
    public abstract class ModuleRegistry<T> : IModuleRegistry where T : ModuleReference
    {
        public abstract string Scheme { get; }

        public RegistryCapabilities GetCapabilities(ModuleReference reference) => this.GetCapabilities(ConvertReference(reference));

        public abstract bool IsModuleRestoreRequired(T reference);

        public abstract Task PublishModule(T reference, Stream compiled);

        public abstract Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<T> references);

        public abstract Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateModulesCache(IEnumerable<T> references);

        public abstract bool TryGetLocalModuleEntryPointUri(T reference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        public abstract bool TryParseModuleReference(string? aliasName, string reference, [NotNullWhen(true)] out ModuleReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        public bool IsModuleRestoreRequired(ModuleReference reference) => this.IsModuleRestoreRequired(ConvertReference(reference));

        public Task PublishModule(ModuleReference moduleReference, Stream compiled) => this.PublishModule(ConvertReference(moduleReference), compiled);

        public Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<ModuleReference> references) =>
            this.RestoreModules(references.Select(ConvertReference));

        public Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateModulesCache(IEnumerable<ModuleReference> references) =>
             this.InvalidateModulesCache(references.Select(ConvertReference));

        public bool TryGetLocalModuleEntryPointUri(ModuleReference reference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder) =>
            this.TryGetLocalModuleEntryPointUri(ConvertReference(reference), out localUri, out failureBuilder);

        public abstract RegistryCapabilities GetCapabilities(T reference);

        private static T ConvertReference(ModuleReference reference) => reference switch
        {
            T typed => typed,
            _ => throw new ArgumentException($"Reference type '{reference.GetType().Name}' is not supported."),
        };
    }
}
