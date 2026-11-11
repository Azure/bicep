// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Bicep.Core.Artifacts.Catalog;

/// <summary>
/// Metadata about a module or a version.
/// </summary>
/// <param name="Description"></param>
/// <param name="DocumentationUri"></param>
public record RegistryMetadataDetails(
    string? Description,
    string? DocumentationUri);

