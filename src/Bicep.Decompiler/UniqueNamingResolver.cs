// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Azure.Deployments.Expression.Expressions;

namespace Bicep.Decompiler
{
    public class UniqueNamingResolver : INamingResolver
    {
        private readonly Dictionary<string, Dictionary<NameType, string>> assignedNames = new Dictionary<string, Dictionary<NameType, string>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, string> assignedResourceNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly IExpressionsProvider expressionsProvider;

        public UniqueNamingResolver(IExpressionsProvider expressionsProvider)
        {
            this.expressionsProvider = expressionsProvider;
        }

        private static string EscapeIdentifier(string identifier)
        {
            return Regex.Replace(identifier, "[^a-zA-Z0-9_]+", "_").Trim('_');
        }

        public string? TryLookupName(NameType nameType, string desiredName)
        {
            desiredName = EscapeIdentifier(desiredName);

            if (!assignedNames.TryGetValue(desiredName, out var nameByType))
            {
                return null;
            }

            if (!nameByType.TryGetValue(nameType, out var name))
            {
                return null;
            }

            return name;
        }

        public string? TryRequestName(NameType nameType, string desiredName)
        {
            desiredName = EscapeIdentifier(desiredName);

            if (!assignedNames.TryGetValue(desiredName, out var nameByType))
            {
                nameByType = new Dictionary<NameType, string>
                {
                    [nameType] = desiredName,
                };
                assignedNames[desiredName] = nameByType;
            }
            else
            {
                if (nameByType.ContainsKey(nameType))
                {
                    return null;
                }
                
                // TODO technically a naming clash is still possible here but unlikely
                nameByType[nameType] = $"{desiredName}_{nameType.ToString().ToLowerInvariant()}";
            }

            return nameByType[nameType];
        }

        public string? TryLookupResourceName(string typeString, LanguageExpression nameExpression)
        {
            // it's valid to include a trailing slash, so we need to normalize it
            typeString = typeString.TrimEnd('/');

            var assignedResourceKey = GetResourceNameKey(typeString, nameExpression);

            if (!assignedResourceNames.TryGetValue(assignedResourceKey, out var name))
            {
                return null;
            }

            return name;
        }

        public string? TryRequestResourceName(string typeString, LanguageExpression nameExpression)
        {
            // it's valid to include a trailing slash, so we need to normalize it
            typeString = typeString.TrimEnd('/');

            var assignedResourceKey = GetResourceNameKey(typeString, nameExpression);
            var nameString = GetNameRecursive(nameExpression);

            // try to get a shorter name first if possible
            // if we've got two resources of different types with the same name, we may be forced to qualify it
            var unqualifiedName = TryRequestName(NameType.Resource, nameString);
            if (unqualifiedName != null)
            {
                assignedResourceNames[assignedResourceKey] = unqualifiedName;
                return unqualifiedName;
            }

            var qualifiedName = TryRequestName(NameType.Resource, $"{typeString}_{nameString}");
            if (qualifiedName != null)
            {
                assignedResourceNames[assignedResourceKey] = qualifiedName;
                return qualifiedName;
            }

            return null;
        }

        private string GetResourceNameKey(string typeString, LanguageExpression nameExpression)
        {
            var nameString = expressionsProvider.SerializeExpression(nameExpression);

            return EscapeIdentifier($"{typeString}_{nameString}");
        }

        private static string GetNameRecursive(LanguageExpression expression)
        {
            if (expression is JTokenExpression jTokenExpression)
            {
                return jTokenExpression.Value.ToString();
            }

            if (expression is FunctionExpression functionExpression)
            {
                var subExpressions = functionExpression.Parameters.Concat(functionExpression.Properties);
                
                return string.Join('_', subExpressions.Select(GetNameRecursive));
            }

            return "_";
        }
    }
}