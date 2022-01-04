// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.CommandLine.Rendering;

namespace Bicep.RegistryModuleTool.Terminal
{
    public sealed class ForegroundColorScope : IDisposable
    {
        private readonly ITerminal? terminal;

        private readonly ConsoleColor originalForegroundColor;

        public ForegroundColorScope(ITerminal? terminal, ConsoleColor color)
        {
            this.terminal = terminal;
            this.originalForegroundColor = terminal?.ForegroundColor ?? Console.ForegroundColor;

            if (terminal is not null)
            {
                terminal.ForegroundColor = color;
            }
        }

        public void Dispose()
        {
            if (this.terminal is not null)
            {
                this.terminal.ForegroundColor = this.originalForegroundColor;
            }
        }
    }
}
