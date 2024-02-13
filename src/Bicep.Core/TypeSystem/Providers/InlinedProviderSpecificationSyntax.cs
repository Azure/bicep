// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax.Providers;

public record InlinedProviderSpecificationSyntax(
    string NamespaceIdentifier,
    string Version,
    string UnexpandedArtifactAddress,
    bool IsValid,
    TextSpan Span) : IProviderSpecificationSyntax;
