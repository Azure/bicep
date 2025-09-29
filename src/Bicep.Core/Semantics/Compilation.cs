// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Semantics
{
    public class Compilation : ISemanticModelLookup
    {
        // Stores semantic model for each source file (map exists for all source files, but semantic model created only when indexed)
        private readonly ImmutableDictionary<IOUri, Lazy<ISemanticModel>> lazySemanticModelLookup;

        public Compilation(
            IEnvironment environment,
            INamespaceProvider namespaceProvider,
            SourceFileGrouping sourceFileGrouping,
            IBicepAnalyzer linterAnalyzer,
            IArtifactReferenceFactory artifactReferenceFactory,
            ISourceFileFactory sourceFileFactory,
            ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            this.Environment = environment;
            this.SourceFileGrouping = sourceFileGrouping;
            this.NamespaceProvider = namespaceProvider;
            this.LinterAnalyzer = linterAnalyzer;
            this.ArtifactReferenceFactory = artifactReferenceFactory;
            this.SourceFileFactory = sourceFileFactory;

            this.lazySemanticModelLookup = sourceFileGrouping.SourceFiles.ToImmutableDictionary(
                sourceFile => sourceFile.FileHandle.Uri,
                sourceFile => modelLookup.TryGetValue(sourceFile, out var existingModel) ?
                    new(existingModel) :
                    new Lazy<ISemanticModel>(() => sourceFile switch // semantic model doesn't yet exist for file, create it
                    {
                        BicepFile bicepFile => CreateSemanticModel(bicepFile),
                        BicepParamFile bicepParamFile => CreateSemanticModel(bicepParamFile),
                        ArmTemplateFile armTemplateFile => new ArmTemplateSemanticModel(armTemplateFile),
                        TemplateSpecFile templateSpecFile => new TemplateSpecSemanticModel(templateSpecFile),
                        BicepReplFile bicepReplFile => CreateSemanticModel(bicepReplFile),
                        _ => throw new ArgumentOutOfRangeException(nameof(sourceFile)),
                    }));
            this.Emitter = new CompilationEmitter(this);
        }

        public SourceFileGrouping SourceFileGrouping { get; }

        public INamespaceProvider NamespaceProvider { get; }

        public IArtifactReferenceFactory ArtifactReferenceFactory { get; }

        public IEnvironment Environment { get; }

        public IBicepAnalyzer LinterAnalyzer;

        public ISourceFileFactory SourceFileFactory { get; }

        public ICompilationEmitter Emitter { get; }

        public SemanticModel GetEntrypointSemanticModel()
            // entry point semantic models are guaranteed to cast successfully
            => GetSemanticModel(SourceFileGrouping.EntryPoint);

        public SemanticModel GetSemanticModel(BicepSourceFile bicepFile)
            => this.GetSemanticModel<SemanticModel>(bicepFile);

        public ArmTemplateSemanticModel GetSemanticModel(ArmTemplateFile armTemplateFile)
            => this.GetSemanticModel<ArmTemplateSemanticModel>(armTemplateFile);

        public ISemanticModel GetSemanticModel(ISourceFile sourceFile)
            => this.GetSemanticModel(sourceFile.FileHandle.Uri);

        public ISemanticModel GetSemanticModel(IOUri sourceFileUri)
            => this.lazySemanticModelLookup[sourceFileUri].Value;

        public ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> GetAllDiagnosticsByBicepFile()
            => SourceFileGrouping.SourceFiles.OfType<BicepSourceFile>().ToImmutableDictionary(bicepFile => bicepFile, this.GetSourceFileDiagnostics);

        public ImmutableArray<IDiagnostic> GetSourceFileDiagnostics(ISourceFile sourceFile) =>
            this.GetSourceFileDiagnostics(sourceFile.FileHandle.Uri);

        // TODO(file-io-abstraction): Remove once tests are migrated.
        public ImmutableArray<IDiagnostic> GetSourceFileDiagnostics(Uri sourceFileUri) =>
            this.GetSourceFileDiagnostics(sourceFileUri.ToIOUri());

        public ImmutableArray<IDiagnostic> GetSourceFileDiagnostics(IOUri sourceFileUri) =>
            (this.GetSemanticModel(sourceFileUri) as SemanticModel)?.GetAllDiagnostics() ?? [];

        public IEnumerable<ISemanticModel> GetAllModels()
            => this.SourceFileGrouping.SourceFiles.Select(GetSemanticModel);

        public IEnumerable<SemanticModel> GetAllBicepModels()
            => GetAllModels().OfType<SemanticModel>();

        public bool HasErrors() => this.GetEntrypointSemanticModel().HasErrors();

        private T GetSemanticModel<T>(ISourceFile sourceFile) where T : class, ISemanticModel =>
            this.GetSemanticModel(sourceFile) as T ??
            throw new ArgumentException($"Expected the semantic model type to be \"{typeof(T).Name}\".");

        private SemanticModel CreateSemanticModel(BicepSourceFile bicepFile) => new(
            this.LinterAnalyzer,
            this.NamespaceProvider,
            this.ArtifactReferenceFactory,
            this,
            this.SourceFileGrouping,
            this.Environment,
            bicepFile);
    }
}
