// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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

        public static TypeSymbol? TryGetPrimitiveType(ParameterDeclarationSyntax parameterDeclarationSyntax)
            => LanguageConstants.TryGetDeclarationType((parameterDeclarationSyntax.ParameterType as SimpleTypeSyntax)?.TypeName);

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

        public static ResourceScope GetTargetScope(BicepFile bicepFile)
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
    }
}
