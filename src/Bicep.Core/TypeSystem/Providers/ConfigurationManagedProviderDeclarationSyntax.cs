// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax.Providers;

public record ConfigurationManagedProviderSpecificationSyntax(string NamespaceIdentifier, bool IsValid, TextSpan Span) : IProviderSpecificationSyntax
{
    public string? Version => null;
};
