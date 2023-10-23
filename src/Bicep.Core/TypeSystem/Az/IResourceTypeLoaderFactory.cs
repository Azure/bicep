// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.TypeSystem
{
    public interface IResourceTypeLoaderFactory
    {
        IResourceTypeLoader? GetResourceTypeLoader(TypesProviderDescriptor providerDescriptor, IFeatureProvider features);

        IResourceTypeLoader GetBuiltInTypeLoader();
    }
}
