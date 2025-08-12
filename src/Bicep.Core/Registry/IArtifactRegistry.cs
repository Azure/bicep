// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.Utils;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// An implementation of a Bicep artifact registry.
    /// </summary>
    public interface IArtifactRegistry
    {
        /// <summary>
        /// Gets the scheme used by this registry in artifact references.
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Gets the capabilities of this registry for the specified artifact reference.
        /// </summary>
        /// <param name="reference">The module reference</param>
        RegistryCapabilities GetCapabilities(ArtifactType artifactType, ArtifactReference reference);

        /// <summary>
        /// Attempts to parse the specified unqualified reference or returns a failure builder.
        /// </summary>
        /// <param name="aliasName">The alias name</param>
        /// <param name="reference">The unqualified artifact reference</param>
        /// <param name="artifactType">The artifact type.</param>
        ResultWithDiagnosticBuilder<ArtifactReference> TryParseArtifactReference(BicepSourceFile referencingFile, ArtifactType artifactType, string? aliasName, string reference);

        /// <summary>
        /// Returns true if the specified artifact is already cached in the local cache.
        /// </summary>
        /// <param name="reference">The reference to the artifact.</param>
        bool IsArtifactRestoreRequired(ArtifactReference reference);

        /// <summary>
        /// Returns true if the specified module exists in the registry.
        /// </summary>
        /// <param name="reference">The reference to the module.</param>
        Task<bool> CheckArtifactExists(ArtifactType artifactType, ArtifactReference reference);

        /// <summary>
        /// Downloads the specified modules from the registry and caches them locally.
        /// Returns a mapping of module references to error builders for modules that failed to be downloaded.
        /// </summary>
        /// <param name="references">module references</param>
        Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<ArtifactReference> references);

        /// <summary>
        /// Called when time to restore artifacts, even if all artifacts are already restored.  Allows the registry provider
        /// an opportunity to deal with registry tasks that are not specific to a specific artifact or version.
        /// </summary>
        Task OnRestoreArtifacts(bool forceRestore);

        /// <summary>
        /// Invalidate the specified cached modules from the registry.
        /// Returns a mapping of module references to error builders for modules that failed to be invalidated.
        /// </summary>
        /// <param name="references">module references</param>
        Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<ArtifactReference> references);

        /// <summary>
        /// Publishes the module at the specified path to the registry.
        /// </summary>
        /// <param name="reference">The module reference</param>
        /// <param name="compiled">The compiled module</param>
        /// <param name="bicepSources">The source archive</param>
        Task PublishModule(ArtifactReference reference, BinaryData compiled, BinaryData? bicepSources, string? documentationUri, string? description);

        /// <summary>
        /// Publishes an extension types package to the registry.
        /// </summary>
        /// <param name="reference">The extension reference</param>
        Task PublishExtension(ArtifactReference reference, ExtensionPackage package);

        /// <summary>
        /// Returns documentationUri for the module.
        /// </summary>
        /// <param name="reference">The module reference</param>
        string? GetDocumentationUri(ArtifactReference reference);

        /// <summary>
        /// Returns description for a module.
        /// </summary>
        Task<string?> TryGetModuleDescription(ModuleSymbol module, ArtifactReference reference);
    }
}
