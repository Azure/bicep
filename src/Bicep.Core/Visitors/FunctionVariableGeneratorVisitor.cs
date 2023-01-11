// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Visitors
{
    public class FunctionVariableGeneratorVisitor : AstVisitor
    {
        private readonly SemanticModel semanticModel;
        private readonly Dictionary<FunctionCallSyntaxBase, FunctionVariable> variables;

        private FunctionVariableGeneratorVisitor(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.variables = new();
        }

        public static ImmutableDictionary<FunctionCallSyntaxBase, FunctionVariable> GetFunctionVariables(SemanticModel semanticModel)
        {
            var visitor = new FunctionVariableGeneratorVisitor(semanticModel);
            visitor.Visit(semanticModel.Root.Syntax);

            return visitor.variables.ToImmutableDictionary();
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
            if (semanticModel.TypeManager.GetMatchedFunctionOverload(syntax) is not {} functionOverload ||
                semanticModel.TypeManager.GetMatchedFunctionResultValue(syntax) is not {} functionResult)
            {
                return;
            }

            var directVariableAssignment = semanticModel.Binder.GetParent(syntax) is VariableDeclarationSyntax;

            if (functionOverload.Flags.HasFlag(FunctionFlags.GenerateIntermediateVariableAlways) ||
                (!directVariableAssignment && functionOverload.Flags.HasFlag(FunctionFlags.GenerateIntermediateVariableOnIndirectAssignment)))
            {
                variables.Add(syntax, new($"$fxv#{variables.Count}", functionResult));
            }
        }
    }
}
