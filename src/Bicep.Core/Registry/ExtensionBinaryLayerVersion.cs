// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry;

/// <summary>
/// The protocol that published extension binaries implement, which selects their OCI layer media type.
/// </summary>
public enum ExtensionBinaryLayerVersion
{
    /// <summary>The v1 gRPC protocol used by Bicep local deploy.</summary>
    V1,

    /// <summary>The Extension API Contract V2 over HTTP.</summary>
    V2,
}
