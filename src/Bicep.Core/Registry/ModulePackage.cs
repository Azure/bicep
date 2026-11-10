// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry;

/// <summary>
/// A compiled module to publish, optionally with its Bicep sources.
/// </summary>
public sealed record ModulePackage(BinaryData Template, BinaryData? Sources, string? DocumentationUri);
