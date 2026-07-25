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
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Bicep.Core.PrettyPrintV2;

namespace Bicep.LanguageServer.Refactor;

/// <summary>
/// Offers quick fixes to declare missing symbols reported as BCP057.
/// </summary>
public class UndefinedSymbolCodeFixProvider : ICodeFixProvider
{
    private const string DiagnosticCode = "BCP057";

    public UndefinedSymbolCodeFixProvider(SemanticModel semanticModel)
    {
        // Constructor kept for ICodeFixProvider interface compatibility
    }

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

        // Track seen symbol names to avoid generating duplicate fixes for the same identifier
        HashSet<string> seen = new(StringComparer.Ordinal);

        foreach (var variableAccess in variableAccesses)
        {
            var moduleParameterTypeString = TryGetModuleParameterTypeString(semanticModel, variableAccess);
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

            // New declarations must be inserted before a statement; if the access isn't
            // inside one (e.g., top-level expression), we can't determine insertion point
            if (semanticModel.Binder.GetNearestAncestor<StatementSyntax>(variableAccess) is not { } parentStatement)
            {
                continue;
            }

            var contextualType = TypeHelper.NullIfErrorOrAny(semanticModel.GetDeclaredType(variableAccess));
            var declaredAssignment = semanticModel.GetDeclaredTypeAssignment(variableAccess);
            var declaredAssignmentType = TypeHelper.NullIfErrorOrAny(declaredAssignment?.Reference.Type);
            var inferredType = TypeHelper.NullIfErrorOrAny(semanticModel.GetTypeInfo(variableAccess));
            var contextInferredType = InferByContext(semanticModel, variableAccess);
            var effectiveType = declaredAssignmentType ?? contextualType ?? inferredType ?? contextInferredType;

            // Type resolution priority for generated parameters:
            // 1. Module parameter types (preserve exact type from referenced module)
            // 2. Clear usage context (bool in conditions, int in arithmetic)
            // 3. Resource-derived types (e.g., resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.sku)
            // 4. User-defined type aliases (preserve named types when possible)
            // 5. Fallback to TypeStringifier with medium strictness
            string parameterTypeString;
            if (moduleParameterTypeString is not null)
            {
                parameterTypeString = moduleParameterTypeString;
            }
            else if (contextInferredType is BooleanType or IntegerType)
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

            // Variable declarations use SyntaxFactory so no string-based initializer needed

            var prettyPrintContext = PrettyPrinterV2Context.From(semanticModel);

            foreach (var fix in CreateQuickFixes(semanticModel, prettyPrintContext, parentStatement, name, parameterTypeString, effectiveType))
            {
                results.Add(fix);
            }
        }

