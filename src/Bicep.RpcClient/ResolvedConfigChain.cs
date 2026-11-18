// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RpcClient.SemanticVersioning;

namespace Bicep.RpcClient;

/// <summary>
/// The effective version constraint and the configuration file that declared it, if any.
/// </summary>
internal sealed record ResolvedConfigChain(VersionRange? BicepVersionRange, string? DeclaringConfigPath);
