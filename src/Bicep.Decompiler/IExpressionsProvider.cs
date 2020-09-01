// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Deployments.Expression.Expressions;

namespace Bicep.Decompiler
{
    public interface IExpressionsProvider
    {
        bool IsLanguageExpression(string value);

        LanguageExpression ParseLanguageExpression(string value);

        string SerializeExpression(LanguageExpression expression);
    }
}