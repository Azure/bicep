// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Auth;

public sealed record RegistryCredential(
    AuthScheme Scheme,
    string Username,
    string? PasswordOrToken,
    DateTimeOffset? ExpiresOn);
