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

namespace Bicep.Cli.Services;

public class ReplEnvironment
{
    private readonly InMemoryFileExplorer fileExplorer;
    private readonly BicepCompiler compiler;
    private readonly Workspace workspace;

    // Persist original variable declaration text (ordered) and lookup to allow redefinition.
    private readonly List<string> variableDeclarationLines = [];
    private readonly Dictionary<string, string> variableDeclarationLookup = new(StringComparer.OrdinalIgnoreCase);
    private static readonly IOUri replFileUri = IOUri.FromFilePath("/session.biceprepl");

    public ReplEnvironment(BicepCompiler compiler)
    {
        this.fileExplorer = new InMemoryFileExplorer();
        this.compiler = compiler;
        this.workspace = new Workspace();
    }

    public async Task<ReplEvaluationResult> EvaluateInput(string input)
    {
        if (TryParseVariableAssignment(input, out var assignmentName, out var assignmentValue, out var assignmentError))
        {
            if (assignmentError is not null)
            {
                return assignmentError;
            }

            return await EvaluateVariableRedefinition(assignmentName!, assignmentValue!);
        }

        var parser = new ReplParser(input);
        var syntax = parser.ParseExpression(out var diags);
        var errors = diags.Where(d => d.Level == DiagnosticLevel.Error).ToList();
        if (errors.Count > 0)
        {
            return CreateAnnotatedDiagnosticResult(errors, input, input.Length);
        }

        if (syntax is VariableDeclarationSyntax varDecl)
        {
            return await EvaluateVariableDeclaration(varDecl);
        }

        return await EvaluateExpression(syntax);
    }

    private bool TryParseVariableAssignment(string input, out string? name, out string? value, out ReplEvaluationResult? errorResult)
    {
        name = null;
        value = null;
        errorResult = null;

        // Check input to see if it is a redefinition of a variable
        // > var a = 'value'
        // value
        // > a = 'new value'
        // new value
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var trimmed = input.TrimStart();
        if (trimmed.StartsWith($"{LanguageConstants.VariableKeyword} ", StringComparison.Ordinal))
        {
            return false;
        }

        var eqIndex = input.IndexOf('=');
        if (eqIndex < 0)
        {
            return false;
        }
        if (eqIndex + 1 < input.Length)
        {
            var next = input[eqIndex + 1];
            if (next is '=' or '>' or '~')
            {
                return false;
            }
        }

        var lhs = input[..eqIndex].Trim();
        if (lhs.Length == 0 || !Lexer.IsValidIdentifier(lhs))
        {
            return false;
        }

        var rhs = input[(eqIndex + 1)..];
        if (string.IsNullOrWhiteSpace(rhs))
        {
            return false;
        }

        // Confirm variable has been declared previously
        name = lhs;
        value = rhs;

        if (!variableDeclarationLookup.ContainsKey(lhs))
        {
            var diagnostic = DiagnosticBuilder.ForPosition(new TextSpan(0, lhs.Length))
                .FailedToEvaluateSubject("assignment", lhs, $"Variable '{lhs}' is not declared. Use 'var {lhs} = ...' to declare it.");
            errorResult = ReplEvaluationResult.For(new[] { diagnostic });
        }

        return true;
    }

    private async Task<ReplEvaluationResult> EvaluateVariableRedefinition(string name, string rhsExpression)
    {
        if (!variableDeclarationLookup.TryGetValue(name, out _))
        {
            var diagnostic = DiagnosticBuilder.ForPosition(new TextSpan(0, name.Length))
                .FailedToEvaluateSubject("assignment", name, $"Variable '{name}' is not declared. Use 'var {name} = ...' to declare it.");
            return ReplEvaluationResult.For(new[] { diagnostic });
        }

        var declarationText = $"var {name} = {rhsExpression}";
        var workingLines = GetWorkingLinesExcludingPrior(name);
        return await CompileEvaluateAndPersist(name, declarationText, rhsExpression, workingLines);
    }

    private async Task<ReplEvaluationResult> EvaluateVariableDeclaration(VariableDeclarationSyntax varDecl)
    {
        var varName = varDecl.Name.IdentifierName;
        var declarationText = varDecl.ToString();
        var workingLines = GetWorkingLinesExcludingPrior(varName);
        return await CompileEvaluateAndPersist(varName, declarationText, declarationText, workingLines);
    }

    // Build a working list excluding prior declaration (to avoid duplicate diagnostic) if redefining.
    private List<string> GetWorkingLinesExcludingPrior(string varName)
    {
        if (variableDeclarationLookup.TryGetValue(varName, out var existingText))
        {
            return [.. variableDeclarationLines.Where(l => !string.Equals(l, existingText, StringComparison.Ordinal))];
        }
        return [.. variableDeclarationLines];
    }

