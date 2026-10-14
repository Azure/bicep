// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Analyzers
{
    internal static class ResourceSymbolicNameMapper
    {
        public const string BicepSymbolicNameProperty = "bicepSymbolicName";

        public record SourceMappingResult(
            Dictionary<string, DeclaredResourceMetadata> SymbolicNameMap,
            JContainer EnrichedResources);

        /// <summary>
        /// Builds the source lookup and enriches emitted resources only when attribution is unambiguous.
        /// </summary>
        public static SourceMappingResult? BuildMapping(string templateJson, SemanticModel model)
        {
            try
            {
                // Enrichment must preserve date-like strings exactly as the emitter produced them.
                using var stringReader = new StringReader(templateJson);
                using var jsonReader = new JsonTextReader(stringReader)
                {
                    DateParseHandling = DateParseHandling.None,
                };
                var resourcesToken = JObject.Load(jsonReader)["resources"];
                if (resourcesToken is null)
                {
                    return null;
                }

                var symbolicNameMap = new Dictionary<string, DeclaredResourceMetadata>(LanguageConstants.IdentifierComparer);
                var resourcesByType = new Dictionary<string, List<(string SymbolicName, DeclaredResourceMetadata Metadata)>>(StringComparer.OrdinalIgnoreCase);
                var hasModules = model.Root.ModuleDeclarations.Any();

                foreach (var resource in model.DeclaredResources)
                {
                    var symbolicName = Emit.ExpressionConverter.GetSymbolicName(model.ResourceAncestors, resource);
                    symbolicNameMap[symbolicName] = resource;

                    var resourceType = resource.Type.TypeReference.FormatType();
                    // Array output cannot distinguish module deployments from explicit deployment resources.
                    if (resource.IsExistingResource ||
                        (hasModules && resourceType.Equals(Emit.TemplateWriter.NestedDeploymentResourceType, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    if (!resourcesByType.TryGetValue(resourceType, out var resources))
                    {
                        resources = [];
                        resourcesByType[resourceType] = resources;
                    }

                    resources.Add((symbolicName, resource));
                }

                switch (resourcesToken)
                {
                    case JArray resourcesArray:
                        for (var index = 0; index < resourcesArray.Count; index++)
                        {
                            if (resourcesArray[index] is JObject resource)
                            {
                                if (MatchResourceToSymbolicName(resource, resourcesByType) is { } symbolicName)
                                {
                                    resource[BicepSymbolicNameProperty] = symbolicName;
                                }
                            }
                        }

                        return new(symbolicNameMap, resourcesArray);

                    case JObject resourcesObject:
                        foreach (var property in resourcesObject.Properties())
                        {
                            if (property.Value is JObject resource)
                            {
                                resource[BicepSymbolicNameProperty] = property.Name;
                            }
                        }

                        return new(symbolicNameMap, resourcesObject);

                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static TextSpan FindSpan(string? symbolicName, Dictionary<string, DeclaredResourceMetadata> symbolicNameMap)
            => symbolicName is not null && symbolicNameMap.TryGetValue(symbolicName, out var metadata)
                ? metadata.Symbol.DeclaringResource.Span
                : TextSpan.TextDocumentStart;

        public static string? GetSymbolicName(JObject resource) => resource[BicepSymbolicNameProperty]?.ToString();

        /// <summary>
        /// Matches a legacy array resource to one literal Bicep resource declaration.
        /// </summary>
        private static string? MatchResourceToSymbolicName(
            JObject resource,
            Dictionary<string, List<(string SymbolicName, DeclaredResourceMetadata Metadata)>> resourcesByType)
        {
            var resourceType = resource["type"]?.ToString();
            var resourceName = resource["name"]?.ToString();

            if (resourceType is not null && resourcesByType.TryGetValue(resourceType, out var candidates))
            {
                if (resourceName is not null && !resourceName.StartsWith('['))
                {
                    // Prefer no attribution over guessing when a literal name does not identify exactly one declaration.
                    var exactMatches = candidates
                        .Where(candidate =>
                            candidate.Metadata.TryGetNameSyntax() is StringSyntax nameSyntax &&
                            nameSyntax.TryGetLiteralValue() is { } bicepName &&
                            resourceName.Equals(bicepName, StringComparison.OrdinalIgnoreCase))
                        .Select(candidate => candidate.SymbolicName)
                        .Distinct(LanguageConstants.IdentifierComparer)
                        .ToArray();
                    if (exactMatches is [var exactMatch])
                    {
                        return exactMatch;
                    }
                    if (exactMatches.Length > 1)
                    {
                        return null;
                    }

                }
            }

            return null;
        }
    }
}