// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax.Providers;

public record LegacyProviderSpecificationSyntax(string NamespaceIdentifier, string Version, bool IsValid, TextSpan Span) : IProviderSpecificationSyntax;
