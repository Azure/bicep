// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Documentation;

/// <summary>
/// A cross-referenced module declared with a <c>module</c> statement in the entrypoint file.
/// </summary>
public record BicepDocumentationReference(string SymbolicName, string? Path, string? Description);
