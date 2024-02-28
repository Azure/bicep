// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.TypeSystem.Providers;

public record LegacyProviderSpecification(string NamespaceIdentifier, string Version, bool IsValid, TextSpan Span) : IProviderSpecification;
