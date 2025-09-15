// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Cli.Services;

public class ReplEnvironment
{
    public ReplEvaluationResult EvaluateInput(string input)
    {
        // TODO: Get syntax from Repl Parser
        var syntax = new Parser(input).Expression(ExpressionFlags.AllowComplexLiterals);
        var evaluator = new ReplEvaluator(null!);
        if (syntax is VariableDeclarationSyntax varDecl)
        {
            // TODO: Handle variable declarations
        }

        return evaluator.EvaluateExpression(syntax);
    }
}
