// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.Parsing
{
    [Flags]
    public enum ExpressionFlags
    {
        None = 0,
        AllowComplexLiterals = 1 << 0,
        AllowResourceDeclarations = 1 << 1,
    }
}
