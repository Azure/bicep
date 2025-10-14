// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Registry.Auth;

public sealed record AuthMetadata(
    string? Scheme,
    string? Realm,
    string? Service,
    ImmutableArray<string> Scopes,
    ImmutableDictionary<string, string> Parameters);
