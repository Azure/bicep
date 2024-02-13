// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Syntax.Providers;

public interface IProviderSpecificationSyntax : ISymbolNameSource
{
    string NamespaceIdentifier { get; }
    string? Version { get; }
}
