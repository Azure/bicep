// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core;
using Bicep.Core.CodeAction;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.LanguageServer.Completions;
using Bicep.Core.Text;
using Bicep.Core.PrettyPrintV2;

namespace Bicep.LanguageServer.Refactor;

/// <summary>
/// Offers quick fixes to declare missing symbols reported as BCP057.
/// </summary>
public class UndefinedSymbolCodeFixProvider : ICodeFixProvider
{
    private const string DiagnosticCode = "BCP057";

    private readonly SemanticModel semanticModel;

    public UndefinedSymbolCodeFixProvider(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
    }

    private string NewLine => semanticModel.Configuration.Formatting.Data.NewlineKind.ToEscapeSequence();

    public IEnumerable<CodeFix> GetFixes(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes)
    {
        try
        {
            return GetFixesInternal(semanticModel, matchingNodes);
        }
        catch
        {
            return [];
        }
    }

    private IEnumerable<CodeFix> GetFixesInternal(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes)
    {
        var results = new List<CodeFix>();

        var allDiagnostics = semanticModel.GetAllDiagnostics();
        var diagnostics = allDiagnostics.Where(diag => diag.Code == DiagnosticCode).ToArray();

        if (diagnostics.Length == 0 || matchingNodes.Count == 0)
        {
            return results;
        }

        // Collect variable accesses that are within the matching nodes (including descendants).
        // The LSP handler gives us a set of nodes spanning the requested range; for cases like
        // string interpolation or unary operators, the VariableAccess may be a descendant, not the node itself.
        IEnumerable<VariableAccessSyntax> variableAccesses = matchingNodes.OfType<VariableAccessSyntax>();

        if (!variableAccesses.Any())
        {
            var mostSpecificNode = matchingNodes.LastOrDefault();
            if (mostSpecificNode is not null)
            {
                variableAccesses = SyntaxAggregator.AggregateByType<VariableAccessSyntax>(mostSpecificNode)
                    .Where(variableAccess => diagnostics.Any(diag => SpansOverlap(variableAccess.Span.Position, variableAccess.GetEndPosition(), diag.Span)));
            }
            else
            {
                variableAccesses = Array.Empty<VariableAccessSyntax>();
            }
        }

        HashSet<string> seen = new(StringComparer.Ordinal);

        foreach (var variableAccess in variableAccesses)
        {
            var diagnostic = diagnostics.FirstOrDefault(diag => SpansOverlap(variableAccess.Span.Position, variableAccess.GetEndPosition(), diag.Span));
            if (diagnostic is null)
            {
                continue;
            }

            var name = variableAccess.Name.IdentifierName;
            if (!seen.Add(name) || !Lexer.IsValidIdentifier(name))
            {
                continue;
            }

            // Find the enclosing statement; skip if the access isn’t inside one
            if (semanticModel.Binder.GetNearestAncestor<StatementSyntax>(variableAccess) is not { } parentStatement)
            {
                continue;
            }

            var contextualType = NullIfErrorOrAny(semanticModel.GetDeclaredType(variableAccess));
            var declaredAssignment = semanticModel.GetDeclaredTypeAssignment(variableAccess);
            var declaredAssignmentType = NullIfErrorOrAny(declaredAssignment?.Reference.Type);
            var inferredType = NullIfErrorOrAny(semanticModel.GetTypeInfo(variableAccess));
            var contextInferredType = InferByContext(semanticModel, variableAccess);
            var effectiveType = declaredAssignmentType ?? contextualType ?? inferredType ?? contextInferredType;

            // Priority order for parameter types:
            // 1. If usage context clearly indicates a type (bool in condition, int in arithmetic), use that
            // 2. Otherwise, try resource-derived types (for complex resource properties)
            // 3. Then try named types
            // 4. Finally fall back to TypeStringifier
            string parameterTypeString;
            if (contextInferredType is BooleanType or IntegerType)
            {
                // Clear usage context - use the inferred primitive type
                parameterTypeString = GetTypeString(contextInferredType);
            }
            else
            {
                // No clear usage context - try resource-derived or complex types
                parameterTypeString = TryGetResourceInputTypeString(semanticModel, variableAccess)
                    ?? TryGetUserDefinedTypeName(semanticModel, variableAccess, declaredAssignment)
                    ?? GetTypeString(effectiveType);
            }

            // For variables, use simpler type string for initializer
            var variableTypeString = GetTypeString(effectiveType);

            foreach (var fix in CreateQuickFixes(parentStatement, name, parameterTypeString, variableTypeString, effectiveType, NewLine))
            {
                results.Add(fix);
            }
        }

        return results;
    }

