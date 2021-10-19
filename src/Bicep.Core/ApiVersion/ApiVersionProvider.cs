// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.ApiVersion
{
    public class ApiVersionProvider : IApiVersionProvider
    {
        private Dictionary<string, List<string>> alphaVersions = new();
        private Dictionary<string, List<string>> betaVersions = new();
        private Dictionary<string, List<string>> gaVersions = new();
        private Dictionary<string, List<string>> previewVersions = new();
        private Dictionary<string, List<string>> privatePreviewVersions = new();
        private Dictionary<string, List<string>> rcVersions = new();

        private static readonly Regex VersionPattern = new Regex(@"^((?<version>(\d{4}-\d{2}-\d{2}))(?<suffix>-(preview|alpha|beta|rc|privatepreview))?$)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private readonly INamespaceProvider NamespaceProvider;

        public ApiVersionProvider(INamespaceProvider namespaceProvider)
        {
            NamespaceProvider = namespaceProvider;

            CacheApiVersions();
        }

        private void CacheApiVersions()
        {
            NamespaceResolver namespaceResolver = NamespaceResolver.Create(NamespaceProvider, TypeSystem.ResourceScope.ResourceGroup, Enumerable.Empty<ImportedNamespaceSymbol>());
            IEnumerable<ResourceTypeReference> resourceTypeReferences = namespaceResolver.GetAvailableResourceTypes();

            foreach (var resourceTypeReference in resourceTypeReferences)
            {
                (string? date, string? suffix) = ParseApiVersion(resourceTypeReference.ApiVersion);

                switch (suffix)
                {
                    case ApiVersionSuffixConstants.GA:
                        UpdateCache(gaVersions, date, resourceTypeReference.FullyQualifiedType);
                        break;
                    case ApiVersionSuffixConstants.Alpha:
                        UpdateCache(alphaVersions, date, resourceTypeReference.FullyQualifiedType);
                        break;
                    case ApiVersionSuffixConstants.Beta:
                        UpdateCache(betaVersions, date, resourceTypeReference.FullyQualifiedType);
                        break;
                    case ApiVersionSuffixConstants.Preview:
                        UpdateCache(previewVersions, date, resourceTypeReference.FullyQualifiedType);
                        break;
                    case ApiVersionSuffixConstants.PrivatePreview:
                        UpdateCache(privatePreviewVersions, date, resourceTypeReference.FullyQualifiedType);
                        break;
                    case ApiVersionSuffixConstants.RC:
                        UpdateCache(rcVersions, date, resourceTypeReference.FullyQualifiedType);
                        break;
                }
            }

            gaVersions = gaVersions.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(y => y).ToList());
            alphaVersions = alphaVersions.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(y => y).ToList());
            betaVersions = betaVersions.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(y => y).ToList());
            previewVersions = previewVersions.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(y => y).ToList());
            privatePreviewVersions = privatePreviewVersions.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(y => y).ToList());
            rcVersions = rcVersions.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(y => y).ToList());
        }

        private void UpdateCache(Dictionary<string, List<string>> cache, string? apiVersion, string fullyQualifiedType)
        {
            if (apiVersion is not null)
            {
                if (cache.TryGetValue(fullyQualifiedType, out List<string> value))
                {
                    value.Add(apiVersion);
                    cache[fullyQualifiedType] = value;
                }
                else
                {
                    cache.Add(fullyQualifiedType, new List<string> { apiVersion });
                }
            }
        }

        public string? GetRecentApiVersion(string fullyQualifiedName, string? suffix)
        {
            switch (suffix)
            {
                case ApiVersionSuffixConstants.GA:
                    return GetRecentApiVersion(fullyQualifiedName, gaVersions);
                case ApiVersionSuffixConstants.Alpha:
                    return GetRecentApiVersion(fullyQualifiedName, alphaVersions);
                case ApiVersionSuffixConstants.Beta:
                    return GetRecentApiVersion(fullyQualifiedName, betaVersions);
                case ApiVersionSuffixConstants.Preview:
                    return GetRecentApiVersion(fullyQualifiedName, previewVersions);
                case ApiVersionSuffixConstants.PrivatePreview:
                    return GetRecentApiVersion(fullyQualifiedName, privatePreviewVersions);
                case ApiVersionSuffixConstants.RC:
                    return GetRecentApiVersion(fullyQualifiedName, rcVersions);
            }

            return null;
        }

        private string? GetRecentApiVersion(string fullyQualifiedName, Dictionary<string, List<string>> cache)
        {
            if (cache.TryGetValue(fullyQualifiedName, out List<string> apiVersionDates) && apiVersionDates.Any())
            {
                return apiVersionDates.First();
            }

            return null;
        }

        public (string?, string?) ParseApiVersion(string apiVersion)
        {
            MatchCollection matches = VersionPattern.Matches(apiVersion);
            string? suffix = null;
            string? version = null;

            foreach (Match match in matches)
            {
                version = match.Groups["version"].Value;
                suffix = match.Groups["suffix"].Value;

                if (version is not null)
                {
                    return (version, suffix);
                }
            }

            return (version, suffix);
        }
    }
}
