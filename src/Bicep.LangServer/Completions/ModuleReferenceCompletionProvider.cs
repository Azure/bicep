// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure;
using Azure.Containers.ContainerRegistry;
using Azure.Identity;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public class ModuleReferenceCompletionProvider : IModuleReferenceCompletionProvider
    {
        private readonly IMcrCompletionProvider mcrCompletionProvider;
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

        public ModuleReferenceCompletionProvider(IServiceClientCredentialsProvider serviceClientCredentialsProvider, IMcrCompletionProvider mcrCompletionProvider)
        {
            this.mcrCompletionProvider = mcrCompletionProvider;
            this.serviceClientCredentialsProvider = serviceClientCredentialsProvider;
        }

        public async Task<IEnumerable<CompletionItem>> GetFilteredCompletions(Uri templateUri, BicepCompletionContext context)
        {
            return GetPublicMcrModuleRegistryCompletions(context)
                .Concat(GetAcrModuleRegistryCompletions(context))
                .Concat(GetPublicMcrModuleRegistryTagCompletions(context))
                .Concat(GetModuleRegistryAliasCompletions(context))
                .Concat(GetModulePathCompletions(context))
                .Concat(await GetOciArtifactModuleRepositoryPathCompletions(templateUri, context));
        }

        private IEnumerable<CompletionItem> GetModulePathCompletions(BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ModulePath))
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

            // To provide intellisense before the quotes are typed
            //if (context.EnclosingDeclaration is not ModuleDeclarationSyntax declarationSyntax
            //    || declarationSyntax.Path is not StringSyntax stringSyntax
            //    || stringSyntax.TryGetLiteralValue() is not string entered)
            //{
            //    entered = "";
            //}

            //if (entered == string.Empty)
            //{
           // return GetBicepRegistryAndTemplateSpecSchemaCompletions(context);
            //}

            //return Enumerable.Empty<CompletionItem>();
        }

        //private List<CompletionItem> GetBicepRegistryAndTemplateSpecSchemaCompletions(BicepCompletionContext context)
        //{
        //    List<CompletionItem> completionItems = new List<CompletionItem>();
        //    foreach (var kvp in BicepRegistryAndTemplateSpecShemaCompletionLabelsWithDetails)
        //    {
        //        var text = kvp.Key;
        //        var sb = new StringBuilder();
        //        if (!text.StartsWith("'"))
        //        {
        //            sb.Append("'");
        //        }

        //        sb.Append(text);
        //        sb.Append("$0");

        //        if (!text.EndsWith("'"))
        //        {
        //            sb.Append("'");
        //        }
        //        var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, text)
        //            .WithSortText(GetSortText(text, CompletionPriority.Low))
        //            .WithSnippetEdit(context.ReplacementRange, sb.ToString())
        //            .WithDetail(kvp.Value)
        //            .Build();
        //        completionItems.Add(completionItem);
        //    }

        //    return completionItems;
        //}

        private IEnumerable<CompletionItem> GetPublicMcrModuleRegistryTagCompletions(BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.McrPublicModuleRegistryTag))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            if (context.EnclosingDeclaration is ModuleDeclarationSyntax declarationSyntax &&
                declarationSyntax.Path is StringSyntax stringSyntax &&
                stringSyntax.TryGetLiteralValue() is string entered)
            {
                if (McrPublicModuleRegistryAliasWithPath.IsMatch(entered))
                {
                    var matches = McrPublicModuleRegistryAliasWithPath.Matches(entered);
                    var filePath = matches[0].Groups["filePath"].Value;

                    return mcrCompletionProvider.GetTags(filePath);
                }
                if (McrPublicModuleRegistryWithoutAliasWithPath.IsMatch(entered))
                {
                    var matches = McrPublicModuleRegistryWithoutAliasWithPath.Matches(entered);
                    var filePath = matches[0].Groups["filePath"].Value;

                    return mcrCompletionProvider.GetTags(filePath);
                }
            }

            return Enumerable.Empty<CompletionItem>();
        }

        private IEnumerable<CompletionItem> GetAcrModuleRegistryCompletions(BicepCompletionContext context)
        {
            List<CompletionItem> completions = new List<CompletionItem>();
            if (!context.Kind.HasFlag(BicepCompletionContextKind.AcrModuleRegistryStart))
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

        private IEnumerable<CompletionItem> GetPublicMcrModuleRegistryCompletions(BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.McrPublicModuleRegistryStart))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            return mcrCompletionProvider.GetModuleNames();
        }

        private IEnumerable<CompletionItem> GetModuleRegistryAliasCompletions(BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ModuleRegistryAliasCompletionStart))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, "public:")
                    .WithSortText(GetSortText("public:", CompletionPriority.High))
                    .Build();
            return new List<CompletionItem> { completionItem };
        }

        private async Task<IEnumerable<CompletionItem>> GetOciArtifactModuleRepositoryPathCompletions(Uri templateUri, BicepCompletionContext context)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            if (context.Kind.HasFlag(BicepCompletionContextKind.ModuleReferenceRegistryName))
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