    private IEnumerable<CodeFix> CreateQuickFixes(StatementSyntax parentStatement, string name, string parameterTypeString, string variableTypeString, TypeSymbol? effectiveType, string newline)
    {
        var parameterInsertionOffset = FindInsertionOffset(parentStatement, typeof(ParameterDeclarationSyntax));
        var parameterText = BuildDeclarationText(parameterInsertionOffset, $"param {name} {parameterTypeString}", newline);
        yield return new CodeFix(
            $"Create parameter '{name}'",
            isPreferred: false,
            CodeFixKind.QuickFix,
            new CodeReplacement(new TextSpan(parameterInsertionOffset, 0), parameterText));

        var variableInsertionOffset = FindInsertionOffset(parentStatement, typeof(VariableDeclarationSyntax));
        var defaultInitializer = GetDefaultInitializer(effectiveType);
        var variableText = BuildDeclarationText(variableInsertionOffset, $"var {name} = {defaultInitializer}", newline);
        yield return new CodeFix(
            $"Create variable '{name}'",
            isPreferred: false,
            CodeFixKind.QuickFix,
            new CodeReplacement(new TextSpan(variableInsertionOffset, 0), variableText));
    }

    private string BuildDeclarationText(int insertionOffset, string declaration, string newline)
    {
        var existingNewlines = CountConsecutiveNewlinesAtOffset(insertionOffset, newline);

        // Target spacing: one blank line after the declaration (2 newlines total).
        // If we're already on a blank line, add just enough to separate cleanly.
        var totalDesiredNewlines = 2;
        var additionalNewlines = Math.Max(totalDesiredNewlines - existingNewlines, 1);
        var spacing = string.Concat(Enumerable.Repeat(newline, additionalNewlines));

        return $"{declaration}{spacing}";
    }

    private int CountConsecutiveNewlinesAtOffset(int insertionOffset, string newline)
    {
        var lineStarts = semanticModel.SourceFile.LineStarts;
        if (lineStarts.Length == 0 || insertionOffset < 0 || insertionOffset > semanticModel.SourceFile.Text.Length)
        {
            return 0;
        }

        var (line, _) = TextCoordinateConverter.GetPosition(lineStarts, insertionOffset);
        if (line < 0 || line >= lineStarts.Length)
        {
            return 0;
        }

        var lineStart = lineStarts[line];
        var lineEnd = line + 1 < lineStarts.Length ? lineStarts[line + 1] : semanticModel.SourceFile.Text.Length;
        var lineLength = lineEnd - lineStart;

        // Count consecutive newline sequences starting at the offset.
        if (lineLength > newline.Length || lineEnd + newline.Length > semanticModel.SourceFile.Text.Length)
        {
            // Not a blank line or at end of file.
            return 0;
        }

        var text = semanticModel.SourceFile.Text;
        var count = 0;
        var cursor = insertionOffset;
        while (cursor + newline.Length <= text.Length && text.AsSpan(cursor).StartsWith(newline, StringComparison.Ordinal))
        {
            count++;
            cursor += newline.Length;
        }

        return count;
    }

    private int FindInsertionOffset(StatementSyntax anchorStatement, Type declarationType)
    {
        var sourceFile = semanticModel.SourceFile;
        var lineStarts = sourceFile.LineStarts;

        var existing = sourceFile.ProgramSyntax.Children.OfType<StatementSyntax>()
            .Where(s => s.GetType() == declarationType && s.Span.Position < anchorStatement.Span.Position)
            .OrderByDescending(s => s.Span.Position)
            .FirstOrDefault();

        if (existing is { })
        {
            var insertLine = TextCoordinateConverter.GetPosition(lineStarts, existing.GetEndPosition()).line + 1;
            return TextCoordinateConverter.GetOffset(lineStarts, insertLine, 0);
        }

        var anchorStartLine = ExpressionAndTypeExtractor.GetFirstLineOfStatementIncludingComments(
            lineStarts,
            sourceFile.ProgramSyntax,
            anchorStatement);
        return TextCoordinateConverter.GetOffset(lineStarts, anchorStartLine, 0);
    }

