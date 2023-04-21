// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core;
using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Completions;

namespace Bicep.LanguageServer.Snippets;

public class SnippetsProvider : ISnippetsProvider
{
    private const string RequiredPropertiesDescription = "Required properties";
    private const string RequiredPropertiesLabel = "required-properties";
    private static readonly Regex ParentPropertyPattern = new Regex(@"^.*parent:.*$[\r\n]*", RegexOptions.Compiled | RegexOptions.Multiline);

    // Used to cache resource body snippets
    private readonly ConcurrentDictionary<(ResourceTypeReference resourceTypeReference, bool isExistingResource), IEnumerable<Snippet>> resourceBodySnippetsCache = new();
    // The common properties should be authored consistently to provide for understandability and consumption of the code.
    // See https://github.com/Azure/azure-quickstart-templates/blob/master/1-CONTRIBUTION-GUIDE/best-practices.md#resources
    // for more information
    private readonly ImmutableArray<string> propertiesSortPreferenceList = ImmutableArray.Create(
        "scope",
        "parent",
        "name",
        "location",
        "zones",
        "sku",
        "kind",
        "scale",
        "plan",
        "identity",
        "tags",
        "properties",
        "dependsOn");

    private static readonly SnippetCache snippetCache = SnippetCache.FromManifest();

    public IEnumerable<Snippet> GetTopLevelNamedDeclarationSnippets() => snippetCache.TopLevelNamedDeclarationSnippets;

    public IEnumerable<Snippet> GetResourceBodyCompletionSnippets(ResourceType resourceType, bool isExistingResource, bool isResourceNested)
    {
        var resourceTypeReference = resourceType.TypeReference;
        if (resourceBodySnippetsCache.TryGetValue((resourceTypeReference, isExistingResource), out var cachedSnippets))
        {
            return cachedSnippets;
        }

        var snippets = new List<Snippet>();

        snippets.Add(GetEmptySnippet());

        // We will not show custom snippets for resources with 'existing' keyword as they are not applicable in that scenario.
        if (!isExistingResource)
        {
            // If the resource is nested, we will only return it's body text from cache. Otherwise, we will return information
            // from the template, which could include parent resource
            if (isResourceNested)
            {
                if (snippetCache.ResourceTypeReferenceInfoMap.TryGetValue(resourceTypeReference, out var resourceTypeInfo))
                {
                    // The property "parent" is not allowed in nested resource. We'll remove the property before creating the snippet
                    var text = ParentPropertyPattern.Replace(resourceTypeInfo.BodyText, string.Empty);
                    var snippet = new Snippet(text, prefix: "snippet", detail: resourceTypeInfo.Description);
                    snippets.Add(snippet);
                }
            }
            else
            {
                if (GetResourceBodyCompletionSnippetFromTemplate(resourceTypeReference) is {} snippetFromExistingTemplate)
                {
                    snippets.Add(snippetFromExistingTemplate);
                }
            }
        }

        var snippetsFromAzTypes = GetRequiredPropertiesForObjectType(resourceType.Body.Type);

        if (snippetsFromAzTypes.Any())
        {
            snippets.AddRange(snippetsFromAzTypes);
        }

        // Add to cache
        // Note: Properties information obtained from TypeSystem may vary for resources with/without 'existing' keyword.
        // ResourceTypeReference obtained from ResourceType might be same in both the cases. In order to differentiate, we'll always
        // cache combination of resourceTypeReference + isExistingResource.
        resourceBodySnippetsCache.TryAdd((resourceTypeReference, isExistingResource), snippets);

        return snippets;
    }

    private Snippet? GetResourceBodyCompletionSnippetFromTemplate(ResourceTypeReference resourceTypeReference)
    {
        var label = "snippet";
        var sb = new StringBuilder();

        // Get resource body completion snippet from checked in static template file, if available
        if (snippetCache.ResourceTypeReferenceInfoMap.TryGetValue(resourceTypeReference, out var resourceBodyWithDescription))
        {
            sb.AppendLine(resourceBodyWithDescription.BodyText);

            if (snippetCache.ResourceTypeReferenceToDependentsMap.TryGetValue(resourceTypeReference, out var resourceDependencies))
            {
                sb.Append(resourceDependencies);
            }

            return new Snippet(sb.ToString(), CompletionPriority.Medium, label, resourceBodyWithDescription.Description);
        }

        return null;
    }

    private IEnumerable<Snippet> GetRequiredPropertiesSnippetsForDisciminatedObjectType(DiscriminatedObjectType discriminatedObjectType)
    {
        foreach (var kvp in discriminatedObjectType.UnionMembersByKey.OrderBy(x => x.Key))
        {
            string disciminatedObjectKey = kvp.Key;
            string label = "{RequiredPropertiesLabel}-" + disciminatedObjectKey.Trim(new char[] { '\'' });
            Snippet? snippet = GetRequiredPropertiesSnippet(kvp.Value, label, disciminatedObjectKey);

            if (snippet is not null)
            {
                yield return snippet;
            }
        }
    }

    private Snippet? GetRequiredPropertiesSnippet(ObjectType objectType, string label, string? discriminatedObjectKey = null)
    {
        int index = 1;
        StringBuilder sb = new StringBuilder();

        var sortedProperties = objectType.Properties.OrderBy(x => {
            var index = propertiesSortPreferenceList.IndexOf(x.Key);

            return (index > -1) ? index : (propertiesSortPreferenceList.Length - 1);
        });

        foreach (var (key, value) in sortedProperties)
        {
            string? snippetText = GetSnippetText(value, indentLevel: 1, ref index, discriminatedObjectKey);

            if (snippetText is not null)
            {
                sb.Append(snippetText);
            }
        }

        if (sb.Length > 0)
        {
            // Insert open curly at the beginning
            sb.Insert(0, "{\n");

            // Insert final tab stop outside the top level object
            sb.Append("}$0");

            return new Snippet(sb.ToString(), CompletionPriority.Medium, label, RequiredPropertiesDescription);
        }

        return null;
    }

