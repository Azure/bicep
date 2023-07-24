// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
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
            typeAliasToTypeExpression = new(StringComparer.Ordinal);
        }

        public DeclaredTypeExpression? GetDeclaredTypeExpression(string name)
        {
            var declaredTypeExpression = typeAliasToTypeExpression.GetValueOrDefault(name);

            return declaredTypeExpression is { Value: TypeAliasReferenceExpression nextAlias }
                ? GetDeclaredTypeExpression(nextAlias.Name)
                : declaredTypeExpression;
        }

        public override void VisitDeclaredTypeExpression(DeclaredTypeExpression expression)
        {
            typeAliasToTypeExpression[expression.Name] = expression;
            base.VisitDeclaredTypeExpression(expression);
        }
    }
}
