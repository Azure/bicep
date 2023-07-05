// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
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

        public static bool TryGetForeignTemplatePath(IForeignTemplateReference foreignTemplateReference,
            [NotNullWhen(true)] out string? path,
            [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (foreignTemplateReference.TryGetPath() is not StringSyntax pathSyntax)
            {
                path = null;
                failureBuilder = OnMissingPathSyntaxErrorBuilder(foreignTemplateReference);
                return false;
            }

            if (pathSyntax.TryGetLiteralValue() is not string pathValue)
            {
                path = null;
                failureBuilder = x => x.FilePathInterpolationUnsupported();
                return false;
            }

            path = pathValue;
            failureBuilder = null;
            return true;
        }

        private static DiagnosticBuilder.ErrorBuilderDelegate OnMissingPathSyntaxErrorBuilder(IForeignTemplateReference syntax) => syntax switch
        {
            ModuleDeclarationSyntax => x => x.ModulePathHasNotBeenSpecified(),
            UsingDeclarationSyntax => x => x.UsingPathHasNotBeenSpecified(),
            _ => x => x.PathHasNotBeenSpecified(),
        };

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
    }
}
