// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public interface INamespaceSymbol
    {
        string Name { get; }

        NamespaceType? TryGetNamespaceType();
    }
}
