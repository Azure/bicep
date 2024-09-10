// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing.Text;
using System.Text;
using System.Text.RegularExpressions;
using Azure.Core.GeoJson;
using Bicep.Core;
using Bicep.Core.CodeAction;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Model;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static Bicep.LanguageServer.Completions.BicepCompletionContext;
using static Bicep.LanguageServer.Refactor.TypeStringifier;
using static Google.Protobuf.Reflection.ExtensionRangeOptions.Types;
using Type = System.Type;

namespace Bicep.LanguageServer.Refactor;

// Provides code actions/fixes for extracting variables, parameters and types in a Bicep document
public class ExpressionAndTypeExtractor
{
    private const int MaxExpressionLengthInCodeAction = 45;
    private static Regex regexCompactWhitespace = new("\\s+", RegexOptions.Compiled);

    private CompilationContext compilationContext;
    private SemanticModel semanticModel;
    private readonly Uri documentUri;

    private record ExtractionContext(
        StatementSyntax ParentStatement, // The statement containing the extraction context
        ExpressionSyntax ExpressionSyntax, // The expression to be extracted
        TypeProperty? TypeProperty, // The property inside a parent object's type whose value is being extracted, if any
        string? ContextDerivedName  // Suggested name based on context
    );

    private enum ExtractionKind
    {
        Variable,
        Parameter,
        Type,
    }

    public ExpressionAndTypeExtractor(CompilationContext compilationContext, SemanticModel semanticModel, Uri documentUri)
    {
        this.compilationContext = compilationContext;
        this.semanticModel = semanticModel;
        this.documentUri = documentUri;
    }

    private string NewLine => semanticModel.Configuration.Formatting.Data.NewlineKind.ToEscapeSequence();

    public IEnumerable<CodeFixWithCommand> GetExtractionCodeFixes(List<SyntaxBase> nodesInRange)
    {
        var extractionContext = GetExtractionContext(nodesInRange);
        if (extractionContext is { })
        {
            return CreateAllExtractions(extractionContext);
        }
        else
        {
            return [];
        }
    }

    private ExtractionContext? GetExtractionContext(List<SyntaxBase> nodesInRange)
    {
        if (SyntaxMatcher.FindLastNodeOfType<ExpressionSyntax, ExpressionSyntax>(nodesInRange) is not (ExpressionSyntax expressionSyntax, _))
        {
            return null;
        }

        TypeProperty? parentTypeProperty = null;
        string? contextDerivedNewName = null;

        // Pick a semi-intelligent default name for the new param and variable.
        // Also, adjust the target expression to replace if a property itself has been selected (as opposed to its value)

        if (semanticModel.Binder.GetParent(expressionSyntax) is ObjectPropertySyntax propertySyntax
            && propertySyntax.TryGetKeyText() is string propertyName)
        {
            // `{ objectPropertyName: <<expression>> }` // entire property value expression selected
            //   -> default to the name "objectPropertyName"
            contextDerivedNewName = propertyName;
            parentTypeProperty = propertySyntax.TryGetTypeProperty(semanticModel);
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
                parentTypeProperty = propertySyntax2.TryGetTypeProperty(semanticModel);
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

        if (semanticModel.Binder.GetNearestAncestor<StatementSyntax>(expressionSyntax) is not StatementSyntax parentStatement)
        {
            return null;
        }

        return new ExtractionContext(parentStatement, expressionSyntax, parentTypeProperty, contextDerivedNewName);
    }

    private IEnumerable<CodeFixWithCommand> CreateAllExtractions(ExtractionContext extractionContext)
    {
        yield return CreateExtraction(extractionContext, ExtractionKind.Variable, "Extract variable");

        // For the new param's type, try to use the declared type if there is one (i.e. the type of
        //   what we're assigning to), otherwise use the actual calculated type of the expression
        var inferredType = semanticModel.GetTypeInfo(extractionContext.ExpressionSyntax);
        var declaredType = semanticModel.GetDeclaredType(extractionContext.ExpressionSyntax);
        var newParamType = NullIfErrorOrAny(declaredType) ?? NullIfErrorOrAny(inferredType);

        // Don't create nullable params - they're not allowed to have default values
        const bool ignoreTopLevelNullability = true;

        // Strict typing for the param doesn't appear useful, providing only loose and medium at the moment
        var stringifiedLooseType = Stringify(newParamType, extractionContext.TypeProperty, Strictness.Loose, ignoreTopLevelNullability);
        var stringifiedUserDefinedType = Stringify(newParamType, extractionContext.TypeProperty, Strictness.Medium, ignoreTopLevelNullability);

        var userDefinedTypeAvailable = !string.Equals(stringifiedLooseType, stringifiedUserDefinedType, StringComparison.Ordinal);

        yield return CreateExtraction(
            extractionContext,
            ExtractionKind.Parameter,
            $"Extract parameter of type {GetQuotedText(stringifiedLooseType)}",
            stringifiedLooseType);

        if (userDefinedTypeAvailable)
        {
            yield return CreateExtraction(
                extractionContext,
                ExtractionKind.Parameter,
                $"Extract parameter of type {GetQuotedText(stringifiedUserDefinedType)}",
                stringifiedUserDefinedType);

            yield return CreateExtraction(
                extractionContext,
                ExtractionKind.Type,
                $"Create type for {GetQuotedText(stringifiedUserDefinedType)}",
                stringifiedUserDefinedType);
        }
    }

    private CodeFixWithCommand CreateExtraction(
        ExtractionContext extractionContext,
        ExtractionKind kind,
        string title,
        string? stringifiedType = null
        )
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
                break;
            case ExtractionKind.Parameter:
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
        var declarationInsertionOffset = FindOffsetToInsertNewDeclarationOfType(compilationContext, parentStatementPosition, declarationStatementSyntaxType);
        var declarationInsertionPosition = TextCoordinateConverter.GetPosition(compilationContext.LineStarts, declarationInsertionOffset);

        var newName = FindUnusedValidName(extractionContext, defaultNoncontextualName);
        var newNamePlusSentinel = newName + "__NEW_NAME_SENTINEL__";

        StatementSyntax declarationSyntax = kind switch
        {
            ExtractionKind.Variable => SyntaxFactory.CreateVariableDeclaration(newNamePlusSentinel, extractionContext.ExpressionSyntax),
            ExtractionKind.Parameter => CreateNewParameterDeclaration(
                extractionContext, stringifiedType!, newNamePlusSentinel, extractionContext.ExpressionSyntax),
            ExtractionKind.Type => SyntaxFactory.CreateTypeDeclaration(newNamePlusSentinel, SyntaxFactory.CreateIdentifierWithTrailingSpace(stringifiedType!)),
            _ => throw new ArgumentOutOfRangeException(nameof(ExtractionKind))
        };

        var declarationTextLines = PrettyPrintDeclaration(declarationSyntax).TrimEnd() + NewLine;

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
            replacements = [..replacements, new CodeReplacement(extractionContext.ExpressionSyntax.Span, newName)];
        }
        return CodeFixWithCommand.CreateWithRenameCommand(
            title,
            isPreferred: false,
            CodeFixKind.RefactorExtract,
            replacements,
            this.documentUri,
            absoluteIdentifierPosition);
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

