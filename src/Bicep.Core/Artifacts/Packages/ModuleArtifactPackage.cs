// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Bicep.Core.Artifacts.Packages;

/// <summary>
/// A compiled module to publish, optionally with its Bicep sources.
/// </summary>
public sealed record ModuleArtifactPackage(BinaryData Template, BinaryData? Sources, string? DocumentationUri);
