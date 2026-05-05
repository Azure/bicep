// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;
using Azure.Deployments.Expression.Intermediate;

namespace Bicep.Core.Intermediate
{
    public abstract class TemplateLanguageExpressionVisitor : Azure.Deployments.Expression.Intermediate.IExpressionVisitor
    {
        public virtual void VisitArrayExpression(Azure.Deployments.Expression.Intermediate.ArrayExpression expression)
        {
            foreach (var item in expression.Items)
            {
                Visit(item);
            }
        }

        public virtual void VisitBooleanExpression(BooleanExpression expression)
        {
        }

        public virtual void VisitFloatExpression(FloatExpression expression)
        {
        }

        public virtual void VisitFunctionExpression(FunctionExpression expression)
        {
            foreach (var arg in expression.Arguments)
            {
                Visit(arg);
            }

            foreach (var property in expression.Properties)
            {
                Visit(property);
            }
        }

        public virtual void VisitIntegerExpression(IntegerExpression expression)
        {
        }

        public void VisitInvalidLanguageExpression(InvalidLanguageExpression expression)
        {
        }

        public virtual void VisitNullExpression(NullExpression expression)
        {
        }

        public virtual void VisitObjectExpression(Azure.Deployments.Expression.Intermediate.ObjectExpression expression)
        {
            foreach (var kvp in expression.Properties)
            {
                Visit(kvp.Key);
                Visit(kvp.Value);
            }
        }

        public virtual void VisitStringExpression(StringExpression expression)
        {
        }

        public virtual void VisitUnevaluableExpression(UnevaluableExpression expression)
        {
        }

        private void Visit(ITemplateLanguageExpression expression)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            expression.Accept(this);
        }
    }
}
