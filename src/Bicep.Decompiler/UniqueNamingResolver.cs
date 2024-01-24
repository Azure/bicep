// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics;
using System.Text.RegularExpressions;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core;
using Bicep.Decompiler.ArmHelpers;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Decompiler
{
    public class UniqueNamingResolver : INamingResolver
    {
        private readonly Dictionary<string, Dictionary<NameType, string>> assignedNames = new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, string> assignedResourceNames = new(StringComparer.OrdinalIgnoreCase);

        // This regex/replacements allows us to change vmName -> vm and vmName2 -> vm2
        private readonly Regex ResourceNameRemoveTrailingNameRegex = new("([a-zA-Z][a-zA-Z0-9_]*)Name([0-9]*)$");
        private const string ResourceNameRemoveTrailingNameReplacement = "$1$2";

        private static string GetNamingSuffix(NameType nameType)
            => nameType switch
            {
                // The naming suffix is just used in case of naming clashes, to pick a unique name for a symbol in the generated bicep file.
                // These do not need to match the keyword names, but it's probably most understandable to the user if they do.
                NameType.Output => LanguageConstants.OutputKeyword,
                NameType.Resource => LanguageConstants.ResourceKeyword,
                NameType.Variable => LanguageConstants.VariableKeyword,
                NameType.Parameter => LanguageConstants.ParameterKeyword,
                _ => nameType.ToString().ToUpperInvariant(),
            };

        public static string EscapeIdentifier(string identifier, bool isGenerated)
        {
            var value = Regex.Replace(identifier, "[^a-zA-Z0-9\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]+", "_");
            if (isGenerated)
            {
                // If the name was generated by us, remove starting/trailing underscores. If it came from the user, leave them alone
                value = value.Trim('_');
            }

            if (Regex.IsMatch(value, "^[0-9].*"))
            {
                // an identifier cannot start with a digit - work around this by prefixing with '_'
                value = "_" + value;
            }

            return value;
        }

        public string? TryLookupName(NameType nameType, string desiredName)
        {
            Debug.Assert(nameType != NameType.Resource, $"Use {nameof(TryLookupResourceName)} for resources");
            return TryLookupNameCore(nameType, desiredName, isGenerated: false);
        }


        private string? TryLookupNameCore(NameType nameType, string desiredName, bool isGenerated)
        {
            desiredName = EscapeIdentifier(desiredName, isGenerated);

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
            Debug.Assert(nameType != NameType.Resource, $"Use {nameof(TryRequestResourceName)} for resources");
            return TryRequestNameCore(nameType, desiredName, isGenerated: false);
        }

        private string? TryRequestNameCore(NameType nameType, string desiredName, bool isGenerated)
        {
            desiredName = EscapeIdentifier(desiredName, isGenerated);

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

                if (nameType == NameType.Output)
                {
                    // output names can't clash with param/var/resource names
                    nameByType[nameType] = desiredName;
                }
                else if (!nameByType.ContainsKey(NameType.Parameter) && !nameByType.ContainsKey(NameType.Variable) && !nameByType.ContainsKey(NameType.Resource))
                {
                    // param/var/resource names can't clash with output names
                    nameByType[nameType] = desiredName;
                }
                else
                {
                    // TODO technically a naming clash is still possible here but unlikely
                    nameByType[nameType] = $"{desiredName}_{GetNamingSuffix(nameType)}";
                }
            }

            return nameByType[nameType];
        }

        public string? TryLookupResourceName(string? typeString, LanguageExpression nameExpression)
        {
            // normalize strings - this flattens nested format() and concat() expressions, and outputs via concat()
            nameExpression = LanguageExpressionRewriter.Rewrite(nameExpression, ExpressionHelpers.FlattenStringOperations);

            if (typeString is null)
            {
                var nameString = ExpressionsEngine.SerializeExpression(nameExpression);
                var resourceKeySuffix = EscapeIdentifier($"_{nameString}", isGenerated: true);

                var matchingResources = assignedResourceNames.Where(kvp => kvp.Key.EndsWith(resourceKeySuffix, StringComparison.OrdinalIgnoreCase));
                if (matchingResources.Count() == 1)
                {
                    // only return a value if we're sure about the match
                    return matchingResources.First().Value;
                }

                return null;
            }

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
            // normalize strings - this flattens nested format() and concat() expressions, and outputs via concat()
            nameExpression = LanguageExpressionRewriter.Rewrite(nameExpression, ExpressionHelpers.FlattenStringOperations);

            // it's valid to include a trailing slash, so we need to normalize it
            typeString = typeString.TrimEnd('/');

            var assignedResourceKey = GetResourceNameKey(typeString, nameExpression);
            var nameString = GetNameRecursive(nameExpression);

            nameString = RemoveTrailingNameFromResourceName(nameString);

            // try to get a shorter name first if possible
            // if we've got two resources of different types with the same name, we may be forced to qualify it
            var unqualifiedName = TryRequestNameCore(NameType.Resource, nameString, isGenerated: true);
            if (unqualifiedName != null)
            {
                assignedResourceNames[assignedResourceKey] = unqualifiedName;
                return unqualifiedName;
            }

            var qualifiedName = TryRequestNameCore(NameType.Resource, $"{typeString}_{nameString}", isGenerated: true);
            if (qualifiedName != null)
            {
                assignedResourceNames[assignedResourceKey] = qualifiedName;
                return qualifiedName;
            }

            return null;
        }

        private string RemoveTrailingNameFromResourceName(string resourceName)
        {
            // A common pattern is:
            //
            // "variables": {
            //   "stgAccountName": "myStorage"
            // },
            // "resources": [{
            //   "name": "[variables('stgAccountName')]",
            //    ...
            //
            // If we choose the name 'stgAccountName' for the resource, it will conflict with the variable name.
            // So, remove a trailing "Name"

            var escapedName = EscapeIdentifier(resourceName, isGenerated: true);
            return ResourceNameRemoveTrailingNameRegex.Replace(escapedName, ResourceNameRemoveTrailingNameReplacement);
        }

        private string GetResourceNameKey(string typeString, LanguageExpression nameExpression)
        {
            var nameString = ExpressionsEngine.SerializeExpression(nameExpression);

            return EscapeIdentifier($"{typeString}_{nameString}", isGenerated: true);
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
