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
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Completions;

namespace Bicep.LanguageServer.Snippets
{
    public class SnippetsProvider : ISnippetsProvider
    {
        private const string RequiredPropertiesDescription = "Required properties";
        private const string RequiredPropertiesLabel = "required-properties";
        private static readonly Regex ParentPropertyPattern = new Regex(@"^.*parent:.*$[\r\n]*", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SnippetPlaceholderCommentPattern = new Regex(@"\/\*(?<snippetPlaceholder>(.*?))\*\/('(.*?)'|\w+|-\d+|.*?)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        // Used to cache resource declaration information. Maps resource type reference to prefix, identifier, body text and description
        private readonly ConcurrentDictionary<ResourceTypeReference, (string prefix, string identifier, string bodyText, string description)> resourceTypeReferenceInfoMap = new(ResourceTypeReferenceComparer.Instance);
        // Used to cache resource dependencies. Maps resource type reference to it's dependencies
        private readonly ConcurrentDictionary<ResourceTypeReference, string> resourceTypeReferenceToDependentsMap = new(ResourceTypeReferenceComparer.Instance);
        // Used to cache information about child type symbols in nested resource scenario. Maps resource type reference to nested type symbols
        private readonly ConcurrentDictionary<ResourceTypeReference, ImmutableArray<ResourceTypeReference>> resourceTypeReferenceToChildTypeSymbolsMap = new(ResourceTypeReferenceComparer.Instance);
        // Used to cache resource body snippets
        private readonly ConcurrentDictionary<(ResourceTypeReference resourceTypeReference, bool isExistingResource), IEnumerable<Snippet>> resourceBodySnippetsCache = new();
        // Used to cache top level declarations
        private readonly HashSet<Snippet> topLevelNamedDeclarationSnippets = new();
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

        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IApiVersionProviderFactory apiVersionProviderFactory;
        private readonly INamespaceProvider namespaceProvider;
        private readonly IFileResolver fileResolver;
        private readonly IBicepAnalyzer bicepAnalyzer;
        private readonly IConfigurationManager configurationManager;
        private readonly IModuleDispatcher moduleDispatcher;

        public SnippetsProvider(IFeatureProviderFactory featureProviderFactory, INamespaceProvider namespaceProvider, IFileResolver fileResolver, IConfigurationManager configurationManager, IApiVersionProviderFactory apiVersionProviderFactory, IModuleDispatcher moduleDispatcher, IBicepAnalyzer bicepAnalyzer)
        {
            this.featureProviderFactory = featureProviderFactory;
            this.apiVersionProviderFactory = apiVersionProviderFactory;
            this.moduleDispatcher = moduleDispatcher;
            this.namespaceProvider = namespaceProvider;
            this.fileResolver = fileResolver;
            this.configurationManager = configurationManager;
            this.bicepAnalyzer = bicepAnalyzer;

            Initialize();
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

                var (description, snippetText) = GetDescriptionAndSnippetText(streamReader.ReadToEnd(), manifestResourceName);
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

        public (string description, string snippet) GetDescriptionAndSnippetText(string template, string manifestResourceName)
        {
            var description = string.Empty;
            var parser = new Parser(template);
            var programSyntax = parser.Program();
            var declarations = programSyntax.Declarations;

            if (declarations.Any() && declarations.First() is StatementSyntax statementSyntax)
            {
                template = template.Substring(statementSyntax.Span.Position);

                var children = programSyntax.Children;

                if (children.Length > 0 &&
                    children[0] is Token firstToken &&
                    firstToken.LeadingTrivia[0] is SyntaxTrivia syntaxTrivia &&
                    syntaxTrivia.Type is SyntaxTriviaType.SingleLineComment)
                {
                    description = syntaxTrivia.Text.Substring("// ".Length);
                }

                CacheResourceDeclarationAndDependencies(template, manifestResourceName, description);

                return (description, RemoveSnippetPlaceholderComments(template));
            }

            return (string.Empty, string.Empty);
        }

        public IEnumerable<Snippet> GetTopLevelNamedDeclarationSnippets() => topLevelNamedDeclarationSnippets;

        private void CacheResourceDeclarationAndDependencies(string template, string manifestResourceName, string description)
        {
            var dependencies = GetResourceDependencies(template, manifestResourceName);

            foreach (var (declaredSymbol, syntax) in dependencies)
            {
                if (declaredSymbol.DeclaringSyntax is ResourceDeclarationSyntax resourceDeclarationSyntax)
                {
                    if (declaredSymbol.Type is ResourceType resourceType && resourceType.TypeKind != TypeKind.Error)
                    {
                        var resourceTypeReference = resourceType.TypeReference;
                        CacheResourceDeclaration(resourceDeclarationSyntax, resourceTypeReference, template, description, manifestResourceName);
                        CacheResourceDependencies(resourceTypeReference, syntax, template);
                    }
                }
            }
        }

        private void CacheResourceDeclaration(ResourceDeclarationSyntax resourceDeclaration, ResourceTypeReference resourceTypeReference, string template, string description, string manifestResourceName)
        {
            if (!resourceTypeReferenceInfoMap.ContainsKey(resourceTypeReference))
            {
                var bodySpan = resourceDeclaration.Value.Span;
                var bodyText = template.Substring(bodySpan.Position, bodySpan.Length);
                bodyText = RemoveSnippetPlaceholderComments(bodyText);

                // snippet placeholders are authored using a multi-line comment syntax. To include this when fetching the identifier,
                // we have to fetch everything from the end of the preceeding syntax (as multi-line comments are stored in trailing trivia on the previous token).
                var nameStart = resourceDeclaration.Keyword.Span.Position + resourceDeclaration.Keyword.Span.Length;
                var nameEnd = resourceDeclaration.Name.Span.Position + resourceDeclaration.Name.Span.Length;
                var identifier = template.Substring(nameStart, nameEnd - nameStart).Trim();
                identifier = RemoveSnippetPlaceholderComments(identifier);

                var prefix = Path.GetFileNameWithoutExtension(manifestResourceName);

                resourceTypeReferenceInfoMap.TryAdd(resourceTypeReference, (prefix, identifier, bodyText, description));
            }
        }

        private void CacheResourceDependencies(ResourceTypeReference childResourceTypeReference, ImmutableHashSet<ResourceDependency> resourceDependencies, string template)
        {
            if (resourceDependencies.Any())
            {
                var sb = new StringBuilder();

                foreach (var resourceDependency in resourceDependencies)
                {
                    if (resourceDependency.Resource is ResourceSymbol resourceSymbol &&
                        resourceSymbol.TryGetResourceTypeReference() is ResourceTypeReference resourceTypeReference)
                    {
                        resourceTypeReferenceToChildTypeSymbolsMap.AddOrUpdate(
                            resourceTypeReference,
                            _ => ImmutableArray.Create(childResourceTypeReference),
                            (_, children) => children.Add(childResourceTypeReference));
                    }

                    var span = resourceDependency.Resource.DeclaringSyntax.Span;
                    var dependentTemplate = template.Substring(span.Position, span.Length);

                    sb.AppendLine(RemoveSnippetPlaceholderComments(dependentTemplate));
                }

                resourceTypeReferenceToDependentsMap.TryAdd(childResourceTypeReference, sb.ToString());
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
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri($"inmemory://{manifestResourceName}.bicep"), template);
            var workspace = new Workspace();
            workspace.UpsertSourceFiles(bicepFile.AsEnumerable());

            var sourceFileGrouping = SourceFileGroupingBuilder.Build(fileResolver, moduleDispatcher, workspace, bicepFile.FileUri, false);

            Compilation compilation = new Compilation(featureProviderFactory, namespaceProvider, sourceFileGrouping, configurationManager, apiVersionProviderFactory, bicepAnalyzer);

            SemanticModel semanticModel = compilation.GetEntrypointSemanticModel();

            return ResourceDependencyVisitor.GetResourceDependencies(semanticModel);
        }

        public IEnumerable<Snippet> GetResourceBodyCompletionSnippets(ResourceType resourceType, bool isExistingResource, bool isResourceNested)
        {
            ResourceTypeReference resourceTypeReference = resourceType.TypeReference;
            if (resourceBodySnippetsCache.TryGetValue((resourceTypeReference, isExistingResource), out IEnumerable<Snippet>? cachedSnippets) && cachedSnippets.Any())
            {
                return cachedSnippets;
            }

            List<Snippet> snippets = new List<Snippet>();

            snippets.Add(GetEmptySnippet());

            // We will not show custom snippets for resources with 'existing' keyword as they are not applicable in that scenario.
            if (!isExistingResource)
            {
                // If the resource is nested, we will only return it's body text from cache. Otherwise, we will return information
                // from the template, which could include parent resource
                if (isResourceNested)
                {
                    if (resourceTypeReferenceInfoMap.TryGetValue(resourceTypeReference, out (string prefix, string identifier, string bodyText, string description) resourceTypeInfo))
                    {
                        // The property "parent" is not allowed in nested resource. We'll remove the property before creating the snippet
                        string text = ParentPropertyPattern.Replace(resourceTypeInfo.bodyText, string.Empty);
                        Snippet snippet = new Snippet(text, prefix: "snippet", detail: resourceTypeInfo.description);
                        snippets.Add(snippet);
                    }
                }
                else
                {
                    Snippet? snippetFromExistingTemplate = GetResourceBodyCompletionSnippetFromTemplate(resourceTypeReference);
                    if (snippetFromExistingTemplate is not null)
                    {
                        snippets.Add(snippetFromExistingTemplate);
                    }
                }
            }

            IEnumerable<Snippet> snippetsFromAzTypes = GetRequiredPropertiesForObjectType(resourceType.Body.Type);

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
            string label = "snippet";
            StringBuilder sb = new StringBuilder();

            // Get resource body completion snippet from checked in static template file, if available
            if (resourceTypeReferenceInfoMap.TryGetValue(resourceTypeReference, out (string prefix, string identifier, string text, string description) resourceBodyWithDescription))
            {
                sb.AppendLine(resourceBodyWithDescription.text);

                if (resourceTypeReferenceToDependentsMap.TryGetValue(resourceTypeReference, out string? resourceDependencies))
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

            if (resourceTypeReferenceToChildTypeSymbolsMap.TryGetValue(resourceTypeReference, out var nestedResourceTypeReferences))
            {
                foreach (var nestedResourceTypeReference in nestedResourceTypeReferences)
                {
                    var nestedTypeReference = new ResourceTypeReference(ImmutableArray.Create<string>(nestedResourceTypeReference.TypeSegments.Last()), nestedResourceTypeReference.ApiVersion);

                    resourceTypeReferenceInfoMap.TryGetValue(nestedResourceTypeReference, out (string prefix, string identifier, string bodyText, string description) resourceInfo);
                    // The property "parent" is not allowed in nested resource. We'll remove the property before creating the snippet
                    var bodyText = ParentPropertyPattern.Replace(resourceInfo.bodyText, string.Empty);
                    var text = LanguageConstants.ResourceKeyword + " " + resourceInfo.identifier + " '" + nestedTypeReference.FormatName() + "' = " + bodyText;

                    yield return new Snippet(text, prefix: resourceInfo.prefix, detail: resourceInfo.description);
                }
            }
        }

        public string RemoveSnippetPlaceholderComments(string text)
        {
            var matches = SnippetPlaceholderCommentPattern.Matches(text);
            // We will be performing multiple string replacements, better to do it in-place
            var buffer = new StringBuilder(text);

            // To avoid recomputing spans, we will perform the replacements in reverse order
            foreach (var match in matches.OrderByDescending(x => x.Index))
            {
                buffer.Replace(oldValue: match.Value,
                                newValue: match.Groups["snippetPlaceholder"].Value,
                                startIndex: match.Index,
                                count: match.Length);
            }

            return buffer.ToString();
        }
    }
}
