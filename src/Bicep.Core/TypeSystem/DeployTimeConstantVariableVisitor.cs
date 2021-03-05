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
        
        public DeployTimeConstantVariableVisitor(SemanticModel model)
        {
            this.model = model;
            this.VisitedStack = new Stack<string>();
        }

        public Stack<string> VisitedStack { get; }

        public ObjectType? InvalidReferencedBodyType { get; private set; }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            VisitedStack.Push(syntax.Name.IdentifierName);
            base.VisitVariableDeclarationSyntax(syntax);
            if (this.InvalidReferencedBodyType != null)
            {
                return;
            }
            // This variable declaration was deployment time constant
            if (VisitedStack.Pop() is var popped &&
                popped != syntax.Name.IdentifierName)
            {
                throw new InvalidOperationException($"{this.GetType().Name} performed an invalid Stack push/pop: expected popped element to be {syntax.Name.IdentifierName} but got {popped}");
            }
        }
        
        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            // Checked for short circuiting: We only show one violation at a time per variable
            if (this.InvalidReferencedBodyType != null)
            {
                return;
            }
            base.VisitObjectPropertySyntax(syntax);
        }

        // This method should not be visited by PropertyAccessSyntax of Resource/Modules (But Variables should visit). This is meant to catch variable 
        // properties which are assigned entire resource/modules, or to recurse through a chain of variable references.
        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (DeployTimeConstantVisitor.ExtractResourceOrModuleSymbolAndBodyType(this.model, syntax) is ({} declaredSymbol, {} referencedBodyType))
            {
                this.InvalidReferencedBodyType = referencedBodyType;
                VisitedStack.Push(declaredSymbol.Name);
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
            if (syntax.BaseExpression is VariableAccessSyntax baseVariableAccess)
            {
                if (DeployTimeConstantVisitor.ExtractResourceOrModuleSymbolAndBodyType(this.model, baseVariableAccess) is ({} declaredSymbol, {} referencedBodyType))
                {
                    if (syntax.IndexExpression is StringSyntax stringSyntax &&
                    stringSyntax.TryGetLiteralValue() is string property &&
                    referencedBodyType.Properties.TryGetValue(property, out var propertyType) &&
                    !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                    {
                        this.InvalidReferencedBodyType = referencedBodyType;
                        VisitedStack.Push(declaredSymbol.Name);
                    }

                    // Do not VisitVariableAccessSyntax on Resources or Modules
                    return;
                }
            }
            base.VisitArrayAccessSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            switch (syntax.BaseExpression)
            {
                case VariableAccessSyntax baseVariableAccess when DeployTimeConstantVisitor.ExtractResourceOrModuleSymbolAndBodyType(this.model, baseVariableAccess) is ({ } declaredSymbol, { } referencedBodyType):
                    {
                        if (referencedBodyType.Properties.TryGetValue(syntax.PropertyName.IdentifierName, out var propertyType) && !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                        {
                            this.InvalidReferencedBodyType = referencedBodyType;
                            VisitedStack.Push(declaredSymbol.Name);
                        }

                        // Do not VisitVariableAccessSyntax on Resources or Modules
                        return;
                    }

                case ArrayAccessSyntax { BaseExpression: VariableAccessSyntax baseBaseVariableAccess } when DeployTimeConstantVisitor.ExtractResourceOrModuleCollectionSymbolAndBodyType(this.model, baseBaseVariableAccess) is ({ } declaredSymbol, { } referencedBodyType):
                    {
                        if (referencedBodyType.Properties.TryGetValue(syntax.PropertyName.IdentifierName, out var propertyType) && !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                        {
                            this.InvalidReferencedBodyType = referencedBodyType;
                            VisitedStack.Push(declaredSymbol.Name);
                        }

                        // Do not VisitVariableAccessSyntax on Resources or Modules
                        return;
                    }
            }

            base.VisitPropertyAccessSyntax(syntax);
        }
        #endregion
    }
}
