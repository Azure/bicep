// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Features;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics.Namespaces;

public record TypesProviderDescriptor
{
    public TypesProviderDescriptor(string name, string? alias = null, string? path = null, string? version = null)
    {
        Name = name;
        Alias = alias ?? name;
        Path = path;
        Version = version;
    }

    public string Name { get; }

    public string Alias { get; }

    public string? Path { get; }

    public string? Version { get; }
}

public interface INamespaceProvider
{
    NamespaceType? TryGetNamespace(
        TypesProviderDescriptor typesProviderDescriptor,
        ResourceScope resourceScope,
        IFeatureProvider features,
        BicepSourceFileKind sourceFileKind);

    IEnumerable<string> AvailableNamespaces { get; }
}
