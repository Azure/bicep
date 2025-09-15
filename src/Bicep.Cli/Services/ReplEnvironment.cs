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

namespace Bicep.Cli.Services;

public class ReplEnvironment
{
    private readonly InMemoryFileExplorer fileExplorer;
    private readonly BicepCompiler compiler;
    private readonly Workspace workspace;

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
            // TODO: Handle VariableDeclaration
            // Placeholder to see variableDeclarationSyntax
            var valueEval = await EvaluateExpression(varDecl.Value);
            if (valueEval.Diagnostics.Count > 0)
            {
                return valueEval;
            }

            var typeSyntaxToken = varDecl.Type is null ? JValue.CreateNull() : JValue.FromObject(varDecl.Type.ToString());

            var obj = new JObject
            {
                ["variable"] = new JObject
                {
                    ["name"] = varDecl.Name.IdentifierName,
                    ["rawValueSyntax"] = varDecl.Value.ToString(),
                    ["evaluatedValue"] = valueEval.Value, // may be null
                    ["hasTypeAnnotation"] = varDecl.Type is not null,
                    ["typeSyntax"] = typeSyntaxToken
                }
            };
            return ReplEvaluationResult.For(obj);
        }

        return await EvaluateExpression(syntax);
    }

    private async Task<ReplEvaluationResult> EvaluateExpression(SyntaxBase expressionSyntax)
    {
        var tempVarName = $"__temp_eval_{Guid.NewGuid():N}";

        // need to write to "some" file in order to compile and get a semantic model
        // a semantic model is needed by ExpressionConverter
        var compilerResult = await Compile(expressionSyntax.ToString(), tempVarName);

        // ignore linter rules..
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

    private async Task<CompilationResult> Compile(string input, string variableName)
    {
        // TODO: Not sure about this... Reconsider if there's a better way
        var tempContent = new StringBuilder();
        tempContent.AppendLine($"var {variableName} = {input}");
        var fullContent = tempContent.ToString();

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
