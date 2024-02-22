// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem.Providers;

namespace Bicep.Core.Syntax.Providers;
public record ProviderSpecificationTrivia(TextSpan Span) : IProviderSpecification
{
    public string NamespaceIdentifier => LanguageConstants.ErrorName;
    public bool IsValid => false;
    public string? Version => null;
};
