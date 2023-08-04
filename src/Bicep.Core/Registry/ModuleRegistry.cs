// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
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

        public abstract Task<bool> CheckModuleExists(T reference);

        public abstract Task PublishModule(T reference, Stream compiledArmTemplate, Stream? bicepSources, string? documentationUri, string? description);

        public abstract Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<T> references);

        public abstract Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateModulesCache(IEnumerable<T> references);

        public abstract bool TryGetLocalModuleEntryPointUri(T reference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        public abstract bool TryParseModuleReference(string? aliasName, string reference, [NotNullWhen(true)] out ModuleReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        public abstract string? TryGetDocumentationUri(T reference);

        public abstract Task<string?> TryGetDescription(T reference);

        public abstract SourceArchive? TryGetSources(T reference);

        public bool IsModuleRestoreRequired(ModuleReference reference) => this.IsModuleRestoreRequired(ConvertReference(reference));

        public Task<bool> CheckModuleExists(ModuleReference reference) => this.CheckModuleExists(ConvertReference(reference));

        public Task PublishModule(ModuleReference moduleReference, Stream compiledArmTemplate, Stream? bicepSources, string? documentationUri, string? description) => this.PublishModule(ConvertReference(moduleReference), compiledArmTemplate, bicepSources, documentationUri, description);

        public Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<ModuleReference> references) =>
            this.RestoreModules(references.Select(ConvertReference));

        public Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateModulesCache(IEnumerable<ModuleReference> references) =>
             this.InvalidateModulesCache(references.Select(ConvertReference));

        public bool TryGetLocalModuleEntryPointUri(ModuleReference reference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder) =>
            this.TryGetLocalModuleEntryPointUri(ConvertReference(reference), out localUri, out failureBuilder);

        public string? GetDocumentationUri(ModuleReference reference) => this.TryGetDocumentationUri(ConvertReference(reference));

        public async Task<string?> TryGetDescription(ModuleReference reference) => await this.TryGetDescription(ConvertReference(reference));

        public SourceArchive? TryGetSources(ModuleReference reference) => this.TryGetSources(ConvertReference(reference));

        public abstract RegistryCapabilities GetCapabilities(T reference);

        private static T ConvertReference(ModuleReference reference) => reference switch
        {
            T typed => typed,
            _ => throw new ArgumentException($"Reference type '{reference.GetType().Name}' is not supported."),
        };
    }
}
