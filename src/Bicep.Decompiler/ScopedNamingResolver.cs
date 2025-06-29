// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Deployments.Expression.Expressions;

namespace Bicep.Decompiler
{
    public class ScopedNamingResolver : INamingResolver
    {
        private readonly INamingResolver parent;
        private readonly ISet<string> scopedParameters;
        private readonly ISet<string> scopedVariables;

        public ScopedNamingResolver(INamingResolver parent, IEnumerable<string> scopedVariables, IEnumerable<string> scopedParameters)
        {
            this.parent = parent;
            this.scopedParameters = scopedParameters.ToHashSet();
            this.scopedVariables = scopedVariables.ToHashSet();
        }

        public string? TryLookupName(NameType nameType, string desiredName)
        {
            if (nameType == NameType.Variable && scopedVariables.Contains(desiredName))
            {
                return desiredName;
            }
            if (nameType == NameType.Parameter && scopedParameters.Contains(desiredName))
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
