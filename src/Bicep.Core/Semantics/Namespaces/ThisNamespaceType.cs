// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Intermediate;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class ThisNamespaceType
    {
        public const string BuiltInName = "this";

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: true,
            BicepExtensionName: BuiltInName,
            ConfigurationType: null,
            TemplateExtensionName: "This",
            TemplateExtensionVersion: "1.0.0");

        private static FunctionResult GetExistsReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            // Check if we're within a resource property context
            if (!IsWithinResourcePropertyContext(model.Binder, functionCall))
            {
                // Use a more generic error message since this can be called from both this() and this.exists()
                var diagnostic = DiagnosticBuilder.ForPosition(functionCall.Name).ThisFunctionOnlyAllowedInResourceProperties();
                diagnostics.Write(diagnostic);
                return new(ErrorType.Create(diagnostic));
            }

            return new(LanguageConstants.Bool, new FunctionCallExpression(functionCall, "targetExists", []));
        }

        private static bool IsWithinResourcePropertyContext(IBinder binder, SyntaxBase syntax)
        {
            // Walk up the syntax tree to find if we're within a resource declaration's body
            var resourceDeclaration = binder.GetNearestAncestor<ResourceDeclarationSyntax>(syntax);
            if (resourceDeclaration == null)
            {
                return false;
            }

            // Check if we're within the resource body (not in the resource identifier or type)
            var resourceBody = resourceDeclaration.TryGetBody();
            if (resourceBody == null)
            {
                return false;
            }

            // Enhanced validation - check specific contexts where this.exists() is NOT allowed
            
            // 1. Top-level resource properties (name, type, scope, etc.)
            if (IsInTopLevelResourceProperty(binder, syntax, resourceDeclaration))
            {
                return false;
            }
            
            // 2. Resource conditions or loops
            if (IsInResourceConditionOrLoop(binder, syntax, resourceDeclaration))
            {
                return false;
            }
            
            // 3. Must be within the 'properties' section or nested child resources
            return IsInResourcePropertiesOrChildResource(binder, syntax, resourceBody);
        }

        private static bool IsInTopLevelResourceProperty(IBinder binder, SyntaxBase syntax, ResourceDeclarationSyntax resourceDeclaration)
        {
            // Check if we're in top-level properties like name, scope, type, etc.
            var objectProperty = binder.GetNearestAncestor<ObjectPropertySyntax>(syntax);
            if (objectProperty == null)
            {
                return false;
            }
            
            var resourceBody = resourceDeclaration.TryGetBody();
            if (resourceBody == null)
            {
                return false;
            }
            
            // If the object property is a direct child of the resource body
            if (binder.GetParent(objectProperty) == resourceBody)
            {
                // Check if it's a restricted property
                var propertyName = objectProperty.TryGetKeyText();
                var restrictedProperties = new[] { "name", "type", "scope", "location", "tags", "dependsOn" };
                return restrictedProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            }
            
            return false;
        }

        private static bool IsInResourceConditionOrLoop(IBinder binder, SyntaxBase syntax, ResourceDeclarationSyntax resourceDeclaration)
        {
            // Check if we're in an if condition or for loop at the resource level
            var ifCondition = binder.GetNearestAncestor<IfConditionSyntax>(syntax);
            var forLoop = binder.GetNearestAncestor<ForSyntax>(syntax);
            
            if (ifCondition != null)
            {
                var parentResource = binder.GetNearestAncestor<ResourceDeclarationSyntax>(ifCondition);
                return parentResource == resourceDeclaration;
            }
            
            if (forLoop != null)
            {
                var parentResource = binder.GetNearestAncestor<ResourceDeclarationSyntax>(forLoop);
                return parentResource == resourceDeclaration;
            }
            
            return false;
        }

        private static bool IsInResourcePropertiesOrChildResource(IBinder binder, SyntaxBase syntax, ObjectSyntax resourceBody)
        {
            // Check if we're within the 'properties' section or a child resource
            var currentSyntax = syntax;
            
            while (currentSyntax != null && currentSyntax != resourceBody)
            {
                // Check if we're in a child resource
                if (currentSyntax is ResourceDeclarationSyntax)
                {
                    return true; // We're in a nested/child resource
                }
                
                // Check if we're in the properties section
                if (currentSyntax is ObjectPropertySyntax objectProperty)
                {
                    var propertyName = objectProperty.TryGetKeyText();
                    if (string.Equals(propertyName, "properties", StringComparison.OrdinalIgnoreCase))
                    {
                        return true; // We're in the properties section
                    }
                }
                
                currentSyntax = binder.GetParent(currentSyntax);
            }
            
            return false;
        }

        private static IEnumerable<FunctionOverload> GetThisFunctions()
        {
            yield return new FunctionOverloadBuilder("exists")
                .WithReturnType(LanguageConstants.Bool)
                .WithReturnResultBuilder(GetExistsReturnResult, LanguageConstants.Bool)
                .WithGenericDescription("Returns whether the current resource exists.")
                .WithFlags(FunctionFlags.RequiresInlining)
                .Build();
        }

        public static NamespaceType Create(string? aliasName, IFeatureProvider? featureProvider = null)
        {
            var functions = featureProvider?.ThisExistsFunctionEnabled == true 
                ? GetThisFunctions() 
                : [];

            return new NamespaceType(
                aliasName ?? BuiltInName,
                Settings,
                ImmutableArray<NamedTypeProperty>.Empty,
                functions,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                new EmptyResourceTypeProvider());
        }
    }
}
