// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry
{
    public class ModuleDispatcher : IModuleDispatcher
    {
        private static readonly TimeSpan FailureExpirationInterval = TimeSpan.FromMinutes(30);

        private readonly ConcurrentDictionary<RestoreFailureKey, RestoreFailureInfo> restoreFailures = new();

        private readonly IArtifactRegistryProvider registryProvider;

        public ModuleDispatcher(IArtifactRegistryProvider registryProvider)
        {
            this.registryProvider = registryProvider;
        }

        public ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference(BicepSourceFile referencingFile, ArtifactType artifactType, string reference)
        {
            var parts = reference.Split(':', 2, StringSplitOptions.None);
            switch (parts.Length)
            {
                case 1:
                    // local path reference
                    if (this.registryProvider.TryGetRegistry(ArtifactReferenceSchemes.Local) is { } localRegistry)
                    {
                        return localRegistry.TryParseArtifactReference(referencingFile, artifactType, null, parts[0]);
                    }

                    return new(x => x.UnknownModuleReferenceScheme(ArtifactReferenceSchemes.Local, this.registryProvider.SupportedSchemes));

                case 2:
                    string scheme = parts[0];
                    string? aliasName = null;

                    if (parts[0].Contains('/'))
                    {
                        // The scheme contains an alias.
                        var schemeParts = parts[0].Split('/', 2, StringSplitOptions.None);
                        scheme = schemeParts[0];
                        aliasName = schemeParts[1];
                    }

                    if (!string.IsNullOrEmpty(scheme) && this.registryProvider.TryGetRegistry(scheme) is { } registry)
                    {
                        // the scheme is recognized
                        var rawValue = parts[1];

                        return registry.TryParseArtifactReference(referencingFile, artifactType, aliasName, rawValue);
                    }

                    // unknown scheme
                    return new(x => x.UnknownModuleReferenceScheme(scheme, this.registryProvider.SupportedSchemes));

                default:
                    // empty string
                    return new(x => x.ModulePathHasNotBeenSpecified());
            }
        }

        public ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference(BicepSourceFile referencingFile, IArtifactReferenceSyntax artifactReferenceSyntax)
        {
            if (artifactReferenceSyntax is ExtensionDeclarationSyntax extensionSyntax)
            {
                var artifactAddressResult = TryGetArtifactAddress(referencingFile, extensionSyntax);
                if (!artifactAddressResult.IsSuccess(out var result, out var pathFailureBuilder))
                {
                    return new(pathFailureBuilder);
                }

                return this.TryGetArtifactReference(referencingFile, artifactReferenceSyntax.GetArtifactType(), result);
            }

            var artifactPathResult = SyntaxHelper.TryGetForeignTemplatePath(artifactReferenceSyntax, GetErrorBuilderDelegate(artifactReferenceSyntax));
            if (!artifactPathResult.IsSuccess(out var artifactPath, out var failureBuilder))
            {
                return new(failureBuilder);
            }
            return this.TryGetArtifactReference(referencingFile, artifactReferenceSyntax.GetArtifactType(), artifactPath);
        }

        private static DiagnosticBuilder.DiagnosticBuilderDelegate GetErrorBuilderDelegate(IArtifactReferenceSyntax artifactReferenceSyntax) => artifactReferenceSyntax switch
        {
            UsingDeclarationSyntax => x => x.UsingPathHasNotBeenSpecified(),
            ExtendsDeclarationSyntax => x => x.ExtendsPathHasNotBeenSpecified(),
            CompileTimeImportDeclarationSyntax => x => x.PathHasNotBeenSpecified(),
            ModuleDeclarationSyntax => x => x.ModulePathHasNotBeenSpecified(),
            ComponentDeclarationSyntax => x => x.PathHasNotBeenSpecified(),
            TestDeclarationSyntax => x => x.PathHasNotBeenSpecified(),
            _ => throw new NotImplementedException($"Unexpected artifact reference syntax type '{artifactReferenceSyntax.GetType().Name}'.")
        };

        private ResultWithDiagnosticBuilder<string> TryGetArtifactAddress(BicepSourceFile referencingFile, ExtensionDeclarationSyntax extensionSyntax)
        {
            switch (extensionSyntax.SpecificationString)
            {
                case StringSyntax inlinedSpec:
                    if (inlinedSpec.TryGetLiteralValue() is not string pathValue)
                    {
                        return new(x => x.ExtensionSpecificationInterpolationUnsupported());
                    }

                    return new(pathValue);
                case IdentifierSyntax configSpec:
                    var config = referencingFile.Configuration;
                    var extensionPathResult = config.Extensions.TryGetExtensionSource(configSpec.IdentifierName).Transform(x => x.Value);

                    if (extensionPathResult.IsSuccess(out var extensionPath, out var error))
                    {
                        if (extensionPath.Contains(':'))
                        {
                            // The extension path contains a scheme.
                            return new(extensionPath);
                        }
                        if (config.ConfigFileUri is null)
                        {
                            throw new InvalidOperationException("The configuration file URI must be set when trying to resolve an extension reference.");
                        }

                        var extensionUri = config.ConfigFileUri.Resolve(extensionPath);

                        return new(extensionUri.GetPathRelativeTo(referencingFile.FileHandle.Uri));
                    }


                    return new(error);

                default:
                    return new(x => x.ExpectedExtensionSpecification());
            }
        }

        public RegistryCapabilities GetRegistryCapabilities(ArtifactType artifactType, ArtifactReference artifactReference)
        {
            var registry = this.GetRegistry(artifactReference);
            return registry.GetCapabilities(artifactType, artifactReference);
        }

        public ArtifactRestoreStatus GetArtifactRestoreStatus(
            ArtifactReference artifactReference,
            out DiagnosticBuilder.DiagnosticBuilderDelegate? failureBuilder)
        {
            var registry = this.GetRegistry(artifactReference);
            var configuration = artifactReference.ReferencingFile.Configuration;

            // have we already failed to restore this artifact?
            if (this.HasRestoreFailed(artifactReference, configuration, out var restoreFailureBuilder))
            {
                failureBuilder = restoreFailureBuilder;
                return ArtifactRestoreStatus.Failed;
            }

            if (registry.IsArtifactRestoreRequired(artifactReference))
            {
                // module is not present on the local file system
                failureBuilder = x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference);
                return ArtifactRestoreStatus.Unknown;
            }

            failureBuilder = null;
            return ArtifactRestoreStatus.Succeeded;
        }

        public ResultWithDiagnosticBuilder<IFileHandle> TryGetLocalArtifactEntryPointFileHandle(ArtifactReference artifactReference)
        {
            var configuration = artifactReference.ReferencingFile.Configuration;
            // has restore already failed for this artifact?
            if (this.HasRestoreFailed(artifactReference, configuration, out var restoreFailureBuilder))
            {
                return new(restoreFailureBuilder);
            }

            return artifactReference.TryGetEntryPointFileHandle();
        }

        public async Task<bool> RestoreArtifacts(IEnumerable<ArtifactReference> references, bool forceRestore)
        {
            // WARNING: The various operations on ModuleReference objects here rely on the custom Equals() implementation and NOT on object identity

            // many module declarations can point to the same module
            var uniqueReferences = references.Distinct().ToArray();

            // Call OnRestoreArtifacts on each registry provider. Can (currently at least) be done in parallel to restore.
            var onRestoreArtifactsTasks = new List<Task>();
            foreach (var scheme in this.registryProvider.SupportedSchemes)
            {
                var registry = this.registryProvider.GetRegistry(scheme);

                try
                {
                    onRestoreArtifactsTasks.Add(registry.OnRestoreArtifacts(forceRestore));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"{nameof(IArtifactRegistry.OnRestoreArtifacts)} failed: {ex.Message}");
                }
            }

            if (!forceRestore &&
                uniqueReferences.All(module => this.GetArtifactRestoreStatus(module, out _) == ArtifactRestoreStatus.Succeeded))
            {
                // all the modules have already been restored - no need to do anything
                return false;
            }

            // split modules refs by registry
            var referencesByRegistry = uniqueReferences.ToLookup(@ref => this.registryProvider.GetRegistry(@ref.Scheme));

            // send each set of refs to its own registry
            foreach (var registry in referencesByRegistry.Select(byRegistry => byRegistry.Key))
            {
                // if we're asked to purge modules cache
                if (forceRestore)
                {
                    var invalidateFailures = await registry.InvalidateArtifactsCache(referencesByRegistry[registry]);

                    // update cache invalidation status for each failed module
                    foreach (var (failedReference, failureBuilder) in invalidateFailures)
                    {
                        this.SetRestoreFailure(failedReference, failedReference.ReferencingFile.Configuration, failureBuilder);
                    }
                }

                var restoreFailures = await registry.RestoreArtifacts(referencesByRegistry[registry]);

                // update restore status for each failed module restore
                foreach (var (failedReference, failureBuilder) in restoreFailures)
                {
                    this.SetRestoreFailure(failedReference, failedReference.ReferencingFile.Configuration, failureBuilder);
                }

                try
                {
                    await Task.WhenAll(onRestoreArtifactsTasks);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"{nameof(IArtifactRegistry.OnRestoreArtifacts)} failed: {ex.Message}");
                }
            }

            return true;
        }

        public async Task PublishModule(ArtifactReference reference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri)
        {
            var registry = this.GetRegistry(reference);

            var description = DescriptionHelper.TryGetFromArmTemplate(compiledArmTemplate);
            await registry.PublishModule(reference, compiledArmTemplate, bicepSources, documentationUri, description);
        }

        public async Task PublishExtension(ArtifactReference reference, ExtensionPackage package)
        {
            var registry = this.GetRegistry(reference);

            await registry.PublishExtension(reference, package);
        }

        public async Task<bool> CheckModuleExists(ArtifactReference reference)
        {
            var registry = this.GetRegistry(reference);
            return await registry.CheckArtifactExists(ArtifactType.Module, reference);
        }

        public async Task<bool> CheckExtensionExists(ArtifactReference reference)
        {
            var registry = this.GetRegistry(reference);
            return await registry.CheckArtifactExists(ArtifactType.Extension, reference);
        }

        public void PruneRestoreStatuses()
        {
            // concurrent dictionary enumeration does NOT provide a point-in-time snapshot of values in the dictionary
            // however it also does not take a lock on all the items
            var dateTime = DateTime.UtcNow;
            foreach (var (key, value) in this.restoreFailures)
            {
                if (IsFailureInfoExpired(value, dateTime))
                {
                    // value is expired - remove it
                    this.restoreFailures.TryRemove(key, out _);
                }
            }
        }

        private IArtifactRegistry GetRegistry(ArtifactReference reference) => this.registryProvider.GetRegistry(reference.Scheme);

        private bool HasRestoreFailed(ArtifactReference reference, RootConfiguration configuration, [NotNullWhen(true)] out DiagnosticBuilder.DiagnosticBuilderDelegate? failureBuilder)
        {
            if (this.restoreFailures.TryGetValue(new(configuration.Cloud, reference), out var failureInfo) && !IsFailureInfoExpired(failureInfo, DateTime.UtcNow))
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

        private void SetRestoreFailure(ArtifactReference reference, RootConfiguration configuration, DiagnosticBuilder.DiagnosticBuilderDelegate failureBuilder)
        {
            // as the user is typing, the modules will keep getting recompiled
            // we can't keep retrying syntactically correct references to non-existent modules on every key press
            // absolute expiration here will ensure that the next retry is delayed until the specified interval passes
            // we're not doing sliding expiration because we want a retry to happen eventually
            // (we may consider adding an ability to immediately retry to the UX in the future as well)
            var expiration = DateTime.UtcNow.Add(FailureExpirationInterval);
            this.restoreFailures.TryAdd(new(configuration.Cloud, reference), new RestoreFailureInfo(reference, failureBuilder, expiration));
        }

        private class RestoreFailureKey
        {
            private readonly CloudConfiguration configuration;

            private readonly ArtifactReference reference;

            public RestoreFailureKey(CloudConfiguration configuration, ArtifactReference reference)
            {
                this.configuration = configuration;
                this.reference = reference;
            }

            public override bool Equals(object? obj) =>
                obj is RestoreFailureKey other &&
                this.configuration.Equals(other.configuration) &&
                this.reference.Equals(other.reference);

            public override int GetHashCode() => HashCode.Combine(this.configuration, this.reference);
        }

        private class RestoreFailureInfo
        {
            public RestoreFailureInfo(ArtifactReference reference, DiagnosticBuilder.DiagnosticBuilderDelegate failureBuilder, DateTime expiration)
            {
                this.Reference = reference;
                this.FailureBuilder = failureBuilder;
                this.Expiration = expiration;
            }

            public ArtifactReference Reference { get; }

            public DiagnosticBuilder.DiagnosticBuilderDelegate FailureBuilder { get; }

            public DateTime Expiration { get; }
        }
    }
}
