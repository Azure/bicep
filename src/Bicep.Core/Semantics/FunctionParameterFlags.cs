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
    /// The argument supplied for this parameter must be a compile-time constant value. Expressions are not permitted.
    /// </summary>
    Constant = 1 << 1,

    /// <summary>
    /// The property only accepts deploy-time constants whose values must be known at the start of the deployment, and do not require inlining. Expressions are permitted, but they must be resolvable during template preprocessing.
    /// </summary>
    DeployTimeConstant = 1 << 2,
}
