// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.DirectoryServices.Protocols;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.LanguageServer.Completions;

namespace Bicep.LanguageServer.Snippets;

public class SnippetsProvider : ISnippetsProvider
{
    private const string RequiredPropertiesDescription = "Required properties";
    private const string RequiredPropertiesLabel = "required-properties";
    private static readonly Regex ParentPropertyPattern = new(@"^.*parent:.*$[\r\n]*", RegexOptions.Compiled | RegexOptions.Multiline);

    // Used to cache resource body snippets
    private readonly ConcurrentDictionary<(ResourceTypeReference resourceTypeReference, bool isExistingResource), IEnumerable<Snippet>> resourceBodySnippetsCache = new();
    // The common properties should be authored consistently to provide for understandability and consumption of the code.
    // See https://github.com/Azure/azure-quickstart-templates/blob/master/1-CONTRIBUTION-GUIDE/best-practices.md#resources
    // for more information
    private static readonly ImmutableArray<string> PropertiesSortPreferenceList = ["scope", "parent", "name", "location", "zones", "sku", "kind", "scale", "plan", "identity", "tags", "properties", "dependsOn"];

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
                if (GetResourceBodyCompletionSnippetFromTemplate(resourceTypeReference) is { } snippetFromExistingTemplate)
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
            string label = "required-properties-" + disciminatedObjectKey.Trim(['\'']);
            Snippet? snippet = GetRequiredPropertiesSnippet(kvp.Value, label, disciminatedObjectKey);