        return results;
    }

    private static IEnumerable<CodeFix> CreateQuickFixes(
        SemanticModel semanticModel,
        PrettyPrinterV2Context prettyPrintContext,
        StatementSyntax parentStatement,
        string name,
        string parameterTypeString,
        TypeSymbol? effectiveType)
    {
        // Parameter declaration
        var (parameterInsertionOffset, _, _) = DeclarationInsertionHelper.FindOffsetToInsertNewDeclaration(
            semanticModel.SourceFile,
            parentStatement,
            parentStatement.Span.Position,
            typeof(ParameterDeclarationSyntax));
        var parameterText = BuildDeclarationText(
            semanticModel,
            prettyPrintContext,
            parameterInsertionOffset,
            $"param {name} {parameterTypeString}");
        yield return new CodeFix(
            $"Create parameter '{name}'",
            isPreferred: false,
            CodeFixKind.QuickFix,
            new CodeReplacement(new TextSpan(parameterInsertionOffset, 0), parameterText));

        // Variable declaration via SyntaxFactory
        var (variableInsertionOffset, _, _) = DeclarationInsertionHelper.FindOffsetToInsertNewDeclaration(
            semanticModel.SourceFile,
            parentStatement,
            parentStatement.Span.Position,
            typeof(VariableDeclarationSyntax));
        var defaultInitializer = GetDefaultInitializer(effectiveType);
        var variableDeclaration = SyntaxFactory.CreateVariableDeclaration(name, defaultInitializer);
        var variableText = BuildDeclarationText(
            semanticModel,
            prettyPrintContext,
            variableInsertionOffset,
            PrettyPrinterV2.Print(variableDeclaration, prettyPrintContext).TrimEnd());
        yield return new CodeFix(
            $"Create variable '{name}'",
            isPreferred: false,
            CodeFixKind.QuickFix,
            new CodeReplacement(new TextSpan(variableInsertionOffset, 0), variableText));
    }

    /// <summary>
    /// Builds the text for a new declaration with appropriate spacing.
    /// Ensures consistent formatting: adds newlines to create one blank line
    /// between the new declaration and existing code.
    /// </summary>
    private static string BuildDeclarationText(SemanticModel semanticModel, PrettyPrinterV2Context prettyPrintContext, int insertionOffset, string declaration)
    {
        var existingNewlines = CountConsecutiveNewlinesAtOffset(semanticModel, insertionOffset, prettyPrintContext.Newline);

        // Target spacing: one blank line after the declaration (2 newlines total).
        // If we're already on a blank line, add just enough to separate cleanly.
        var totalDesiredNewlines = 2;
        var additionalNewlines = Math.Max(totalDesiredNewlines - existingNewlines, 1);
        var spacing = string.Concat(Enumerable.Repeat(prettyPrintContext.Newline, additionalNewlines));

        return $"{declaration}{spacing}";
    }

    private static int CountConsecutiveNewlinesAtOffset(SemanticModel semanticModel, int insertionOffset, string newline)
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

    private static string GetTypeString(TypeSymbol? type)
    {
        // Use TypeStringifier for consistent type string generation
        // Medium strictness gives us reasonable types (e.g., 'int' instead of literal '123')
        // Remove top-level nullability since parameters should typically not be nullable by default
        return TypeStringifier.Stringify(type, typeProperty: null, TypeStringifier.Strictness.Medium, removeTopLevelNullability: true);
    }

    private static SyntaxBase GetDefaultInitializer(TypeSymbol? type)
    {
        return GetDefaultInitializerCore(type, []);
    }

    private static SyntaxBase GetDefaultInitializerCore(TypeSymbol? type, HashSet<TypeSymbol> visitedTypes)
    {
        if (type is null)
        {
            return SyntaxFactory.CreateStringLiteral(string.Empty);
        }

        // Prevent infinite recursion for recursive types
        if (visitedTypes.Contains(type))
        {
            return SyntaxFactory.CreateObject([]);
        }

        // Handle nullable types - use the non-null default
        if (TypeHelper.TryRemoveNullability(type) is TypeSymbol nonNullableType)
        {
            return GetDefaultInitializerCore(nonNullableType, visitedTypes);
        }

        return type switch
        {
            BooleanLiteralType boolLit => SyntaxFactory.CreateBooleanLiteral(boolLit.Value),
            BooleanType => SyntaxFactory.CreateBooleanLiteral(false),
            IntegerLiteralType intLit => SyntaxFactory.CreatePositiveOrNegativeInteger(intLit.Value),
            IntegerType => SyntaxFactory.CreateIntegerLiteral(0),
            StringLiteralType strLit => SyntaxFactory.CreateStringLiteral(strLit.RawStringValue),
            StringType => SyntaxFactory.CreateStringLiteral(string.Empty),
            ArrayType or TypedArrayType or TupleType => SyntaxFactory.CreateArray([]),
            ObjectType objectType => GetDefaultInitializerForObject(objectType, visitedTypes),
            UnionType union => GetDefaultInitializerForUnion(union, visitedTypes),
            _ => SyntaxFactory.CreateStringLiteral(string.Empty)
        };
    }

    private static SyntaxBase GetDefaultInitializerForObject(ObjectType objectType, HashSet<TypeSymbol> visitedTypes)
    {
        var writeableProperties = objectType.Properties.Values
            .Where(p => !p.Flags.HasFlag(TypePropertyFlags.ReadOnly))
            .ToArray();

        // For empty objects or objects with only optional properties, just use {}
        if (writeableProperties.Length == 0)
        {
            return SyntaxFactory.CreateObject([]);
        }

        // Limit object expansion to 5 properties to keep generated code readable;
        // larger objects are better left as {} for manual population
        if (writeableProperties.Length > 5)
        {
            return SyntaxFactory.CreateObject([]);
        }

        visitedTypes = [.. visitedTypes, objectType];

        var properties = writeableProperties
            .Select(p => SyntaxFactory.CreateObjectProperty(
                p.Name,
                GetDefaultInitializerCore(p.TypeReference.Type, visitedTypes)));

        return SyntaxFactory.CreateObject(properties);
    }

    private static SyntaxBase GetDefaultInitializerForUnion(UnionType union, HashSet<TypeSymbol> visitedTypes)
    {
        // For unions, try to pick a reasonable default from the first non-null member
        var firstNonNullMember = union.Members.FirstOrDefault(m => m.Type is not NullType)?.Type;
        return firstNonNullMember is not null
            ? GetDefaultInitializerCore(firstNonNullMember, visitedTypes)
            : SyntaxFactory.CreateNullLiteral();
    }

    /// <summary>
    /// Detects when the undefined symbol is used as a resource property value,
    /// allowing generation of resource-derived types.
    /// Delegates to <see cref="TypeStringifier.TryGetResourceDerivedTypeName"/> to avoid duplicating
    /// the resource-derived type path-building logic.
    /// </summary>
    private static string? TryGetResourceInputTypeString(SemanticModel semanticModel, VariableAccessSyntax variableAccess)
    {
        // Find the nearest parent ObjectPropertySyntax — that's the starting point
        // TypeStringifier.TryGetResourceDerivedTypeName expects.
        var parentProperty = semanticModel.Binder.GetNearestAncestor<ObjectPropertySyntax>(variableAccess);
        if (parentProperty is null)
        {
            return null;
        }

        return TypeStringifier.TryGetResourceDerivedTypeName(semanticModel, parentProperty, includeLeafProperties: true);
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

    private static bool SpansOverlap(int requestStart, int requestEnd, TextSpan span) =>
        requestStart <= span.GetEndPosition() && requestEnd >= span.Position;

    /// <summary>
    /// Checks if the child syntax node is contained within the parent syntax node's span.
    /// This is used instead of reference equality when walking up the syntax tree,
    /// because the child may be wrapped in intermediate nodes (e.g., parentheses).
    /// Uses <see cref="IPositionableExtensions.IsOverlapping(IPositionable,int)"/> to
    /// avoid duplicating span comparison logic.
    /// </summary>
    private static bool IsContainedIn(SyntaxBase child, SyntaxBase parent)
    {
        var span = child.Span;
        return parent.IsOverlapping(span.Position) &&
               parent.IsOverlapping(span.GetEndPosition());
    }

    private static string? TryGetModuleParameterTypeString(SemanticModel semanticModel, VariableAccessSyntax variableAccess)
    {
        string? moduleParameterName = null;
        bool insideModuleParams = false;
        SyntaxBase? current = variableAccess;

        while (current is not null)
        {
            if (current is ObjectPropertySyntax propertySyntax && propertySyntax.TryGetKeyText() is string propertyName)
            {
                moduleParameterName ??= propertyName;

                if (LanguageConstants.IdentifierComparer.Equals(propertyName, LanguageConstants.ModuleParamsPropertyName))
                {
                    insideModuleParams = true;
                }
            }

            if (current is ModuleDeclarationSyntax moduleDeclaration)
            {
                if (!insideModuleParams || moduleParameterName is null)
                {
                    return null;
                }

                if (semanticModel.Binder.GetSymbolInfo(moduleDeclaration) is not ModuleSymbol moduleSymbol)
                {
                    return null;
                }

                if (!moduleSymbol.TryGetSemanticModel().IsSuccess(out var moduleSemanticModel, out _))
                {
                    return null;
                }

                if (!moduleSemanticModel.Parameters.TryGetValue(moduleParameterName, out var parameterMetadata))
                {
                    return null;
                }

                if (moduleSemanticModel is SemanticModel bicepSemanticModel &&
                    bicepSemanticModel.SourceFile is BicepSourceFile moduleSourceFile)
                {
                    var paramSyntax = moduleSourceFile.ProgramSyntax.Declarations
                        .OfType<ParameterDeclarationSyntax>()
                        .FirstOrDefault(p => LanguageConstants.IdentifierComparer.Equals(p.Name.IdentifierName, moduleParameterName));

                    if (paramSyntax?.Type is { } typeSyntax)
                    {
                        var typeText = moduleSourceFile.Text.Substring(typeSyntax.Span.Position, typeSyntax.Span.Length).Trim();
                        if (!string.IsNullOrWhiteSpace(typeText))
                        {
                            return typeText;
                        }
                    }
                }

                var parameterType = TypeHelper.TryRemoveNullability(parameterMetadata.TypeReference.Type) ?? parameterMetadata.TypeReference.Type;

                if (parameterType is IUnresolvedResourceDerivedType unresolvedResourceDerivedType)
                {
                    return TypeStringifier.FormatResourceDerivedType(unresolvedResourceDerivedType);
                }

                return GetTypeString(parameterType);
            }

            current = semanticModel.Binder.GetParent(current);
        }

        return null;
    }

    /// <summary>
    /// Infers type from usage context when the semantic model doesn't provide one.
    /// Checks (in order): comparison with literals, boolean context, arithmetic,
    /// string interpolation, for-loop iteration, and parent property type.
    /// </summary>
    private static TypeSymbol? InferByContext(SemanticModel semanticModel, VariableAccessSyntax variableAccess)
    {
        if (InferFromComparisonWithLiteral(semanticModel, variableAccess) is { } comparisonType)
        {
            return comparisonType;
        }

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

        // Walk ancestors to find a non-error declared type that applies to this
        // expression position, handling cases where the access is wrapped in
        // neutral syntax nodes (e.g., parentheses).
        SyntaxBase? current = semanticModel.Binder.GetParent(variableAccess);
        while (current is not null)
        {
            var declaredType = TypeHelper.NullIfErrorOrAny(semanticModel.GetDeclaredType(current));
            if (declaredType is not null)
            {
                return declaredType;
            }

            current = semanticModel.Binder.GetParent(current);
        }

        return null;
    }

    /// <summary>
    /// Returns true for any context where a boolean value is expected or implied,
    /// including: ternary conditions, negation, logical operators, comparisons,
    /// and if-condition expressions.
    /// </summary>
    private static bool IsBooleanContext(SemanticModel model, VariableAccessSyntax access)
    {
        SyntaxBase? current = access;
        while (current is not null)
        {
            if (current is TernaryOperationSyntax ternary && IsContainedIn(access, ternary.ConditionExpression))
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
            if (current is IfConditionSyntax ifCondition && IsContainedIn(access, ifCondition.ConditionExpression))
            {
                return true;
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
            // Use span-based containment to check if access is within any interpolation expression
            // This handles cases where access is wrapped (e.g., in parentheses)
            if (current is StringSyntax stringSyntax && stringSyntax.Expressions.Any(expr => IsContainedIn(access, expr)))
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

    /// <summary>
    /// When comparing with a literal (e.g., <c>x == 'foo'</c> or <c>x > 5</c>),
    /// infers the variable's type from the literal's type.
    /// </summary>
    private static TypeSymbol? InferFromComparisonWithLiteral(SemanticModel model, VariableAccessSyntax access)
    {
        SyntaxBase? current = access;
        while (current is not null)
        {
            // for ==, !=, <, <=, >, >= pick the other operand's primitive type
            if (current is BinaryOperationSyntax binary &&
                binary.Operator is BinaryOperator.Equals or BinaryOperator.NotEquals or BinaryOperator.LessThan or BinaryOperator.LessThanOrEqual or BinaryOperator.GreaterThan or BinaryOperator.GreaterThanOrEqual)
            {
                // Use span-based containment to determine which operand contains the access
                // This handles cases where access is wrapped (e.g., in parentheses)
                var otherExpression = IsContainedIn(access, binary.LeftExpression) ? binary.RightExpression : binary.LeftExpression;
                var otherType = model.GetTypeInfo(otherExpression);

                if (TypeHelper.TryGetArmPrimitiveType(otherType) is { } primitiveType &&
                    primitiveType is StringType or IntegerType or BooleanType)
                {
                    return primitiveType;
                }
            }

            current = model.Binder.GetParent(current);
        }

        return null;
    }

}

