// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Documentation;

/// <summary>
/// Represents a local usage example.
/// </summary>
public record BicepDocumentationUsageExample(string Name, string RelativePath, string? Description, string Contents);
