// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Namespaces;

public record ResourceTypesProviderDescriptor
{
    public ResourceTypesProviderDescriptor(
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
