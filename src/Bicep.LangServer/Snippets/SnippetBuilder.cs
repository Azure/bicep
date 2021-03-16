// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bicep.LanguageServer.Snippets
{
    public static class SnippetBuilder
    {
        private static readonly Dictionary<string, string> _snippetNameToDetailMap = new Dictionary<string, string>()
        {
            { "aks-cluster", "Kubernetes Service Cluster" },
            { "app-security-group", "Application Security Group" },
            { "automation-account", "Automation Account" },
            { "availability-set", "Availability Set" },
            { "container-group", "Container Group" },
            { "container-registry", "Container Registry" },
            { "cosmos-account", "Cosmos DB Database Account" },
            { "data-lake", "Data Lake Store Account" },
            { "dns-zone", "DNS Zone" },
            { "ip", "Public IP Address"},
            { "ip-prefix", "Public IP Prefix"}
        };

        private static List<ResourceSnippet> resourceSnippets = new List<ResourceSnippet>();

        public static void CreateResourceSnippets()
        {
            if (!resourceSnippets.Any())
            {
                string? currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
                string templatesFolder = Path.Combine(currentDirectory!, "Snippets\\Templates");

                foreach (KeyValuePair<string, string> kvp in _snippetNameToDetailMap)
                {
                    string template = Path.Combine(templatesFolder, kvp.Key + ".bicep");
                    ResourceSnippet resourceSnippet = new ResourceSnippet(kvp.Key, kvp.Value, File.ReadAllText(template));

                    resourceSnippets.Add(resourceSnippet);
                }
            }
        }

        public static List<ResourceSnippet> GetResourceSnippets()
        {
            if (!resourceSnippets.Any())
            {
                string? currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
                string templatesFolder = Path.Combine(currentDirectory!, "Snippets\\Templates");

                foreach (KeyValuePair<string, string> kvp in _snippetNameToDetailMap)
                {
                    string template = Path.Combine(templatesFolder, kvp.Key + ".bicep");
                    ResourceSnippet resourceSnippet = new ResourceSnippet(kvp.Key, kvp.Value, File.ReadAllText(template));

                    resourceSnippets.Add(resourceSnippet);
                }
            }

            return resourceSnippets;
        }
    }
}
