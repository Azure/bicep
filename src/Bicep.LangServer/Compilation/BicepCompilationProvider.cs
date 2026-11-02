// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol;
using BicepCompilation = Bicep.Core.Semantics.Compilation;

namespace Bicep.LanguageServer.Compilation
{
    /// <summary>
    /// Creates compilation contexts.
    /// </summary>
    /// <remarks>This class exists only so we can mock fatal exceptions in tests.</remarks>
    public class BicepCompilationProvider : ICompilationProvider
    {
        private readonly IBicepAnalyzer bicepAnalyzer;
        private readonly IEnvironment environment;
        private readonly INamespaceProvider namespaceProvider;
        private readonly IFileExplorer fileExplorer;
        private readonly IArtifactReferenceResolver referenceResolver;
        private readonly IArtifactRestorer restorer;
        private readonly ISourceFileFactory sourceFileFactory;
        private readonly CompilerVersionCheckOptions compilerVersionCheckOptions;

        public BicepCompilationProvider(
            IEnvironment environment,
            INamespaceProvider namespaceProvider,
            IFileExplorer fileExplorer,
            IArtifactReferenceResolver referenceResolver,
            IArtifactRestorer restorer,
            IBicepAnalyzer bicepAnalyzer,
            ISourceFileFactory sourceFileFactory,
            CompilerVersionCheckOptions compilerVersionCheckOptions)
        {
            this.environment = environment;
            this.namespaceProvider = namespaceProvider;
            this.fileExplorer = fileExplorer;
            this.referenceResolver = referenceResolver;
            this.restorer = restorer;
            this.bicepAnalyzer = bicepAnalyzer;
            this.sourceFileFactory = sourceFileFactory;
            this.compilerVersionCheckOptions = compilerVersionCheckOptions;
        }

        public CompilationContext Create(
            IActiveSourceFileLookup workspace,
            DocumentUri documentUri,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(
                fileExplorer,
                referenceResolver,
                restorer,
                workspace,
                sourceFileFactory,
                documentUri.ToIOUri());
            return this.CreateContext(sourceFileGrouping, modelLookup);
        }

        public CompilationContext Update(
            IActiveSourceFileLookup workspace,
            CompilationContext current,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(
                fileExplorer,
                referenceResolver,
                restorer,
                workspace,
                sourceFileFactory,
                current.Compilation.SourceFileGrouping);
            return this.CreateContext(sourceFileGrouping, modelLookup);
        }

        private CompilationContext CreateContext(
            SourceFileGrouping syntaxTreeGrouping,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var compilation = new BicepCompilation(
                environment,
                namespaceProvider,
                syntaxTreeGrouping,
                bicepAnalyzer,
                sourceFileFactory,
                modelLookup,
                compilerVersionCheckOptions);

            return new(compilation);
        }
    }
}
