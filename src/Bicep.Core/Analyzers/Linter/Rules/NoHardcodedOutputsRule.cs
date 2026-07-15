// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class NoHardcodedOutputsRule : LinterRuleBase
{
    public new const string Code = "no-hardcoded-outputs";

    public NoHardcodedOutputsRule() : base(
        code: Code,
        description: CoreResources.NoHardcodedOutputsRuleDescription,
        LinterRuleCategory.BestPractice,
        overrideCategoryDefaultDiagnosticLevel: DiagnosticLevel.Off)
    { }

    public override string FormatMessage(params object[] values) =>
        string.Format(CoreResources.NoHardcodedOutputsRuleMessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        var visitor = new Visitor(this, model, diagnosticLevel);
        visitor.Visit(model.SourceFile.ProgramSyntax);

        return visitor.Diagnostics;
    }

    private static bool IsHardcodedValue(SyntaxBase syntax) => syntax switch
    {
        StringSyntax stringSyntax => stringSyntax.TryGetLiteralValue() is not null,
        IntegerLiteralSyntax or BooleanLiteralSyntax or NullLiteralSyntax => true,
        UnaryOperationSyntax unaryOperation => IsHardcodedValue(unaryOperation.Expression),
        ParenthesizedExpressionSyntax parenthesizedExpression => IsHardcodedValue(parenthesizedExpression.Expression),
        ArraySyntax array => array.Items.All(item => IsHardcodedValue(item.Value)),
        ObjectSyntax @object => @object.Properties.All(property => IsHardcodedValue(property.Value)),
        _ => false,
    };

    private static string GetUnusedTopLevelName(string baseName, SemanticModel model, OutputDeclarationSyntax outputSyntax)
    {
        var increment = 1;
        while (true)
        {
            var newName = increment == 1 ? baseName : $"{baseName}{increment}";
            if (!model.Root.GetDeclarationsByName(newName).Any(declaration => declaration.DeclaringSyntax != outputSyntax))
            {
                return newName;
            }

            increment++;
        }
    }

    private static CodeFix CreateCodeFix(SemanticModel model, OutputDeclarationSyntax syntax)
    {
        var variableName = GetUnusedTopLevelName(syntax.Name.IdentifierName, model, syntax);
        var valueText = syntax.Value.ToString();
        var variableDeclaration = $"@export()\nvar {variableName} = {valueText}";
        var lastVariable = GetLastVariable(model);

        var title = string.Format(CoreResources.NoHardcodedOutputsRuleFixTitle, variableName, syntax.Name.IdentifierName);
        if (lastVariable is null)
        {
            var insertionReplacement = TryCreateInsertReplacementWithoutExistingVariables(model, syntax, variableDeclaration);
            if (insertionReplacement is not { } insertExportedVariable)
            {
                return new CodeFix(
                    title,
                    isPreferred: true,
                    CodeFixKind.QuickFix,
                    CreateReplaceOutputWithVariableReplacement(model, syntax, variableDeclaration));
            }

            return new CodeFix(
                title,
                isPreferred: true,
                CodeFixKind.QuickFix,
                CreateRemoveOutputReplacement(model, syntax, CanIncludeLeadingWhitespaceBeforeOutput(model, syntax, insertExportedVariable)),
                insertExportedVariable);
        }

        var insertAfterVariable = CreateInsertExportedVariableReplacement(model, syntax, lastVariable, variableDeclaration);

        return new CodeFix(
            title,
            isPreferred: true,
            CodeFixKind.QuickFix,
            CreateRemoveOutputReplacement(model, syntax, CanIncludeLeadingWhitespaceBeforeOutput(model, syntax, insertAfterVariable)),
            insertAfterVariable);
    }

    private static VariableDeclarationSyntax? GetLastVariable(SemanticModel model) =>
        model.SourceFile.ProgramSyntax.Declarations
            .OfType<VariableDeclarationSyntax>()
            .OrderBy(variable => variable.Span.Position)
            .LastOrDefault();

    private static CodeReplacement? TryCreateInsertReplacementWithoutExistingVariables(SemanticModel model, OutputDeclarationSyntax outputSyntax, string variableDeclaration)
    {
        var outputRemovalSpan = GetOutputRemovalSpan(model, outputSyntax, includeLeadingWhitespaceIfAtEnd: false);
        var declarations = model.SourceFile.ProgramSyntax.Declarations.OrderBy(declaration => declaration.Span.Position).ToArray();
        var firstResource = declarations.OfType<ResourceDeclarationSyntax>().FirstOrDefault();

        if (firstResource is not null)
        {
            var lastParameterBeforeResource = declarations
                .OfType<ParameterDeclarationSyntax>()
                .Where(parameter => parameter.Span.Position < firstResource.Span.Position)
                .LastOrDefault();

            return lastParameterBeforeResource is not null
                ? CreateInsertAfterDeclarationReplacement(model, lastParameterBeforeResource, outputRemovalSpan, variableDeclaration)
                : CreateInsertBeforeDeclarationReplacement(firstResource, variableDeclaration);
        }

        var lastParameter = declarations.OfType<ParameterDeclarationSyntax>().LastOrDefault();

        return lastParameter is not null
            ? CreateInsertAfterDeclarationReplacement(model, lastParameter, outputRemovalSpan, variableDeclaration)
            : null;
    }

    private static CodeReplacement CreateReplaceOutputWithVariableReplacement(SemanticModel model, OutputDeclarationSyntax outputSyntax, string variableDeclaration)
    {
        var removalSpan = GetOutputRemovalSpan(model, outputSyntax, includeLeadingWhitespaceIfAtEnd: false);
        var suffix = HasRemainingNonWhitespaceAfter(model.SourceFile.Text, removalSpan.Position + removalSpan.Length, removalSpan) ? "\n\n" : string.Empty;

        return new CodeReplacement(removalSpan, $"{variableDeclaration}{suffix}");
    }

    private static CodeReplacement CreateRemoveOutputReplacement(SemanticModel model, OutputDeclarationSyntax outputSyntax, bool includeLeadingWhitespaceIfAtEnd) =>
        new(GetOutputRemovalSpan(model, outputSyntax, includeLeadingWhitespaceIfAtEnd), string.Empty);

    private static CodeReplacement CreateInsertExportedVariableReplacement(SemanticModel model, OutputDeclarationSyntax outputSyntax, VariableDeclarationSyntax lastVariable, string variableDeclaration)
    {
        var outputRemovalSpan = GetOutputRemovalSpan(model, outputSyntax, includeLeadingWhitespaceIfAtEnd: false);
        return CreateInsertAfterDeclarationReplacement(model, lastVariable, outputRemovalSpan, variableDeclaration);
    }

    private static CodeReplacement CreateInsertAfterDeclarationReplacement(SemanticModel model, SyntaxBase declaration, TextSpan deletedSpan, string variableDeclaration)
    {
        var insertPosition = declaration.Span.Position + declaration.Span.Length;
        var whitespaceEnd = insertPosition;
        while (whitespaceEnd < model.SourceFile.Text.Length && char.IsWhiteSpace(model.SourceFile.Text[whitespaceEnd]))
        {
            whitespaceEnd++;
        }

        var prefix = declaration is ParameterDeclarationSyntax ? "\n\n" : "\n";
        var suffix = HasRemainingNonWhitespaceAfter(model.SourceFile.Text, whitespaceEnd, deletedSpan) ? "\n\n" : string.Empty;

        return new CodeReplacement(
            new TextSpan(insertPosition, whitespaceEnd - insertPosition),
            $"{prefix}{variableDeclaration}{suffix}");
    }

    private static CodeReplacement CreateInsertBeforeDeclarationReplacement(SyntaxBase declaration, string variableDeclaration) =>
        new(new TextSpan(declaration.Span.Position, 0), $"{variableDeclaration}\n\n");

    private static TextSpan GetOutputRemovalSpan(SemanticModel model, OutputDeclarationSyntax outputSyntax, bool includeLeadingWhitespaceIfAtEnd)
    {
        var position = outputSyntax.Span.Position;
        var endPosition = outputSyntax.Span.Position + outputSyntax.Span.Length;
        while (endPosition < model.SourceFile.Text.Length && char.IsWhiteSpace(model.SourceFile.Text[endPosition]))
        {
            endPosition++;
        }

        if (includeLeadingWhitespaceIfAtEnd && !HasNonWhitespaceAfter(model.SourceFile.Text, endPosition))
        {
            while (position > 0 && char.IsWhiteSpace(model.SourceFile.Text[position - 1]))
            {
                position--;
            }
        }

        return new TextSpan(position, endPosition - position);
    }

    private static bool CanIncludeLeadingWhitespaceBeforeOutput(SemanticModel model, OutputDeclarationSyntax outputSyntax, CodeReplacement insertionReplacement)
    {
        var leadingWhitespaceStart = outputSyntax.Span.Position;
        while (leadingWhitespaceStart > 0 && char.IsWhiteSpace(model.SourceFile.Text[leadingWhitespaceStart - 1]))
        {
            leadingWhitespaceStart--;
        }

        return leadingWhitespaceStart >= insertionReplacement.Span.Position + insertionReplacement.Span.Length;
    }

    private static bool HasNonWhitespaceAfter(string text, int position)
    {
        for (var current = position; current < text.Length; current++)
        {
            if (!char.IsWhiteSpace(text[current]))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasRemainingNonWhitespaceAfter(string text, int position, TextSpan deletedSpan)
    {
        var current = position;
        var deletionEnd = deletedSpan.Position + deletedSpan.Length;

        while (current < text.Length)
        {
            while (current < text.Length && char.IsWhiteSpace(text[current]))
            {
                current++;
            }

            if (current >= text.Length)
            {
                return false;
            }

            if (current >= deletedSpan.Position && current < deletionEnd)
            {
                current = deletionEnd;
                continue;
            }

            return true;
        }

        return false;
    }

    private sealed class Visitor(NoHardcodedOutputsRule parent, SemanticModel model, DiagnosticLevel diagnosticLevel) : AstVisitor
    {
        public List<IDiagnostic> Diagnostics { get; } = [];

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            if (IsHardcodedValue(syntax.Value))
            {
                var fix = CreateCodeFix(model, syntax);
                Diagnostics.Add(parent.CreateFixableDiagnosticForSpan(diagnosticLevel, syntax.Value.Span, fix, syntax.Name.IdentifierName));
            }

            base.VisitOutputDeclarationSyntax(syntax);
        }
    }
}
