// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
