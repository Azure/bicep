// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// The resource types the Resource Palette offers, cached per resource type provider.
    /// </summary>
    public interface IVisualResourceTypeCatalogService
    {
        /// <summary>
        /// Returns every resource type deployable at the document's target scope, unless the client already holds
        /// that catalog (<paramref name="knownCatalogId"/>).
        /// </summary>
        VisualResourceTypesResult GetResourceTypes(SemanticModel model, string? knownCatalogId);

        VisualResourceTypeVersionsResult GetResourceTypeVersions(SemanticModel model, string fullyQualifiedType);
    }
}
