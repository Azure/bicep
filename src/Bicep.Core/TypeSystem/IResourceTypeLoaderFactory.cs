// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;

namespace Bicep.Core.TypeSystem
{
    public interface IResourceTypeLoaderFactory
    {
        IResourceTypeLoader? GetResourceTypeLoader(string? version, IFeatureProvider features);

        IResourceTypeLoader GetBuiltInTypeLoader();
    }
}