    private async Task<ReplEvaluationResult> CompileEvaluateAndPersist(string varName, string declarationText, string diagnosticsFocusText, List<string> workingLines)
    {
        var fullContent = BuildFullContent(workingLines, declarationText);
        var compilation = await CompileInternal(fullContent);
        var model = compilation.GetEntrypointSemanticModel();
        var diagnostics = model.GetAllDiagnostics().Where(d => d.Source != DiagnosticSource.CoreLinter).ToList();
        if (diagnostics.Count != 0)
        {
            return CreateAnnotatedDiagnosticResult(diagnostics, diagnosticsFocusText, fullContent.Length);
        }

        // Find and evaluate the newly declared variable symbol.
        var declaredVariable = model.Root.VariableDeclarations.First(v => v.Name.Equals(varName, StringComparison.OrdinalIgnoreCase));
        var evaluator = new ReplEvaluator(model);
        var valueEval = evaluator.EvaluateExpression(declaredVariable.DeclaringVariable.Value);
        if (valueEval.Diagnostics.Any())
        {
            return valueEval;
        }

        // Persist only after successful evaluation.
        variableDeclarationLines.Clear();
        variableDeclarationLines.AddRange(workingLines);
        variableDeclarationLines.Add(declarationText);
        variableDeclarationLookup[varName] = declarationText;

        return valueEval;
    }

    private string BuildFullContent(IEnumerable<string> existingLines, string lastLine)
    {
        var sb = new StringBuilder();
        foreach (var line in existingLines)
        {
            sb.AppendLine(line);
        }
        sb.Append(lastLine);
        return sb.ToString();
    }

    private async Task<ReplEvaluationResult> EvaluateExpression(SyntaxBase expressionSyntax)
    {
        var tempVarName = $"__temp_eval_{Guid.NewGuid():N}";
        var userExpression = expressionSyntax.ToString();

        // need to write to "some" file in order to compile and get a semantic model
        // a semantic model is needed by ExpressionConverter
        var fullContent = BuildFullContent(variableDeclarationLines, $"var {tempVarName} = {userExpression}");
        var compilation = await CompileInternal(fullContent);

        var model = compilation.GetEntrypointSemanticModel();

        var diagnostics = model.GetAllDiagnostics().Where(d => d.Source != DiagnosticSource.CoreLinter).ToList();
        
        if (diagnostics.Count > 0)
        {
            return CreateAnnotatedDiagnosticResult(diagnostics, userExpression, fullContent.Length);
        }

        var evaluator = new ReplEvaluator(model);

        // find the variable we created to hold the expression and evaluate its value
        // (it doesn't seem like we can evaluate the expression directly because we need a symbol bound to a semantic model)
        var boundVariable = model.Root.VariableDeclarations.First(v => v.Name == tempVarName);

        return evaluator.EvaluateExpression(boundVariable.DeclaringVariable.Value);
    }

    private async Task<Compilation> CompileInternal(string fullContent)
    {
        var fileHandle = fileExplorer.GetFile(replFileUri);
        await fileHandle.WriteAllTextAsync(fullContent);

        var sourceFile = compiler.SourceFileFactory.CreateBicepReplFile(fileHandle, fullContent);
        workspace.UpsertSourceFile(sourceFile);
        return compiler.CreateCompilationWithoutRestore(replFileUri, workspace);
    }

    private static ReplEvaluationResult CreateAnnotatedDiagnosticResult(List<IDiagnostic> diagnostics, string userText, int compiledTextLength)
    {
        if (diagnostics.Count == 0)
        {
            return ReplEvaluationResult.For(diagnostics);
        }

        // Calculate the offset where user's text starts in the compiled content
        // User's text is always at the end, so: totalLength - userTextLength = startOffset
        var textOffset = compiledTextLength - userText.Length;
        
        // Filter diagnostics to only those within the user's text span
        var relevantDiagnostics = diagnostics
            .Where(d => d.Span.Position >= textOffset && d.Span.Position <= compiledTextLength)
            .ToList();
            
        if (relevantDiagnostics.Count == 0)
        {
            return ReplEvaluationResult.For(diagnostics);
        }

        // Adjust diagnostic positions to be relative to user's text
        var adjustedAnnotations = relevantDiagnostics.Select(d => new PrintHelper.Annotation(
            new TextSpan(d.Span.Position - textOffset, d.Span.Length), 
            $"[{d.Code} ({d.Level})] {d.Message}"));

        // Generate annotated output
        var userTextLineStarts = TextCoordinateConverter.GetLineStarts(userText);
        var printResult = PrintHelper.PrintWithAnnotations(
            userText,
            userTextLineStarts,
            adjustedAnnotations, 
            0);
            
        return ReplEvaluationResult.For(new AnnotatedDiagnostic(printResult));
    }
}
