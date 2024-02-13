// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax.Providers;
public record ProviderSpecificationSyntaxTrivia(TextSpan Span) : IProviderSpecificationSyntax
{
    public string NamespaceIdentifier => LanguageConstants.ErrorName;
    public bool IsValid => false;
    public string? Version => null;
};
