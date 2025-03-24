// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Utils;
using Bicep.LanguageServer.CompilationManager;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Providers
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
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly ISourceFileFactory sourceFileFactory;

        public BicepCompilationProvider(
            IEnvironment environment,
            INamespaceProvider namespaceProvider,
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            IBicepAnalyzer bicepAnalyzer,
            ISourceFileFactory sourceFileFactory)
        {
            this.environment = environment;
            this.namespaceProvider = namespaceProvider;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.bicepAnalyzer = bicepAnalyzer;
            this.sourceFileFactory = sourceFileFactory;
        }

        public CompilationContext Create(
            IReadOnlyWorkspace workspace,
            DocumentUri documentUri,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(
                fileResolver,
                moduleDispatcher,
                workspace,
                sourceFileFactory,
                documentUri.ToUriEncoded());
            return this.CreateContext(sourceFileGrouping, modelLookup);
        }

        public CompilationContext Update(
            IReadOnlyWorkspace workspace,
            CompilationContext current,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(
                fileResolver,
                moduleDispatcher,
                workspace,
                sourceFileFactory,
                current.Compilation.SourceFileGrouping);
            return this.CreateContext(sourceFileGrouping, modelLookup);
        }

        private CompilationContext CreateContext(
            SourceFileGrouping syntaxTreeGrouping,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var compilation = new Compilation(
                environment,
                namespaceProvider,
                syntaxTreeGrouping,
                bicepAnalyzer,
                moduleDispatcher,
                sourceFileFactory,
                modelLookup);

            return new(compilation);
        }
    }
}
