// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.TypeSystem.Types;

public record TypeParameter(string Name, string? Description, TypeSymbol? Type = null, bool Required = true)
{
    public override string ToString() => Name;
}
