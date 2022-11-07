// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
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
        private readonly IApiVersionProviderFactory apiVersionProviderFactory;
        private readonly INamespaceProvider namespaceProvider;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;

        public BicepCompilationProvider(IFeatureProviderFactory featureProviderFactory, INamespaceProvider namespaceProvider, IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IApiVersionProviderFactory apiVersionProviderFactory, IConfigurationManager configurationManager, IBicepAnalyzer bicepAnalyzer)
        {
            this.featureProviderFactory = featureProviderFactory;
            this.apiVersionProviderFactory = apiVersionProviderFactory;
            this.namespaceProvider = namespaceProvider;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
            this.bicepAnalyzer = bicepAnalyzer;
        }

        public CompilationContext Create(IReadOnlyWorkspace workspace, DocumentUri documentUri, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(fileResolver, moduleDispatcher, workspace, documentUri.ToUri());
            return this.CreateContext(sourceFileGrouping, modelLookup);
        }

        public CompilationContext Update(IReadOnlyWorkspace workspace, CompilationContext current, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(moduleDispatcher, workspace, current.Compilation.SourceFileGrouping);
            return this.CreateContext(sourceFileGrouping, modelLookup);
        }

        private CompilationContext CreateContext(SourceFileGrouping syntaxTreeGrouping, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var compilation = new Compilation(featureProviderFactory, namespaceProvider, syntaxTreeGrouping, configurationManager, apiVersionProviderFactory, bicepAnalyzer, modelLookup);
            return new CompilationContext(compilation);
        }
    }
}
