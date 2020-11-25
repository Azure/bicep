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
using Bicep.Core.Workspaces;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Decompiler.Rewriters;

namespace Bicep.Decompiler
{
    public static class TemplateDecompiler
    {
        private static Uri ChangeExtension(Uri prevUri, string newExtension)
        {
            var uriString = prevUri.ToString();
            var finalDot = uriString.LastIndexOf('.');
            uriString = (finalDot >= 0 ? uriString.Substring(0, finalDot) : uriString) + $".{newExtension}";

            return new Uri(uriString);
        }

        public static (Uri entrypointUri, ImmutableDictionary<Uri, string> filesToSave) DecompileFileWithModules(IResourceTypeProvider resourceTypeProvider, IFileResolver fileResolver, Uri jsonUri)
        {
            var decompileQueue = new Queue<Uri>();

            var entryUri = ChangeExtension(jsonUri, "bicep");
            var workspace = new Workspace();

            decompileQueue.Enqueue(entryUri);

            while (decompileQueue.Any())
            {
                var bicepUri = decompileQueue.Dequeue();
                if (!bicepUri.AbsolutePath.EndsWith(".bicep", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var currentJsonUri = ChangeExtension(bicepUri, "json");

                if (workspace.TryGetSyntaxTree(bicepUri, out _))
                {
                    continue;
                }

                if (!fileResolver.TryRead(currentJsonUri, out var jsonInput, out _))
                {
                    throw new InvalidOperationException($"Failed to read {currentJsonUri}");
                }

                var program = TemplateConverter.DecompileTemplate(fileResolver, currentJsonUri, jsonInput);
                var syntaxTree = new SyntaxTree(bicepUri, ImmutableArray<int>.Empty, program);
                workspace.UpsertSyntaxTrees(syntaxTree.AsEnumerable());

                foreach (var module in program.Children.OfType<ModuleDeclarationSyntax>())
                {
                    var moduleRelativePath = SyntaxHelper.TryGetModulePath(module, out _);
                    if (moduleRelativePath == null ||
                        !SyntaxTreeGroupingBuilder.ValidateModulePath(moduleRelativePath, out _) ||
                        !Uri.TryCreate(bicepUri, moduleRelativePath, out var moduleUri))
                    {
                        // Do our best, but keep going if we fail to resolve a module file
                        continue;
                    }

                    if (!workspace.TryGetSyntaxTree(moduleUri, out _))
                    {
                        decompileQueue.Enqueue(moduleUri);
                    }
                }
            }

            RewriteSyntax(resourceTypeProvider, workspace, entryUri, semanticModel => new ParentChildResourceNameRewriter(semanticModel));
            RewriteSyntax(resourceTypeProvider, workspace, entryUri, semanticModel => new DependsOnRemovalRewriter(semanticModel));
            for (var i = 0; i < 5; i++)
            {
                // This is a little weird. If there are casing issues nested inside casing issues (e.g. in an object), then the inner casing issue will have no type information
                // available, as the compilation will not have associated a type with it (since there was no match on the outer object). So we need to correct the outer issue first,
                // and then move to the inner one. We need to recompute the entire compilation to do this. It feels simpler to just do this in passes over the file, rather than on demand.
                if (!RewriteSyntax(resourceTypeProvider, workspace, entryUri, semanticModel => new TypeCasingFixerRewriter(semanticModel)))
                {
                    break;
                }
            }

            return (entryUri, PrintFiles(workspace));
        }

        private static ImmutableDictionary<Uri, string> PrintFiles(Workspace workspace)
        {
            var filesToSave = new Dictionary<Uri, string>();
            foreach (var (fileUri, syntaxTree) in workspace.GetActiveSyntaxTrees())
            {
                filesToSave[fileUri] = PrettyPrinter.PrintProgram(syntaxTree.ProgramSyntax, new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, false));
            }

            return filesToSave.ToImmutableDictionary();
        }

        private static bool RewriteSyntax(IResourceTypeProvider resourceTypeProvider, Workspace workspace, Uri entryUri, Func<SemanticModel, SyntaxRewriteVisitor> rewriteVisitorBuilder)
        {
            var hasChanges = false;
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), workspace, entryUri);
            var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

            foreach (var (fileUri, syntaxTree) in workspace.GetActiveSyntaxTrees())
            {
                var entryFile = syntaxTreeGrouping.EntryPoint;
                var entryModel = compilation.GetEntrypointSemanticModel();

                var newProgramSyntax = rewriteVisitorBuilder(compilation.GetSemanticModel(syntaxTree)).Rewrite(syntaxTree.ProgramSyntax);

                if (!object.ReferenceEquals(syntaxTree.ProgramSyntax, newProgramSyntax))
                {
                    hasChanges = true;
                    var newSyntaxTree = new SyntaxTree(fileUri, ImmutableArray<int>.Empty, newProgramSyntax);
                    workspace.UpsertSyntaxTrees(newSyntaxTree.AsEnumerable());

                    syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), workspace, entryUri);
                    compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);
                }
            }

            return hasChanges;
        }
    }
}