// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Intermediate;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Namespaces;

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
        if (!IsWithinResourcePropertyContext(model.Binder, functionCall))
        {
            var diagnostic = DiagnosticBuilder.ForPosition(functionCall.Name).ThisFunctionOnlyAllowedInResourceProperties();
            diagnostics.Write(diagnostic);
            return new(ErrorType.Create(diagnostic));
        }

        return new(LanguageConstants.Bool, new FunctionCallExpression(functionCall, "target", [new StringLiteralExpression(functionCall, "exists")]));
    }

    private static FunctionResult GetExistingPropertiesReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
    {
        if (!IsWithinResourcePropertyContext(model.Binder, functionCall))
        {
            var diagnostic = DiagnosticBuilder.ForPosition(functionCall.Name).ThisFunctionOnlyAllowedInResourceProperties();
            diagnostics.Write(diagnostic);
            return new(ErrorType.Create(diagnostic));
        }

        var derived = TryGetExistingPropertiesType(model, functionCall) ?? LanguageConstants.Object;
        return new(derived, new FunctionCallExpression(functionCall, "target", []));
    }

    private static FunctionResult GetExistingResourceReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
    {
        if (!IsWithinResourcePropertyContext(model.Binder, functionCall))
        {
            var diagnostic = DiagnosticBuilder.ForPosition(functionCall.Name).ThisFunctionOnlyAllowedInResourceProperties();
            diagnostics.Write(diagnostic);
            return new(ErrorType.Create(diagnostic));
        }

        var derived = TryGetExistingResourceType(model, functionCall) ?? LanguageConstants.Object;
        return new(derived, new FunctionCallExpression(functionCall, "target", [new StringLiteralExpression(functionCall, "full")]));
    }

    private static bool IsWithinResourcePropertyContext(IBinder binder, SyntaxBase syntax)
    {
        var resourceDeclaration = binder.GetNearestAncestor<ResourceDeclarationSyntax>(syntax);
        if (resourceDeclaration is null)
        {
            return false;
        }

        var resourceBody = resourceDeclaration.TryGetBody();
        if (resourceBody is null)
        {
            return false;
        }

        if (IsInTopLevelResourceProperty(binder, syntax, resourceDeclaration) ||
            IsInResourceConditionOrLoop(binder, syntax, resourceDeclaration))
        {
            return false;
        }

        return IsInResourcePropertiesOrChildResource(binder, syntax, resourceBody);
    }

    private static bool IsInTopLevelResourceProperty(IBinder binder, SyntaxBase syntax, ResourceDeclarationSyntax resourceDeclaration)
    {
        var objectProperty = binder.GetNearestAncestor<ObjectPropertySyntax>(syntax);
        if (objectProperty is null)
        {
            return false;
        }

        var resourceBody = resourceDeclaration.TryGetBody();
        if (resourceBody is null)
        {
            return false;
        }

        if (binder.GetParent(objectProperty) == resourceBody)
        {
            var propertyName = objectProperty.TryGetKeyText();
            var restricted = new[] { "name", "type", "scope", "location", "tags", "dependsOn" };
            return restricted.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
        }

        return false;
    }

    private static bool IsInResourceConditionOrLoop(IBinder binder, SyntaxBase syntax, ResourceDeclarationSyntax resourceDeclaration)
    {
        var ifCondition = binder.GetNearestAncestor<IfConditionSyntax>(syntax);
        if (ifCondition != null && binder.GetNearestAncestor<ResourceDeclarationSyntax>(ifCondition) == resourceDeclaration)
        {
            return true;
        }

        var forLoop = binder.GetNearestAncestor<ForSyntax>(syntax);
        if (forLoop != null && binder.GetNearestAncestor<ResourceDeclarationSyntax>(forLoop) == resourceDeclaration)
        {
            return true;
        }

        return false;
    }

    private static bool IsInResourcePropertiesOrChildResource(IBinder binder, SyntaxBase syntax, ObjectSyntax resourceBody)
    {
        var current = syntax;
        while (current != null && current != resourceBody)
        {
            if (current is ResourceDeclarationSyntax)
            {
                return true; // nested child resource
            }

            if (current is ObjectPropertySyntax op && string.Equals(op.TryGetKeyText(), "properties", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            current = binder.GetParent(current);
        }
        return false;
    }

    private static ResourceType? TryGetEnclosingResourceType(SemanticModel model, SyntaxBase syntax)
    {
        var decl = model.Binder.GetNearestAncestor<ResourceDeclarationSyntax>(syntax);
        if (decl is null)
        {
            return null;
        }

        if (model.GetDeclaredType(decl) is ResourceType rt)
        {
            return rt;
        }

        return null;
    }

    private static TypeSymbol? TryGetExistingPropertiesType(SemanticModel model, SyntaxBase syntax)
    {
        var resourceType = TryGetEnclosingResourceType(model, syntax);
        if (resourceType is null)
        {
            return null;
        }

        var body = (resourceType.Body.Type as ObjectType);

        if (body?.Properties.TryGetValue("properties", out var p) is true && p.TypeReference.Type is ObjectType props)
        {
            return props;
        }
        else
        {
            return LanguageConstants.Object;
        }
    }

    private static TypeSymbol? TryGetExistingResourceType(SemanticModel model, SyntaxBase syntax)
    {
        var resourceType = TryGetEnclosingResourceType(model, syntax);
        if (resourceType is null)
        {
            return null;
        }

        return resourceType.Body.Type;
    }

    private static IEnumerable<FunctionOverload> GetThisFunctions()
    {
        yield return new FunctionOverloadBuilder("exists")
            .WithReturnType(LanguageConstants.Bool)
            .WithReturnResultBuilder(GetExistsReturnResult, LanguageConstants.Bool)
            .WithGenericDescription("Returns whether the current resource exists.")
            .WithFlags(FunctionFlags.RequiresInlining)
            .Build();

        yield return new FunctionOverloadBuilder("existingProperties")
            .WithReturnType(LanguageConstants.Object)
            .WithReturnResultBuilder(GetExistingPropertiesReturnResult, LanguageConstants.Object)
            .WithGenericDescription("Returns the existing properties bag of the current resource.")
            .WithFlags(FunctionFlags.RequiresInlining)
            .Build();

        yield return new FunctionOverloadBuilder("existingResource")
            .WithReturnType(LanguageConstants.Object)
            .WithReturnResultBuilder(GetExistingResourceReturnResult, LanguageConstants.Object)
            .WithGenericDescription("Returns the existing resource body (same schema as declaration body).")
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
