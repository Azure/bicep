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
        Assert,
        Module,
        Test,
        Output,
        Namespace,
        ImportedNamespace,
        Function,
        Local,
        Scope,
        Property,
        ParameterAssignment,
        Metadata,
        TypeAlias,
        ExtensionConfigAssignment,
        BaseParameters
    }
}
