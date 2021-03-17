// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Bicep.LanguageServer.Snippets
{
    public class ResourceSnippetsProvider : IResourceSnippetsProvider
    {
        private static readonly Dictionary<string, string> _snippetNameToDetailMap = new Dictionary<string, string>()
        {
            { "res-aks-cluster", "Kubernetes Service Cluster" },
            { "res-app-security-group", "Application Security Group" },
            { "res-automation-account", "Automation Account" },
            { "res-availability-set", "Availability Set" },
            { "res-container-group", "Container Group" },
            { "res-container-registry", "Container Registry" },
            { "res-cosmos-account", "Cosmos DB Database Account" },
            { "res-data-lake", "Data Lake Store Account" },
            { "res-dns-zone", "DNS Zone" },
            { "res-ip", "Public IP Address"},
            { "res-ip-prefix", "Public IP Prefix"}
        };

        private HashSet<ResourceSnippet> resourceSnippets = new HashSet<ResourceSnippet>();

        public ResourceSnippetsProvider()
        {
            Initialize();
        }

        private void Initialize()
        {
            string? currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            string templatesFolder = Path.Combine(currentDirectory ?? throw new ArgumentNullException("Path is null"),
                                                  "Snippets",
                                                  "Templates");

            foreach (KeyValuePair<string, string> kvp in _snippetNameToDetailMap)
            {
                string template = Path.Combine(templatesFolder, kvp.Key + ".bicep");
                ResourceSnippet resourceSnippet = new ResourceSnippet(kvp.Key, kvp.Value, File.ReadAllText(template));

                resourceSnippets.Add(resourceSnippet);
            }
        }

        public IEnumerable<ResourceSnippet> GetResourceSnippets() => resourceSnippets;
    }
}
