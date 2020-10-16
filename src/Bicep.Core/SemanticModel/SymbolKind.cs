// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.SemanticModel
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
        Function
    }
}
