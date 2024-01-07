// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class Compilation : ISemanticModelLookup
    {
        // Stores semantic model for each source file (map exists for all source files, but semantic model created only when indexed)
        private readonly ImmutableDictionary<ISourceFile, Lazy<ISemanticModel>> lazySemanticModelLookup;
        private readonly IConfigurationManager configurationManager;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IEnvironment environment;
        private readonly IBicepAnalyzer linterAnalyzer;

        public Compilation(
            IFeatureProviderFactory featureProviderFactory,
            IEnvironment environment,
            INamespaceProvider namespaceProvider,
            SourceFileGrouping sourceFileGrouping,
            IConfigurationManager configurationManager,
            IBicepAnalyzer linterAnalyzer,
            IArtifactReferenceFactory artifactReferenceFactory,
            AuxiliaryFileCache fileCache,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            this.featureProviderFactory = featureProviderFactory;
            this.environment = environment;
            this.SourceFileGrouping = sourceFileGrouping;
            this.NamespaceProvider = namespaceProvider;
            this.FileCache = fileCache;
            this.configurationManager = configurationManager;
            this.linterAnalyzer = linterAnalyzer;
            this.ArtifactReferenceFactory = artifactReferenceFactory;

            this.lazySemanticModelLookup = sourceFileGrouping.SourceFiles.ToImmutableDictionary(
                sourceFile => sourceFile,
                sourceFile => modelLookup.TryGetValue(sourceFile, out var existingModel) ?
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

        public IArtifactReferenceFactory ArtifactReferenceFactory { get; }

        public AuxiliaryFileCache FileCache { get; }

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

        private SemanticModel CreateSemanticModel(BicepSourceFile bicepFile) => new(
            this,
            bicepFile,
            environment,
            FileCache,
            linterAnalyzer,
            configurationManager.GetConfiguration(bicepFile.FileUri),
            featureProviderFactory.GetFeatureProvider(bicepFile.FileUri));

        public void TrimCaches()
        {
            var activeModels = this.lazySemanticModelLookup.Values.Select(x => x.Value).OfType<SemanticModel>();
            FileCache.RemoveStaleEntries(activeModels);
        }
    }
}
