// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Sessions;

public sealed record RegistryRef(string Host, string Repository, string? Tag, string? Digest);
