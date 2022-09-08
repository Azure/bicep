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
using Bicep.Core.Analyzers.Linter.ApiVersions;

namespace Bicep.Core.Semantics
{
    public class Compilation
    {
        private readonly ImmutableDictionary<ISourceFile, Lazy<ISemanticModel>> lazySemanticModelLookup;
        private readonly IConfigurationManager configurationManager;
        private readonly INamespaceProviderManager namespaceProviderManager;
        private readonly IApiVersionProviderManager apiVersionProviderManager;
        private readonly IFeatureProviderManager featureProviderManager;
        private readonly IBicepAnalyzer linterAnalyzer;

        public Compilation(
            IFeatureProviderManager featureProviderManager,
            INamespaceProviderManager namespaceProviderManager,
            SourceFileGrouping sourceFileGrouping,
            IConfigurationManager configurationManager,
            IApiVersionProviderManager apiVersionProviderManager,
            IBicepAnalyzer linterAnalyzer,
            ImmutableDictionary<ISourceFile, ISemanticModel>? modelLookup = null)
        {
            this.featureProviderManager = featureProviderManager;
            this.SourceFileGrouping = sourceFileGrouping;
            this.namespaceProviderManager = namespaceProviderManager;
            this.configurationManager = configurationManager;
            this.apiVersionProviderManager = apiVersionProviderManager;
            this.linterAnalyzer = linterAnalyzer;

            var fileResolver = SourceFileGrouping.FileResolver;

            this.lazySemanticModelLookup = sourceFileGrouping.SourceFiles.ToImmutableDictionary(
                sourceFile => sourceFile,
                sourceFile => (modelLookup is not null && modelLookup.TryGetValue(sourceFile, out var existingModel)) ?
                    new(existingModel) :
                    new Lazy<ISemanticModel>(() => sourceFile switch
                    {
                        BicepFile bicepFile => CreateSemanticModel(bicepFile),
                        ArmTemplateFile armTemplateFile => new ArmTemplateSemanticModel(armTemplateFile),
                        TemplateSpecFile templateSpecFile => new TemplateSpecSemanticModel(templateSpecFile),
                        _ => throw new ArgumentOutOfRangeException(nameof(sourceFile)),
                    }));
        }

        public SourceFileGrouping SourceFileGrouping { get; }

        public SemanticModel GetEntrypointSemanticModel()
            => GetSemanticModel(SourceFileGrouping.EntryPoint);

        public SemanticModel GetSemanticModel(BicepFile bicepFile)
            => this.GetSemanticModel<SemanticModel>(bicepFile);

        public ArmTemplateSemanticModel GetSemanticModel(ArmTemplateFile armTemplateFile)
            => this.GetSemanticModel<ArmTemplateSemanticModel>(armTemplateFile);

        public ISemanticModel GetSemanticModel(ISourceFile sourceFile)
            => this.lazySemanticModelLookup[sourceFile].Value;

        public ImmutableDictionary<BicepFile, ImmutableArray<IDiagnostic>> GetAllDiagnosticsByBicepFile()
            => SourceFileGrouping.SourceFiles.OfType<BicepFile>().ToImmutableDictionary(
                bicepFile => bicepFile,
                bicepFile => this.GetSemanticModel(bicepFile).GetAllDiagnostics());

        private T GetSemanticModel<T>(ISourceFile sourceFile) where T : class, ISemanticModel =>
            this.GetSemanticModel(sourceFile) as T ??
            throw new ArgumentException($"Expected the semantic model type to be \"{typeof(T).Name}\".");

        private SemanticModel CreateSemanticModel(BicepFile bicepFile) => new SemanticModel(this,
            bicepFile,
            SourceFileGrouping.FileResolver,
            linterAnalyzer,
            configurationManager.GetConfiguration(bicepFile.FileUri),
            featureProviderManager.GetFeatureProvider(bicepFile.FileUri),
            namespaceProviderManager.GetNamespaceProvider(bicepFile.FileUri),
            apiVersionProviderManager.GetApiVersionProvider(bicepFile.FileUri));
    }
}
