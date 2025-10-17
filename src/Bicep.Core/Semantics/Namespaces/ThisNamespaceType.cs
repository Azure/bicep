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
        // Compile to not(empty(target('full')))
        var targetExpression = new FunctionCallExpression(functionCall, "target", [new StringLiteralExpression(functionCall, "full")]);
        var emptyExpression = new FunctionCallExpression(functionCall, "empty", [targetExpression]);
        var notExpression = new FunctionCallExpression(functionCall, "not", [emptyExpression]);

        return new(LanguageConstants.Bool, notExpression);
    }

    private static FunctionResult GetExistingPropertiesReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
    {
        var derived = TryGetExistingPropertiesType(model, functionCall) ?? LanguageConstants.Object;
        return new(derived, new FunctionCallExpression(functionCall, "target", []));
    }

    private static FunctionResult GetExistingResourceReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
    {
        var derived = TryGetExistingResourceType(model, functionCall) ?? LanguageConstants.Object;
        var resourceType = TryGetEnclosingResourceType(model, functionCall);

        // For extensible resources, use target() without 'full' since they don't have the standard Azure resource structure
        var isExtensibleResource = resourceType is not null && !resourceType.IsAzResource();
        var targetExpression = isExtensibleResource
            ? new FunctionCallExpression(functionCall, "target", [])
            : new FunctionCallExpression(functionCall, "target", [new StringLiteralExpression(functionCall, "full")]);

        return new(derived, targetExpression);
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

        var body = resourceType.Body.Type as ObjectType;

        // Check if this is an extensible resource (non-Azure resource)
        var isExtensibleResource = !resourceType.IsAzResource();

        if (isExtensibleResource)
        {
            // For extensible resources, return the full resource body type
            return body ?? LanguageConstants.Object;
        }
        else if (body?.Properties.TryGetValue("properties", out var p) is true && p.TypeReference.Type is ObjectType props)
        {
            return props;
        }
        else
        {
            // For Azure resources without properties, return generic object
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

    public static NamespaceType Create(string? aliasName)
    {
        return new NamespaceType(
            aliasName ?? BuiltInName,
            Settings,
            ImmutableArray<NamedTypeProperty>.Empty,
            GetThisFunctions(),
            ImmutableArray<BannedFunction>.Empty,
            ImmutableArray<Decorator>.Empty,
            new EmptyResourceTypeProvider());
    }
}
