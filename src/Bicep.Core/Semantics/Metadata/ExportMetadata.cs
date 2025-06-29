// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Metadata;

public enum ExportMetadataKind
{
    Error = 0,
    Type,
    Variable,
    Function,
}

public abstract record ExportMetadata(ExportMetadataKind Kind, string Name, ITypeReference TypeReference, string? Description);

public record ExportedTypeMetadata(string Name, ITypeReference TypeReference, string? Description)
    : ExportMetadata(ExportMetadataKind.Type, Name, TypeReference, Description);

public record ExportedVariableMetadata(string Name, ITypeReference TypeReference, string? Description, ITypeReference? DeclaredType)
    : ExportMetadata(ExportMetadataKind.Variable, Name, TypeReference, Description);

public record ExportedFunctionParameterMetadata(string Name, ITypeReference TypeReference, string? Description);

public record ExportedFunctionReturnMetadata(ITypeReference TypeReference, string? Description);

public record ExportedFunctionMetadata(string Name, ImmutableArray<ExportedFunctionParameterMetadata> Parameters, ExportedFunctionReturnMetadata Return, string? Description)
    : ExportMetadata(ExportMetadataKind.Function, Name, new LambdaType([.. Parameters.Select(md => md.TypeReference)], [], Return.TypeReference), Description);

public record DuplicatedExportMetadata(string Name, ImmutableArray<string> ExportKindsWithSameName)
    : ExportMetadata(ExportMetadataKind.Error, Name, ErrorType.Empty(), $"The name \"{Name}\" is ambiguous because it refers to exports of the following kinds: {string.Join(", ", ExportKindsWithSameName)}.");
