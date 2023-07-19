// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Bicep.Core.Intermediate
{
    /// <summary>
    /// Generates a look up of declared type expressions by their symbol name.
    /// </summary>
    public class ExpressionTypeAliasLookupVisitor : ExpressionVisitor
    {
        private readonly Dictionary<string, DeclaredTypeExpression> typeAliasToTypeExpression;

        public ExpressionTypeAliasLookupVisitor()
        {
            typeAliasToTypeExpression = new();
        }

        public DeclaredTypeExpression? GetDeclaredTypeExpression(string name)
            => typeAliasToTypeExpression.GetValueOrDefault(name);

        public override void VisitDeclaredTypeExpression(DeclaredTypeExpression expression)
        {
            typeAliasToTypeExpression[expression.Name] = expression;
            base.VisitDeclaredTypeExpression(expression);
        }
    }
}
