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
    // Note: self() function should be added to this namespace when it is implemented in bicep. 
    public const string BuiltInName = "this";

    public static NamespaceSettings Settings { get; } = new(
        IsSingleton: true,
        BicepExtensionName: BuiltInName,
        ConfigurationType: null,
        TemplateExtensionName: "This",
        TemplateExtensionVersion: "0.0.1");

    private static FunctionResult GetExistsReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
    {
        // Compile to not(empty(target('full')))
        var targetExpression = new FunctionCallExpression(functionCall, "target", [new StringLiteralExpression(functionCall, "full")]);
        var emptyExpression = new FunctionCallExpression(functionCall, "empty", [targetExpression]);
        var notExpression = new FunctionCallExpression(functionCall, "not", [emptyExpression]);

        return new(LanguageConstants.Bool, notExpression);
    }

    private static FunctionResult GetExistingResourceReturnResult(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
    {
        var derived = TypeHelper.MakeNullable(TryGetExistingResourceType(model, functionCall) ?? LanguageConstants.Object);
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
            .WithGenericDescription("Returns whether the current resource already exists when the template is deployed.")
            // The flag RequiresInlining is used here to indicate that this is an ARM runtime function to provide diagnostics where runtime values are blocked.
            .WithFlags(FunctionFlags.RequiresInlining)
            .Build();

        yield return new FunctionOverloadBuilder("existingResource")
            .WithReturnType(TypeHelper.MakeNullable(LanguageConstants.Object))
            .WithReturnResultBuilder(GetExistingResourceReturnResult, TypeHelper.MakeNullable(LanguageConstants.Object))
            .WithGenericDescription("Returns the resource as it is defined immediately preceding the deployment if the deployment will update the resource, or null if the deployment will create the resource.")
            // The flag RequiresInlining is used here to indicate that this is an ARM runtime function to provide diagnostics where runtime values are blocked.
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
