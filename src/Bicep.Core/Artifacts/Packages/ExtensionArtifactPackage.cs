// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Artifacts.Codecs;

namespace Bicep.Core.Artifacts.Packages;

public record ExtensionBinary(SupportedArchitecture Architecture, BinaryData Data);

/// <summary>
/// An extension's types and its binaries, one per architecture. An extension with binaries supports local deployment.
/// </summary>
public sealed record ExtensionArtifactPackage(BinaryData Types, ImmutableArray<ExtensionBinary> Binaries);
