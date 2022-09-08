// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Analyzers.Linter;
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
        private readonly IFeatureProviderManager featureProviderManager;
        private readonly IApiVersionProviderManager apiVersionProviderManager;
        private readonly INamespaceProviderManager namespaceProviderManager;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;

        public BicepCompilationProvider(IFeatureProviderManager featureProviderManager, INamespaceProviderManager namespaceProviderManager, IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IApiVersionProviderManager apiVersionProviderManager, IConfigurationManager configurationManager)
        {
            this.featureProviderManager = featureProviderManager;
            this.apiVersionProviderManager = apiVersionProviderManager;
            this.namespaceProviderManager = namespaceProviderManager;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
        }

        public CompilationContext Create(IReadOnlyWorkspace workspace, DocumentUri documentUri, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup, LinterAnalyzer linterAnalyzer)
        {
            var syntaxTreeGrouping = SourceFileGroupingBuilder.Build(fileResolver, moduleDispatcher, workspace, documentUri.ToUri());
            return this.CreateContext(syntaxTreeGrouping, modelLookup, linterAnalyzer);
        }

        public CompilationContext Update(IReadOnlyWorkspace workspace, CompilationContext current, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup, LinterAnalyzer linterAnalyzer)
        {
            var syntaxTreeGrouping = SourceFileGroupingBuilder.Rebuild(moduleDispatcher, workspace, current.Compilation.SourceFileGrouping);
            return this.CreateContext(syntaxTreeGrouping, modelLookup, linterAnalyzer);
        }

        private CompilationContext CreateContext(SourceFileGrouping syntaxTreeGrouping, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup, LinterAnalyzer linterAnalyzer)
        {
            var compilation = new Compilation(featureProviderManager, namespaceProviderManager, syntaxTreeGrouping, configurationManager, apiVersionProviderManager, linterAnalyzer, modelLookup);
            return new CompilationContext(compilation);
        }
    }
}