    private static string GetTypeString(TypeSymbol? type)
    {
        // Use TypeStringifier for consistent type string generation
        // Medium strictness gives us reasonable types (e.g., 'int' instead of literal '123')
        // Remove top-level nullability since parameters should typically not be nullable by default
        return TypeStringifier.Stringify(type, typeProperty: null, TypeStringifier.Strictness.Medium, removeTopLevelNullability: true);
    }

    private static string GetDefaultInitializer(TypeSymbol? type)
    {
        return GetDefaultInitializerCore(type, []);
    }

    private static string GetDefaultInitializerCore(TypeSymbol? type, HashSet<TypeSymbol> visitedTypes)
    {
        if (type is null)
        {
            return "''";
        }

        // Prevent infinite recursion for recursive types
        if (visitedTypes.Contains(type))
        {
            return "{}";
        }

        // Handle nullable types - use the non-null default
        if (TypeHelper.TryRemoveNullability(type) is TypeSymbol nonNullableType)
        {
            return GetDefaultInitializerCore(nonNullableType, visitedTypes);
        }

        return type switch
        {
            BooleanLiteralType boolLit => boolLit.Value.ToString().ToLowerInvariant(),
            BooleanType => "false",
            IntegerLiteralType intLit => intLit.Value.ToString(),
            IntegerType => "0",
            StringLiteralType strLit => StringUtils.EscapeBicepString(strLit.RawStringValue),
            StringType => "''",
            ArrayType or TypedArrayType or TupleType => "[]",
            ObjectType objectType => GetDefaultInitializerForObject(objectType, visitedTypes),
            UnionType union => GetDefaultInitializerForUnion(union, visitedTypes),
            _ => "''"
        };
    }

    private static string GetDefaultInitializerForObject(ObjectType objectType, HashSet<TypeSymbol> visitedTypes)
    {
        var writeableProperties = objectType.Properties.Values
            .Where(p => !p.Flags.HasFlag(TypePropertyFlags.ReadOnly))
            .ToArray();

        // For empty objects or objects with only optional properties, just use {}
        if (writeableProperties.Length == 0)
        {
            return "{}";
        }

        // Don't generate full objects for types that are too complex or have many properties
        // Keep it simple and readable
        if (writeableProperties.Length > 5)
        {
            return "{}";
        }

        visitedTypes = [.. visitedTypes, objectType];

        var properties = writeableProperties
            .Select(p =>
            {
                var propName = StringUtils.EscapeBicepPropertyName(p.Name);
                var defaultValue = GetDefaultInitializerCore(p.TypeReference.Type, visitedTypes);
                return $"{propName}: {defaultValue}";
            });

        return $"{{ {string.Join(", ", properties)} }}";
    }

    private static string GetDefaultInitializerForUnion(UnionType union, HashSet<TypeSymbol> visitedTypes)
    {
        // For unions, try to pick a reasonable default from the first non-null member
        var firstNonNullMember = union.Members.FirstOrDefault(m => m.Type is not NullType)?.Type;
        return firstNonNullMember is not null ? GetDefaultInitializerCore(firstNonNullMember, visitedTypes) : "null";
    }

    private static string? TryGetResourceInputTypeString(SemanticModel semanticModel, VariableAccessSyntax variableAccess)
    {
        // Walk up to see if we're in a resource property assignment
        SyntaxBase? current = variableAccess;
        List<string> propertyPath = new();

        // Build the property path by walking up through ObjectPropertySyntax nodes
        while (current is not null)
        {
            current = semanticModel.Binder.GetParent(current);

            if (current is ObjectPropertySyntax objProp && objProp.TryGetKeyText() is string propName)
            {
                // Add property name to the front of the path (we're walking backwards)
                propertyPath.Insert(0, propName);
            }
            else if (current is ResourceDeclarationSyntax resourceDecl)
            {
                // We've reached the resource declaration
                // Generate resourceInput type for any resource property with a path
                if (propertyPath.Count == 0)
                {
                    // No property path found
                    return null;
                }

                // Try to get the resource type string
                if (resourceDecl.Type is StringSyntax stringSyntax &&
                    stringSyntax.TryGetLiteralValue() is string resourceTypeString)
                {
                    // Build the full type path: resourceInput<'Type@version'>.sku or .properties.encryption
                    var fullPath = string.Join(".", propertyPath);
                    return $"resourceInput<'{resourceTypeString}'>.{fullPath}";
                }
                break;
            }
        }

        return null;
    }

