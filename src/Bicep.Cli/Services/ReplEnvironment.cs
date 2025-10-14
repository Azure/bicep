// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Bicep.Cli.Helpers.Repl;
using System.Management;
using Bicep.Core.Highlighting;
using Bicep.Cli.Helpers.WhatIf;
using System.Net;
using Bicep.Core.Emit;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.PrettyPrintV2;

namespace Bicep.Cli.Services;

public class ReplEnvironment
{
    private readonly InMemoryFileExplorer fileExplorer;
    private readonly BicepCompiler compiler;

    // Persist original variable declaration text (ordered) and lookup to allow redefinition.
    private readonly List<string> declarationLines = [];
    private readonly Dictionary<string, string> declarationLookup = new(StringComparer.OrdinalIgnoreCase);
    private static readonly IOUri replFileUri = IOUri.FromFilePath("/session.biceprepl");
    private readonly List<string> history = [];
    private int lastHistoryIndex = -1;

    public ReplEnvironment(BicepCompiler compiler)
    {
        this.fileExplorer = new InMemoryFileExplorer();
        this.compiler = compiler;
    }

    public string HighlightInputLine(string prefix, string prevLines, IReadOnlyList<Rune> lineBuffer, int cursorOffset, bool printPrevLines)
    {
        var currentLine = string.Concat(lineBuffer);
        var fullContent = $"{prevLines}{currentLine}";
        var lineStart = prevLines.Length;

        var compilation = CompileInternal(fullContent);
        var model = compilation.GetEntrypointSemanticModel();
        var width = lineBuffer.Take(cursorOffset).Sum(r => r.Utf16SequenceLength);

        if (printPrevLines)
        {
            var historyHighlighted = PrintHelper.PrintWithSyntaxHighlighting(model, fullContent, 0);

            return PrintHelper.PrintInputLine(prefix, historyHighlighted, width);
        }

        var highlighted = PrintHelper.PrintWithSyntaxHighlighting(model, fullContent, lineStart);

        return PrintHelper.PrintInputLine(prefix, highlighted, width);
    }

    public string HighlightSyntax(SyntaxBase syntax)
    {
        var context = PrettyPrinterV2Context.Create(PrettyPrinterV2Options.Default, EmptyDiagnosticLookup.Instance, EmptyDiagnosticLookup.Instance);
        var content = PrettyPrinterV2.Print(syntax, context);

        return HighlightSyntax(content);
    }

    public string HighlightSyntax(string content)
    {
        var compilation = CompileInternal(content);
        var model = compilation.GetEntrypointSemanticModel();

        return PrintHelper.PrintWithSyntaxHighlighting(model, content);
    }

    public AnnotatedReplResult EvaluateInput(string input)
    {
        history.Add(input);
        lastHistoryIndex = -1;

        var parser = new ReplParser(input);
        IDiagnostic[] diagnostics = [.. parser.ParsingErrorLookup, .. parser.LexingErrorLookup];
        var errors = diagnostics.Where(d => d.Level == DiagnosticLevel.Error).ToList();
        if (errors.Count > 0)
        {
            return new AnnotatedReplResult(null, CreateAnnotatedDiagnostics(errors, input, input.Length));
        }

        var finalExpression = parser.Program().Children
            .Where(x => x is not Token { Type: TokenType.NewLine })
            .LastOrDefault();

        return finalExpression switch {
            NamedDeclarationSyntax named => EvalutateDeclaration(named),
            { } child => EvaluateExpression(child),
            _ => new AnnotatedReplResult(null, []),
        };
    }

    private AnnotatedReplResult EvalutateDeclaration(NamedDeclarationSyntax declaration)
    {
        var declarationName = declaration.Name.IdentifierName;
        var declarationText = declaration.ToString();

        // Build a working list excluding prior declaration (to avoid duplicate diagnostic) if redefining.
        List<string> workingLines;
        if (declarationLookup.TryGetValue(declarationName, out var existingText))
        {
            workingLines = [.. declarationLines.Where(l => !string.Equals(l, existingText, StringComparison.Ordinal))];
        }
        else
        {
            workingLines = [.. declarationLines];
        }

        // Compile working lines + new declaration (without mutating persisted state yet).
        var sb = new StringBuilder();
        foreach (var line in workingLines)
        {
            sb.AppendLine(line);
        }
        sb.Append(declarationText);
        var fullContent = sb.ToString();
        var compilation = CompileInternal(fullContent);

        var model = compilation.GetEntrypointSemanticModel();
        var diagnostics = model.GetAllDiagnostics().Where(d => d.Source != DiagnosticSource.CoreLinter).ToList();
        if (diagnostics.Count > 0)
        {
            return new AnnotatedReplResult(null, CreateAnnotatedDiagnostics(diagnostics, declarationText, fullContent.Length));
        }

        ExpressionEvaluationResult? evalResult = null;
        if (model.GetSymbolInfo(declaration) is VariableSymbol variableSymbol)
        {
            // Find and evaluate the newly declared variable symbol.
            var evaluator = new ParameterAssignmentEvaluator(model);
            evalResult = evaluator.EvaluateExpression(variableSymbol.DeclaringVariable.Value);
        }

        if (evalResult?.Diagnostics.Any() == true)
        {
            return new AnnotatedReplResult(null, CreateAnnotatedDiagnostics(evalResult.Diagnostics, declarationText, fullContent.Length));
        }

        // Persist only after successful evaluation.
        declarationLines.Clear();
        declarationLines.AddRange(workingLines);
        declarationLines.Add(declarationText);
        declarationLookup[declarationName] = declarationText;

        return new AnnotatedReplResult(ParseJToken(evalResult?.Value), []);
    }