    private TypeSymbol? NullIfErrorOrAny(TypeSymbol? type) => type is ErrorType || type is AnyType ? null : type;

    private string FindUnusedValidName(ExtractionContext extractionContext, string defaultNonContextualName)
    {
        var preferredName = extractionContext.ContextDerivedName ?? defaultNonContextualName;

        var validName = new string(preferredName.Select(ch => IsValidChar(ch) ? ch : '_').ToArray());
        validName = validName.Length > 0 && IsValidInitialChar(validName[0]) ? validName : "_" + validName;

        string uniqueName = validName;
        int offset = extractionContext.ExpressionSyntax.Span.Position;
        var activeScopes = ActiveScopesVisitor.GetActiveScopes(compilationContext.Compilation.GetEntrypointSemanticModel().Root, offset);
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

    // Finds a suitable location to create a new declaration, putting it near existing declarations of that type
    //   *above the extraction point*, if there are any.
    private int FindOffsetToInsertNewDeclarationOfType(CompilationContext compilationContext, int extractionOffset, Type declarationSyntaxType)
    {
        ImmutableArray<int> lineStarts = compilationContext.LineStarts;

        var extractionLine = GetPosition(extractionOffset).line;
        var startSearchingAtLine = extractionLine - 1;

        for (int line = startSearchingAtLine; line >= 0; --line)
        {
            var existingDeclarationStatement = StatementOfTypeAtLine(line);
            if (existingDeclarationStatement != null)
            {
                // Insert on the line right after the existing declaration
                var insertionLine = line + 1;

                // Is there a blank line above this existing statement that we found (excluding its leading nodes)?
                //   If so, assume user probably wants one after as well.
                var beginningOffsetOfExistingDeclaration = existingDeclarationStatement.Span.Position;
                var beginningLineOfExistingDeclaration =
                    GetPosition(beginningOffsetOfExistingDeclaration)
                    .line;

                return GetOffset(insertionLine, 0);
            }
        }

        // If no existing declarations of the desired type, insert right before the statement containing the extraction expression
        return GetOffset(extractionLine, 0);

        StatementSyntax? StatementOfTypeAtLine(int line)
        {
            var lineOffset = GetOffset(line, 0);
            var statementAtLine = SyntaxMatcher.FindNodesSpanningRange(compilationContext.ProgramSyntax, lineOffset, lineOffset)
                .OfType<StatementSyntax>()
                .FirstOrDefault(s => s.GetType() == declarationSyntaxType);
            return statementAtLine;
        }

        (int line, int character) GetPosition(int offset) => TextCoordinateConverter.GetPosition(lineStarts, offset);
        int GetOffset(int line, int character) => TextCoordinateConverter.GetOffset(lineStarts, line, character);
    }
}