    private static string? TryGetUserDefinedTypeName(SemanticModel semanticModel, VariableAccessSyntax variableAccess, DeclaredTypeAssignment? assignment)
    {
        // If we already have a declared assignment with a type alias, prefer it
        if (assignment?.DeclaringSyntax is TypeVariableAccessSyntax declaredTypeAccess)
        {
            var declaredType = assignment.Reference.Type;
            if (TryGetUserDefinedTypeNameFromTypeSyntax(semanticModel, declaredTypeAccess, declaredType) is { } declaredTypeName)
            {
                return declaredTypeName;
            }
        }

        // Walk up from the variable access to find a parent declaration that carries a type alias
        SyntaxBase? current = variableAccess;
        while (current is not null)
        {
            current = semanticModel.Binder.GetParent(current);

            switch (current)
            {
                case OutputDeclarationSyntax outputDecl:
                    var outputType = semanticModel.GetDeclaredTypeAssignment(outputDecl)?.Reference.Type;
                    if (outputDecl.Type is { } outputTypeSyntax &&
                        TryGetUserDefinedTypeNameFromTypeSyntax(semanticModel, outputTypeSyntax, outputType) is { } outputAlias)
                    {
                        return outputAlias;
                    }
                    break;

                case ParameterDeclarationSyntax paramDecl:
                    var paramType = semanticModel.GetDeclaredTypeAssignment(paramDecl)?.Reference.Type;
                    if (paramDecl.Type is { } paramTypeSyntax &&
                        TryGetUserDefinedTypeNameFromTypeSyntax(semanticModel, paramTypeSyntax, paramType) is { } paramAlias)
                    {
                        return paramAlias;
                    }
                    break;

                case VariableDeclarationSyntax variableDecl:
                    var varType = semanticModel.GetDeclaredTypeAssignment(variableDecl)?.Reference.Type;
                    if (variableDecl.Type is { } varTypeSyntax &&
                        TryGetUserDefinedTypeNameFromTypeSyntax(semanticModel, varTypeSyntax, varType) is { } varAlias)
                    {
                        return varAlias;
                    }
                    break;

                case TypeVariableAccessSyntax typeAccess:
                    return TryGetUserDefinedTypeNameFromTypeSyntax(semanticModel, typeAccess, assignment?.Reference.Type);
            }
        }

        return null;
    }

    private static string? TryGetUserDefinedTypeNameFromTypeSyntax(SemanticModel semanticModel, SyntaxBase typeSyntax, TypeSymbol? typeSymbol)
    {
        if (typeSyntax is null)
        {
            return null;
        }

        return typeSyntax switch
        {
            NullableTypeSyntax nullable => TryGetUserDefinedTypeNameFromTypeSyntax(
                semanticModel,
                nullable.Base,
                typeSymbol is null ? null : TypeHelper.TryRemoveNullability(typeSymbol))
                is { } innerName
                ? $"{innerName}{((typeSymbol is null || TypeHelper.IsNullable(typeSymbol)) ? "?" : string.Empty)}"
                : null,

            TypeVariableAccessSyntax typeAccess => TryGetUserDefinedTypeNameFromTypeAccess(semanticModel, typeAccess, typeSymbol),
            _ => null,
        };
    }

    private static string? TryGetUserDefinedTypeNameFromTypeAccess(SemanticModel semanticModel, TypeVariableAccessSyntax typeAccess, TypeSymbol? typeSymbol)
    {
        var symbol = semanticModel.Binder.GetSymbolInfo(typeAccess);
        if (symbol is not TypeAliasSymbol typeAlias)
        {
            return null;
        }

        var isNullable = typeSymbol is not null && TypeHelper.IsNullable(typeSymbol);
        return isNullable ? $"{typeAlias.Name}?" : typeAlias.Name;
    }

