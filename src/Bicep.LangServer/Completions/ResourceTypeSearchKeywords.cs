// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.LanguageServer.Snippets;

namespace Bicep.LanguageServer.Completions
{
    public partial class ResourceTypeSearchKeywords
    {
        private ImmutableDictionary<string, string[]> keywordsMap;

        public ResourceTypeSearchKeywords() : this
            (new Dictionary<string, string[]>()
            {
                // Keys must be in the form 'xxx' or 'xxx/yyy' - 'xxx' matches 'xxx' and 'xxx/*', 'xxx/yyy' matches 'xxx/yyy' and 'xxx/yyy/*'
                // Key casing doesn't matter
                ["Microsoft.Web/sites"] = ["appservice", "webapp", "function"],
                ["Microsoft.Web/serverFarms"] = ["asp", "appserviceplan", "hostingplan"],
                ["Microsoft.App"] = ["containerapp"],
                ["Microsoft.ContainerService"] = ["aks", "kubernetes", "k8s", "cluster"],
                ["Microsoft.Authorization"] = ["rbac"],
                ["Microsoft.OperationalInsights/workspaces"] = ["loganalytics"],
            })
        {
        }

        public ResourceTypeSearchKeywords(IDictionary<string, string[]> keywordsMap)
        {
            this.keywordsMap = keywordsMap.ToImmutableDictionary(x => x.Key.ToLowerInvariant(), x => x.Value, StringComparer.InvariantCultureIgnoreCase);

            // Validate
            foreach ((var key, var keywords) in keywordsMap)
            {
                if (key.Split('/').Length > 2)
                {
                    throw new ArgumentException($"Keys in {nameof(keywordsMap)} can have at most one slash: {key}");
                }
            }
        }

        public string? TryGetResourceTypeFilterText(ResourceTypeReference resourceType, bool surroundWithSingleQuotes = false)
        {
            var filter = resourceType.Type;
            string[]? keywords;

            // We want to search using the top-level resource type, including subtypes
            // Microsoft.web/serverFarms/xxx -> top-level key is Microsoft.web
            // Microsoft.web/serverFarms/xxx -> second-level key is Microsoft.web/serverFarms
            var indexFirstSlash = filter.IndexOf('/');
            var topLevelKey = indexFirstSlash > 0 ? filter[0..indexFirstSlash] : filter;
            if (!keywordsMap.TryGetValue(topLevelKey, out keywords))
            {
                if (indexFirstSlash > 0)
                {
                    var indexSecondSlash = filter.IndexOf('/', indexFirstSlash + 1);
                    var secondLevelKey = indexSecondSlash > 0 ? filter[0..indexSecondSlash] : filter;
                    keywordsMap.TryGetValue(secondLevelKey, out keywords);
                }
            }

            var result = keywords is { } && keywords.Length > 0 ?
                $"{string.Join(',', keywords)},{filter}" :
                null; // null - let vscode use the default (label)

            return result is string && surroundWithSingleQuotes ? StringUtils.EscapeBicepString(result) : result;
        }

        public string? TryGetSnippetFilterText(Snippet snippet)
        {
            var resourceTypesUsed = GetResourceTypesFromBicepText(snippet.Text);
            if (resourceTypesUsed.Length == 0)
            {
                // Don't provide filter text for non-resource snippets (e.g. param, var, output), just let vscode use label
                return null;
            }

            // Add the resource type and its keywords for all resourced used in the snippet
            var resourceTypeFilters = resourceTypesUsed.Select(rt => TryGetResourceTypeFilterText(new ResourceTypeReference(rt, null)) ?? rt).ToArray();
            return $"{snippet.Prefix},{snippet.Detail},{string.Join(',', resourceTypeFilters)}";
        }

        // Example match: resource /*${1:appServicePlan}*/appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
        //   => "Microsoft.Web/serverfarms"
        [GeneratedRegex("""'(?<resourceType>[a-z][a-z0-9.]+/[a-z0-9./]+)@[a-z0-9-]+'""", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)]
        private static partial Regex RegexResourceType();

        private string[] GetResourceTypesFromBicepText(string snippetText)
        {
            var matches = RegexResourceType().Matches(snippetText);
            return matches.Select(m => m.Groups[1].Value)
                .Distinct()
                .ToArray();
        }
    }
}
