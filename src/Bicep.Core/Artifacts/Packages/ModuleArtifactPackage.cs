// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.Artifacts.Packages;

/// <summary>
/// A compiled module, optionally with its Bicep sources.
/// </summary>
public sealed record ModuleArtifactPackage(BinaryData Template, BinaryData? Sources, string? DocumentationUri, string? Description = null)
{
    /// <summary>
    /// Creates a package to publish, taking the description from the template's metadata.
    /// </summary>
    public static ModuleArtifactPackage FromTemplate(BinaryData template, BinaryData? sources, string? documentationUri) =>
        new(template, sources, documentationUri, DescriptionHelper.TryGetFromArmTemplate(template));
}
