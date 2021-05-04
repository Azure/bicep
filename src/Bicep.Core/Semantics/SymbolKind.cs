// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Semantics
{
    public enum SymbolKind
    {
        Error,
        Type,
        File,
        Parameter,
        Variable,
        Resource,
        Module,
        Output,
        Namespace,
        Function,
        Local,
        Scope,
        Property
    }
}
