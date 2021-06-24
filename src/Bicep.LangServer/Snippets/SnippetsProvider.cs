// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer.Completions;

namespace Bicep.LanguageServer.Snippets
{
    public class SnippetsProvider : ISnippetsProvider
    {
        private static readonly Regex PlaceholderPattern = new Regex(@"\$({(?<index>\d+):(?<name>[^}]+)}|(?<index>\d+)|{(?<index>\d+)\|((?<name>[^,]+)(.*))\|})", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private const string RequiredPropertiesDescription = "Required properties";
        private const string RequiredPropertiesLabel = "required-properties";

        // Used to cache resource declarations. Maps resource type to resource prefix, identifier, body text and description
        private readonly ConcurrentDictionary<string, (string prefix, string identifier, string bodyText, string description)> resourceTypeInfoMap = new();
        // Used to cache resource dependencies. Maps resource type to it's dependencies
        private readonly ConcurrentDictionary<string, string> resourceTypeToDependentsMap = new();
        private readonly ConcurrentDictionary<string, string> resourceTypeToTypeMap = new();
        private readonly ConcurrentDictionary<string, List<TypeSymbol>> resourceTypeToChildSymbolsMap = new();
        // Used to cache resource body snippets
        private readonly ConcurrentDictionary<(string typeName, bool isExistingResource), IEnumerable<Snippet>> resourceBodySnippetsCache = new();
        // Used to cache top level declarations
        private readonly HashSet<Snippet> topLevelNamedDeclarationSnippets = new();
        // The common properties should be authored consistently to provide for understandability and consumption of the code.
        // See https://github.com/Azure/azure-quickstart-templates/blob/master/1-CONTRIBUTION-GUIDE/best-practices.md#resources
        // for more information
        private readonly List<string> propertiesSortPreferenceList = new()
        {
            "comments",
            "condition",
            "scope",
            "type",
            "apiVersion",
            "name",
            "location",
            "zones",
            "sku",
            "kind",
            "scale",
            "plan",
            "identity",
            "copy",
            "dependsOn",
            "tags",
            "properties"
        };
        private readonly IFileResolver fileResolver;

        public SnippetsProvider(IFileResolver fileResolver)
        {
            Initialize();
            this.fileResolver = fileResolver;
        }

        private void Initialize()
        {
            string pathPrefix = "Snippets/Templates/";
            Assembly assembly = typeof(SnippetsProvider).Assembly;
            IEnumerable<string> manifestResourceNames = assembly.GetManifestResourceNames().Where(p => p.StartsWith(pathPrefix, StringComparison.Ordinal));

            foreach (var manifestResourceName in manifestResourceNames)
            {
                Stream? stream = assembly.GetManifestResourceStream(manifestResourceName);
                var streamReader = new StreamReader(stream ?? throw new ArgumentNullException("Stream is null"), Encoding.Default);

                (string description, string snippetText) = GetDescriptionAndText(streamReader.ReadToEnd(), manifestResourceName);
                string prefix = Path.GetFileNameWithoutExtension(manifestResourceName);
                CompletionPriority completionPriority = CompletionPriority.Medium;

                if (prefix.StartsWith("resource"))
                {
                    completionPriority = CompletionPriority.High;
                }

                var snippet = new Snippet(snippetText, completionPriority, prefix, description);

                topLevelNamedDeclarationSnippets.Add(snippet);
            }
        }

        public (string, string) GetDescriptionAndText(string? template, string manifestResourceName)
        {
            string description = string.Empty;
            string text = string.Empty;

            if (!string.IsNullOrWhiteSpace(template))
            {
                Parser parser = new Parser(template);
                ProgramSyntax programSyntax = parser.Program();
                IEnumerable<SyntaxBase> declarations = programSyntax.Declarations;

                if (declarations.Any() && declarations.First() is StatementSyntax statementSyntax)
                {
                    text = template.Substring(statementSyntax.Span.Position);

                    ImmutableArray<SyntaxBase> children = programSyntax.Children;

                    if (children.Length > 0 &&
                        children[0] is Token firstToken &&
                        firstToken is not null &&
                        firstToken.LeadingTrivia[0] is SyntaxTrivia syntaxTrivia &&
                        syntaxTrivia.Type is SyntaxTriviaType.SingleLineComment)
                    {
                        description = syntaxTrivia.Text.Substring("// ".Length);
                    }

                    CacheResourceDeclarationAndDependencies(template, manifestResourceName, description);
                }
            }

            return (description, text);
        }

        public IEnumerable<Snippet> GetTopLevelNamedDeclarationSnippets() => topLevelNamedDeclarationSnippets;

        private void CacheResourceDeclarationAndDependencies(string template, string manifestResourceName, string description)
        {
            ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> dependencies = GetResourceDependencies(template, manifestResourceName);

            foreach (KeyValuePair<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> kvp in dependencies)
            {
                DeclaredSymbol declaredSymbol = kvp.Key;

                if (declaredSymbol.DeclaringSyntax is ResourceDeclarationSyntax resourceDeclarationSyntax)
                {
                    if (declaredSymbol.Type is TypeSymbol typeSymbol && typeSymbol.TypeKind != TypeKind.Error)
                    {
                        CacheResourceDeclaration(resourceDeclarationSyntax, typeSymbol, template, description, manifestResourceName);
                        CacheResourceDependencies(typeSymbol, kvp.Value, template);
                    }
                }
            }
        }

        private void CacheResourceDeclaration(ResourceDeclarationSyntax resourceDeclarationSyntax, TypeSymbol typeSymbol, string template, string description, string manifestResourceName)
        {
            string type = typeSymbol.Name;

            if (!resourceTypeInfoMap.ContainsKey(type))
            {
                TextSpan bodySpan = resourceDeclarationSyntax.Value.Span;
                string bodyText = template.Substring(bodySpan.Position, bodySpan.Length);
                string prefix = Path.GetFileNameWithoutExtension(manifestResourceName);
                TextSpan resourceDeclarationSyntaxNameSpan = resourceDeclarationSyntax.Name.Span;
                string identifier = template.Substring(resourceDeclarationSyntaxNameSpan.Position, resourceDeclarationSyntaxNameSpan.Length);

                resourceTypeInfoMap.TryAdd(type, (prefix, identifier, bodyText, description));
            }
        }

        private void CacheResourceDependencies(TypeSymbol typeSymbol, ImmutableHashSet<ResourceDependency> resourceDependencies, string template)
        {
            if (resourceDependencies.Any())
            {
                StringBuilder sb = new StringBuilder();

                foreach (ResourceDependency resourceDependency in resourceDependencies)
                {
                    TextSpan span = resourceDependency.Resource.DeclaringSyntax.Span;

                    string parentType = resourceDependency.Resource.Type.Name;

                    if (resourceTypeToChildSymbolsMap.ContainsKey(parentType))
                    {
                        resourceTypeToChildSymbolsMap[parentType].Add(typeSymbol);
                    }
                    else
                    {
                        resourceTypeToChildSymbolsMap.TryAdd(parentType, new List<TypeSymbol> { typeSymbol });
                    }

                    CacheNestedChildResourceType(parentType, typeSymbol);


                    sb.AppendLine(template.Substring(span.Position, span.Length));
                }

                resourceTypeToDependentsMap.TryAdd(typeSymbol.Name, sb.ToString());
            }
        }

        private void CacheNestedChildResourceType(string parentType, TypeSymbol childTypeSymbol)
        {
            if (childTypeSymbol is ResourceType resourceType)
            {
                foreach (string type in resourceType.TypeReference.Types)
                {
                    if (!parentType.Contains(type))
                    {
                        resourceTypeToTypeMap.TryAdd(childTypeSymbol.Name, type + "@" + resourceType.TypeReference.ApiVersion);
                    }
                }
            }
        }

        private ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> GetResourceDependencies(string template, string manifestResourceName)
        {
            // Snippets with prefix resource will not have valid type, so there can't be any dependencies
            if (manifestResourceName.Contains("resource"))
            {
                return ImmutableDictionary.Create<DeclaredSymbol, ImmutableHashSet<ResourceDependency>>();
            }

            // We need to provide uri for syntax tree creation, but it's not used anywhere. In order to avoid 
            // cross platform issues, we'll provide a placeholder uri.
            SyntaxTree syntaxTree = SyntaxTree.Create(new Uri("inmemory://snippet.bicep"), template);
            SyntaxTreeGrouping syntaxTreeGrouping = new SyntaxTreeGrouping(
                syntaxTree,
                ImmutableHashSet.Create(syntaxTree),
                ImmutableDictionary.Create<ModuleDeclarationSyntax, SyntaxTree>(),
                ImmutableDictionary.Create<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate>(),
                fileResolver);

            Compilation compilation = new Compilation(AzResourceTypeProvider.CreateWithAzTypes(), syntaxTreeGrouping);
            SemanticModel semanticModel = compilation.GetEntrypointSemanticModel();

            return ResourceDependencyVisitor.GetResourceDependencies(semanticModel);
        }

        public IEnumerable<Snippet> GetResourceBodyCompletionSnippets(TypeSymbol typeSymbol, bool isExistingResource, bool isResourceNested)
        {
            if (resourceBodySnippetsCache.TryGetValue((typeSymbol.Name, isExistingResource), out IEnumerable<Snippet>? cachedSnippets) && cachedSnippets.Any())
            {
                return cachedSnippets;
            }

            List<Snippet> snippets = new List<Snippet>();

            snippets.Add(GetEmptySnippet());

            // We will not show custom snippets for resources with 'existing' keyword as they are not applicable in that scenario.
            if (!isExistingResource)
            {
                if (isResourceNested)
                {
                    if (resourceTypeInfoMap.TryGetValue(typeSymbol.Name, out (string prefix, string identifier, string bodyText, string description) resourceTypeInfo))
                    {
                        Snippet snippet = new Snippet(RemoveParentFromResourceBodyText(resourceTypeInfo.bodyText), prefix: "snippet", detail: resourceTypeInfo.description);
                        snippets.Add(snippet);
                    }
                }
                else
                {
                    Snippet? snippetFromExistingTemplate = GetResourceBodyCompletionSnippetFromTemplate(typeSymbol);
                    if (snippetFromExistingTemplate is not null)
                    {
                        snippets.Add(snippetFromExistingTemplate);
                    }
                }
            }

            if (typeSymbol is ResourceType resourceType)
            {
                IEnumerable<Snippet> snippetsFromAzTypes = GetRequiredPropertiesForObjectType(resourceType.Body.Type);

                if (snippetsFromAzTypes.Any())
                {
                    snippets.AddRange(snippetsFromAzTypes);
                }
            }

            // Add to cache
            // Note: Properties information obtained from TypeSystem may vary for resources with/without 'existing' keyword.
            // TypeName obtained from TypeSymbol might be same in both the cases. In order to differentiate, we'll always
            // cache combination of typeSymbol.Name + isExistingResource.
            resourceBodySnippetsCache.TryAdd((typeSymbol.Name, isExistingResource), snippets);

            return snippets;
        }

        private Snippet? GetResourceBodyCompletionSnippetFromTemplate(TypeSymbol typeSymbol)
        {
            string label = "snippet";
            string type = typeSymbol.Name;

            StringBuilder sb = new StringBuilder();

            // Get resource body completion snippet from checked in static template file, if available
            if (resourceTypeInfoMap.TryGetValue(type, out (string prefix, string identifier, string text, string description) resourceBodyWithDescription))
            {
                sb.AppendLine(resourceBodyWithDescription.text);

                if (resourceTypeToDependentsMap.TryGetValue(type, out string? resourceDependencies))
                {
                    sb.Append(resourceDependencies);
                }

                return new Snippet(sb.ToString(), CompletionPriority.Medium, label, resourceBodyWithDescription.description);
            }

            return null;
        }

        private IEnumerable<Snippet> GetRequiredPropertiesSnippetsForDisciminatedObjectType(DiscriminatedObjectType discriminatedObjectType)
        {
            foreach (KeyValuePair<string, ObjectType> kvp in discriminatedObjectType.UnionMembersByKey.OrderBy(x => x.Key))
            {
                string disciminatedObjectKey = kvp.Key;
                string label = "required-properties-" + disciminatedObjectKey.Trim(new char[] { '\'' });
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

            IOrderedEnumerable<KeyValuePair<string, TypeProperty>> sortedProperties = objectType.Properties.OrderBy(x => propertiesSortPreferenceList.Exists(y => y == x.Key) ?
                                                                                                 propertiesSortPreferenceList.FindIndex(y => y == x.Key) :
                                                                                                 propertiesSortPreferenceList.Count - 1);

            foreach (KeyValuePair<string, TypeProperty> kvp in sortedProperties)
            {
                string? snippetText = GetSnippetText(kvp.Value, indentLevel: 1, ref index, discriminatedObjectKey);

                if (snippetText is not null)
                {
                    sb.Append(snippetText);
                }
            }

            if (sb.Length > 0)
            {
                // Insert open curly at the beginning
                sb.Insert(0, "{\n");

                // Append final tab stop
                sb.Append("\t$0\n}");

                return new Snippet(sb.ToString(), CompletionPriority.Medium, label, RequiredPropertiesDescription);
            }

            return null;
        }

        private string? GetSnippetText(TypeProperty typeProperty, int indentLevel, ref int index, string? discrimatedObjectKey = null)
        {
            if (typeProperty.Flags.HasFlag(TypePropertyFlags.Required))
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

                    if (discrimatedObjectKey is not null &&
                        typeProperty.TypeReference.Type is TypeSymbol typeSymbol &&
                        typeSymbol.Name == discrimatedObjectKey)
                    {
                        value = ": " + discrimatedObjectKey;
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

        public IEnumerable<Snippet> GeNestedChildResourceSnippets(TypeSymbol typeSymbol)
        {
            // leaving out the API version on this, because we expect its more common to inherit from the containing resource.
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

            string parentType = typeSymbol.Name;

            if (resourceTypeToChildSymbolsMap.ContainsKey(parentType))
            {
                foreach (TypeSymbol childTypeSymbol in resourceTypeToChildSymbolsMap[typeSymbol.Name])
                {
                    resourceTypeInfoMap.TryGetValue(childTypeSymbol.Name, out (string prefix, string identifier, string bodyText, string description) resourceInfo);

                    if (childTypeSymbol is ResourceType resourceType)
                    {
                        foreach (string type in resourceType.TypeReference.Types)
                        {
                            if (!parentType.Contains(type))
                            {
                                string childType = type + "@" + resourceType.TypeReference.ApiVersion;
                                string bodyText = RemoveParentFromResourceBodyText(resourceInfo.bodyText);
                                string text = LanguageConstants.ResourceKeyword + " " + resourceInfo.identifier + " '" + childType + "' = " + bodyText;

                                yield return new Snippet(text, prefix: resourceInfo.prefix, detail: resourceInfo.description);
                            }
                        }
                    }
                }
            }
        }

        private string RemoveParentFromResourceBodyText(string text)
        {
            return Regex.Replace(text, @"^.*parent:.*$[\r\n]*", string.Empty, RegexOptions.Multiline);
        }
    }
}
