// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure;
using Azure.Containers.ContainerRegistry;
using Azure.Identity;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Bicep.LanguageServer.Completions
{
    public class ModuleReferenceCompletionProvider : IModuleReferenceCompletionProvider
    {
        private readonly IModulesMetadataProvider modulesMetadataProvider;
        private readonly IServiceClientCredentialsProvider serviceClientCredentialsProvider;

        private static readonly Dictionary<string, string> BicepRegistryAndTemplateSpecShemaCompletionLabelsWithDetails = new Dictionary<string, string>()
        {
            {"br:", "Bicep registry schema name" },
            {"br/", "Bicep registry schema name" },
            {"ts:", "Template spec schema name" },
            {"ts/", "Template spec schema name" },
        };

        private static readonly Regex AcrModuleRegistryPath = new Regex(@"br:(?<registryName>(.*).azurecr.io)/", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        private static readonly Regex McrPublicModuleRegistryAliasWithPath = new Regex(@"br/public:(?<filePath>(.*?)):", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private static readonly Regex McrPublicModuleRegistryWithoutAliasWithPath = new Regex(@"br:mcr.microsoft.com/bicep/(?<filePath>(.*?)):", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        public ModuleReferenceCompletionProvider(IModulesMetadataProvider modulesMetadataProvider, IServiceClientCredentialsProvider serviceClientCredentialsProvider)
        {
            this.modulesMetadataProvider = modulesMetadataProvider;
            this.serviceClientCredentialsProvider = serviceClientCredentialsProvider;
        }

        public async Task<IEnumerable<CompletionItem>> GetFilteredCompletions(Uri templateUri, BicepCompletionContext context)
        {
            var replacementText = string.Empty;

            if (context.ReplacementTarget is Token token)
            {
                replacementText = token.Text;
            }

            return GetPublicMcrModuleRegistryCompletions(context, replacementText)
                .Concat(GetAcrModuleRegistryCompletions(context, replacementText))
                .Concat(GetPublicMcrModuleRegistryVersionCompletions(context, replacementText))
                .Concat(GetModuleRegistryAliasCompletions(context, replacementText))
                .Concat(GetBicepRegistryAndTemplateSpecShemaCompletions(context, replacementText))
                .Concat(await GetOciArtifactModuleRepositoryPathCompletions(templateUri, context, replacementText));
        }

        private IEnumerable<CompletionItem> GetBicepRegistryAndTemplateSpecShemaCompletions(BicepCompletionContext context, string replacementText)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ModulePath))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            if (!string.IsNullOrWhiteSpace(replacementText.Trim('\'')))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            List<CompletionItem> completionItems = new List<CompletionItem>();
            foreach (var kvp in BicepRegistryAndTemplateSpecShemaCompletionLabelsWithDetails)
            {
                var text = kvp.Key;
                var sb = new StringBuilder();
                sb.Append("'");
                sb.Append(text);
                sb.Append("$0");
                sb.Append("'");

                var completionText = sb.ToString();

                BicepTelemetryEvent telemetryEvent = BicepTelemetryEvent.CreateBicepRegistryOrTemplateSpecShemaCompletion(text);
                var command = TelemetryHelper.CreateCommand
                (
                    title: "Bicep registry or template spec shema completion",
                    name: TelemetryConstants.CommandName,
                    args: JArray.FromObject(new List<object> { telemetryEvent })
                );

                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, text)
                    .WithFilterText(completionText)
                    .WithSortText(GetSortText(text, CompletionPriority.Medium))
                    .WithSnippetEdit(context.ReplacementRange, completionText)
                    .WithDetail(kvp.Value)
                    .WithCommand(command)
                    .Build();
                completionItems.Add(completionItem);
            }

            return completionItems;
        }

        private IEnumerable<CompletionItem> GetPublicMcrModuleRegistryVersionCompletions(BicepCompletionContext context, string replacementText)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.OciModuleRegistryReference))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            string? filePath = null;

            if (McrPublicModuleRegistryAliasWithPath.IsMatch(replacementText))
            {
                var matches = McrPublicModuleRegistryAliasWithPath.Matches(replacementText);
                filePath = matches[0].Groups["filePath"].Value;
            }
            if (McrPublicModuleRegistryWithoutAliasWithPath.IsMatch(replacementText))
            {
                var matches = McrPublicModuleRegistryWithoutAliasWithPath.Matches(replacementText);
                filePath = matches[0].Groups["filePath"].Value;
            }

            if (filePath is null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            List<CompletionItem> completions = new List<CompletionItem>();
            replacementText = replacementText.TrimEnd('\'');
            foreach (var version in modulesMetadataProvider.GetVersions(filePath))
            {
                StringBuilder sb = new StringBuilder(replacementText);
                sb.Append(version);
                sb.Append("'$0");

                var insertText = sb.ToString();

                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, version)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithFilterText(insertText)
                    .WithSortText(GetSortText(version, CompletionPriority.High))
                    .Build();

                completions.Add(completionItem);
            }

            return completions;
        }

        private IEnumerable<CompletionItem> GetAcrModuleRegistryCompletions(BicepCompletionContext context, string replacementText)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.OciModuleRegistryReference))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            List<CompletionItem> completions = new List<CompletionItem>();
            if (!AcrModuleRegistryPath.IsMatch(replacementText))
            {
                return completions;
            }

            if (context.EnclosingDeclaration is ModuleDeclarationSyntax declarationSyntax &&
                declarationSyntax.Path is StringSyntax stringSyntax &&
                stringSyntax.TryGetLiteralValue() is string entered)
            {
                if (AcrModuleRegistryPath.IsMatch(entered))
                {
                    var matches = AcrModuleRegistryPath.Matches(entered);
                    var registryName = matches[0].Groups["registryName"].Value;

                    // Create a new ContainerRegistryClient
                    var containerRegistryClientOptions = new ContainerRegistryClientOptions()
                    {
                        Audience = ContainerRegistryAudience.AzureResourceManagerPublicCloud
                    };

                    var uri = "https://" + registryName + "/";

                    ContainerRegistryClient client = new ContainerRegistryClient(new Uri(uri), new DefaultAzureCredential(), containerRegistryClientOptions);

                    Pageable<string> repositories = client.GetRepositoryNames();

                    foreach (string repository in repositories)
                    {
                        completions.Add(CompletionItemBuilder.Create(CompletionItemKind.File, repository)
                        .WithFilterText(repository)
                        .WithSortText(GetSortText(repository, CompletionPriority.Medium)).Build());
                    }
                }
            }

            return completions;
        }

        private IEnumerable<CompletionItem> GetPublicMcrModuleRegistryCompletions(BicepCompletionContext context, string replacementText)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.OciModuleRegistryReference))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            List<CompletionItem> completions = new List<CompletionItem>();

            if (replacementText == "'br/public:'" ||
                replacementText == "'br:mcr.microsoft.com/bicep/'" ||
                replacementText == "'br/public:" ||
                replacementText == "'br:mcr.microsoft.com/bicep/")
            {
                foreach (var moduleName in modulesMetadataProvider.GetModuleNames())
                {
                    StringBuilder sb = new StringBuilder(replacementText.TrimEnd('\''));
                    sb.Append(moduleName);
                    sb.Append(":$0'");

                    var insertText = sb.ToString();

                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, moduleName)
                        .WithSnippetEdit(context.ReplacementRange, insertText)
                        .WithFilterText(insertText)
                        .WithSortText(GetSortText(moduleName, CompletionPriority.High))
                        .Build();

                    completions.Add(completionItem);
                }
            }

            return completions;
        }

        private IEnumerable<CompletionItem> GetModuleRegistryAliasCompletions(BicepCompletionContext context, string replacementText)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.OciModuleRegistryReference))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');
            if (replacementTextWithTrimmedEnd != "'br/")
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var insertText = replacementTextWithTrimmedEnd + "public:$0'";

            var label = "public:";
            var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                .WithFilterText(insertText)
                .WithSnippetEdit(context.ReplacementRange, insertText)
                .WithSortText(GetSortText(label, CompletionPriority.High))
                .Build();

            return new List<CompletionItem> { completionItem };
        }

        private async Task<IEnumerable<CompletionItem>> GetOciArtifactModuleRepositoryPathCompletions(Uri templateUri, BicepCompletionContext context, string replacementText)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.OciModuleRegistryReference))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            List<CompletionItem> completions = new List<CompletionItem>();

            if (replacementText == "'br:'")
            {
                IEnumerable<CompletionItem> acrCompletions = await GetAcrModuleRegistriesCompletions(templateUri);
                completions.AddRange(acrCompletions);

                var mcrCompletion = CompletionItemBuilder.Create(CompletionItemKind.Reference, "mcr.microsoft.com/bicep/")
                    .WithSortText(GetSortText("mcr.microsoft.com/bicep/", CompletionPriority.High))
                    .Build();
                completions.Insert(0, mcrCompletion);
            }

            return completions;
        }

        private async Task<IEnumerable<CompletionItem>> GetAcrModuleRegistriesCompletions(Uri templateUri)
        {
            ClientCredentials clientCredentials = await serviceClientCredentialsProvider.GetServiceClientCredentials(templateUri); ;

            ResourceGraphClient resourceGraphClient = new ResourceGraphClient(clientCredentials);
            QueryRequest queryRequest = new QueryRequest(@"Resources
| where type == ""microsoft.containerregistry/registries""
| project properties[""loginServer""]
");
            QueryResponse queryResponse = resourceGraphClient.Resources(queryRequest);
            JArray jArray = JArray.FromObject(queryResponse.Data);
            List<CompletionItem> repositories = new List<CompletionItem>();

            foreach (JObject item in jArray)
            {
                if (item is not null &&
                    item.GetValue("properties_loginServer") is JToken jToken &&
                    jToken is not null &&
                    jToken.Value<string>() is string loginServer)
                {
                    repositories.Add(CompletionItemBuilder.Create(CompletionItemKind.Reference, loginServer)
                        .WithSortText(GetSortText("mcr.microsoft.com/bicep/", CompletionPriority.Medium))
                        .WithFilterText(loginServer).Build());
                }
            }

            return repositories;
        }

        // the priority must be a number in the sort text
        private static string GetSortText(string label, CompletionPriority priority) => $"{(int)priority}_{label}";
    }
}
