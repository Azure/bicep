// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.TypeSystem.Providers;

public record InlinedProviderSpecification(
    string NamespaceIdentifier,
    string Version,
    string UnexpandedArtifactAddress,
    bool IsValid,
    TextSpan Span) : IProviderSpecification;
