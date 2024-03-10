// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem.Providers;

public interface IProviderSpecification : ISymbolNameSource
{
    string NamespaceIdentifier { get; }
    string? Version { get; }
}

public record LegacyProviderSpecification(
    string NamespaceIdentifier,
    string Version,
    bool IsValid,
    TextSpan Span) : IProviderSpecification;

public record InlinedProviderSpecification(
    string NamespaceIdentifier,
    string Version,
    string UnexpandedArtifactAddress,
    bool IsValid,
    TextSpan Span) : IProviderSpecification;

public record ConfigurationManagedProviderSpecification(
    string NamespaceIdentifier,
    bool IsValid,
    TextSpan Span) : IProviderSpecification
{
    public string? Version => null;
}

public record ProviderSpecificationTrivia(TextSpan Span) : IProviderSpecification
{
    public string NamespaceIdentifier => LanguageConstants.ErrorName;
    public bool IsValid => false;
    public string? Version => null;
}
