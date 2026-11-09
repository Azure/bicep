// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit;

public class ReplEvaluator(Func<string, Compilation> compile)
{
    private readonly List<string> declarationLines = [];
    private readonly Dictionary<string, string> declarationLookup = new(StringComparer.OrdinalIgnoreCase);

    public record Result(JToken? Value, IEnumerable<IDiagnostic> Diagnostics, string UserText, int CompiledTextLength);

    public Result Evaluate(string input) => Evaluate(input, null);

    public Result Evaluate(string input, Action<ProgramSyntax>? validate)
    {
        var parser = new ReplParser(input);
        IDiagnostic[] diagnostics = [.. parser.ParsingErrorLookup, .. parser.LexingErrorLookup];
        var program = parser.Program();
        validate?.Invoke(program);
        var errors = diagnostics.Where(d => d.Level == DiagnosticLevel.Error).ToList();
        if (errors.Count > 0)
        {
            return new(null, errors, input, input.Length);
        }

        var finalExpression = program.Children
            .Where(x => x is not Token { Type: TokenType.NewLine })
            .LastOrDefault();

        return finalExpression switch
        {
            NamedDeclarationSyntax named => EvaluateDeclaration(named),
            { } child => EvaluateExpression(child),
            _ => new(null, [], input, input.Length),
        };
    }

    private Result EvaluateDeclaration(NamedDeclarationSyntax declaration)
    {
        var declarationName = declaration.Name.IdentifierName;
        var declarationText = declaration.ToString();
        List<string> workingLines;
        if (declarationLookup.TryGetValue(declarationName, out var existingText))
        {
            workingLines = [.. declarationLines.Where(l => !string.Equals(l, existingText, StringComparison.Ordinal))];
        }
        else
        {
            workingLines = [.. declarationLines];
        }

        var (_, diagnostics, textLength) = CompileSubmission(workingLines, declarationText);

        if (!diagnostics.Any(x => x.IsError()))
        {
            declarationLines.Clear();
            declarationLines.AddRange(workingLines);
            declarationLines.Add(declarationText);
            declarationLookup[declarationName] = declarationText;
        }

        return new(null, diagnostics, declarationText, textLength);
    }

    private Result EvaluateExpression(SyntaxBase expressionSyntax)
    {
        var tempVarName = $"__temp_eval_{Guid.NewGuid():N}";
        var userExpression = expressionSyntax.ToString();
        var (model, diagnostics, textLength) = CompileSubmission(declarationLines, $"var {tempVarName} = {userExpression}");
        if (diagnostics.Any(x => x.IsError()))
        {
            return new(null, diagnostics, userExpression, textLength);
        }

        var evaluator = new ParameterAssignmentEvaluator(model);
        var boundVariable = model.Root.VariableDeclarations.First(v => v.Name == tempVarName);
        var result = evaluator.EvaluateExpression(boundVariable.DeclaringVariable.Value);
        return new(result.Value, result.Diagnostics, userExpression, textLength);
    }

    private (SemanticModel Model, List<IDiagnostic> Diagnostics, int TextLength) CompileSubmission(IEnumerable<string> declarations, string submission)
    {
        var sb = new StringBuilder();
        foreach (var declaration in declarations)
        {
            sb.AppendLine(declaration);
        }
        sb.Append(submission);
        var content = sb.ToString();
        var model = compile(content).GetEntrypointSemanticModel();
        var diagnostics = model.GetAllDiagnostics().Where(d => d.Source != DiagnosticSource.CoreLinter).ToList();
        return (model, diagnostics, content.Length);
    }
}
