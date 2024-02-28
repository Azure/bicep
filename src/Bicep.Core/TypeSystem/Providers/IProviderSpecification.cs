// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem.Providers;

public interface IProviderSpecification : ISymbolNameSource
{
    string NamespaceIdentifier { get; }
    string? Version { get; }
}