            if (snippet is not null)
            {
                yield return snippet;
            }
        }
    }

    private static ObjectSyntax GetObjectSnippetSyntax(ObjectType objectType, ref int tabStopIndex, string? discriminatedObjectKey)
    {
        var typeProperties = objectType.Properties.Values.OrderBy(x =>
            PropertiesSortPreferenceList.IndexOf(x.Name) switch
            {
                -1 => int.MaxValue,
                int index => index,
            })
            .Where(TypeHelper.IsRequired);

        var objectProperties = new List<ObjectPropertySyntax>();
        foreach (var typeProperty in typeProperties)
        {
            // Here we deliberately want to iterate in the correct order, and use a DFS approach, to ensure that the tab stops are correctly ordered.
            // For example, we want to ensure we output: {\n  foo: $1\n  nested: {\n    bar: $2\n  }\n  baz: $3\n}
            // Instead of:                               {\n  foo: $1\n  nested: {\n    bar: $3\n  }\n  baz: $2\n}
            objectProperties.Add(GetObjectPropertySnippetSyntax(typeProperty, ref tabStopIndex, discriminatedObjectKey));
        }

        return SyntaxFactory.CreateObject(objectProperties);
    }

    private static ObjectPropertySyntax GetObjectPropertySnippetSyntax(NamedTypeProperty typeProperty, ref int tabStopIndex, string? discriminatedObjectKey)
    {
        var valueType = typeProperty.TypeReference.Type;
        if (valueType is ObjectType objectType)
        {
            return SyntaxFactory.CreateObjectProperty(
                typeProperty.Name,
                GetObjectSnippetSyntax(objectType, ref tabStopIndex, null));
        }
        else if (discriminatedObjectKey is { } &&
            valueType is StringLiteralType stringLiteralType &&
            stringLiteralType.Name == discriminatedObjectKey)
        {
            return SyntaxFactory.CreateObjectProperty(
                typeProperty.Name,
                SyntaxFactory.CreateStringLiteral(stringLiteralType.RawStringValue));
        }
        else
        {
            var newTabStopIndex = tabStopIndex++;
            return SyntaxFactory.CreateObjectProperty(
                typeProperty.Name,
                SyntaxFactory.CreateFreeformToken(TokenType.Unrecognized, GetTabStop(newTabStopIndex)));
        }
    }

    private static string GetTabStop(int index)
        => $"${index}";

    private Snippet? GetRequiredPropertiesSnippet(ObjectType objectType, string label, string? discriminatedObjectKey = null)
    {
        if (!objectType.Properties.Values.Any(TypeHelper.IsRequired))
        {
            return null;
        }

        var tabStopIndex = 1;
        var syntax = GetObjectSnippetSyntax(objectType, ref tabStopIndex, discriminatedObjectKey);

        var output = PrettyPrinterV2.PrintValid(syntax, PrettyPrinterV2Options.Default with { IndentKind = IndentKind.Tab }) + GetTabStop(0);
        return new Snippet(output, CompletionPriority.Medium, label, RequiredPropertiesDescription);
    }

    private Snippet GetEmptySnippet()
    {
        string label = "{}";

        return new Snippet("{\n\t$0\n}", CompletionPriority.Medium, label, label);
    }

    public IEnumerable<Snippet> GetModuleBodyCompletionSnippets(TypeSymbol typeSymbol)
    {
        yield return GetEmptySnippet();

        if (typeSymbol is ModuleType moduleType && moduleType.Body is ObjectType objectType)
        {
            Snippet? snippet = GetRequiredPropertiesSnippet(objectType, RequiredPropertiesLabel, RequiredPropertiesDescription);

            if (snippet is not null)
            {
                yield return snippet;
            }
        }
    }
    public IEnumerable<Snippet> GetTestBodyCompletionSnippets(TypeSymbol typeSymbol)
    {
        yield return GetEmptySnippet();

        if (typeSymbol is TestType testType && testType.Body is ObjectType objectType)
        {
            Snippet? snippet = GetRequiredPropertiesSnippet(objectType, RequiredPropertiesLabel, RequiredPropertiesDescription);

            if (snippet is not null)
            {
                yield return snippet;
            }
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
                var nestedTypeReference = new ResourceTypeReference(nestedResourceTypeReference.TypeSegments.Last(), nestedResourceTypeReference.ApiVersion);

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

    public IEnumerable<Snippet> GetIdentitySnippets(bool isResource)
    {
        string userAssignedIdentityLabel = "user-assigned-identity";
        string userAssignedIdentityDescription = "User assigned identity";
        string userAssignedIdentityArrayLabel = "user-assigned-identity-array";
        string userAssignedIdentityArrayDescription = "User assigned identity array";
        string noneIdentityLabel = "none-identity";
        string noneIdentityDescription = "None identity";

        string systemAssignedIdentityLabel = "system-assigned-identity";
        string systemAssignedIdentityDescription = "System assigned identity";
        string userAndSystemAssignedIdentityLabel = "user-and-system-assigned-identity";
        string userAndSystemAssignedIdentityDescription = "User and system assigned identity";

        yield return new Snippet("""
            {
              type: 'UserAssigned'
              userAssignedIdentities: {
                '${${0:identityId}}': {}
              }
            }
            """, CompletionPriority.High, userAssignedIdentityLabel, userAssignedIdentityDescription);

        yield return new Snippet("""
            {
              type: 'UserAssigned'
              userAssignedIdentities: toObject(${0:identityIdArray}, x => x, x => {})
            }
            """, CompletionPriority.High, userAssignedIdentityArrayLabel, userAssignedIdentityArrayDescription);

        yield return new Snippet("""
            {
              type: 'None'
            }
            """, CompletionPriority.High, noneIdentityLabel, noneIdentityDescription);

        if (isResource)
        {
            yield return new Snippet("""
            {
              type: 'SystemAssigned'
            }
            """, CompletionPriority.High, systemAssignedIdentityLabel, systemAssignedIdentityDescription);

            yield return new Snippet("""
            {
              type: 'SystemAssigned,UserAssigned'
              userAssignedIdentities: {
                '${${0:identityId}}': {}
              }
            }
            """, CompletionPriority.High, userAndSystemAssignedIdentityLabel, userAndSystemAssignedIdentityDescription);
        }
    }
}
