// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Semantics;

public enum ScopeResolution
{
    /// <summary>
    ///   Inherit all scoped symbols from the parent scope.
    /// </summary>
    InheritAll,

    /// <summary>
    ///   Inherit only function symbols from the parent scope.
    /// </summary>
    InheritFunctionsOnly,

    /// <summary>
    ///   Only symbols that have not been declared by a parent (or above) scope.
    /// </summary>
    GlobalsOnly,
}
