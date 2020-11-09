// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Syntax;
using System.IO;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.FileSystem;

namespace Bicep.Decompiler
{
    public static class Decompiler
    {
        public static Uri DecompileFileWithModules(string filePath)
        {
            var decompiled = new Dictionary<Uri, ProgramSyntax>();
            var decompileQueue = new Queue<Uri>();

            var entryFile = Path.ChangeExtension(filePath, "bicep");
            var entryUri = PathHelper.FilePathToFileUrl(entryFile);

            decompileQueue.Enqueue(entryUri);

            while (decompileQueue.Any())
            {
                var bicepUri = decompileQueue.Dequeue();
                var jsonFile = Path.ChangeExtension(bicepUri.LocalPath, "json");

                if (decompiled.ContainsKey(bicepUri))
                {
                    continue;
                }

                var jsonInput = File.ReadAllText(jsonFile);
                var program = TemplateConverter.DecompileTemplate(jsonFile, jsonInput);
                decompiled[bicepUri] = program;

                foreach (var module in program.Children.OfType<ModuleDeclarationSyntax>())
                {
                    var moduleRelativePath = SyntaxHelper.TryGetModulePath(module, out _);

                    if (moduleRelativePath == null || !Uri.TryCreate(bicepUri, moduleRelativePath, out var moduleUri))
                    {
                        throw new ArgumentException($"Failed to resolve {moduleRelativePath} relative to {bicepUri}");
                    }

                    if (!decompiled.ContainsKey(moduleUri))
                    {
                        decompileQueue.Enqueue(moduleUri);
                    }                    
                }
            }

            foreach (var (fileUri, program) in decompiled)
            {
                var bicepOutput = PrettyPrinter.PrintProgram(program, new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, false));

                if (bicepOutput == null)
                {
                    throw new ArgumentException($"Failed to pring bicep file {fileUri}");
                }

                File.WriteAllText(fileUri.LocalPath, bicepOutput);
            }

            return entryUri;
        }
    }
}