// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.IO.Abstraction;

namespace Bicep.Core.TypeSystem.Providers
{
    public interface IResourceTypeProviderFactory
    {
        ResultWithDiagnosticBuilder<IResourceTypeProvider> GetResourceTypeProvider(IFileHandle typesTgzFileHandle);

        IResourceTypeProvider GetBuiltInAzResourceTypesProvider();
    }
}
