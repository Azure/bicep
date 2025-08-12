// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Semantics;

[Flags]
public enum FunctionParameterFlags
{
    None = 0,

    /// <summary>
    /// An argument for this parameter must be supplied when the function is called.
    /// </summary>
    Required = 1,

    /// <summary>
    /// The argument supplied for this parameter must be a compile-time constant value.
    /// </summary>
    Constant = 1 << 1,
}
