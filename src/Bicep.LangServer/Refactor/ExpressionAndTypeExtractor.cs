// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Text.RegularExpressions;
using Bicep.Core;
using Bicep.Core.CodeAction;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Model;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static Bicep.LanguageServer.Refactor.TypeStringifier;
using static Bicep.LanguageServer.Telemetry.BicepTelemetryEvent;
using Type = System.Type;

namespace Bicep.LanguageServer.Refactor;

// Provides code actions/fixes for extracting variables, parameters and types in a Bicep document
public class ExpressionAndTypeExtractor : ICodeFixProvider
{
    private const int MaxExpressionLengthInCodeAction = 45;
    private static readonly Regex regexCompactWhitespace = new("\\s+", RegexOptions.Compiled);

    private readonly SemanticModel semanticModel;

    private record ExtractionContext(
        StatementSyntax ParentStatement, // The statement containing the extraction context
        ExpressionSyntax ExpressionSyntax, // The expression to be extracted
        ObjectPropertySyntax? PropertySyntax, // The property syntax containing the expression, if any
        TypeProperty? TypeProperty, // The type property for PropertySyntax
        string? ContextDerivedName  // Suggested name based on context
    );

    public ExpressionAndTypeExtractor(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
    }

    private string NewLine => semanticModel.Configuration.Formatting.Data.NewlineKind.ToEscapeSequence();

    public IEnumerable<CodeFix> GetFixes(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes)
    {
        if (TryGetExtractionContext(semanticModel, [.. matchingNodes]) is { } extractionContext)
        {
            return CreateAllExtractions(extractionContext);
        }
        else
        {
            return [];
        }
    }

    private static ExtractionContext? TryGetExtractionContext(SemanticModel model, List<SyntaxBase> nodesInRange)
    {
        if (SyntaxMatcher.FindLastNodeOfType<ExpressionSyntax, ExpressionSyntax>(nodesInRange) is not (ExpressionSyntax expressionSyntax, _))
        {
            return null;
        }

        ObjectPropertySyntax? containingProperty = null;
        TypeProperty? containingTypeProperty = null;
        string? contextDerivedNewName = null;

        // Pick a semi-intelligent default name for the new param and variable.
        // Also, adjust the target expression to replace if a property itself has been selected (as opposed to its value)

        if (model.Binder.GetParent(expressionSyntax) is ObjectPropertySyntax propertySyntax
            && propertySyntax.TryGetKeyText() is string propertyName)
        {
            // `{ objectPropertyName: <<expression>> }` // entire property value expression selected
            //   -> default to the name "objectPropertyName"
            contextDerivedNewName = propertyName;
            containingProperty = propertySyntax;
            containingTypeProperty = containingProperty.TryGetTypeProperty(model);
        }
        else if (expressionSyntax is ObjectPropertySyntax propertySyntax2
            && propertySyntax2.TryGetKeyText() is string propertyName2)
        {
            // `{ <<objectPropertyName>>: expression }` // property itself is selected
            //   -> default to the name "objectPropertyName"
            contextDerivedNewName = propertyName2;

            // The expression we want to replace is the property value, not the property syntax
            var propertyValueSyntax = propertySyntax2.Value as ExpressionSyntax;
            if (propertyValueSyntax != null)
            {
                expressionSyntax = propertyValueSyntax;
                containingProperty = propertySyntax2;
                containingTypeProperty = containingProperty.TryGetTypeProperty(model);
            }
            else
            {
                return null;
            }
        }
        else if (expressionSyntax is PropertyAccessSyntax propertyAccessSyntax)
        {
            // `object.topPropertyName.propertyName`
            //   -> default to the name "topPropertyNamePropertyName"
            //
            // `object.topPropertyName.propertyName`
            //   -> default to the name "propertyName"
            //
            // More than two levels is less likely to be desirable

            string lastPartName = propertyAccessSyntax.PropertyName.IdentifierName;
            var parent = propertyAccessSyntax.BaseExpression;
            string? firstPartName = parent switch
            {
                PropertyAccessSyntax propertyAccess => propertyAccess.PropertyName.IdentifierName,
                VariableAccessSyntax variableAccess => variableAccess.Name.IdentifierName,
                FunctionCallSyntax functionCall => functionCall.Name.IdentifierName,
                _ => null
            };

            contextDerivedNewName = firstPartName is { } ? firstPartName + lastPartName.UppercaseFirstLetter() : lastPartName;
        }

        if (model.Binder.GetNearestAncestor<StatementSyntax>(expressionSyntax) is not StatementSyntax parentStatement)
        {
            return null;
        }

        return new ExtractionContext(parentStatement, expressionSyntax, containingProperty, containingTypeProperty, contextDerivedNewName);
    }

