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
            this.VisitedStack = new Stack<DeclaredSymbol>();
        }

        public Stack<DeclaredSymbol> VisitedStack { get; }

        public ObjectType? InvalidReferencedBodyType { get; private set; }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            var variableSymbol = this.model.GetSymbolInfo(syntax) as VariableSymbol ?? throw new InvalidOperationException($"{nameof(syntax)} must be bond to a VariableSymbol.");
            VisitedStack.Push(variableSymbol);

            base.VisitVariableDeclarationSyntax(syntax);
            if (this.InvalidReferencedBodyType != null)
            {
                return;
            }
            // This variable declaration was deployment time constant
            if (VisitedStack.Pop() is var popped && popped != variableSymbol)
            {
                throw new InvalidOperationException($"{this.GetType().Name} performed an invalid Stack push/pop: expected popped element to be {variableSymbol.Name} but got {popped?.Name}");
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
            if (DeployTimeConstantVisitor.ExtractResourceOrModuleSymbolAndBodyType(this.model, syntax) is ({} referencedSymbol, {} referencedBodyType))
            {
                this.InvalidReferencedBodyType = referencedBodyType;
                VisitedStack.Push(referencedSymbol);
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
            if (syntax.BaseExpression is VariableAccessSyntax baseVariableAccess && DeployTimeConstantVisitor.ExtractResourceOrModuleSymbolAndBodyType(this.model, baseVariableAccess) is ({ } referencedSymbol, { } referencedBodyType))
            {
                if (syntax.IndexExpression is not StringSyntax stringSyntax)
                {
                    return;
                }

                if (stringSyntax.TryGetLiteralValue() is not string propertyName)
                {
                    return;
                }

                if (!referencedBodyType.Properties.TryGetValue(propertyName, out var propertyType))
                {
                    return;
                }

                if (!propertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime) ||
                    DeployTimeConstantVisitor.ReferencedSymbolIsResourceAndPropertyIsAbsent(referencedSymbol, propertyName))
                {
                    this.InvalidReferencedBodyType = referencedBodyType;
                    VisitedStack.Push(referencedSymbol);
                }

                // Do not VisitVariableAccessSyntax on Resources or Modules
                return;
            }
            base.VisitArrayAccessSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            switch (syntax.BaseExpression)
            {
                case VariableAccessSyntax baseVariableAccess when DeployTimeConstantVisitor.ExtractResourceOrModuleSymbolAndBodyType(this.model, baseVariableAccess) is ({ } referencedSymbol, { } referencedBodyType):
                    {
                        if (!referencedBodyType.Properties.TryGetValue(syntax.PropertyName.IdentifierName, out var propertyType))
                        {
                            return;
                        }

                        if (!propertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime) ||
                            DeployTimeConstantVisitor.ReferencedSymbolIsResourceAndPropertyIsAbsent(referencedSymbol, syntax.PropertyName.IdentifierName))
                        {
                            this.InvalidReferencedBodyType = referencedBodyType;
                            VisitedStack.Push(referencedSymbol);
                        }

                        // Do not VisitVariableAccessSyntax on Resources or Modules
                        return;
                    }

                case ArrayAccessSyntax { BaseExpression: VariableAccessSyntax baseBaseVariableAccess } when DeployTimeConstantVisitor.ExtractResourceOrModuleCollectionSymbolAndBodyType(this.model, baseBaseVariableAccess) is ({ } declaredSymbol, { } referencedBodyType):
                    {
                        if (!referencedBodyType.Properties.TryGetValue(syntax.PropertyName.IdentifierName, out var propertyType))
                        {
                            return;
                        }

                        if (!propertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime) ||
                            DeployTimeConstantVisitor.ReferencedSymbolIsResourceAndPropertyIsAbsent(declaredSymbol, syntax.PropertyName.IdentifierName))
                        {
                            this.InvalidReferencedBodyType = referencedBodyType;
                            VisitedStack.Push(declaredSymbol);
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
