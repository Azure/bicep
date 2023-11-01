// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core.Features;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics.Namespaces;

public record TypesProviderDescriptor
{
    public TypesProviderDescriptor(
        string name,
        string? alias = null,
        Uri? path = null,
        string version = IResourceTypeProvider.BuiltInVersion,
        TextSpan? span = null)
    {
        this.Span = span ?? TextSpan.TextDocumentStart;
        Name = name;
        Alias = alias ?? name;
        Version = version;
        Path = path is null ? "builtin" : path.AbsolutePath;
    }

    public string Name { get; }

    public string Alias { get; }

    public string Path { get; }

    public string Version { get; }

    public TextSpan Span { get; }
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
