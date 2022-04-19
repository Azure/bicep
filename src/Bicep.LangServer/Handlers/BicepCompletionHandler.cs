// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Configuration;
using Bicep.LanguageServer.Snippets;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepCompletionHandler : CompletionHandlerBase
    {
        private readonly ILogger<BicepCompletionHandler> logger;
        private readonly ICompilationManager compilationManager;
        private readonly ICompletionProvider completionProvider;
        private readonly IFeatureProvider featureProvider;

        public BicepCompletionHandler(ILogger<BicepCompletionHandler> logger, ICompilationManager compilationManager, ICompletionProvider completionProvider, IFeatureProvider featureProvider)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.completionProvider = completionProvider;
            this.featureProvider = featureProvider;
        }

        public override Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var completions = Enumerable.Empty<CompletionItem>();
            if (ConfigurationHelper.IsBicepConfigFile(request.TextDocument.Uri))
            {
                //asdfg launch LS on bicepconfig.json open
                //if (BicepConfigChangeHandler.activeBicepConfigCache.TryGetValue(request.TextDocument.Uri, out RootConfiguration? rootConfiguration))
                {
                    //var fakeJson = rootConfiguration.ToUtf8Json(); //asdfg

                    if (request.Position.Line == 0)//asdfg? && (fakeJson == "" || fakeJson == "{}"/*asdfg*/))
                    {
                        var replacementRange = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(0, 0, 0, 10/*asdfg*/); //asdfg
                        var text = "{\n" +
                                "\t// See https://aka.ms/bicep/config for more information on Bicep configuration options\n" +
                                "\t// Press CTRL+SPACE at any location to see Intellisense suggestions\n" +
                                "\t\"analyzers\": {\n" +
                                "\t\t\"core\": {\n" +
                                "\t\t\t\"rules\": {\n" +
                                "\t\t\t\t\"no-unused-params\": {\n" +
                                "\t\t\t\t\t\"level\": ${1|\"warning\",\"off\",\"info\",\"error\"|}\n" +
                                "\t\t\t\t}$0\n" +
                                "\t\t\t}\n" +
                                "\t\t}\n" +
                                "\t}\n" +
                                "}";
                        completions = new CompletionItem[] {
                            CreateContextualSnippetCompletion(
                                "Default Bicep Analyzers Configuration",
                                "Scaffolds default configuration for a bicepconfig.json file (analysis section only)",
                                text,
                                "{Default Bicep Analyzers Configuration",
                                replacementRange,
                                preselect: false /*asdfg*/)
                        };
                    }
                }
            }
            else
            {

                var compilationContext = this.compilationManager.GetCompilation(request.TextDocument.Uri);
                if (compilationContext == null)
                {
                    return Task.FromResult(new CompletionList());
                }

                int offset = PositionHelper.GetOffset(compilationContext.LineStarts, request.Position);
                var completionContext = BicepCompletionContext.Create(featureProvider, compilationContext.Compilation, offset);
                try
                {
                    completions = this.completionProvider.GetFilteredCompletions(compilationContext.Compilation, completionContext);
                }
                catch (Exception e)
                {
                    this.logger.LogError("Error with Completion in file {Uri} with {Context}. Underlying exception is: {Exception}", request.TextDocument.Uri, completionContext, e.ToString());
                }
            }

            return Task.FromResult(new CompletionList(completions, isIncomplete: false));
        }


        private static CompletionItem CreateContextualSnippetCompletion(string label, string detail, string snippet, string filterText, OmniSharp.Extensions.LanguageServer.Protocol.Models.Range/*asdfg*/ replacementRange, CompletionPriority priority = CompletionPriority.Medium, bool preselect = false) =>
            CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                .WithSnippetEdit(replacementRange, snippet)
                .WithDetail(detail)
                .WithDocumentation($"```bicep\n{new Snippet(snippet).FormatDocumentation()}\n```")
                .WithSortText(GetSortText(label, priority))
                .WithFilterText(filterText)
                .Preselect(preselect)
                .Build();

        private static string GetSortText(string label, CompletionPriority priority) => $"{(int)priority}_{label}";

        public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request);
        }

        protected override CompletionRegistrationOptions CreateRegistrationOptions(CompletionCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.CreateForTextDocumentSync(),
            AllCommitCharacters = new Container<string>(),
            ResolveProvider = false,
            TriggerCharacters = new Container<string>(":", " ", ".", "/", "'", "@", "{", "#")
        };
    }
}