    private static TypeSymbol? NullIfErrorOrAny(TypeSymbol? type) => type is ErrorType or AnyType ? null : type;

    private static bool SpansOverlap(int requestStart, int requestEnd, TextSpan span) =>
        requestStart <= span.GetEndPosition() && requestEnd >= span.Position;

    private static TypeSymbol? InferByContext(SemanticModel semanticModel, VariableAccessSyntax variableAccess)
    {
        // If used in a conditional/ternary/logical context, assume bool.
        if (IsBooleanContext(semanticModel, variableAccess))
        {
            return LanguageConstants.Bool;
        }

        // If used in arithmetic, assume int.
        if (IsArithmeticContext(semanticModel, variableAccess))
        {
            return LanguageConstants.Int;
        }

        // If used inside string interpolation, assume string.
        if (IsStringInterpolationContext(semanticModel, variableAccess))
        {
            return LanguageConstants.String;
        }

        // If used as the iterable expression in a for-loop, assume array.
        if (IsForExpressionContext(semanticModel, variableAccess))
        {
            return LanguageConstants.Array;
        }

        // Try to derive from enclosing property type (e.g., resource property).
        if (semanticModel.Binder.GetParent(variableAccess) is SyntaxBase parent)
        {
            var declaredType = semanticModel.GetDeclaredType(parent);
            if (declaredType is not null && declaredType is not ErrorType && declaredType is not AnyType)
            {
                return declaredType;
            }
        }

        return null;
    }

    private static bool IsBooleanContext(SemanticModel model, VariableAccessSyntax access)
    {
        SyntaxBase? current = access;
        while (current is not null)
        {
            if (current is TernaryOperationSyntax ternary && ReferenceEquals(ternary.ConditionExpression, access))
            {
                return true;
            }
            if (current is UnaryOperationSyntax unary && unary.Operator == UnaryOperator.Not)
            {
                return true;
            }
            if (current is BinaryOperationSyntax binary)
            {
                if (binary.Operator is BinaryOperator.LogicalAnd or BinaryOperator.LogicalOr or BinaryOperator.Equals or BinaryOperator.NotEquals or BinaryOperator.LessThan or BinaryOperator.LessThanOrEqual or BinaryOperator.GreaterThan or BinaryOperator.GreaterThanOrEqual)
                {
                    return true;
                }
            }
            if (current is IfConditionSyntax ifCondition)
            {
                // Check if the access is within the condition expression (not the body)
                if (access.Span.Position >= ifCondition.ConditionExpression.Span.Position &&
                    access.Span.GetEndPosition() <= ifCondition.ConditionExpression.Span.GetEndPosition())
                {
                    return true;
                }
            }

            current = model.Binder.GetParent(current);
        }

        return false;
    }

    private static bool IsArithmeticContext(SemanticModel model, VariableAccessSyntax access)
    {
        SyntaxBase? current = access;
        while (current is not null)
        {
            if (current is BinaryOperationSyntax binary &&
                binary.Operator is BinaryOperator.Add or BinaryOperator.Subtract or BinaryOperator.Multiply or BinaryOperator.Divide or BinaryOperator.Modulo)
            {
                return true;
            }
            current = model.Binder.GetParent(current);
        }

        return false;
    }

    private static bool IsStringInterpolationContext(SemanticModel model, VariableAccessSyntax access)
    {
        SyntaxBase? current = access;
        while (current is not null)
        {
            if (current is StringSyntax stringSyntax && stringSyntax.Expressions.Contains(access))
            {
                return true;
            }

            current = model.Binder.GetParent(current);
        }

        return false;
    }

    private static bool IsForExpressionContext(SemanticModel model, VariableAccessSyntax access)
    {
        SyntaxBase? current = access;
        while (current is not null)
        {
            if (current is ForSyntax forSyntax)
            {
                var exprSpan = forSyntax.Expression.Span;
                return access.Span.Position >= exprSpan.Position &&
                       access.Span.GetEndPosition() <= exprSpan.GetEndPosition();
            }

            current = model.Binder.GetParent(current);
        }

        return false;
    }
}

