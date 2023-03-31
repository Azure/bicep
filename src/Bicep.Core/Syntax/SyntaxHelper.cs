// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Syntax
{
    public static class SyntaxHelper
    {
        public static SyntaxBase? TryGetDefaultValue(ParameterDeclarationSyntax parameterDeclarationSyntax)
        {
            if (parameterDeclarationSyntax.Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
            {
                return defaultValueSyntax.DefaultValue;
            }

            return null;
        }

        public static SyntaxBase? TryGetDefaultValue(ParameterSymbol parameterSymbol)
        {
            if (parameterSymbol.DeclaringSyntax is ParameterDeclarationSyntax syntax)
            {
                return SyntaxHelper.TryGetDefaultValue(syntax);
            }

            return null;
        }

        public static string? TryGetModulePath(ModuleDeclarationSyntax moduleDeclarationSyntax, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var pathSyntax = moduleDeclarationSyntax.TryGetPath();
            if (pathSyntax == null)
            {
                failureBuilder = x => x.ModulePathHasNotBeenSpecified();
                return null;
            }

            var pathValue = pathSyntax.TryGetLiteralValue();
            if (pathValue == null)
            {
                failureBuilder = x => x.FilePathInterpolationUnsupported();
                return null;
            }

            failureBuilder = null;
            return pathValue;
        }

        public static string? TryGetUsingPath(UsingDeclarationSyntax usingDeclarationSyntax, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var pathSyntax = usingDeclarationSyntax.TryGetPath();
            if (pathSyntax == null)
            {
                failureBuilder = x => x.TemplatePathHasNotBeenSpecified();
                return null;
            }

            var pathValue = pathSyntax.TryGetLiteralValue();
            if (pathValue == null)
            {
                failureBuilder = x => x.FilePathInterpolationUnsupported();
                return null;
            }

            failureBuilder = null;
            return pathValue;
        }

        public static ResourceScope GetTargetScope(TargetScopeSyntax targetScopeSyntax)
        {
            // TODO: Revisit when adding support for multiple target scopes

            // Type checking will pick up any errors if we fail to process the syntax correctly in this function.
            // There's no need to do error checking here - just return "None" as the scope type.

            if (targetScopeSyntax.Value is not StringSyntax stringSyntax)
            {
                return ResourceScope.None;
            }

            var literalValue = stringSyntax.TryGetLiteralValue();
            if (literalValue == null)
            {
                return ResourceScope.None;
            }

            return literalValue switch
            {
                LanguageConstants.TargetScopeTypeTenant => ResourceScope.Tenant,
                LanguageConstants.TargetScopeTypeManagementGroup => ResourceScope.ManagementGroup,
                LanguageConstants.TargetScopeTypeSubscription => ResourceScope.Subscription,
                LanguageConstants.TargetScopeTypeResourceGroup => ResourceScope.ResourceGroup,
                _ => ResourceScope.None,
            };
        }

        public static ResourceScope GetTargetScope(BicepSourceFile bicepFile)
        {
            var defaultTargetScope = ResourceScope.ResourceGroup;
            var targetSyntax = bicepFile.ProgramSyntax.Children.OfType<TargetScopeSyntax>().FirstOrDefault();
            if (targetSyntax == null)
            {
                return defaultTargetScope;
            }

            var targetScope = SyntaxHelper.GetTargetScope(targetSyntax);
            if (targetScope == ResourceScope.None)
            {
                return defaultTargetScope;
            }

            return targetScope;
        }

        public static (SyntaxBase baseSyntax, SyntaxBase? indexSyntax) UnwrapArrayAccessSyntax(SyntaxBase syntax)
            => syntax switch
            {
                ArrayAccessSyntax arrayAccess => (arrayAccess.BaseExpression, arrayAccess.IndexExpression),
                _ => (syntax, null),
            };

        /// <summary>
        /// Tries to retrieve a string literal from the expression. Will evaluate variables and optionally parameter default values
        /// </summary>
        public static (string stringValue, StringSyntax stringSyntax, string? pathToValueIfNonTrivial)? TryGetEvaluatedStringLiteral(SemanticModel model, SyntaxBase? expression, bool evaluateParameters)
        {
            return TryGetEvaluatedStringLiteral(model, expression, evaluateParameters, Array.Empty<DeclaredSymbol>());
        }

        private static (string stringValue, StringSyntax stringSyntax, string? pathToValueIfNonTrivial)? TryGetEvaluatedStringLiteral(
            SemanticModel model, SyntaxBase? expression, bool evaluateParameters, DeclaredSymbol[] currentPaths
        )
        {
            if (expression is StringSyntax stringSyntax
                && stringSyntax.TryGetLiteralValue() is string literalValue)
            {
                var path = currentPaths.Length > 0 ? string.Join(" => ", currentPaths.Select(symbol => symbol.Name)) : null;
                return (literalValue, stringSyntax, path);
            }
            else if (expression is VariableAccessSyntax)
            {
                if (model.GetSymbolInfo(expression) is DeclaredSymbol symbol)
                {
                    // Create nested path for recursive call
                    var nestedPath = new DeclaredSymbol[currentPaths.Length + 1];
                    Array.Copy(currentPaths, nestedPath, currentPaths.Length);
                    nestedPath[^1] = symbol;

                    if (symbol is VariableSymbol variable)
                    {
                        // Evaluate the variable's definition
                        var variableValue = (variable.DeclaringSyntax as VariableDeclarationSyntax)?.Value;
                        return variableValue is null ? null : TryGetEvaluatedStringLiteral(model, variableValue, evaluateParameters, nestedPath);
                    }
                    else if (evaluateParameters && symbol is ParameterSymbol parameter)
                    {
                        // Evaluate the parameter's default value
                        var defaultValue = TryGetDefaultValue(parameter);
                        if (defaultValue is null)
                        {
                            // Using a parameter with no default value is acceptable
                            return null;
                        }
                        else
                        {
                            // Analyze parameter's default value
                            return TryGetEvaluatedStringLiteral(model, defaultValue, evaluateParameters, nestedPath);
                        }
                    }
                }
            }

            return null;
        }
    }
}
