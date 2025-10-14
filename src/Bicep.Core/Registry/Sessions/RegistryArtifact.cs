// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry.Oci;

namespace Bicep.Core.Registry.Sessions;

public sealed record RegistryArtifact(
    string? MediaType,
    string? ArtifactType,
    OciDescriptor Config,
    ImmutableArray<OciDescriptor> Layers,
    ImmutableDictionary<string, string> Annotations);
