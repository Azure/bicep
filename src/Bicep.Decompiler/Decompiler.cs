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
using System.Collections.Immutable;

namespace Bicep.Decompiler
{
    public static class Decompiler
    {
        private static Uri ChangeExtension(Uri prevUri, string newExtension)
        {
            var uriBuilder = new UriBuilder(prevUri);
            var finalDot = uriBuilder.Path.LastIndexOf('.');
            uriBuilder.Path = (finalDot >= 0 ? uriBuilder.Path.Substring(0, finalDot) : uriBuilder.Path) + $".{newExtension}";

            return uriBuilder.Uri;
        }

        public static (Uri entrypointUri, ImmutableDictionary<Uri, string> filesToSave) DecompileFileWithModules(IFileResolver fileResolver, Uri jsonUri)
        {
            var decompiled = new Dictionary<Uri, ProgramSyntax>();
            var decompileQueue = new Queue<Uri>();

            var entryUri = ChangeExtension(jsonUri, "bicep");

            decompileQueue.Enqueue(entryUri);

            while (decompileQueue.Any())
            {
                var bicepUri = decompileQueue.Dequeue();
                if (!bicepUri.LocalPath.EndsWith(".bicep", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var currentJsonUri = ChangeExtension(bicepUri, "json");

                if (decompiled.ContainsKey(bicepUri))
                {
                    continue;
                }

                if (!fileResolver.TryRead(currentJsonUri, out var jsonInput, out _))
                {
                    throw new InvalidOperationException($"Failed to read {currentJsonUri}");
                }

                var program = TemplateConverter.DecompileTemplate(fileResolver, currentJsonUri, jsonInput);
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

            var filesToSave = new Dictionary<Uri, string>();
            foreach (var (fileUri, program) in decompiled)
            {
                var bicepOutput = PrettyPrinter.PrintProgram(program, new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, false));

                if (bicepOutput == null)
                {
                    throw new ArgumentException($"Failed to pring bicep file {fileUri}");
                }

                filesToSave[fileUri] = bicepOutput;
            }

            return (entryUri, filesToSave.ToImmutableDictionary());
        }
    }
}