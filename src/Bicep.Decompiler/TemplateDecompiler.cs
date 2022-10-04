// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Decompiler.Rewriters;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Registry;
using Bicep.Core.Rewriters;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Decompiler
{
    public class TemplateDecompiler
    {
        private readonly INamespaceProvider namespaceProvider;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IFileResolver fileResolver;
        private readonly IModuleRegistryProvider registryProvider;
        private readonly IApiVersionProviderFactory apiVersionProviderFactory;
        private readonly IBicepAnalyzer bicepAnalyzer;

        public TemplateDecompiler(IFeatureProviderFactory featureProviderFactory, INamespaceProvider namespaceProvider, IFileResolver fileResolver, IModuleRegistryProvider registryProvider, IApiVersionProviderFactory apiVersionProviderFactory, IBicepAnalyzer bicepAnalyzer)
        {
            this.featureProviderFactory = featureProviderFactory;
            this.namespaceProvider = namespaceProvider;
            this.fileResolver = fileResolver;
            this.registryProvider = registryProvider;
            this.apiVersionProviderFactory = apiVersionProviderFactory;
            this.bicepAnalyzer = bicepAnalyzer;
        }

        public (Uri entrypointUri, ImmutableDictionary<Uri, string> filesToSave) DecompileFileWithModules(Uri entryJsonUri, Uri entryBicepUri)
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

            RewriteSyntax(workspace, entryBicepUri, semanticModel => new ParentChildResourceNameRewriter(semanticModel));
            RewriteSyntax(workspace, entryBicepUri, semanticModel => new DependsOnRemovalRewriter(semanticModel));
            RewriteSyntax(workspace, entryBicepUri, semanticModel => new ForExpressionSimplifierRewriter(semanticModel));
            for (var i = 0; i < 5; i++)
            {
                // This is a little weird. If there are casing issues nested inside casing issues (e.g. in an object), then the inner casing issue will have no type information
                // available, as the compilation will not have associated a type with it (since there was no match on the outer object). So we need to correct the outer issue first,
                // and then move to the inner one. We need to recompute the entire compilation to do this. It feels simpler to just do this in passes over the file, rather than on demand.
                if (!RewriteSyntax(workspace, entryBicepUri, semanticModel => new TypeCasingFixerRewriter(semanticModel)))
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

        private bool RewriteSyntax(Workspace workspace, Uri entryUri, Func<SemanticModel, SyntaxRewriteVisitor> rewriteVisitorBuilder)
        {
            var hasChanges = false;
            var configuration = IConfigurationManager.GetBuiltInConfiguration().WithAllAnalyzersDisabled();
            var configurationManager = IConfigurationManager.WithStaticConfiguration(configuration);
            var dispatcher = new ModuleDispatcher(this.registryProvider, configurationManager);
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(fileResolver, dispatcher, workspace, entryUri);
            var compilation = new Compilation(featureProviderFactory, namespaceProvider, sourceFileGrouping, configurationManager, apiVersionProviderFactory, bicepAnalyzer);

            // force enumeration here with .ToImmutableArray() as we're going to be modifying the sourceFileGrouping collection as we iterate
            var fileUris = sourceFileGrouping.SourceFiles.Select(x => x.FileUri).ToImmutableArray();
            foreach (var fileUri in fileUris)
            {
                if (sourceFileGrouping.SourceFiles.FirstOrDefault(x => x.FileUri == fileUri) is not BicepFile bicepFile)
                {
                    throw new InvalidOperationException($"Failed to find a bicep source file for URI {fileUri}.");
                }

                var newProgramSyntax = rewriteVisitorBuilder(compilation.GetSemanticModel(bicepFile)).Rewrite(bicepFile.ProgramSyntax);

                if (!object.ReferenceEquals(bicepFile.ProgramSyntax, newProgramSyntax))
                {
                    hasChanges = true;
                    var newFile = SourceFileFactory.CreateBicepFile(fileUri, newProgramSyntax.ToText());
                    workspace.UpsertSourceFile(newFile);

                    sourceFileGrouping = SourceFileGroupingBuilder.Build(fileResolver, dispatcher, workspace, entryUri);
                    compilation = new Compilation(featureProviderFactory, namespaceProvider, sourceFileGrouping, configurationManager, apiVersionProviderFactory, bicepAnalyzer);
                }
            }

            return hasChanges;
        }
    }
}