    private IEnumerable<CodeFixWithCommand> CreateAllExtractions(ExtractionContext extractionContext)
    {
        // For the new param's type, try to use the declared type if there is one (i.e. the type of
        //   what we're assigning to), otherwise use the actual calculated type of the expression
        var inferredType = semanticModel.GetTypeInfo(extractionContext.ExpressionSyntax);
        var declaredType = semanticModel.GetDeclaredType(extractionContext.ExpressionSyntax);
        var newParamType = TypeHelper.NullIfErrorOrAny(declaredType) ?? TypeHelper.NullIfErrorOrAny(inferredType);

        // Don't create nullable params - they're not allowed to have default values
        const bool ignoreTopLevelNullability = true;

        // Strict typing for the param doesn't appear useful, providing only loose and medium at the moment
        var stringifiedLooseType = Stringify(newParamType, extractionContext.TypeProperty, Strictness.Loose, ignoreTopLevelNullability);
        var stringifiedUserDefinedType = Stringify(newParamType, extractionContext.TypeProperty, Strictness.Medium, ignoreTopLevelNullability);
        var resourceDerivedType = extractionContext.PropertySyntax is null ? null : TryGetResourceDerivedTypeName(semanticModel, extractionContext.PropertySyntax);

        var simpleTypeAvailable = true;
        var userDefinedTypeAvailable = !string.Equals(stringifiedLooseType, stringifiedUserDefinedType, StringComparison.Ordinal);

        ExtractKindsAvailable extractKindsAvailable = new(
            simpleTypeAvailable: simpleTypeAvailable,
            userDefinedTypeAvailable: userDefinedTypeAvailable,
            resourceDerivedTypeAvailable: resourceDerivedType is not null);

        yield return CreateExtraction(
            extractionContext,
            ExtractionKind.Variable,
            "Extract variable",
            null,
            extractKindsAvailable);

        if (simpleTypeAvailable)
        {
            yield return CreateExtraction(
                extractionContext,
                ExtractionKind.SimpleParam,
                $"Extract parameter of type {GetQuotedText(stringifiedLooseType)}",
                stringifiedLooseType,
                extractKindsAvailable);
        }

        if (userDefinedTypeAvailable)
        {
            yield return CreateExtraction(
                extractionContext,
                ExtractionKind.UserDefParam,
                $"Extract parameter of type {GetQuotedText(stringifiedUserDefinedType)}",
                stringifiedUserDefinedType,
                extractKindsAvailable);
        }

        if (resourceDerivedType is not null)
        {
            yield return CreateExtraction(
                extractionContext,
                ExtractionKind.ResDerivedParam,
                $"Extract parameter of type {GetQuotedText(resourceDerivedType!)}",
                resourceDerivedType,
                extractKindsAvailable);
        }

        if (userDefinedTypeAvailable)
        {
            yield return CreateExtraction(
                extractionContext,
                ExtractionKind.Type,
                $"Create user-defined type for {GetQuotedText(stringifiedUserDefinedType)}",
                stringifiedUserDefinedType,
                extractKindsAvailable);
        }
    }