    private AnnotatedReplResult EvaluateExpression(SyntaxBase expressionSyntax)
    {
        var tempVarName = $"__temp_eval_{Guid.NewGuid():N}";
        var userExpression = expressionSyntax.ToString();

        // need to write to "some" file in order to compile and get a semantic model
        // a semantic model is needed by ExpressionConverter
        var sb = new StringBuilder();
        foreach (var line in declarationLines)
        {
            sb.AppendLine(line);
        }
        sb.Append("var ").Append(tempVarName).Append(" = ").Append(userExpression);
        var fullContent = sb.ToString();

        var compilation = CompileInternal(fullContent);

        var model = compilation.GetEntrypointSemanticModel();

        var diagnostics = model.GetAllDiagnostics().Where(d => d.Source != DiagnosticSource.CoreLinter).ToList();

        if (diagnostics.Count > 0)
        {
            return new AnnotatedReplResult(null, CreateAnnotatedDiagnostics(diagnostics, userExpression, fullContent.Length));
        }

        var evaluator = new ParameterAssignmentEvaluator(model);

        // find the variable we created to hold the expression and evaluate its value
        // (it doesn't seem like we can evaluate the expression directly because we need a symbol bound to a semantic model)
        var boundVariable = model.Root.VariableDeclarations.First(v => v.Name == tempVarName);

        var expressionEvalResult = evaluator.EvaluateExpression(boundVariable.DeclaringVariable.Value);
        if (expressionEvalResult.Diagnostics.Any())
        {
            return new AnnotatedReplResult(null, CreateAnnotatedDiagnostics(expressionEvalResult.Diagnostics, userExpression, fullContent.Length));
        }

        return new AnnotatedReplResult(ParseJToken(expressionEvalResult.Value), []);
    }

    public string? TryGetHistory(bool backwards)
    {
        if (backwards)
        {
            if (lastHistoryIndex <= -1)
            {
                lastHistoryIndex = history.Count - 1;
            }
            else if (lastHistoryIndex > 0)
            {
                lastHistoryIndex--;
            }
            else
            {
                // remain at oldest entry
            }
        }
        else
        {
            if (lastHistoryIndex > -1 && lastHistoryIndex < history.Count - 1)
            {
                lastHistoryIndex++;
            }
            else
            {
                // remain at newest entry
            }
        }

        if (lastHistoryIndex == -1)
        {
            return null;
        }

        return history[lastHistoryIndex].TrimEnd('\n');
    }

    private Compilation CompileInternal(string fullContent)
    {
        var fileHandle = fileExplorer.GetFile(replFileUri);
        var sourceFile = compiler.SourceFileFactory.CreateBicepReplFile(fileHandle, fullContent);
        var workspace = new ActiveSourceFileSet();
        workspace.UpsertSourceFile(sourceFile);

        return compiler.CreateCompilationWithoutRestore(replFileUri, workspace);
    }

    // TODO: There's probably a better way to do this...
    private static IEnumerable<PrintHelper.AnnotatedDiagnostic> CreateAnnotatedDiagnostics(IEnumerable<IDiagnostic> diagnostics, string userText, int compiledTextLength)
    {
        // Calculate the offset where user's text starts in the compiled content
        // User's text is always at the end, so: totalLength - userTextLength = startOffset
        var textOffset = compiledTextLength - userText.Length;
        var annotatedDiagnostics = diagnostics
            .Where(d => d.Span.Position >= textOffset && d.Span.Position <= compiledTextLength) // only diagnostics that overlap with user text
            .Select(d => new PrintHelper.AnnotatedDiagnostic(d, () => new TextSpan(d.Span.Position - textOffset, d.Span.Length)));
        return annotatedDiagnostics;
    }

    [return:NotNullIfNotNull(nameof(value))]
    private static SyntaxBase? ParseJToken(JToken? value)
        => value switch
        {
            JObject jObject => ParseJObject(jObject),
            JArray jArray => ParseJArray(jArray),
            JValue jValue => ParseJValue(jValue),
            null => null,
            _ => throw new NotImplementedException($"Unrecognized token type {value.Type}"),
        };

    private static SyntaxBase ParseJValue(JValue value)
        => value.Type switch {
            JTokenType.Integer => SyntaxFactory.CreatePositiveOrNegativeInteger(value.Value<long>()),
            JTokenType.String => SyntaxFactory.CreateStringLiteral(value.ToString()),
            JTokenType.Boolean => SyntaxFactory.CreateBooleanLiteral(value.Value<bool>()),
            JTokenType.Null => SyntaxFactory.CreateNullLiteral(),
            _ => throw new NotImplementedException($"Unrecognized token type {value.Type}"),
        };

    private static SyntaxBase ParseJArray(JArray jArray)
        => SyntaxFactory.CreateArray(
            jArray.Select(ParseJToken).WhereNotNull());

    private static SyntaxBase ParseJObject(JObject jObject)
        => SyntaxFactory.CreateObject(
            jObject.Properties()
                .Select(x => SyntaxFactory.CreateObjectProperty(x.Name, ParseJToken(x.Value))));
}

public record AnnotatedReplResult(SyntaxBase? Value, IEnumerable<PrintHelper.AnnotatedDiagnostic> AnnotatedDiagnostics);
