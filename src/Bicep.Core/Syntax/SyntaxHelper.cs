// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Syntax
{
    public static class SyntaxHelper
    {
        private static SyntaxBase? TryGetObjectProperty(ObjectSyntax objectSyntax, string propertyName)
            => objectSyntax.Properties.SingleOrDefault(p => p.TryGetKeyText() == propertyName)?.Value;

        public static ArraySyntax? TryGetAllowedSyntax(ParameterDeclarationSyntax parameterDeclarationSyntax)
        {
            if (parameterDeclarationSyntax.Modifier is not ObjectSyntax modifierObject)
            {
                return null;
            }

            var allowedValuesSyntax = TryGetObjectProperty(modifierObject, LanguageConstants.ParameterAllowedPropertyName);
            if (allowedValuesSyntax is not ArraySyntax allowedArraySyntax)
            {
                return null;
            }

            return allowedArraySyntax;
        }

        public static SyntaxBase? TryGetDefaultValue(ParameterDeclarationSyntax parameterDeclarationSyntax)
        {
            if (parameterDeclarationSyntax.Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
            {
                return defaultValueSyntax.DefaultValue;
            }

            if (parameterDeclarationSyntax.Modifier is ObjectSyntax modifierObject)
            {
                return TryGetObjectProperty(modifierObject, LanguageConstants.ParameterDefaultPropertyName);
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
                failureBuilder = x => x.ModulePathInterpolationUnsupported();
                return null;
            }

            failureBuilder = null;
            return pathValue;
        }

        public static TypeSymbol? TryGetPrimitiveType(ParameterDeclarationSyntax parameterDeclarationSyntax)
            => LanguageConstants.TryGetDeclarationType(parameterDeclarationSyntax.ParameterType?.TypeName);

        public static ResourceScope GetTargetScope(TargetScopeSyntax targetScopeSyntax)
        {
            // TODO: Revisit when adding support for multiple target scopes

            // Type checking will pick up any errors if we fail to process the syntax correctly in this function.
            // There's no need to do error checking here - just return "None" as the scope type.

            if (!(targetScopeSyntax.Value is StringSyntax stringSyntax))
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

        public static ResourceScope GetTargetScope(SyntaxTree syntaxTree)
        {
            var defaultTargetScope = ResourceScope.ResourceGroup;
            var targetSyntax = syntaxTree.ProgramSyntax.Children.OfType<TargetScopeSyntax>().FirstOrDefault();
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
    }
}
