// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Artifacts.Codecs;

namespace Bicep.Core.Artifacts.Packages;

public record ExtensionBinary(SupportedArchitecture Architecture, BinaryData Data);

public sealed record ExtensionPackage(BinaryData Types, bool LocalDeployEnabled, ImmutableArray<ExtensionBinary> Binaries);
