// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
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

        public Compilation(
            IFeatureProviderFactory featureProviderFactory,
            IEnvironment environment,
            INamespaceProvider namespaceProvider,
            SourceFileGrouping sourceFileGrouping,
            IConfigurationManager configurationManager,
            IBicepAnalyzer linterAnalyzer,
            IArtifactReferenceFactory artifactReferenceFactory,
            IReadableFileCache fileCache,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            this.FeatureProviderFactory = featureProviderFactory;
            this.Environment = environment;
            this.SourceFileGrouping = sourceFileGrouping;
            this.NamespaceProvider = namespaceProvider;
            this.FileCache = fileCache;
            this.ConfigurationManager = configurationManager;
            this.LinterAnalyzer = linterAnalyzer;
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
            this.Emitter = new CompilationEmitter(this);
        }

        public SourceFileGrouping SourceFileGrouping { get; }

        public INamespaceProvider NamespaceProvider { get; }

        public IArtifactReferenceFactory ArtifactReferenceFactory { get; }

        public IReadableFileCache FileCache { get; }

        public IEnvironment Environment { get; }

        public IBicepAnalyzer LinterAnalyzer;

        public IConfigurationManager ConfigurationManager { get; }

        public IFeatureProviderFactory FeatureProviderFactory { get; }

        public ICompilationEmitter Emitter { get; }

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
                bicepFile => this.GetSemanticModel(bicepFile) is SemanticModel semanticModel ? semanticModel.GetAllDiagnostics() : []);

        private T GetSemanticModel<T>(ISourceFile sourceFile) where T : class, ISemanticModel =>
            this.GetSemanticModel(sourceFile) as T ??
            throw new ArgumentException($"Expected the semantic model type to be \"{typeof(T).Name}\".");

        public IEnumerable<ISemanticModel> GetAllModels()
            => this.SourceFileGrouping.SourceFiles.Select(GetSemanticModel);

        public IEnumerable<SemanticModel> GetAllBicepModels()
            => GetAllModels().OfType<SemanticModel>();

        private SemanticModel CreateSemanticModel(BicepSourceFile bicepFile) => new(this, bicepFile);
    }
}
