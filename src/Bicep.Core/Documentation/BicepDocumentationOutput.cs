// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Documentation;

/// <summary>
/// A module output.
/// </summary>
public record BicepDocumentationOutput(string Name, string TypeName, bool IsSecure, string? Description);
