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

    public async Task<AnnotatedReplResult> EvaluateInput(string input)
    {
        var parser = new ReplParser(input);
        var syntax = parser.ParseExpression(out var diags);
        var errors = diags.Where(d => d.Level == DiagnosticLevel.Error).ToList();
        if (errors.Count > 0)
        {
            return new AnnotatedReplResult(null, CreateAnnotatedDiagnostics(errors, input, input.Length));
        }

        if (syntax is VariableDeclarationSyntax varDecl)
        {
            return await EvaluateVariableDeclaration(varDecl);
        }

        return await EvaluateExpression(syntax);
    }

    private async Task<AnnotatedReplResult> EvaluateVariableDeclaration(VariableDeclarationSyntax varDecl)
    {
        var varName = varDecl.Name.IdentifierName;
        var declarationText = varDecl.ToString();

        // Build a working list excluding prior declaration (to avoid duplicate diagnostic) if redefining.
        List<string> workingLines;
        if (variableDeclarationLookup.TryGetValue(varName, out var existingText))
        {
            workingLines = [.. variableDeclarationLines.Where(l => !string.Equals(l, existingText, StringComparison.Ordinal))];
        }
        else
        {
            workingLines = [.. variableDeclarationLines];
        }

        // Compile working lines + new declaration (without mutating persisted state yet).
        var sb = new StringBuilder();
        foreach (var line in workingLines)
        {
            sb.AppendLine(line);
        }
        sb.Append(declarationText);
        var fullContent = sb.ToString();
        var compilation = await CompileInternal(fullContent);

        var model = compilation.GetEntrypointSemanticModel();
        var diagnostics = model.GetAllDiagnostics().Where(d => d.Source != DiagnosticSource.CoreLinter).ToList();
        if (diagnostics.Count > 0)
        {
            return new AnnotatedReplResult(null, CreateAnnotatedDiagnostics(diagnostics, declarationText, fullContent.Length));
        }

        // Find and evaluate the newly declared variable symbol.
        var declaredVariable = model.Root.VariableDeclarations.First(v => v.Name.Equals(varName, StringComparison.OrdinalIgnoreCase));
        var evaluator = new ReplEvaluator(model);
        var variableEvalResult = evaluator.EvaluateExpression(declaredVariable.DeclaringVariable.Value);
        if (variableEvalResult.Diagnostics.Any())
        {
            return new AnnotatedReplResult(null, CreateAnnotatedDiagnostics(variableEvalResult.Diagnostics, declarationText, fullContent.Length));
        }

        // Persist only after successful evaluation.
        variableDeclarationLines.Clear();
        variableDeclarationLines.AddRange(workingLines);
        variableDeclarationLines.Add(declarationText);
        variableDeclarationLookup[varName] = declarationText;

        return new AnnotatedReplResult(variableEvalResult.Value, []);
    }

    private async Task<AnnotatedReplResult> EvaluateExpression(SyntaxBase expressionSyntax)
    {
        var tempVarName = $"__temp_eval_{Guid.NewGuid():N}";
        var userExpression = expressionSyntax.ToString();

        // need to write to "some" file in order to compile and get a semantic model
        // a semantic model is needed by ExpressionConverter
        var sb = new StringBuilder();
        foreach (var line in variableDeclarationLines)
        {
            sb.AppendLine(line);
        }
        sb.Append("var ").Append(tempVarName).Append(" = ").Append(userExpression);
        var fullContent = sb.ToString();

        var compilation = await CompileInternal(fullContent);

        var model = compilation.GetEntrypointSemanticModel();

        var diagnostics = model.GetAllDiagnostics().Where(d => d.Source != DiagnosticSource.CoreLinter).ToList();

        if (diagnostics.Count > 0)
        {
            return new AnnotatedReplResult(null, CreateAnnotatedDiagnostics(diagnostics, userExpression, fullContent.Length));
        }

        var evaluator = new ReplEvaluator(model);

        // find the variable we created to hold the expression and evaluate its value
        // (it doesn't seem like we can evaluate the expression directly because we need a symbol bound to a semantic model)
        var boundVariable = model.Root.VariableDeclarations.First(v => v.Name == tempVarName);

        var expressionEvalResult = evaluator.EvaluateExpression(boundVariable.DeclaringVariable.Value);
        if (expressionEvalResult.Diagnostics.Any())
        {
            return new AnnotatedReplResult(null, CreateAnnotatedDiagnostics(expressionEvalResult.Diagnostics, userExpression, fullContent.Length));
        }

        return new AnnotatedReplResult(expressionEvalResult.Value, []);
    }

    private async Task<Compilation> CompileInternal(string fullContent)
    {
        var fileHandle = fileExplorer.GetFile(replFileUri);
        await fileHandle.WriteAllTextAsync(fullContent);

        var sourceFile = compiler.SourceFileFactory.CreateBicepReplFile(fileHandle, fullContent);
        workspace.UpsertSourceFile(sourceFile);

        var compilation = compiler.CreateCompilationWithoutRestore(replFileUri, workspace);
        return compilation;
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
}

public record AnnotatedReplResult(JToken? Value, IEnumerable<PrintHelper.AnnotatedDiagnostic> AnnotatedDiagnostics);
