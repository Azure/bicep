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
        /// Returns true if this registry handles the specified reference.
        /// </summary>
        bool CanHandle(ExternalArtifactReference reference);

        /// <summary>
        /// Gets the capabilities of this registry for the specified artifact reference.
        /// </summary>
        /// <param name="reference">The module reference</param>
        RegistryCapabilities GetCapabilities(ArtifactType artifactType, ExternalArtifactReference reference);

        /// <summary>
        /// Returns true if the specified artifact is already cached in the local cache.
        /// </summary>
        /// <param name="reference">The reference to the artifact.</param>
        bool IsArtifactRestoreRequired(ExternalArtifactReference reference);

        /// <summary>
        /// Returns true if the specified module exists in the registry.
        /// </summary>
        /// <param name="reference">The reference to the module.</param>
        Task<bool> CheckArtifactExists(ArtifactType artifactType, ExternalArtifactReference reference);

        /// <summary>
        /// Downloads the specified modules from the registry and caches them locally.
        /// Returns a mapping of module references to error builders for modules that failed to be downloaded.
        /// </summary>
        /// <param name="references">module references</param>
        Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<ExternalArtifactReference> references);

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
        Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<ExternalArtifactReference> references);

        /// <summary>
        /// Publishes the module at the specified path to the registry.
        /// </summary>
        /// <param name="reference">The module reference</param>
        /// <param name="compiled">The compiled module</param>
        /// <param name="bicepSources">The source archive</param>
        Task PublishModule(ExternalArtifactReference reference, BinaryData compiled, BinaryData? bicepSources, string? documentationUri, string? description);

        /// <summary>
        /// Publishes an extension types package to the registry.
        /// </summary>
        /// <param name="reference">The extension reference</param>
        Task PublishExtension(ExternalArtifactReference reference, ExtensionPackage package);

        /// <summary>
        /// Returns documentationUri for the module.
        /// </summary>
        /// <param name="reference">The module reference</param>
        string? GetDocumentationUri(ExternalArtifactReference reference);

        /// <summary>
        /// Returns description for a module.
        /// </summary>
        Task<string?> TryGetModuleDescription(ModuleSymbol module, ExternalArtifactReference reference);
    }
}
