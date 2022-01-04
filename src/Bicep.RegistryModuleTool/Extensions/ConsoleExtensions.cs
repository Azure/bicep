// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Terminal;
using System;
using System.CommandLine;
using System.CommandLine.Rendering;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class ConsoleExtensions
    {
        public static ForegroundColorScope YellowForegroundColorScope(this IConsole console) =>
            new(console.GetTerminal(preferVirtualTerminal: false), ConsoleColor.Yellow);

        public static ForegroundColorScope RedForegroundColorScope(this IConsole console) =>
            new(console.GetTerminal(preferVirtualTerminal: false), ConsoleColor.Red);
    }
}
