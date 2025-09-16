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
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Bicep.Cli.Services;

public class ReplEnvironment
{
    private readonly InMemoryFileExplorer fileExplorer;
    private readonly BicepCompiler compiler;
    private readonly Workspace workspace;

    // Persist original variable declaration text (ordered) and lookup to allow redefinition.
    private readonly List<string> variableDeclarationLines = new();
    private readonly Dictionary<string, string> variableDeclarationLookup = new(StringComparer.OrdinalIgnoreCase);

    public ReplEnvironment(BicepCompiler compiler)
    {
        this.fileExplorer = new InMemoryFileExplorer();
        this.compiler = compiler;
        this.workspace = new Workspace();
    }

    public async Task<ReplEvaluationResult> EvaluateInput(string input)
    {
        var parser = new ReplParser(input);
        var syntax = parser.ParseExpression(out var diags);
        var errors = diags.Where(d => d.Level == DiagnosticLevel.Error).ToList();
        if (errors.Any())
        {
            return ReplEvaluationResult.For(errors);
        }

        if (syntax is VariableDeclarationSyntax varDecl)
        {
            return await EvaluateVariableDeclaration(varDecl);
        }

        return await EvaluateExpression(syntax);
    }

    private async Task<ReplEvaluationResult> EvaluateVariableDeclaration(VariableDeclarationSyntax varDecl)
    {
        var varName = varDecl.Name.IdentifierName;
        var declarationText = varDecl.ToString();

        // Build a working list excluding prior declaration (to avoid duplicate diagnostic) if redefining.
        List<string> workingLines;
        if (variableDeclarationLookup.TryGetValue(varName, out var existingText))
        {
            workingLines = variableDeclarationLines
                .Where(l => !string.Equals(l, existingText, StringComparison.Ordinal))
                .ToList();
        }
        else
        {
            workingLines = variableDeclarationLines.ToList();
        }

        // Compile working lines + new declaration (without mutating persisted state yet).
        var sb = new StringBuilder();
        foreach (var line in workingLines)
        {
            sb.AppendLine(line);
        }
        sb.AppendLine(declarationText);
        var fullContent = sb.ToString();
        var compilerResult = await CompileInternal(fullContent);

        var diagnostics = compilerResult.Model.GetAllDiagnostics().Where(d => d.Source != DiagnosticSource.CoreLinter);
        if (diagnostics.Any())
        {
            return ReplEvaluationResult.For(diagnostics);
        }

        // Find and evaluate the newly declared variable symbol.
        var declaredVariable = compilerResult.Model.Root.VariableDeclarations.First(v => v.Name.Equals(varName, StringComparison.OrdinalIgnoreCase));
        var evaluator = new ReplEvaluator(compilerResult.Model);
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

    private async Task<ReplEvaluationResult> EvaluateExpression(SyntaxBase expressionSyntax)
    {
        var tempVarName = $"__temp_eval_{Guid.NewGuid():N}";

        // need to write to "some" file in order to compile and get a semantic model
        // a semantic model is needed by ExpressionConverter
        var compilerResult = await CompileExpression(expressionSyntax.ToString(), tempVarName);

        var diagnostics = compilerResult.Model.GetAllDiagnostics().Where(d => d.Source != DiagnosticSource.CoreLinter);
        
        if (diagnostics.Any())
        {
            return ReplEvaluationResult.For(diagnostics);
        }

        var evaluator = new ReplEvaluator(compilerResult.Model);

        // find the variable we created to hold the expression and evaluate its value
        // (it doesn't seem like we can evaluate the expression directly because we need a symbol bound to a semantic model)
        var boundVariable = compilerResult.Model.Root.VariableDeclarations.First(v => v.Name == tempVarName);

        return evaluator.EvaluateExpression(boundVariable.DeclaringVariable.Value);
    }

    private async Task<CompilationResult> CompileExpression(string expression, string variableName)
    {
        // TODO: Not sure about this... Reconsider if there's a better way
        var sb = new StringBuilder();
        foreach (var line in variableDeclarationLines)
        {
            sb.AppendLine(line);
        }
        sb.Append("var ").Append(variableName).Append(" = ").AppendLine(expression);
        var fullContent = sb.ToString();
        return await CompileInternal(fullContent);
    }

    private async Task<CompilationResult> CompileInternal(string fullContent)
    {
        var replFileUri = IOUri.FromFilePath("/session.biceprepl");
        var fileHandle = fileExplorer.GetFile(replFileUri);
        await fileHandle.WriteAllTextAsync(fullContent);

        var sourceFile = compiler.SourceFileFactory.CreateBicepReplFile(fileHandle, fullContent);
        workspace.UpsertSourceFile(sourceFile);

        var compliaton = compiler.CreateCompilationWithoutRestore(replFileUri, workspace);
        var model = compliaton.GetEntrypointSemanticModel();
        return new CompilationResult(model);
    }

    private record CompilationResult(SemanticModel Model);
}
