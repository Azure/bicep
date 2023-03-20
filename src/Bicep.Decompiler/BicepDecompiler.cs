// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Decompiler.Rewriters;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Rewriters;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Bicep.Core.Diagnostics;

namespace Bicep.Decompiler;

public class BicepDecompiler
{
    private readonly BicepCompiler bicepCompiler;
    private readonly IFileResolver fileResolver;

    public static string DecompilerDisclaimerMessage => DecompilerResources.DecompilerDisclaimerMessage;

    public BicepDecompiler(BicepCompiler bicepCompiler, IFileResolver fileResolver)
    {
        this.bicepCompiler = bicepCompiler;
        this.fileResolver = fileResolver;
    }

    public async Task<DecompileResult> Decompile(Uri entryJsonUri, Uri entryBicepUri, DecompileOptions? options = null)
    {
        var workspace = new Workspace();
        var decompileQueue = new Queue<(Uri, Uri)>();
        options ??= new DecompileOptions();

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

            var (program, jsonTemplateUrisByModule) = TemplateConverter.DecompileTemplate(workspace, fileResolver, bicepUri, jsonInput, options);
            var bicepFile = SourceFileFactory.CreateBicepFile(bicepUri, program.ToText());
            workspace.UpsertSourceFile(bicepFile);

            foreach (var module in program.Children.OfType<ModuleDeclarationSyntax>())
            {
                var moduleRelativePath = SyntaxHelper.TryGetModulePath(module, out _);
                if (moduleRelativePath == null ||
                    !LocalModuleReference.Validate(moduleRelativePath, out _) ||
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

        await RewriteSyntax(workspace, entryBicepUri, semanticModel => new ParentChildResourceNameRewriter(semanticModel));
        await RewriteSyntax(workspace, entryBicepUri, semanticModel => new DependsOnRemovalRewriter(semanticModel));
        await RewriteSyntax(workspace, entryBicepUri, semanticModel => new ForExpressionSimplifierRewriter(semanticModel));
        for (var i = 0; i < 5; i++)
        {
            // This is a little weird. If there are casing issues nested inside casing issues (e.g. in an object), then the inner casing issue will have no type information
            // available, as the compilation will not have associated a type with it (since there was no match on the outer object). So we need to correct the outer issue first,
            // and then move to the inner one. We need to recompute the entire compilation to do this. It feels simpler to just do this in passes over the file, rather than on demand.
            if (!await RewriteSyntax(workspace, entryBicepUri, semanticModel => new TypeCasingFixerRewriter(semanticModel)))
            {
                break;
            }
        }

        return new DecompileResult(
            entryBicepUri,
            PrintFiles(workspace));
    }

    public string? DecompileJsonValue(string jsonInput, DecompileOptions? options = null)
    {
        var workspace = new Workspace();
        options ??= new DecompileOptions();

        var bicepUri = new Uri("file://jsonInput.json", UriKind.Absolute);
        try
        {
            var syntax = TemplateConverter.DecompileJsonValue(workspace, fileResolver, bicepUri, jsonInput, options);
            return syntax is null ? null : PrintSyntax(syntax);
        }
        catch (Exception)
        {
            return null;
        }
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

            filesToSave[fileUri] = PrettyPrinter.PrintProgram(bicepFile.ProgramSyntax, GetPrettyPrintOptions());
        }

        return filesToSave.ToImmutableDictionary();
    }

    private static string PrintSyntax(SyntaxBase syntax)
    {
        return PrettyPrinter.PrintSyntax(syntax, GetPrettyPrintOptions(), new DiagnosticTree(), new DiagnosticTree());
    }

    private static PrettyPrintOptions GetPrettyPrintOptions() => new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, false);

    private async Task<bool> RewriteSyntax(Workspace workspace, Uri entryUri, Func<SemanticModel, SyntaxRewriteVisitor> rewriteVisitorBuilder)
    {
        var hasChanges = false;
        var compilation = await bicepCompiler.CreateCompilation(entryUri, skipRestore: true, workspace);

        // force enumeration here with .ToImmutableArray() as we're going to be modifying the sourceFileGrouping collection as we iterate
        var sourceFiles = compilation.SourceFileGrouping.SourceFiles.ToImmutableArray();
        foreach (var sourceFile in sourceFiles)
        {
            var bicepFile = sourceFile as BicepFile ??
                throw new InvalidOperationException($"Failed to find a bicep source file for URI {sourceFile.FileUri}.");

            var newProgramSyntax = rewriteVisitorBuilder(compilation.GetSemanticModel(bicepFile)).Rewrite(bicepFile.ProgramSyntax);

            if (!object.ReferenceEquals(bicepFile.ProgramSyntax, newProgramSyntax))
            {
                hasChanges = true;
                var newFile = SourceFileFactory.CreateBicepFile(sourceFile.FileUri, newProgramSyntax.ToText());
                workspace.UpsertSourceFile(newFile);

                compilation = await bicepCompiler.CreateCompilation(entryUri, skipRestore: true, workspace);
            }
        }

        return hasChanges;
    }
}
