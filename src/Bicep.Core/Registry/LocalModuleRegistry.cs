// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public LocalModuleRegistry(IFileResolver fileResolver)
        {
            this.fileResolver = fileResolver;
        }

        public override string Scheme => ModuleReferenceSchemes.Local;

        public override RegistryCapabilities Capabilities => RegistryCapabilities.Default;

        public override ModuleReference? TryParseModuleReference(string reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder) => LocalModuleReference.TryParse(reference, out failureBuilder);

        public override Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, LocalModuleReference reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            parentModuleUri = parentModuleUri ?? throw new ArgumentException($"{nameof(parentModuleUri)} must not be null for local module references.");
            var localUri = fileResolver.TryResolveFilePath(parentModuleUri, reference.Path);
            if (localUri is not null)
            {
                failureBuilder = null;
                return localUri;
            }

            failureBuilder = x => x.FilePathCouldNotBeResolved(reference.Path, parentModuleUri.LocalPath);
            return null;
        }

        public override Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<LocalModuleReference> references)
        {
            // local modules are already present on the file system
            // and do not require init
            return Task.FromResult<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>>(ImmutableDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>.Empty);
        }

        public override bool IsModuleRestoreRequired(LocalModuleReference reference) => false;

        public override Task PublishModule(LocalModuleReference moduleReference, Stream compiled) => throw new NotSupportedException("Local modules cannot be published.");
    }
}