    private CodeFixWithCommand CreateExtraction(
        ExtractionContext extractionContext,
        ExtractionKind kind,
        string title,
        string? stringifiedType,
        ExtractKindsAvailable extractKindsAvailable)
    {
        string defaultNoncontextualName;
        string declarationKeywordPlusSpace;
        Type declarationStatementSyntaxType;
        switch (kind)
        {
            case ExtractionKind.Variable:
                defaultNoncontextualName = "newVariable";
                declarationKeywordPlusSpace = "var ";
                declarationStatementSyntaxType = typeof(VariableDeclarationSyntax);
                Debug.Assert(stringifiedType == null);
                break;
            case ExtractionKind.SimpleParam or ExtractionKind.UserDefParam or ExtractionKind.ResDerivedParam:
                defaultNoncontextualName = "newParameter";
                declarationKeywordPlusSpace = "param ";
                declarationStatementSyntaxType = typeof(ParameterDeclarationSyntax);
                Debug.Assert(stringifiedType != null);
                break;
            case ExtractionKind.Type:
                defaultNoncontextualName = "newType";
                declarationKeywordPlusSpace = "type ";
                declarationStatementSyntaxType = typeof(TypeDeclarationSyntax);
                Debug.Assert(stringifiedType != null);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ExtractionKind));
        }

        var parentStatementPosition = extractionContext.ParentStatement.Span.Position;
        var (declarationInsertionOffset, insertNewlineBefore, insertNewlineAfter) = DeclarationInsertionHelper.FindOffsetToInsertNewDeclaration(semanticModel.SourceFile, extractionContext.ParentStatement, parentStatementPosition, declarationStatementSyntaxType);
        var declarationInsertionPosition = TextCoordinateConverter.GetPosition(semanticModel.SourceFile.LineStarts, declarationInsertionOffset);

        var newName = FindUnusedValidName(extractionContext, defaultNoncontextualName);
        var newNamePlusSentinel = newName + "__NEW_NAME_SENTINEL__";

        StatementSyntax declarationSyntax = kind switch
        {
            ExtractionKind.Variable => SyntaxFactory.CreateVariableDeclaration(newNamePlusSentinel, extractionContext.ExpressionSyntax),
            ExtractionKind.SimpleParam or ExtractionKind.UserDefParam or ExtractionKind.ResDerivedParam => CreateNewParameterDeclaration(
                extractionContext, stringifiedType!, newNamePlusSentinel, extractionContext.ExpressionSyntax),
            ExtractionKind.Type => SyntaxFactory.CreateTypeDeclaration(newNamePlusSentinel, SyntaxFactory.CreateIdentifierWithTrailingSpace(stringifiedType!)),
            _ => throw new ArgumentOutOfRangeException(nameof(ExtractionKind))
        };

        var declarationTextLines = PrettyPrintDeclaration(declarationSyntax).TrimEnd() + NewLine;
        if (insertNewlineBefore)
        {
            declarationTextLines = NewLine + declarationTextLines;
        }
        if (insertNewlineAfter)
        {
            declarationTextLines += NewLine;
        }

        // Declaration text can have leading nodes. Find actual location of the declaration and identifier inside the printed lines
        //   so we know where to request a rename.
        var relativeIdentifierOffsetInDeclaration = declarationTextLines.IndexOf(declarationKeywordPlusSpace + newNamePlusSentinel);
        Debug.Assert(relativeIdentifierOffsetInDeclaration >= 0);
        relativeIdentifierOffsetInDeclaration += declarationKeywordPlusSpace.Length;
        declarationTextLines = declarationTextLines.Replace(newNamePlusSentinel, newName);

        // Find the line/character position of the new identifier inside the declaration, this is where we will ask
        //   the host to show a popup for a rename of the new identifier.
        // This position is in terms of the text after the extraction replacements are made (note that
        //   the new declaration is always added above the line where the expression may be extracted).
        var relativeIdentifierPositionInDeclaration = TextCoordinateConverter.GetPosition(
            TextCoordinateConverter.GetLineStarts(declarationTextLines),
            relativeIdentifierOffsetInDeclaration);
        var absoluteIdentifierPosition = new Position()
        {
            Line = declarationInsertionPosition.line + relativeIdentifierPositionInDeclaration.line,
            Character = declarationInsertionPosition.character + relativeIdentifierPositionInDeclaration.character
        };

        CodeReplacement[] replacements = [new CodeReplacement(new TextSpan(declarationInsertionOffset, 0), declarationTextLines)];
        if (kind != ExtractionKind.Type)
        {
            replacements = [.. replacements, new CodeReplacement(extractionContext.ExpressionSyntax.Span, newName)];
        }

        return CodeFixWithCommand.CreateWithPostExtractionCommand(
            title,
            isPreferred: false,
            CodeFixKind.RefactorExtract,
            replacements,
            semanticModel.SourceFile.FileHandle.Uri,
            absoluteIdentifierPosition,
            telemetryEvent: BicepTelemetryEvent.ExtractionRefactoring(
                kind,
                extractKindsAvailable));
    }

    private ParameterDeclarationSyntax CreateNewParameterDeclaration(
        ExtractionContext extractionContext,
        string stringifiedNewParamType,
        string newParamName,
        SyntaxBase defaultValueSyntax)
    {
        var newParamTypeIdentifier = SyntaxFactory.CreateIdentifierWithTrailingSpace(stringifiedNewParamType);

        var description = extractionContext.TypeProperty?.Description;
        SyntaxBase[]? leadingNodes = description == null
            ? null
            : [
                SyntaxFactory.CreateDecorator(
                    "description",
                    SyntaxFactory.CreateStringLiteral(description)),
                SyntaxFactory.GetNewlineToken()
               ];

        return SyntaxFactory.CreateParameterDeclaration(
            newParamName,
            new TypeVariableAccessSyntax(newParamTypeIdentifier),
            defaultValueSyntax,
            leadingNodes);
    }

    private string PrettyPrintDeclaration(StatementSyntax paramDeclarationSyntax)
    {
        var declarationText = PrettyPrinterV2.PrintValid(paramDeclarationSyntax, PrettyPrinterV2Options.Default);
        var p = new Parser(declarationText);
        var prettyDeclarationText = PrettyPrinterV2.PrintValid(p.Program(), PrettyPrinterV2Options.Default);
        return prettyDeclarationText;
    }

    private string FindUnusedValidName(ExtractionContext extractionContext, string defaultNonContextualName)
    {
        var preferredName = extractionContext.ContextDerivedName ?? defaultNonContextualName;

        var validName = new string(preferredName.Select(ch => IsValidChar(ch) ? ch : '_').ToArray());
        validName = validName.Length > 0 && IsValidInitialChar(validName[0]) ? validName : "_" + validName;

        string uniqueName = validName;
        int offset = extractionContext.ExpressionSyntax.Span.Position;
        var activeScopes = ActiveScopesVisitor.GetActiveScopes(semanticModel.Root, offset);
        for (int i = 1; i < int.MaxValue; ++i)
        {
            var tryingName = $"{uniqueName}{(i < 2 ? "" : i)}";
            if (!activeScopes.Any(s => s.GetDeclarationsByName(tryingName).Any()))
            {
                uniqueName = tryingName;
                break;
            }
        }

        bool IsValidChar(char ch) => char.IsAsciiLetterOrDigit(ch);
        bool IsValidInitialChar(char ch) => char.IsAsciiLetter(ch) || ch == '|';

        return uniqueName;
    }

    private string GetQuotedText(string text)
    {
        return "\""
            + regexCompactWhitespace.Replace(text, " ")
                .TruncateWithEllipses(MaxExpressionLengthInCodeAction)
                .Trim()
            + "\"";
    }

    public static (bool hasContent, bool hasComments) CheckLineContent(IReadOnlyList<int> lineStarts, SyntaxBase programSyntax, int line)
    {
        return StatementLineHelper.CheckLineContent(lineStarts, programSyntax, line);
    }

    public static int GetFirstLineOfStatementIncludingComments(IReadOnlyList<int> lineStarts, ProgramSyntax programSyntax, StatementSyntax statementSyntax)
    {
        return StatementLineHelper.GetFirstLineOfStatementIncludingComments(lineStarts, programSyntax, statementSyntax);
    }
}
