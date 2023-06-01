// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;

namespace Bicep.Core.TypeSystem.Az
{
    public interface IAzResourceTypeLoaderFactory
    {
        IAzResourceTypeLoader? GetResourceTypeLoader(string? version, IFeatureProvider features);
    }
}