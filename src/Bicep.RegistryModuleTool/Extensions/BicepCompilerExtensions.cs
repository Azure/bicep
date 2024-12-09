// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;

namespace Bicep.RegistryModuleTool.Extensions
{
    internal static class BicepCompilerExtensions
    {
        public static async Task<Compilation> CompileAsync(this BicepCompiler compiler, string path, IConsole console, string? skipWritingDiagnosticsPath = null)
        {
            var hasError = false;
            var uri = PathHelper.FilePathToFileUrl(path);
            var compilation = await compiler.CreateCompilation(uri);

            foreach (var (file, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                foreach (var diagnostic in diagnostics)
                {
                    if (diagnostic.IsError())
                    {
                        hasError = true;
                    }

                    if (!file.Uri.LocalPath.Equals(skipWritingDiagnosticsPath, StringComparison.Ordinal))
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
