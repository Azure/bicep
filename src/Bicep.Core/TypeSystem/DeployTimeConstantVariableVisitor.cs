// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Visitor used to determine if a variable and its chained references refer to any runtime values.
    /// </summary>
    public sealed class DeployTimeConstantVariableVisitor : SyntaxVisitor
    {
        private readonly SemanticModel model;
        public Stack<string> visitedStack;
        public ObjectType? invalidReferencedBodyObj;

        public DeployTimeConstantVariableVisitor(SemanticModel model)
        {
            this.model = model;
            this.visitedStack = new Stack<string>();
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            visitedStack.Push(syntax.Name.IdentifierName);
            base.VisitVariableDeclarationSyntax(syntax);
            if (this.invalidReferencedBodyObj != null)
            {
                return;
            }
            // This variable declaration was deployment time constant
            if (visitedStack.Pop() is var popped &&
                popped != syntax.Name.IdentifierName)
            {
                throw new InvalidOperationException($"{this.GetType().Name} performed an invalid Stack push/pop: expected popped element to be {syntax.Name.IdentifierName} but got {popped}");
            }
        }
        
        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            // Checked for short circuiting: We only show one violation at a time per variable
            if (this.invalidReferencedBodyObj != null)
            {
                return;
            }
            base.VisitObjectPropertySyntax(syntax);
        }

        // This method should not be visited by PropertyAccessSyntax of Resource/Modules (But Variables should visit). This is meant to catch variable 
        // properties which are assigned entire resource/modules, or to recurse through a chain of variable references.
        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (DeployTimeConstantVisitor.ExtractResourceOrModuleSymbolAndBodyObj(this.model, syntax) is ({} declaredSymbol, {} referencedBodyObj))
            {
                this.invalidReferencedBodyObj = referencedBodyObj;
                visitedStack.Push(declaredSymbol.Name);
            }
            else if (model.GetSymbolInfo(syntax) is VariableSymbol variableSymbol)
            {
                this.Visit(variableSymbol.DeclaringSyntax);
            }
        }

        #region AccessSyntax
        // these need to be kept synchronized.
        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            if (syntax.BaseExpression is VariableAccessSyntax baseSyntax)
            {
                if (DeployTimeConstantVisitor.ExtractResourceOrModuleSymbolAndBodyObj(this.model, baseSyntax) is ({} declaredSymbol, {} referencedBodyObj))
                {
                    if (syntax.IndexExpression is StringSyntax stringSyntax &&
                    stringSyntax.TryGetLiteralValue() is string property &&
                    referencedBodyObj.Properties.TryGetValue(property, out var propertyType) &&
                    !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                    {
                        this.invalidReferencedBodyObj = referencedBodyObj;
                        visitedStack.Push(declaredSymbol.Name);
                    }
                    // Do not VisitVariableAccessSyntax on Resources or Modules
                    return;
                }
            }
            base.VisitArrayAccessSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            if (syntax.BaseExpression is VariableAccessSyntax baseSyntax)
            {
                if (DeployTimeConstantVisitor.ExtractResourceOrModuleSymbolAndBodyObj(this.model, baseSyntax) is ({} declaredSymbol, {} referencedBodyObj))
                {
                    if (referencedBodyObj.Properties.TryGetValue(syntax.PropertyName.IdentifierName, out var propertyType) &&
                    !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                    {
                        this.invalidReferencedBodyObj = referencedBodyObj;
                        visitedStack.Push(declaredSymbol.Name);
                    }
                }
                // Do not VisitVariableAccessSyntax on Resources or Modules
                return;
            }
            base.VisitPropertyAccessSyntax(syntax);
        }
        #endregion
    }
}
