// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Bicep.Types;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;

namespace Bicep.Core.TypeSystem.Providers
{
    public interface IResourceTypeProviderFactory
    {
        ResultWithDiagnostic<IResourceTypeProvider> GetResourceTypeProviderFromFilePath(ResourceTypesProviderDescriptor providerDescriptor);

        IResourceTypeProvider GetBuiltInAzResourceTypesProvider();
    }
}
