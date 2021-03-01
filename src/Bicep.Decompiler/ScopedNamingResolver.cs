// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Deployments.Expression.Expressions;

namespace Bicep.Decompiler
{
    public class ScopedNamingResolver : INamingResolver
    {
        private readonly INamingResolver parent;
        private readonly ISet<string> scopedVariables;

        public ScopedNamingResolver(INamingResolver parent, IEnumerable<string> scopedVariables)
        {
            this.parent = parent;
            this.scopedVariables = scopedVariables.ToHashSet();
        }

        public string? TryLookupName(NameType nameType, string desiredName)
        {
            if (nameType == NameType.Variable && scopedVariables.Contains(desiredName))
            {
                return desiredName;
            }

            return parent.TryLookupName(nameType, desiredName);
        }

        public string? TryLookupResourceName(string? typeString, LanguageExpression nameExpression)
            => parent.TryLookupResourceName(typeString, nameExpression);

        public string? TryRequestName(NameType nameType, string desiredName)
            => throw new NotImplementedException();

        public string? TryRequestResourceName(string typeString, LanguageExpression nameExpression)
            => throw new NotImplementedException();
    }
}