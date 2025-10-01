// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.IO.Abstraction;

namespace Bicep.RegistryModuleTool.Extensions
{
    internal static class BicepCompilerExtensions
    {
        public static async Task<Compilation> CompileAsync(this BicepCompiler compiler, string path, IConsole console, string? skipWritingDiagnosticsPath = null)
        {
            var hasError = false;
            var uri = IOUri.FromFilePath(path);
            var compilation = await compiler.CreateCompilation(uri);

            foreach (var (file, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                foreach (var diagnostic in diagnostics)
                {
                    if (diagnostic.IsError())
                    {
                        hasError = true;
                    }

                    if (!file.FileHandle.Uri.GetFilePath().Equals(skipWritingDiagnosticsPath, StringComparison.Ordinal))
                    {
                        console.WriteDiagnostic(file, diagnostic);
                    }
                }
            }

            if (hasError)
            {
                throw new BicepException(@$"Failed to build ""{path}"".");
            }

            return compilation;
        }
    }
}
