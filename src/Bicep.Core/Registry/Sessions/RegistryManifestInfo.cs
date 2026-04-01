// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Sessions;

public sealed record RegistryManifestInfo(
    string Digest,
    string? MediaType,
    string? ArtifactType,
    IReadOnlyDictionary<string, string> Annotations);
