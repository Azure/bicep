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
        bool isImplicitImport,
        string? alias = null,
        string? artifactRef = null,
        Uri? typesBaseUri = null)
    {

        Name = name;
        Alias = alias ?? name;
        ArtifactReference = artifactRef ?? (isImplicitImport ? name : $"{name}@{version}");
        Version = version;
        TypesBaseUri = typesBaseUri;
    }

    public string Name { get; }

    public string Alias { get; }

    public Uri? TypesBaseUri { get; }

    public string Version { get; }

    public string ArtifactReference { get; }
}
