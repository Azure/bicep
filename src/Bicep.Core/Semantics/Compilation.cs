// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class Compilation
    {
        // Stores semantic model for each source file (map exists for all source files, but semantic model created only when indexed)
        private readonly ImmutableDictionary<ISourceFile, Lazy<ISemanticModel>> lazySemanticModelLookup;
        private readonly IConfigurationManager configurationManager;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IBicepAnalyzer linterAnalyzer;

        public Compilation(
            IFeatureProviderFactory featureProviderFactory, 
            INamespaceProvider namespaceProvider, 
            SourceFileGrouping sourceFileGrouping, 
            IConfigurationManager configurationManager, 
            IBicepAnalyzer linterAnalyzer, 
            ImmutableDictionary<ISourceFile, ISemanticModel>? modelLookup = null)
        {
            this.featureProviderFactory = featureProviderFactory;
            this.SourceFileGrouping = sourceFileGrouping;
            this.NamespaceProvider = namespaceProvider;
            this.configurationManager = configurationManager;
            this.linterAnalyzer = linterAnalyzer;

            this.lazySemanticModelLookup = sourceFileGrouping.SourceFiles.ToImmutableDictionary(
                sourceFile => sourceFile,
                sourceFile => (modelLookup is not null && modelLookup.TryGetValue(sourceFile, out var existingModel)) ?
                    new(existingModel) :
                    new Lazy<ISemanticModel>(() => sourceFile switch // semantic model doesn't yet exist for file, create it
                    {
                        BicepFile bicepFile => CreateSemanticModel(bicepFile),
                        BicepParamFile bicepParamFile => CreateSemanticModel(bicepParamFile),
                        ArmTemplateFile armTemplateFile => new ArmTemplateSemanticModel(armTemplateFile),
                        TemplateSpecFile templateSpecFile => new TemplateSpecSemanticModel(templateSpecFile),
                        _ => throw new ArgumentOutOfRangeException(nameof(sourceFile)),
                    }));
        }

        public SourceFileGrouping SourceFileGrouping { get; }

        public INamespaceProvider NamespaceProvider { get; }

        public SemanticModel GetEntrypointSemanticModel()
            // entry point semantic models are guaranteed to cast successfully
            => GetSemanticModel(SourceFileGrouping.EntryPoint);

        public SemanticModel GetSemanticModel(BicepSourceFile bicepFile)
            => this.GetSemanticModel<SemanticModel>(bicepFile);

        public ArmTemplateSemanticModel GetSemanticModel(ArmTemplateFile armTemplateFile)
            => this.GetSemanticModel<ArmTemplateSemanticModel>(armTemplateFile);

        public ISemanticModel GetSemanticModel(ISourceFile sourceFile)
            => this.lazySemanticModelLookup[sourceFile].Value;

        public ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> GetAllDiagnosticsByBicepFile()
            => SourceFileGrouping.SourceFiles.OfType<BicepSourceFile>().ToImmutableDictionary(
                bicepFile => bicepFile,
                bicepFile => this.GetSemanticModel(bicepFile) is SemanticModel semanticModel ? semanticModel.GetAllDiagnostics() : ImmutableArray<IDiagnostic>.Empty);

        private T GetSemanticModel<T>(ISourceFile sourceFile) where T : class, ISemanticModel =>
            this.GetSemanticModel(sourceFile) as T ??
            throw new ArgumentException($"Expected the semantic model type to be \"{typeof(T).Name}\".");

        private SemanticModel CreateSemanticModel(BicepSourceFile bicepFile) => new SemanticModel(this,
            bicepFile,
            SourceFileGrouping.FileResolver,
            linterAnalyzer,
            configurationManager.GetConfiguration(bicepFile.FileUri),
            featureProviderFactory.GetFeatureProvider(bicepFile.FileUri));
    }
}
