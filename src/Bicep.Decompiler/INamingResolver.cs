// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Deployments.Expression.Expressions;

namespace Bicep.Decompiler
{
    public interface INamingResolver
    {
        string? TryLookupName(NameType nameType, string desiredName);

        string? TryRequestName(NameType nameType, string desiredName);

        string? TryLookupResourceName(string? typeString, LanguageExpression nameExpression);

        string? TryRequestResourceName(string typeString, LanguageExpression nameExpression);
    }
}