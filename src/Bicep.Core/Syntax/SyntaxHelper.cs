// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.Syntax
{
    public static class SyntaxHelper
    {
        private static SyntaxBase? TryGetObjectProperty(ObjectSyntax objectSyntax, string propertyName)
            => objectSyntax.Properties.SingleOrDefault(p => p.TryGetKeyText() == propertyName)?.Value;

        public static ArraySyntax? TryGetAllowedSyntax(ParameterDeclarationSyntax parameterDeclarationSyntax)
        {
            if (!(parameterDeclarationSyntax.Modifier is ObjectSyntax modifierObject))
            {
                return null;
            }

            var allowedValuesSyntax = TryGetObjectProperty(modifierObject, LanguageConstants.ParameterAllowedPropertyName);
            if (!(allowedValuesSyntax is ArraySyntax allowedArraySyntax))
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

        public static AzResourceScope GetTargetScope(TargetScopeSyntax targetScopeSyntax)
        {
            var targetScope = AzResourceScope.None;
            IEnumerable<SyntaxBase> targets;
            if (targetScopeSyntax.Value is ArraySyntax arraySyntax)
            {
                targets = arraySyntax.Items.Select(x => x.Value);
            }
            else
            {
                targets = targetScopeSyntax.Value.AsEnumerable();
            }

            foreach (var target in targets)
            {
                if (!(target is StringSyntax stringSyntax))
                {
                    // type checking will pick up any errors - no need to do so here.
                    continue;
                }

                var literalValue = stringSyntax.TryGetLiteralValue();
                if (literalValue == null)
                {
                    // type checking will pick up any errors - no need to do so here.
                    continue;
                }

                switch (literalValue)
                {
                    case LanguageConstants.TargetScopeTypeTenant:
                        targetScope |= AzResourceScope.Tenant;
                        break;
                    case LanguageConstants.TargetScopeTypeManagementGroup:
                        targetScope |= AzResourceScope.ManagementGroup;
                        break;
                    case LanguageConstants.TargetScopeTypeSubscription:
                        targetScope |= AzResourceScope.Subscription;
                        break;
                    case LanguageConstants.TargetScopeTypeResourceGroup:
                        targetScope |= AzResourceScope.ResourceGroup;
                        break;
                    default:
                        // type checking will pick up any errors - no need to do so here.
                        break;
                }
            }

            return targetScope;
        }
    }
}