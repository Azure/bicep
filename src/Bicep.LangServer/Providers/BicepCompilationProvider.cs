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
using Bicep.Core.Utils;
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
        private readonly IEnvironment environment;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly INamespaceProvider namespaceProvider;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;

        public BicepCompilationProvider(
            IFeatureProviderFactory featureProviderFactory,
            IEnvironment environment,
            INamespaceProvider namespaceProvider,
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager,
            IBicepAnalyzer bicepAnalyzer)
        {
            this.environment = environment;
            this.featureProviderFactory = featureProviderFactory;
            this.namespaceProvider = namespaceProvider;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
            this.bicepAnalyzer = bicepAnalyzer;
        }

        public CompilationContext Create(
            IReadOnlyWorkspace workspace,
            IReadableFileCache fileCache,
            DocumentUri documentUri,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(
                fileResolver,
                moduleDispatcher,
                configurationManager,
                workspace,
                documentUri.ToUriEncoded(),
                featureProviderFactory);
            return this.CreateContext(fileCache, sourceFileGrouping, modelLookup);
        }

        public CompilationContext Update(
            IReadOnlyWorkspace workspace,
            IReadableFileCache fileCache,
            CompilationContext current,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(
                fileResolver,
                featureProviderFactory,
                moduleDispatcher,
                configurationManager,
                workspace,
                current.Compilation.SourceFileGrouping);
            return this.CreateContext(fileCache, sourceFileGrouping, modelLookup);
        }

        private CompilationContext CreateContext(
            IReadableFileCache fileCache,
            SourceFileGrouping syntaxTreeGrouping,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var compilation = new Compilation(
                featureProviderFactory,
                environment,
                namespaceProvider,
                syntaxTreeGrouping,
                configurationManager,
                bicepAnalyzer,
                moduleDispatcher,
                fileCache,
                modelLookup);

            return new(compilation);
        }
    }
}
