// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Parsing;

namespace Bicep.Core.TypeSystem.Providers;

public record ResourceTypesProviderDescriptor
{
    public ResourceTypesProviderDescriptor(
        string name,
        string version,
        string? artifactRef = null,
        string? alias = null,
        Uri? typesBaseUri = null)
    {
        Name = name;
        Alias = alias ?? name;
        ArtifactReference = artifactRef ?? "unkown";
        Version = version;
        TypesBaseUri = typesBaseUri;
    }

    public string Name { get; }

    public string Alias { get; }

    public Uri? TypesBaseUri { get; }

    public string Version { get; }

    public string ArtifactReference { get; }
}
