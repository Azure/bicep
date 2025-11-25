// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Bicep.Types.Concrete;

namespace Bicep.Local.Extension.Types;
public class TypeDefinitionBuilderOptions(FrozenDictionary<Type, Func<TypeBase>> typeToTypeBaseMap, Type? configurationType = null)
{
    /// <summary>
    /// Configuration type for custom extension configuration.
    /// </summary>
    public Type? ConfigurationType => configurationType;

    /// <summary>
    /// .net type to Bicep TypeBase mapping functions.
    /// </summary>
    public FrozenDictionary<Type, Func<TypeBase>> TypeToTypeBaseMap => typeToTypeBaseMap;

}