    private string? GetSnippetText(TypeProperty typeProperty, int indentLevel, ref int index, string? discriminatedObjectKey = null)
    {
        if (TypeHelper.IsRequired(typeProperty))
        {
            StringBuilder sb = new StringBuilder();

            if (typeProperty.TypeReference.Type is ObjectType objectType)
            {
                sb.AppendLine(GetIndentString(indentLevel) + typeProperty.Name + ": {");

                indentLevel++;

                foreach (KeyValuePair<string, TypeProperty> kvp in objectType.Properties.OrderBy(x => x.Key))
                {
                    string? snippetText = GetSnippetText(kvp.Value, indentLevel, ref index);
                    if (snippetText is not null)
                    {
                        sb.Append(snippetText);
                    }
                }

                indentLevel--;
                sb.AppendLine(GetIndentString(indentLevel) + "}");
            }
            else
            {
                string value = ": $" + (index).ToString();
                bool shouldIncrementIndent = true;

                if (discriminatedObjectKey is not null &&
                    typeProperty.TypeReference.Type is TypeSymbol typeSymbol &&
                    typeSymbol.Name == discriminatedObjectKey)
                {
                    value = ": " + discriminatedObjectKey;
                    shouldIncrementIndent = false;
                }

                sb.AppendLine(GetIndentString(indentLevel) + typeProperty.Name + value);

                if (shouldIncrementIndent)
                {
                    index++;
                }
            }

            return sb.ToString();
        }

        return null;
    }

    private string GetIndentString(int indentLevel)
    {
        return new string('\t', indentLevel);
    }

    private Snippet GetEmptySnippet()
    {
        string label = "{}";

        return new Snippet("{\n\t$0\n}", CompletionPriority.Medium, label, label);
    }

    public IEnumerable<Snippet> GetModuleBodyCompletionSnippets(ModuleDeclarationSyntax moduleDeclarationSyntax, TypeSymbol typeSymbol)
    {
        yield return GetEmptySnippet();

        if (typeSymbol is ModuleType moduleType && moduleType.Body is ObjectType objectType)
        {
            Snippet? snippet = GetRequiredPropertiesSnippet(objectType, RequiredPropertiesLabel, RequiredPropertiesDescription);

            if (snippet is not null)
            {
                yield return snippet;
            }
        } else
        {
            yield return new Snippet("{\n\tname: $1\n\tparams: {\n\t\t// Module had not yet been restored. There may be required parameters.\n\t}\n}$0", CompletionPriority.Medium, RequiredPropertiesLabel, RequiredPropertiesDescription);
        }
    }

    public IEnumerable<Snippet> GetObjectBodyCompletionSnippets(TypeSymbol typeSymbol)
    {
        yield return GetEmptySnippet();

        foreach (Snippet snippet in GetRequiredPropertiesForObjectType(typeSymbol))
        {
            yield return snippet;
        }
    }

    private IEnumerable<Snippet> GetRequiredPropertiesForObjectType(TypeSymbol typeSymbol)
    {
        if (typeSymbol is ObjectType objectType)
        {
            Snippet? snippet = GetRequiredPropertiesSnippet(objectType, RequiredPropertiesLabel, RequiredPropertiesDescription);

            if (snippet is not null)
            {
                yield return snippet;
            }
        }
        else if (typeSymbol is DiscriminatedObjectType discriminatedObjectType)
        {
            foreach (Snippet snippet in GetRequiredPropertiesSnippetsForDisciminatedObjectType(discriminatedObjectType))
            {
                yield return snippet;
            }
        }
    }

    public IEnumerable<Snippet> GetNestedResourceDeclarationSnippets(ResourceTypeReference resourceTypeReference)
    {
        // Leaving out the API version on this, because we expect its more common to inherit from the containing resource.
        yield return new Snippet(@"resource ${1:Identifier} '${2:Type}' = {
  name: $3
  properties: {
    $0
  }
}", prefix: "resource-with-defaults", detail: "Nested resource with defaults");

        yield return new Snippet(@"resource ${1:Identifier} '${2:Type}' = {
  name: $3
  $0
}", prefix: "resource-without-defaults", detail: "Nested resource without defaults");

        if (snippetCache.ResourceTypeReferenceToChildTypeSymbolsMap.TryGetValue(resourceTypeReference, out var nestedResourceTypeReferences))
        {
            foreach (var nestedResourceTypeReference in nestedResourceTypeReferences)
            {
                var nestedTypeReference = new ResourceTypeReference(ImmutableArray.Create<string>(nestedResourceTypeReference.TypeSegments.Last()), nestedResourceTypeReference.ApiVersion);

                if (snippetCache.ResourceTypeReferenceInfoMap.TryGetValue(nestedResourceTypeReference, out var resourceInfo))
                {
                    // The property "parent" is not allowed in nested resource. We'll remove the property before creating the snippet
                    var bodyText = ParentPropertyPattern.Replace(resourceInfo.BodyText, string.Empty);
                    var text = LanguageConstants.ResourceKeyword + " " + resourceInfo.Identifier + " '" + nestedTypeReference.FormatName() + "' = " + bodyText;

                    yield return new Snippet(text, prefix: resourceInfo.Prefix, detail: resourceInfo.Description);
                }
            }
        }
    }
}
