// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
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
                // use of extensions or extensible resources
                model.Root.ExtensionDeclarations.Any() ||
                model.DeclaredResources.Any(x => !x.IsAzResource) ||
                // resourceinfo codegen has been enabled
                model.Features.ResourceInfoCodegenEnabled ||
                // there are typed variables
                model.Root.VariableDeclarations.Any(x => x.DeclaringVariable.Type is { }) ||
                // there are any user-defined type declarations
                model.Root.TypeDeclarations.Any() ||
                // there are any user-defined function declarations
                model.Root.FunctionDeclarations.Any() ||
                // there are any compile-time imports (imported functions or variables may enclose user-defined types, and determining definitively requires calculating the full import closure)
                model.Root.ImportedSymbols.Any() ||
                // there are any wildcard compile-time imports
                model.Root.WildcardImports.Any() ||
                // there are secure outputs
                model.Outputs.Any(output => output.IsSecure) ||
                // there are secure outputs in modules
                model.Root.ModuleDeclarations.Any(module => module.TryGetSemanticModel().TryUnwrap()?.Outputs.Any(output => output.IsSecure) ?? false) ||
                // direct access to a looped resource, requiring use of 'references()'
                RequiresReferencesFunctionVisitor.RequiresReferencesFunction(model.Binder) ||
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
                model.Root.ModuleDeclarations.Any(module => module.TryGetBodyProperty(LanguageConstants.ModuleNamePropertyName) is null) ||
                SyntaxAggregator.Aggregate(model.SourceFile.ProgramSyntax,
                    seed: false,
                    function: (found, syntax) => found ||
                        (syntax is ResourceDeclarationSyntax rds && ResourceRequiresSymbolicNames(model, rds)),
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

        private static bool ResourceRequiresSymbolicNames(SemanticModel model, ResourceDeclarationSyntax resourceSyntax)
        {
            if (resourceSyntax.IsExistingResource() &&
                resourceSyntax.TryGetBody()?.TryGetPropertyByName(LanguageConstants.ResourceDependsOnPropertyName) is not null)
            {
                // Existing resources with explicit dependencies require symbolic names
                return true;
            }

            if (SemanticModelHelper.TryGetDecoratorInNamespace(
                model,
                resourceSyntax,
                SystemNamespaceType.BuiltInName,
                LanguageConstants.OnlyIfNotExistsPropertyName) is not null)
            {
                // Resources with the 'onlyIfNotExists' decorator require symbolic names
                return true;
            }

            return false;
        }
    }
}
