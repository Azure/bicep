// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

namespace Bicep.Core.Visitors
{
    public class FunctionVariableGeneratorVisitor : AstVisitor
    {
        private readonly SemanticModel semanticModel;
        private readonly Dictionary<FunctionCallSyntaxBase, FunctionVariable> variables;

        private readonly VisitorRecorder<SyntaxBase> syntaxRecorder = new();

        private FunctionVariableGeneratorVisitor(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.variables = new();
        }

        public static IDictionary<FunctionCallSyntaxBase, FunctionVariable> GetFunctionVariables(SemanticModel semanticModel)
        {
            var visitor = new FunctionVariableGeneratorVisitor(semanticModel);
            visitor.Visit(semanticModel.Root.Syntax);

            return visitor.variables;
        }

        protected override void VisitInternal(SyntaxBase node)
        {
            using var _ = syntaxRecorder.Scope(node);
            base.VisitInternal(node);
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
            if (symbol is not FunctionSymbol || semanticModel.TypeManager.GetMatchedFunctionOverload(syntax) is not { VariableGenerator: { } } functionOverload)
            {
                return;
            }

            var directVariableAssignment = syntaxRecorder.Skip(1).FirstOrDefault() is VariableDeclarationSyntax;
            var variable = functionOverload.VariableGenerator(syntax, symbol, semanticModel.GetTypeInfo(syntax), directVariableAssignment, semanticModel.TypeManager.GetMatchedFunctionResultValue(syntax));
            if (variable is not null)
            {
                variables.Add(syntax, new($"$fxv#{variables.Count}", variable));
            }
        }

    }
}
