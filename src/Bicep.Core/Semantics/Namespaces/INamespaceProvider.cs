// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Namespaces;

public interface INamespaceProvider
{
    NamespaceType? TryGetNamespace(string providerName, string? providerVersion, string aliasName, ResourceScope resourceScope);

    IEnumerable<string> AvailableNamespaces { get; }

    bool AllowImportStatements { get; }
}
