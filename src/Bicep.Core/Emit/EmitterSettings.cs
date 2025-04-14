// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;

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
                // resourceinfo codegen has been enabled
                model.Features.ResourceInfoCodegenEnabled ||
                model.Features.TypedVariablesEnabled ||
                // there are any user-defined type declarations
                model.Root.TypeDeclarations.Any() ||
                // there are any user-defined function declarations
                model.Root.FunctionDeclarations.Any() ||
                // there are any compile-time imports (imported functions or variables may enclose user-defined types, and determining definitively requires calculating the full import closure)
                model.Root.ImportedSymbols.Any() ||
                // there are any wildcard compile-time imports
                model.Root.WildcardImports.Any() ||
                // there are any existing resources with explicit dependencies
                model.Root.ResourceDeclarations.Any(r => r.DeclaringResource.IsExistingResource() &&
                    r.DeclaringResource.TryGetBody()?.TryGetPropertyByName(LanguageConstants.ResourceDependsOnPropertyName) is not null) ||
                // there are secure outputs
                model.Outputs.Any(output => output.IsSecure) ||
                // there are secure outputs in modules
                model.Root.ModuleDeclarations.Any(module => module.TryGetSemanticModel().TryUnwrap()?.Outputs.Any(output => output.IsSecure) ?? false) ||
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
                    continuationFunction: (result, syntax) => !result) ||
                // there are optional module names
                model.Root.ModuleDeclarations.Any(module => module.TryGetBodyProperty(LanguageConstants.ModuleNamePropertyName) is null);
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
