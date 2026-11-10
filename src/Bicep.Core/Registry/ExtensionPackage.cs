// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry.Oci;

namespace Bicep.Core.Registry;

public record ExtensionBinary(SupportedArchitecture Architecture, BinaryData Data);

public sealed record ExtensionPackage(BinaryData Types, bool LocalDeployEnabled, ImmutableArray<ExtensionBinary> Binaries)
{
    public ExtensionBinaryLayerVersion BinaryLayerVersion { get; init; } = ExtensionBinaryLayerVersion.V1;
}
