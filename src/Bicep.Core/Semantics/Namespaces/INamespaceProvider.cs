// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using System.Collections.Generic;

namespace Bicep.Core.Semantics.Namespaces;

public interface INamespaceProvider
{
    NamespaceType? TryGetNamespace(
        string providerName,
        string aliasName,
        ResourceScope resourceScope,
        IFeatureProvider features,
        BicepSourceFileKind sourceFileKind,
        string? providerVersion = null
    );

    IEnumerable<string> AvailableNamespaces { get; }
}
