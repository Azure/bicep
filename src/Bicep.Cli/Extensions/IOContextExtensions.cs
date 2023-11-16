// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Cli.Extensions
{
    public static class IOContextExtensions
    {
        public static void WriteParameterDeprecationWarning(this IOContext io, string deprecatingParameter, string newParameter)
        {
            io.Error.Write($"DEPRECATED: The parameter {deprecatingParameter} is deprecated and will be removed in a future version of Bicpe CLI. Use {newParameter} instead.");
        }
    }
}
