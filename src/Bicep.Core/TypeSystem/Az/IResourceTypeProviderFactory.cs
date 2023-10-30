// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Bicep.Types;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.TypeSystem
{
    public interface IResourceTypeProviderFactory
    {
        ResultWithDiagnostic<IResourceTypeProvider> GetResourceTypeProvider(TypesProviderDescriptor providerDescriptor, IFeatureProvider features);

        IResourceTypeProvider GetBuiltInAzResourceTypesProvider();
    }
}
