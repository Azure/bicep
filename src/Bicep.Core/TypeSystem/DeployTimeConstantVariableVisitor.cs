// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Visitor used to collect errors caused by property assignments to run-time values when that is not allowed
    /// </summary>
    public sealed class DeployTimeConstantVariableVisitior : SyntaxVisitor
    {
        private readonly SemanticModel model;
        public Stack<string> visitedStack;
        public ObjectType? invalidReferencedBodyObj;

        public DeployTimeConstantVariableVisitior(SemanticModel model)
        {
            this.model = model;
            this.visitedStack = new Stack<string>();
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            if (this.invalidReferencedBodyObj != null)
            {
                return;
            }
            visitedStack.Push(syntax.Name.IdentifierName);
            base.Visit(syntax.Value);
            if (this.invalidReferencedBodyObj == null)
            {
                visitedStack.Pop();
            }
        }
        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            if (this.invalidReferencedBodyObj != null)
            {
                return;
            }
            base.VisitObjectPropertySyntax(syntax);
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            if (syntax.BaseExpression is VariableAccessSyntax baseSyntax)
            {
                var baseSymbol = model.GetSymbolInfo(baseSyntax);
                switch(baseSymbol)
                {
                    // do not visit the baseExpression if we have a symbol match. Return instead of break
                    case ResourceSymbol:
                    case ModuleSymbol:
                        if (TypeAssignmentVisitor.UnwrapType(((DeclaredSymbol)baseSymbol).Type) is ObjectType referencedBodyObj)
                        {
                            switch (syntax.IndexExpression)
                            {
                                case StringSyntax stringSyntax:
                                if (stringSyntax.TryGetLiteralValue() is string property &&
                                    referencedBodyObj.Properties.TryGetValue(property, out var propertyType) &&
                                    !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                                    {
                                        this.invalidReferencedBodyObj = referencedBodyObj;
                                        visitedStack.Push(baseSymbol.Name);
                                    }
                                    break;
                            }
                        }
                        return;
                }
            }
            base.Visit(syntax.BaseExpression);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            if (syntax.BaseExpression is VariableAccessSyntax baseSyntax)
            {
                var baseSymbol = model.GetSymbolInfo(baseSyntax);
                switch(baseSymbol)
                {
                    // do not visit the baseExpression if we have a symbol match. Return instead of break
                    case ResourceSymbol:
                    case ModuleSymbol:
                        if (TypeAssignmentVisitor.UnwrapType(((DeclaredSymbol)baseSymbol).Type) is ObjectType referencedBodyObj)
                        {
                            var property = syntax.PropertyName.IdentifierName;
                            if (referencedBodyObj.Properties.TryGetValue(property, out var propertyType) &&
                            !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                            {
                                this.invalidReferencedBodyObj = referencedBodyObj;
                                visitedStack.Push(baseSymbol.Name);
                            }
                        }
                        return;
                }
            }
            base.Visit(syntax.BaseExpression);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            var baseSymbol = model.GetSymbolInfo(syntax);
            switch (baseSymbol)
            {
                case ResourceSymbol:
                case ModuleSymbol:
                    if (TypeAssignmentVisitor.UnwrapType(((DeclaredSymbol)baseSymbol).Type) is ObjectType referencedBodyObj)
                    {
                        this.invalidReferencedBodyObj = referencedBodyObj;
                    }
                    visitedStack.Push(baseSymbol.Name);
                    break;
                case VariableSymbol variableSymbol:
                    this.Visit(variableSymbol.DeclaringSyntax);
                    break;
            }
        }
    }
}
