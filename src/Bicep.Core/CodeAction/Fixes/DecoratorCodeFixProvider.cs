// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.CodeAction.Fixes;

public class DecoratorCodeFixProvider : ICodeFixProvider
{
    private readonly string decoratorName;
    private readonly Decorator decorator;

    public DecoratorCodeFixProvider(string decoratorName, Decorator decorator)
    {
        this.decoratorName = decoratorName;
        this.decorator = decorator;
    }

    public IEnumerable<CodeFix> GetFixes(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes)
    {
        if (matchingNodes.OfType<DecorableSyntax>().FirstOrDefault() is not {} decorableSyntax || decorableSyntax.Decorators.Any(IsTargetDecorator))
        {
            yield break;
        }

        if(!decorator.Overload.Flags.HasFlag(GetRequiredFlags(decorableSyntax)))
        {
            yield break;
        }

        if (GetPotentialTargetType(semanticModel, decorableSyntax) is not {} targetType || !decorator.CanAttachTo(targetType))
        {
            yield break;
        }

        var decoratorSyntax = SyntaxFactory.CreateDecorator(decoratorName, GetEmptyParams());
        var decoratorText = $"{decoratorSyntax.ToText()}{Environment.NewLine}";
        var newSpan = new TextSpan(decorableSyntax.Span.Position, 0);
        var codeReplacement = new CodeReplacement(newSpan, decoratorText);

        yield return new CodeFix(
            $"Add @{decoratorName}",
            false,
            CodeFixKind.Refactor,
            codeReplacement);
    }

    private bool IsTargetDecorator(DecoratorSyntax decoratorSyntax)
        => decoratorSyntax.Expression is FunctionCallSyntax functionCallSyntax && functionCallSyntax.NameEquals(decoratorName);

    private FunctionFlags GetRequiredFlags(DecorableSyntax syntax) => syntax switch
    {
        ParameterDeclarationSyntax => FunctionFlags.ParameterDecorator,
        VariableDeclarationSyntax => FunctionFlags.VariableDecorator,
        ResourceDeclarationSyntax => FunctionFlags.ResourceDecorator,
        ModuleDeclarationSyntax => FunctionFlags.ModuleDecorator,
        OutputDeclarationSyntax => FunctionFlags.OutputDecorator,
        ImportDeclarationSyntax => FunctionFlags.ImportDecorator,
        MetadataDeclarationSyntax => FunctionFlags.MetadataDecorator,
        TypeDeclarationSyntax or ObjectTypePropertySyntax => FunctionFlags.TypeDecorator,
        _ => FunctionFlags.AnyDecorator,
    };

    private TypeSymbol? GetPotentialTargetType(SemanticModel model, DecorableSyntax potentialTarget) => potentialTarget switch
    {
        // The properties of explicitly declared object types will not be bound to a specific symbol, but the TypeManager will have cached the property's type
        ObjectTypePropertySyntax objectTypeProperty when model.GetDeclaredType(objectTypeProperty) is {} typePropertyType => typePropertyType,
        // Type declaration statements have a type of Type<T>, but decorators evaluate T (e.g., string, not Type<string>) to determine whether they can attach to a given type declaration
        TypeDeclarationSyntax typeDeclaration when model.GetDeclaredType(typeDeclaration) is {} declaredType => declaredType is TypeType typeType ? typeType.Unwrapped : declaredType,
        // All other statements should use their assigned type
        StatementSyntax declaration when model.GetSymbolInfo(declaration) is DeclaredSymbol symbol => symbol.Type,
        _ => null,
    };

    private SyntaxBase[] GetEmptyParams()
    {
        if (decorator.Overload.MinimumArgumentCount == 1 &&
            decorator.Overload.FixedParameters.Length == 1 &&
            decorator.Overload.FixedParameters[0].Required)
        {
            switch (decorator.Overload.FixedParameters[0].Type)
            {
                case ArrayType:
                    return new[] { SyntaxFactory.CreateArray(Enumerable.Empty<SyntaxBase>()) };
                case PrimitiveType pt when pt.Name == LanguageConstants.TypeNameString:
                    return new[] { SyntaxFactory.CreateStringLiteral(String.Empty) };
            }
        }

        return Array.Empty<SyntaxBase>();
    }
}
