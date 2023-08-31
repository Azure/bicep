// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Deployments.Expression.Expressions;
using System;

namespace Bicep.Decompiler
{
    public static class LanguageExpressionExtensions
    {
        public static bool NameEquals(this FunctionExpression expression, string name)
            => StringComparer.OrdinalIgnoreCase.Equals(expression.Function, name);
    }
}
