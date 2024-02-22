// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.TypeSystem.Providers;

public record ConfigurationManagedProviderSpecification(string NamespaceIdentifier, bool IsValid, TextSpan Span) : IProviderSpecification
{
    public string? Version => null;
};
