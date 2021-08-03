// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class Compilation
    {
        private readonly ImmutableDictionary<ISourceFile, Lazy<ISemanticModel>> lazySemanticModelLookup;

        public Compilation(IResourceTypeProvider resourceTypeProvider, SourceFileGrouping sourceFileGrouping, ImmutableDictionary<ISourceFile, ISemanticModel>? modelLookup = null)
        {
            this.SourceFileGrouping = sourceFileGrouping;
            this.ResourceTypeProvider = resourceTypeProvider;
            this.lazySemanticModelLookup = sourceFileGrouping.SourceFiles.ToImmutableDictionary(
                sourceFile => sourceFile,
                sourceFile => (modelLookup is not null && modelLookup.TryGetValue(sourceFile, out var existingModel)) ? 
                    new(existingModel) : 
                    new Lazy<ISemanticModel>(() => sourceFile switch
                    {
                        BicepFile bicepFile => new SemanticModel(this, bicepFile, SourceFileGrouping.FileResolver),
                        ArmTemplateFile armTemplateFile => new ArmTemplateSemanticModel(armTemplateFile),
                        _ => throw new ArgumentOutOfRangeException(nameof(sourceFile)),
                    }));
        }

        public SourceFileGrouping SourceFileGrouping { get; }

        public IResourceTypeProvider ResourceTypeProvider { get; }

        public SemanticModel GetEntrypointSemanticModel()
            => GetSemanticModel(SourceFileGrouping.EntryPoint);

        public SemanticModel GetSemanticModel(BicepFile bicepFile)
            => this.GetSemanticModel<SemanticModel>(bicepFile);

        public ArmTemplateSemanticModel GetSemanticModel(ArmTemplateFile armTemplateFile)
            => this.GetSemanticModel<ArmTemplateSemanticModel>(armTemplateFile);

        public ISemanticModel GetSemanticModel(ISourceFile sourceFile)
            => this.lazySemanticModelLookup[sourceFile].Value;

        public IReadOnlyDictionary<BicepFile, IEnumerable<IDiagnostic>> GetAllDiagnosticsByBicepFile(ConfigHelper? overrideConfig = default)
            => SourceFileGrouping.SourceFiles.OfType<BicepFile>().ToDictionary(
                bicepFile => bicepFile,
                bicepFile => this.GetSemanticModel(bicepFile).GetAllDiagnostics(overrideConfig));

        private T GetSemanticModel<T>(ISourceFile sourceFile) where T : class, ISemanticModel =>
            this.GetSemanticModel(sourceFile) as T ??
            throw new ArgumentException($"Expected the semantic model type to be \"{typeof(T).Name}\".");
    }
}
