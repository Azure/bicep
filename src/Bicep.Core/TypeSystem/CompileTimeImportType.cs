// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem;

public sealed class CompileTimeImportType : ObjectType
{
    public CompileTimeImportType(string importPath, IEnumerable<TypeTypeProperty> importableTypes)
        : base(importPath, TypeSymbolValidationFlags.PreventAssignment, importableTypes, null, TypePropertyFlags.None, obj => new FunctionResolver(obj))
    {
        ImportableTypes = importableTypes.DistinctBy(p => p.Name).ToImmutableDictionary(p => p.Name);
    }

    public override TypeKind TypeKind => TypeKind.Import;

    public ImmutableDictionary<string, TypeTypeProperty> ImportableTypes { get; }
}
