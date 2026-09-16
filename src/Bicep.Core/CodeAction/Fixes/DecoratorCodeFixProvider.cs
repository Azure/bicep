// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

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
        if (matchingNodes.OfType<DecorableSyntax>().LastOrDefault() is not { } decorableSyntax || decorableSyntax.Decorators.Any(IsTargetDecorator))
        {
            yield break;
        }

        if (!decorator.Overload.Flags.HasFlag(GetRequiredFlags(decorableSyntax)))
        {
            yield break;
        }

        if (GetPotentialTargetType(semanticModel, decorableSyntax) is not { } targetType || !decorator.CanAttachTo(targetType))
        {
            yield break;
        }

        var decoratorSyntax = SyntaxFactory.CreateDecorator(decoratorName, GetEmptyParams());
        var codeReplacement = CreateCodeReplacement(semanticModel, decorableSyntax, decoratorSyntax);

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
        ExtensionDeclarationSyntax => FunctionFlags.ExtensionDecorator,
        MetadataDeclarationSyntax => FunctionFlags.MetadataDecorator,
        TypeDeclarationSyntax or ObjectTypePropertySyntax or ObjectTypeAdditionalPropertiesSyntax or TupleTypeItemSyntax => FunctionFlags.TypeDecorator,
        _ => FunctionFlags.AnyDecorator,
    };

    private TypeSymbol? GetPotentialTargetType(SemanticModel model, DecorableSyntax potentialTarget) => potentialTarget switch
    {
        // Members of explicitly declared aggregate types will not be bound to a specific symbol,
        // but the TypeManager will have cached the member's type.
        ObjectTypePropertySyntax or ObjectTypeAdditionalPropertiesSyntax or TupleTypeItemSyntax
            when model.GetDeclaredType(potentialTarget) is { } typeMemberType => typeMemberType,
        // Type declaration statements have a type of Type<T>, but decorators evaluate T (e.g., string, not Type<string>) to determine whether they can attach to a given type declaration
        TypeDeclarationSyntax typeDeclaration when model.GetDeclaredType(typeDeclaration) is { } declaredType => declaredType is TypeType typeType ? typeType.Unwrapped : declaredType,
        // All other statements should use their assigned type
        StatementSyntax declaration when model.GetSymbolInfo(declaration) is DeclaredSymbol symbol => symbol.Type,
        _ => null,
    };

    private static CodeReplacement CreateCodeReplacement(SemanticModel model, DecorableSyntax syntax, DecoratorSyntax decorator)
    {
        var formatting = model.Configuration.Formatting.Data;
        var newline = formatting.NewlineKind.ToEscapeSequence();
        var position = syntax.Span.Position;
        var (_, character) = TextCoordinateConverter.GetPosition(model.SourceFile.LineStarts, position);
        var linePrefix = model.SourceFile.Text[(position - character)..position];
        var indentation = linePrefix[..(linePrefix.Length - linePrefix.TrimStart(' ', '\t').Length)];

        if (indentation.Length == linePrefix.Length)
        {
            return new(new TextSpan(position, 0), $"{decorator}{newline}{indentation}");
        }

        indentation += formatting.IndentKind == IndentKind.Tab ? "\t" : new string(' ', formatting.IndentSize);

        var whitespaceLength = linePrefix.Length - linePrefix.TrimEnd(' ', '\t').Length;
        var replacementSpan = new TextSpan(position - whitespaceLength, whitespaceLength);

        return new(replacementSpan, $"{newline}{indentation}{decorator}{newline}{indentation}");
    }

    private SyntaxBase[] GetEmptyParams()
    {
        if (decorator.Overload.MinimumArgumentCount == 1 &&
            decorator.Overload.FixedParameters.Length == 1 &&
            decorator.Overload.FixedParameters[0].Required)
        {
            switch (decorator.Overload.FixedParameters[0].Type)
            {
                case ArrayType:
                    return new[] { SyntaxFactory.CreateArray([]) };
                case StringType:
                    return new[] { SyntaxFactory.CreateStringLiteral(String.Empty) };
            }
        }

        return [];
    }
}
