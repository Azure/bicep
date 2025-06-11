// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SourceGraph.Artifacts;

namespace Bicep.Core.SourceGraph.ArtifactReferences
{
    public interface IExtensionArtifactReference
    {
        public IExtensionArtifact ResolveExtensionArtifact();
    }
}
