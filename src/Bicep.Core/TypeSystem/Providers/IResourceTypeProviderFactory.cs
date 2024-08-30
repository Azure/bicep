// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;

namespace Bicep.Core.TypeSystem.Providers
{
    public interface IResourceTypeProviderFactory
    {
        ResultWithDiagnosticBuilder<IResourceTypeProvider> GetResourceTypeProvider(ArtifactReference? artifactReference, Uri typesTgzUri);

        IResourceTypeProvider GetBuiltInAzResourceTypesProvider();
    }
}
