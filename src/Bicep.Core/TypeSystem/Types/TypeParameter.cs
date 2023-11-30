// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.TypeSystem.Types;

public class TypeParameter
{
    public TypeParameter(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString() => Name;
}
