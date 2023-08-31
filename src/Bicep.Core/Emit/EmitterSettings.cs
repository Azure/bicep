// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Workspaces;
using System.Linq;

namespace Bicep.Core.Emit
{
    public class EmitterSettings
    {
        public EmitterSettings(SemanticModel model)
        {
            FileKind = model.SourceFileKind;
            UseExperimentalTemplateLanguageVersion = model.Features.EnabledFeatureMetadata.Any(feature => feature.usesExperimentalArmEngineFeature);

            // Symbolic names are used if (evaluated in increasing order of computational cost):
            EnableSymbolicNames =
                // we're targeting an experimental language version
                UseExperimentalTemplateLanguageVersion ||
                // symbolic name codegen has been explicitly enabled
                model.Features.SymbolicNameCodegenEnabled ||
                // there are any user-defined type declarations
                model.Root.TypeDeclarations.Any() ||
                // any user-defined type declaration syntax is used (e.g., in a `param` or `output` statement)
                SyntaxAggregator.Aggregate(model.SourceFile.ProgramSyntax,
                    seed: false,
                    function: (hasUserDefinedTypeSyntax, syntax) => hasUserDefinedTypeSyntax ||
                        syntax is ObjectTypeSyntax ||
                        syntax is ArrayTypeSyntax ||
                        syntax is TupleTypeSyntax ||
                        syntax is UnionTypeSyntax ||
                        syntax is NullableTypeSyntax,
                    resultSelector: result => result,
                    continuationFunction: (result, syntax) => !result);
        }

        /// <summary>
        /// Generate symbolic names in template output?
        /// </summary>
        public bool EnableSymbolicNames { get; }

        /// <summary>
        /// Use an experimental version of the ARM JSON template syntax. Only used if an experimental Bicep feature has been explicitly enabled.
        /// </summary>
        public bool UseExperimentalTemplateLanguageVersion { get; }

        public BicepSourceFileKind FileKind { get; }
    }
}
