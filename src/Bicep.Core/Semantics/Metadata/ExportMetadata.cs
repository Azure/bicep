// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Metadata;

public abstract record ExportMetadata(string Name, ITypeReference TypeReference, string? Description) {}

public record ExportedTypeMetadata(string Name, ITypeReference TypeReference, string? Description)
    : ExportMetadata(Name, TypeReference, Description) {}

public record ExportedVariableMetadata(string Name, ITypeReference TypeReference, string? Description)
    : ExportMetadata(Name, TypeReference, Description) {}
