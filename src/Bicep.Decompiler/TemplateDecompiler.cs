// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Decompiler.Rewriters;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Rewriters;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Decompiler
{
    public static class TemplateDecompiler
    {
        public static (Uri entrypointUri, ImmutableDictionary<Uri, string> filesToSave) DecompileFileWithModules(IResourceTypeProvider resourceTypeProvider, IFileResolver fileResolver, Uri entryJsonUri, Uri entryBicepUri)
        {
            var workspace = new Workspace();
            var decompileQueue = new Queue<(Uri, Uri)>();

            decompileQueue.Enqueue((entryJsonUri, entryBicepUri));

            while (decompileQueue.Count > 0)
            {
                var (jsonUri, bicepUri) = decompileQueue.Dequeue();

                if (PathHelper.HasBicepExtension(jsonUri))
                {
                    throw new InvalidOperationException($"Cannot decompile the file with .bicep extension: {jsonUri}.");
                }

                if (workspace.TryGetSourceFile(bicepUri, out _))
                {
                    continue;
                }

                if (!fileResolver.TryRead(jsonUri, out var jsonInput, out _))
                {
                    throw new InvalidOperationException($"Failed to read {jsonUri}");
                }

                var (program, jsonTemplateUrisByModule) = TemplateConverter.DecompileTemplate(workspace, fileResolver, bicepUri, jsonInput);
                var bicepFile = new BicepFile(bicepUri, ImmutableArray<int>.Empty, program);
                workspace.UpsertSourceFile(bicepFile);

                foreach (var module in program.Children.OfType<ModuleDeclarationSyntax>())
                {
                    var moduleRelativePath = SyntaxHelper.TryGetModulePath(module, out _);
                    if (moduleRelativePath == null ||
                        !SourceFileGroupingBuilder.ValidateFilePath(moduleRelativePath, out _) ||
                        !Uri.TryCreate(bicepUri, moduleRelativePath, out var moduleUri))
                    {
                        // Do our best, but keep going if we fail to resolve a module file
                        continue;
                    }

                    if (!workspace.TryGetSourceFile(moduleUri, out _) && jsonTemplateUrisByModule.TryGetValue(module, out var linkedTemplateUri))
                    {
                        decompileQueue.Enqueue((linkedTemplateUri, moduleUri));
                    }
                }
            }

            RewriteSyntax(resourceTypeProvider, workspace, entryBicepUri, semanticModel => new ParentChildResourceNameRewriter(semanticModel));
            RewriteSyntax(resourceTypeProvider, workspace, entryBicepUri, semanticModel => new DependsOnRemovalRewriter(semanticModel));
            RewriteSyntax(resourceTypeProvider, workspace, entryBicepUri, semanticModel => new ForExpressionSimplifierRewriter(semanticModel));
            for (var i = 0; i < 5; i++)
            {
                // This is a little weird. If there are casing issues nested inside casing issues (e.g. in an object), then the inner casing issue will have no type information
                // available, as the compilation will not have associated a type with it (since there was no match on the outer object). So we need to correct the outer issue first,
                // and then move to the inner one. We need to recompute the entire compilation to do this. It feels simpler to just do this in passes over the file, rather than on demand.
                if (!RewriteSyntax(resourceTypeProvider, workspace, entryBicepUri, semanticModel => new TypeCasingFixerRewriter(semanticModel)))
                {
                    break;
                }
            }

            return (entryBicepUri, PrintFiles(workspace));
        }

        private static ImmutableDictionary<Uri, string> PrintFiles(Workspace workspace)
        {
            var filesToSave = new Dictionary<Uri, string>();
            foreach (var (fileUri, sourceFile) in workspace.GetActiveSourceFilesByUri())
            {
                if (sourceFile is not BicepFile bicepFile)
                {
                    continue;
                }

                filesToSave[fileUri] = PrettyPrinter.PrintProgram(bicepFile.ProgramSyntax, new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, false));
            }

            return filesToSave.ToImmutableDictionary();
        }

        private static bool RewriteSyntax(IResourceTypeProvider resourceTypeProvider, Workspace workspace, Uri entryUri, Func<SemanticModel, SyntaxRewriteVisitor> rewriteVisitorBuilder)
        {
            var hasChanges = false;
            var fileResolver = new FileResolver();
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(fileResolver, workspace, entryUri);
            var compilation = new Compilation(resourceTypeProvider, sourceFileGrouping);

            foreach (var (fileUri, sourceFile) in workspace.GetActiveSourceFilesByUri())
            {
                if (sourceFile is not BicepFile bicepFile)
                {
                    throw new InvalidOperationException("Expected a bicep source file.");
                }

                var newProgramSyntax = rewriteVisitorBuilder(compilation.GetSemanticModel(bicepFile)).Rewrite(bicepFile.ProgramSyntax);

                if (!object.ReferenceEquals(bicepFile.ProgramSyntax, newProgramSyntax))
                {
                    hasChanges = true;
                    var newFile = new BicepFile(fileUri, ImmutableArray<int>.Empty, newProgramSyntax);
                    workspace.UpsertSourceFile(newFile);

                    sourceFileGrouping = SourceFileGroupingBuilder.Build(fileResolver, workspace, entryUri);
                    compilation = new Compilation(resourceTypeProvider, sourceFileGrouping);
                }
            }

            return hasChanges;
        }
    }
}
