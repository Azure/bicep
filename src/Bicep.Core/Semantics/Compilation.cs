// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class Compilation
    {
        private readonly ImmutableDictionary<ISourceFile, Lazy<ISemanticModel>> lazySemanticModelLookup;

        public Compilation(IResourceTypeProvider resourceTypeProvider, SyntaxTreeGrouping syntaxTreeGrouping)
        {
            this.SyntaxTreeGrouping = syntaxTreeGrouping;
            this.ResourceTypeProvider = resourceTypeProvider;
            this.lazySemanticModelLookup = syntaxTreeGrouping.SyntaxTrees.ToImmutableDictionary(
                sourceFile => sourceFile,
                sourceFile => new Lazy<ISemanticModel>(() => sourceFile switch
                {
                    SyntaxTree bicepFile => new SemanticModel(this, bicepFile, SyntaxTreeGrouping.FileResolver),
                    ArmTemplateFile armTemplateFile => new ArmTemplateSemanticModel(armTemplateFile),
                    _ => throw new ArgumentOutOfRangeException(nameof(sourceFile)),
                }));
        }

        public SyntaxTreeGrouping SyntaxTreeGrouping { get; }

        public IResourceTypeProvider ResourceTypeProvider { get; }

        public SemanticModel GetEntrypointSemanticModel()
            => GetSemanticModel(SyntaxTreeGrouping.EntryPoint);

        public SemanticModel GetSemanticModel(SyntaxTree bicepFile)
            => this.GetSemanticModel<SemanticModel>(bicepFile);

        public ArmTemplateSemanticModel GetSemanticModel(ArmTemplateFile armTemplateFile)
            => this.GetSemanticModel<ArmTemplateSemanticModel>(armTemplateFile);

        public ISemanticModel GetSemanticModel(ISourceFile sourceFile)
            => this.lazySemanticModelLookup[sourceFile].Value;

        public IReadOnlyDictionary<SyntaxTree, IEnumerable<IDiagnostic>> GetAllDiagnosticsByBicepFile(ConfigHelper? overrideConfig = default)
            => SyntaxTreeGrouping.SyntaxTrees.OfType<SyntaxTree>().ToDictionary(
                syntaxTree => syntaxTree,
                syntaxTree => GetSemanticModel(syntaxTree).GetAllDiagnostics(overrideConfig));

        private T GetSemanticModel<T>(ISourceFile sourceFile) where T : class, ISemanticModel =>
            this.GetSemanticModel(sourceFile) as T ??
            throw new ArgumentException($"Expected the semantic model type to be \"{typeof(T).Name}\".");
    }
}
