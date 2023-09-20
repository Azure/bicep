// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Metadata;

public enum ExportMetadataKind
{
    Error = 0,
    Type,
    Variable,
}

public abstract record ExportMetadata(ExportMetadataKind Kind, string Name, ITypeReference TypeReference, string? Description) {}

public record ExportedTypeMetadata(string Name, ITypeReference TypeReference, string? Description)
    : ExportMetadata(ExportMetadataKind.Type, Name, TypeReference, Description) {}

public record ExportedVariableMetadata(string Name, ITypeReference TypeReference, string? Description)
    : ExportMetadata(ExportMetadataKind.Variable, Name, TypeReference, Description) {}

public record DuplicatedExportMetadata(string Name, ImmutableArray<string> ExportKindsWithSameName)
    : ExportMetadata(ExportMetadataKind.Error, Name, ErrorType.Empty(), $"The name \"{Name}\" is ambiguous because it refers to exports of the following kinds: {string.Join(", ", ExportKindsWithSameName)}.");
