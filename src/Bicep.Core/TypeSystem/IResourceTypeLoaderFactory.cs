// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;

namespace Bicep.Core.TypeSystem
{
    public interface IResourceTypeLoaderFactory
    {
        IResourceTypeProvider? GetResourceTypeLoader(string? version, IFeatureProvider features);

        IResourceTypeProvider GetBuiltInTypeLoader();
    }
}
