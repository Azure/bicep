// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Namespaces;

public interface INamespaceProvider
{
    NamespaceType? TryGetNamespace(string providerName, string aliasName, ResourceScope resourceScope, IFeatureProvider features);

    IEnumerable<string> AvailableNamespaces { get; }
}
