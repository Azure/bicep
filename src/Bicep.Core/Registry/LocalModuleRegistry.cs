// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;

namespace Bicep.Core.Registry
{
    public class LocalModuleRegistry : ModuleRegistry<LocalModuleReference>
    {
        private readonly IFileResolver fileResolver;
        private readonly Uri parentModuleUri;

        public LocalModuleRegistry(IFileResolver fileResolver, Uri parentModuleUri)
        {
            this.fileResolver = fileResolver;
            this.parentModuleUri = parentModuleUri;
        }

        public override string Scheme => ModuleReferenceSchemes.Local;

        public override RegistryCapabilities GetCapabilities(LocalModuleReference reference) => RegistryCapabilities.Default;

        public override bool TryParseModuleReference(string? alias, string reference, [NotNullWhen(true)] out ModuleReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (LocalModuleReference.TryParse(reference, parentModuleUri, out var @ref, out failureBuilder))
            {
                moduleReference = @ref;
                return true;
            }

            moduleReference = null;
            return false;
        }


        public override bool TryGetLocalModuleEntryPointUri(LocalModuleReference reference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            localUri = fileResolver.TryResolveFilePath(reference.ParentModuleUri, reference.Path);
            if (localUri is not null)
            {
                failureBuilder = null;
                return true;
            }

            failureBuilder = x => x.FilePathCouldNotBeResolved(reference.Path, reference.ParentModuleUri.LocalPath);
            return false;
        }

        public override Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<LocalModuleReference> references)
        {
            // local modules are already present on the file system
            // and do not require init
            return Task.FromResult<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>>(ImmutableDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>.Empty);
        }

        public override Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateModulesCache(IEnumerable<LocalModuleReference> references)
        {
            // local modules are already present on the file system, there's no cache concept for this one
            // we do nothing
            return Task.FromResult<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>>(ImmutableDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>.Empty);
        }

        public override bool IsModuleRestoreRequired(LocalModuleReference reference) => false;

        public override Task PublishModule(LocalModuleReference moduleReference, Stream compiled) => throw new NotSupportedException("Local modules cannot be published.");
    }
}
