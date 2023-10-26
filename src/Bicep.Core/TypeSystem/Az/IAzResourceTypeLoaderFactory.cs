// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;

namespace Bicep.Core.TypeSystem.Az
{
    public interface IAzResourceTypeLoaderFactory
    {
        IResourceTypeLoader? GetResourceTypeLoader(string? version, IFeatureProvider features);

        IResourceTypeLoader GetBuiltInTypeLoader();
    }
}
