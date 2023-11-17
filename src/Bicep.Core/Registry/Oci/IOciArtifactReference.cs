// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bicep.Core.Registry.Oci;

public interface IOciArtifactReference : IArtifactIdParts
{
    /// <summary>
    /// Gets the fully qualified artifact reference, which includes the scheme.
    /// </summary>
    string FullyQualifiedReference { get; }
}
