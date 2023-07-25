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
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Providers
{
    /// <summary>
    /// Creates compilation contexts.
    /// </summary>
    /// <remarks>This class exists only so we can mock fatal exceptions in tests.</remarks>
    public class BicepCompilationProvider : ICompilationProvider
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IBicepAnalyzer bicepAnalyzer;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly INamespaceProvider namespaceProvider;
        private readonly IFileResolver fileResolver;
        private readonly IArtifactDispatcher artifactDispatcher;

        public BicepCompilationProvider(
            IFeatureProviderFactory featureProviderFactory,
            INamespaceProvider namespaceProvider,
            IFileResolver fileResolver,
            IArtifactDispatcher artifactDispatcher,
            IConfigurationManager configurationManager,
            IBicepAnalyzer bicepAnalyzer)
        {
            this.featureProviderFactory = featureProviderFactory;
            this.namespaceProvider = namespaceProvider;
            this.fileResolver = fileResolver;
            this.artifactDispatcher = artifactDispatcher;
            this.configurationManager = configurationManager;
            this.bicepAnalyzer = bicepAnalyzer;
        }

        public CompilationContext Create(
            IReadOnlyWorkspace workspace,
            DocumentUri documentUri,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(
                fileResolver,
                artifactDispatcher,
                workspace,
                documentUri.ToUri());
            return this.CreateContext(sourceFileGrouping, modelLookup);
        }

        public CompilationContext Update(
            IReadOnlyWorkspace workspace,
            CompilationContext current,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(
                artifactDispatcher,
                workspace,
                current.Compilation.SourceFileGrouping);
            return this.CreateContext(sourceFileGrouping, modelLookup);
        }

        private CompilationContext CreateContext(
            SourceFileGrouping syntaxTreeGrouping,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var compilation = new Compilation(
                featureProviderFactory,
                namespaceProvider,
                syntaxTreeGrouping,
                configurationManager,
                bicepAnalyzer,
                modelLookup);
            return new CompilationContext(compilation);
        }
    }
}
