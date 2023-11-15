// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Parsing;

namespace Bicep.Core.TypeSystem;

public record ResourceTypesProviderDescriptor
{
    public ResourceTypesProviderDescriptor(
        string name,
        string version,
        string? alias = null,
        Uri? path = null,
        TextSpan? span = null)
    {
        Span = span ?? TextSpan.TextDocumentStart;
        Name = name;
        Alias = alias ?? name;
        Version = version;
        Path = path?.AbsolutePath;
    }

    public string Name { get; }

    public string Alias { get; }

    public string? Path { get; }

    public string Version { get; }

    public TextSpan Span { get; }
}
