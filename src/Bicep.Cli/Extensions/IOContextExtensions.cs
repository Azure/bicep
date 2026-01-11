// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Extensions
{
    public static class IOContextExtensions
    {
        public static void WriteCommandDeprecationWarning(this IOContext io, string deprecatingCommand, string newCommand)
        {
            io.Error.Writer.WriteLine($"DEPRECATED: The command {deprecatingCommand} is deprecated and will be removed in a future version of Bicep CLI. Use {newCommand} instead.");
        }

        public static void WriteParameterDeprecationWarning(this IOContext io, string deprecatingParameter, string newParameter)
        {
            io.Error.Writer.WriteLine($"DEPRECATED: The parameter {deprecatingParameter} is deprecated and will be removed in a future version of Bicep CLI. Use {newParameter} instead.");
        }
    }
}
