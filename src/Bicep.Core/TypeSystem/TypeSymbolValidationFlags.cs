// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem
{
    [Flags]
    public enum TypeSymbolValidationFlags
    {
        Default = 0,

        Permissive = 1 << 0,
    }
}
