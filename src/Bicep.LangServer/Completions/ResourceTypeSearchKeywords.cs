// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;

namespace Bicep.LanguageServer.Completions
{
    public class ResourceTypeSearchKeywords
    {
        private readonly ImmutableDictionary<string, ImmutableArray<string>> keywordsMap;

        public ResourceTypeSearchKeywords() : this
            (new Dictionary<string, string[]>()
            {
                // Keys must be in the form 'xxx' or 'xxx/yyy' - 'xxx' matches 'xxx' and 'xxx/*', 'xxx/yyy' matches 'xxx/yyy' and 'xxx/yyy/*'
                // Key casing doesn't matter
                ["Microsoft.Web/sites"] = ["appservice", "webapp", "function"],
                ["Microsoft.Web/serverFarms"] = ["asp", "appserviceplan", "hostingplan"],
                ["Microsoft.App"] = ["containerapp"],
                ["Microsoft.ContainerService"] = ["aks", "kubernetes", "k8s", "cluster"],
                ["Microsoft.Authorization/roleAssignments"] = ["rbac"],
                ["Microsoft.DocumentDb"] = ["cosmosdb"],
            })
        {
        }

        public ResourceTypeSearchKeywords(IDictionary<string, string[]> keywordsMap)
        {
            // Validate
            foreach ((var key, var keywords) in keywordsMap)
            {
                if (key.Split('/').Length > 2)
                {
                    throw new ArgumentException($"Keys in {nameof(keywordsMap)} can have at most one slash: {key}");
                }
            }

            this.keywordsMap = keywordsMap.ToImmutableDictionary(x => x.Key.ToLowerInvariant(), x => x.Value.ToImmutableArray(), StringComparer.InvariantCultureIgnoreCase);
        }

        public string? TryGetResourceTypeFilterText(ResourceTypeReference resourceType)
        {
            var filter = resourceType.Type;
            ImmutableArray<string> keywords;

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

            // The filter text, like the insertText, must include the single quotes that surround the resource type in the resource declaration
            return !keywords.IsDefaultOrEmpty ?
                StringUtils.EscapeBicepString($"{string.Join(',', keywords)},{filter}") :
                null; // null - let vscode use the default (label)
        }
    }
}
