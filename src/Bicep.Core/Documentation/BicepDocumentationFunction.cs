// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Documentation;

/// <summary>
/// A user-defined function exported from a module (via <c>@export()</c>).
/// </summary>
public record BicepDocumentationFunction(
    string Name,
    ImmutableArray<BicepDocumentationFunctionParameter> Parameters,
    string ReturnTypeName,
    string? Description);

/// <summary>
/// A single parameter of an exported user-defined function.
/// </summary>
public record BicepDocumentationFunctionParameter(string Name, string TypeName, string? Description);
