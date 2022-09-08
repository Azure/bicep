// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.Semantics.Namespaces;

public interface INamespaceProviderManager
{
    INamespaceProvider GetNamespaceProvider(Uri templateUri);

    static INamespaceProviderManager ForNamespaceProvider(INamespaceProvider namespaceProvider)
        => new ConstantNamespaceProviderManager(namespaceProvider);
    
    private class ConstantNamespaceProviderManager : INamespaceProviderManager
    {
        private readonly INamespaceProvider namespaceProvider;

        internal ConstantNamespaceProviderManager(INamespaceProvider namespaceProvider)
        {
            this.namespaceProvider = namespaceProvider;
        }

        public INamespaceProvider GetNamespaceProvider(Uri templateUri) => namespaceProvider;
    }
}
