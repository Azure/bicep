// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Registry
{
    public class LocalModuleRegistry : ModuleRegistry<LocalModuleReference>
    {
        private readonly IFileResolver fileResolver;
        private readonly Uri parentModuleUri;
        private readonly BicepCompiler? bicepCompiler;

        public LocalModuleRegistry(IFileResolver fileResolver, Uri parentModuleUri, BicepCompiler? bicepCompiler)
        {
            this.fileResolver = fileResolver;
            this.parentModuleUri = parentModuleUri;
            this.bicepCompiler = bicepCompiler;
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

        public override Task PublishModule(LocalModuleReference moduleReference, Stream compiled, string? documentationUri, string? description) => throw new NotSupportedException("Local modules cannot be published.");

        public override Task<bool> CheckModuleExists(LocalModuleReference reference) => throw new NotSupportedException("Local modules cannot be published.");

        public override string? TryGetDocumentationUri(LocalModuleReference moduleReference) => null;

        public override async Task<string?> TryGetDescription(LocalModuleReference moduleReference)
        {
            try
            {
                if (this.TryGetLocalModuleEntryPointUri(moduleReference, out Uri? localUri, out _) && this.bicepCompiler is not null)
                {
                    var compilation = await this.bicepCompiler.CreateCompilation(localUri, skipRestore: true);
                    if (compilation.SourceFileGrouping.FileResultByUri.TryGetValue(localUri, out var result)
                        && result.File is { } source
                        && compilation.GetSemanticModel(source) is { } semanticModel)
                    {
                        return DescriptionHelper.TryGetFromSemanticModel(semanticModel);
                    }
                }
            }
            catch
            {
                // ignore
            }

            return null;
        }
    }
}
