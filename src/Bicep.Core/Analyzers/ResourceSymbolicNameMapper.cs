// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Types;
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
                var resourcesToken = ParsePreservingNumbers(templateJson)["resources"];
                if (resourcesToken is null)
                {
                    return null;
                }

                var symbolicNameMap = new Dictionary<string, DeclaredResourceMetadata>(LanguageConstants.IdentifierComparer);
                var resourcesByType = new Dictionary<string, List<(string SymbolicName, DeclaredResourceMetadata Metadata)>>(StringComparer.OrdinalIgnoreCase);
                var moduleNames = model.Root.ModuleDeclarations
                    .Select(module => module.TryGetBodyProperty(LanguageConstants.ModuleNamePropertyName)?.Value)
                    .Select(value => value is null ? null : model.GetTypeInfo(value) as StringLiteralType)
                    .Select(type => type?.RawStringValue)
                    .ToArray();
                var hasDynamicModuleName = moduleNames.Any(name => name is null);
                var literalModuleNames = moduleNames.OfType<string>().ToHashSet(StringComparer.OrdinalIgnoreCase);

                foreach (var resource in model.DeclaredResources)
                {
                    var symbolicName = Emit.ExpressionConverter.GetSymbolicName(model.ResourceAncestors, resource);
                    symbolicNameMap[symbolicName] = resource;

                    var resourceType = resource.Type.TypeReference.FormatType();
                    // Array output cannot distinguish module deployments from explicit deployment resources.
                    if (resource.IsExistingResource ||
                        (resourceType.Equals(Emit.TemplateWriter.NestedDeploymentResourceType, StringComparison.OrdinalIgnoreCase) &&
                         (hasDynamicModuleName ||
                          TryGetLiteralFullName(model, resource) is { } deploymentName &&
                          literalModuleNames.Contains(deploymentName))))
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
                        var emittedIdentityCounts = CountEmittedResourceIdentities(resourcesArray);
                        for (var index = 0; index < resourcesArray.Count; index++)
                        {
                            if (resourcesArray[index] is JObject resource)
                            {
                                if (resource["copy"] is null &&
                                    HasUniqueEmittedIdentity(resource, emittedIdentityCounts) &&
                                    MatchResourceToSymbolicName(resource, resourcesByType) is { } symbolicName)
                                {
                                    resource[BicepSymbolicNameProperty] = symbolicName;
                                }
                            }
                        }

                        return new(symbolicNameMap, resourcesArray);

                    case JObject resourcesObject:
                        foreach (var property in resourcesObject.Properties())
                        {
                            if (property.Value is JObject resource && symbolicNameMap.ContainsKey(property.Name))
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

        private static JObject ParsePreservingNumbers(string json)
        {
            using var document = JsonDocument.Parse(json);
            return (JObject)ConvertJsonElement(document.RootElement);
        }

        private static JToken ConvertJsonElement(JsonElement element) => element.ValueKind switch
        {
            JsonValueKind.Object => new JObject(element.EnumerateObject()
                .Select(property => new JProperty(property.Name, ConvertJsonElement(property.Value)))),
            JsonValueKind.Array => new JArray(element.EnumerateArray().Select(ConvertJsonElement)),
            JsonValueKind.String => new JValue(element.GetString()),
            JsonValueKind.Number => new JRaw(element.GetRawText()),
            JsonValueKind.True => new JValue(true),
            JsonValueKind.False => new JValue(false),
            JsonValueKind.Null => JValue.CreateNull(),
            _ => throw new InvalidOperationException($"Unsupported JSON value kind {element.ValueKind}."),
        };

        private static Dictionary<string, Dictionary<string, int>> CountEmittedResourceIdentities(JArray resources)
        {
            var counts = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);
            foreach (var resource in resources.OfType<JObject>())
            {
                if (TryGetEmittedIdentity(resource) is not { } identity)
                {
                    continue;
                }

                if (!counts.TryGetValue(identity.Type, out var names))
                {
                    names = new(StringComparer.OrdinalIgnoreCase);
                    counts[identity.Type] = names;
                }

                names.TryGetValue(identity.Name, out var count);
                names[identity.Name] = count + 1;
            }

            return counts;
        }

        private static bool HasUniqueEmittedIdentity(
            JObject resource,
            Dictionary<string, Dictionary<string, int>> emittedIdentityCounts) =>
            TryGetEmittedIdentity(resource) is { } identity &&
            emittedIdentityCounts.TryGetValue(identity.Type, out var names) &&
            names.TryGetValue(identity.Name, out var count) &&
            count == 1;

        private static (string Type, string Name)? TryGetEmittedIdentity(JObject resource)
        {
            var type = resource["type"]?.ToString();
            var name = resource["name"]?.ToString();

            return type is not null && name is not null
                ? (type, name)
                : null;
        }

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

                if (candidates is [var onlyCandidate] &&
                    !resourceType.Equals(Emit.TemplateWriter.NestedDeploymentResourceType, StringComparison.OrdinalIgnoreCase))
                {
                    return onlyCandidate.SymbolicName;
                }
            }

            return null;
        }

        private static string? TryGetLiteralFullName(SemanticModel model, DeclaredResourceMetadata resource)
        {
            var resources = model.ResourceAncestors.GetAncestors(resource)
                .Select(ancestor => ancestor.Resource)
                .Append(resource);
            var names = new List<string>();
            foreach (var item in resources)
            {
                if (item.TryGetNameSyntax() is not { } nameSyntax ||
                    model.GetTypeInfo(nameSyntax) is not StringLiteralType literalName)
                {
                    return null;
                }

                names.Add(literalName.RawStringValue);
            }

            return string.Join("/", names);
        }
    }
}