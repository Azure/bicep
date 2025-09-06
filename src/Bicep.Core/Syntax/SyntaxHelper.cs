// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;

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
                return TryGetDefaultValue(syntax);
            }

            return null;
        }

        public static ResultWithDiagnosticBuilder<string> TryGetForeignTemplatePath(
            IArtifactReferenceSyntax foreignTemplateReference,
            DiagnosticBuilder.DiagnosticBuilderDelegate onUnspecifiedPath)
        {
            if (foreignTemplateReference.Path is not StringSyntax && foreignTemplateReference.Path is not NoneLiteralSyntax)
            {
                return new(onUnspecifiedPath);
            }

            var pathSyntax = foreignTemplateReference.Path is StringSyntax syntax ? syntax : null;

            if (pathSyntax?.TryGetLiteralValue() is not string pathValue)
            {
                return new(x => x.FilePathInterpolationUnsupported());
            }

            return new(pathValue);
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
                // The feature flag is checked during scope validation, so just handle it here.
                LanguageConstants.TargetScopeTypeDesiredStateConfiguration => ResourceScope.DesiredStateConfiguration,
                LanguageConstants.TargetScopeTypeLocal => ResourceScope.Local,
                LanguageConstants.TargetScopeTypeOrchestrator => ResourceScope.Orchestrator,
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

        public static SyntaxBase UnwrapNonNullAssertion(SyntaxBase syntax) => syntax switch
        {
            NonNullAssertionSyntax nonNullAssertion => UnwrapNonNullAssertion(nonNullAssertion.BaseExpression),
            _ => syntax,
        };
    }
}
