// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Visitors
{
    public class FunctionVariableGeneratorVisitor : SyntaxVisitor
    {
        private readonly SemanticModel semanticModel;
        private readonly Dictionary<FunctionCallSyntaxBase, InternalVariableSymbol> variables;

        private FunctionVariableGeneratorVisitor(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.variables = new();
        }

        public static IDictionary<FunctionCallSyntaxBase, InternalVariableSymbol> GetVariables(SemanticModel semanticModel, SyntaxBase syntax)
        {
            var visitor = new FunctionVariableGeneratorVisitor(semanticModel);
            visitor.Visit(syntax);

            return visitor.variables;
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            GenerateVariableFromFunctionCall(syntax);
            base.VisitFunctionCallSyntax(syntax);
        }
        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            GenerateVariableFromFunctionCall(syntax);
            base.VisitInstanceFunctionCallSyntax(syntax);
        }

        private void GenerateVariableFromFunctionCall(FunctionCallSyntaxBase syntax)
        {
            var symbol = semanticModel.GetSymbolInfo(syntax);
            if (symbol is FunctionSymbol &&
                semanticModel.TypeManager.GetMatchedFunctionOverload(syntax) is { VariableGenerator: { } } functionOverload)
            {
                var variable = functionOverload.VariableGenerator(syntax, symbol, semanticModel.GetTypeInfo(syntax));
                variables.Add(syntax, new InternalVariableSymbol($"$fxv#{variables.Count}", variable));
            }
        }

    }
}
