// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Arm.Expression.Expressions;
using Bicep.Core.Navigation;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Emit
{
    public class ScopeEmittingVisitor : SyntaxVisitor
    {
        private readonly SemanticModel.SemanticModel semanticModel;
        private readonly ExpressionConverter expressionConverter;
        private LanguageExpression? scopeExpression;

        private ScopeEmittingVisitor(EmitterContext emitterContext)
        {
            this.semanticModel = emitterContext.SemanticModel;
            this.expressionConverter = new ExpressionConverter(emitterContext);
        }

        public static LanguageExpression GetScopeExpression(EmitterContext emitterContext, SyntaxBase syntax)
        {
            var visitor = new ScopeEmittingVisitor(emitterContext);
            visitor.Visit(syntax);

            if (visitor.scopeExpression == null)
            {
                throw new InvalidOperationException($"Unable to generate scope expression");
            }

            return visitor.scopeExpression;
        }

        private static LanguageExpression GetDeploymentScopeExpression()
            => new FunctionExpression(
                "deployment",
                Array.Empty<LanguageExpression>(),
                new LanguageExpression[]
                {
                    new JTokenExpression("scope"),
                });

        private LanguageExpression? GetScopeFromFunction(IdentifierSyntax functionIdentifierSyntax, ImmutableArray<FunctionArgumentSyntax> arguments)
        {       
            switch (functionIdentifierSyntax.IdentifierName)
            {
                case "tenant":
                    switch (arguments.Length)
                    {
                        case 0:
                            return new JTokenExpression("/");
                        default:
                            throw new ArgumentException($"Unexpected number of arguments for {functionIdentifierSyntax.IdentifierName}");
                    }
                case "managementGroup":
                    switch (arguments.Length)
                    {
                        case 0:
                            return GetDeploymentScopeExpression();
                        case 1:
                            return new FunctionExpression(
                                "format",
                                new LanguageExpression[] { 
                                    new JTokenExpression("Microsoft.Management/managementGroups/{0}"),
                                    expressionConverter.ConvertExpression(arguments[0].Expression),
                                },
                                Array.Empty<LanguageExpression>());
                        default:
                            throw new ArgumentException($"Unexpected number of arguments for {functionIdentifierSyntax.IdentifierName}");
                    }
                case "subscription":
                    switch (arguments.Length)
                    {
                        case 0:
                            return GetDeploymentScopeExpression();
                        case 1:
                            return new FunctionExpression(
                                "format",
                                new LanguageExpression[] { 
                                    new JTokenExpression("/subscriptions/{0}"),
                                    expressionConverter.ConvertExpression(arguments[0].Expression),
                                },
                                new LanguageExpression [0]);
                        default:
                            throw new ArgumentException($"Unexpected number of arguments for {functionIdentifierSyntax.IdentifierName}");
                    }
                case "resourceGroup":
                    switch (arguments.Length)
                    {
                        case 0:
                            return GetDeploymentScopeExpression();
                        case 1:
                            return new FunctionExpression(
                                "format",
                                new LanguageExpression[] { 
                                    new JTokenExpression("/subscriptions/{0}/resourceGroups/{1}"),
                                    new FunctionExpression(
                                        "subscription",
                                        Array.Empty<LanguageExpression>(),
                                        new LanguageExpression[] {
                                            new JTokenExpression("subscriptionId"),
                                        }),
                                    expressionConverter.ConvertExpression(arguments[0].Expression),
                                },
                                new LanguageExpression [0]);
                        case 2:
                            return new FunctionExpression(
                                "format",
                                new LanguageExpression[] { 
                                    new JTokenExpression("/subscriptions/{0}/resourceGroups/{1}"),
                                    expressionConverter.ConvertExpression(arguments[0].Expression),
                                    expressionConverter.ConvertExpression(arguments[1].Expression),
                                },
                                new LanguageExpression [0]);
                        default:
                            throw new ArgumentException($"Unexpected number of arguments for {functionIdentifierSyntax.IdentifierName}");
                    }
            }

            throw new ArgumentException($"Unrecognized scope generating function {functionIdentifierSyntax.IdentifierName}");
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            var assignedType = semanticModel.GetTypeInfo(syntax);
            if (assignedType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.DeclaresResourceScope))
            {
                scopeExpression = GetScopeFromFunction(syntax.Name, syntax.Arguments);
            }
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            var assignedType = semanticModel.GetTypeInfo(syntax);
            if (assignedType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.DeclaresResourceScope))
            {
                scopeExpression = GetScopeFromFunction(syntax.Name, syntax.Arguments);
            }
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            throw new ArgumentException($"Unable to generate scope expression for resource {syntax.Name.IdentifierName}");
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            var declarationSyntax = semanticModel.GetSymbolInfo(syntax);
            if (declarationSyntax is DeclaredSymbol declaredSymbol)
            {
                this.Visit(declaredSymbol.DeclaringSyntax);
            }
        }
    }
}