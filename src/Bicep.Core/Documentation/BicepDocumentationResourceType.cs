// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Documentation;

/// <summary>
/// A resource type declared (directly or nested) within a module.
/// </summary>
public record BicepDocumentationResourceType(string Type, bool IsExisting);
