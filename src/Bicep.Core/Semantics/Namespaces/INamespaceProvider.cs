// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Namespaces
{
    public interface INamespaceProvider
    {
        NamespaceType? TryGetNamespace(string providerName, string aliasName, ResourceScope resourceScope);

        bool AllowImportStatements { get; }
    }
}