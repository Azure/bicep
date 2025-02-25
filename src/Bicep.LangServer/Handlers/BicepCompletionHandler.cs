// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
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
        private readonly DocumentSelectorFactory documentSelectorFactory;

        public BicepCompletionHandler(ILogger<BicepCompletionHandler> logger, ICompilationManager compilationManager, ICompletionProvider completionProvider, DocumentSelectorFactory documentSelectorFactory)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.completionProvider = completionProvider;
            this.documentSelectorFactory = documentSelectorFactory;
        }

        public override async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var completions = Enumerable.Empty<CompletionItem>();

            var compilationContext = this.compilationManager.GetCompilation(request.TextDocument.Uri);
            if (compilationContext is null)
            {
                // no compilation context or this is a param file and params are disabled
                return new CompletionList();
            }

            int offset = PositionHelper.GetOffset(compilationContext.LineStarts, request.Position);

            var completionContext = BicepCompletionContext.Create(compilationContext.Compilation, offset);

            try
            {
                completions = await this.completionProvider.GetFilteredCompletions(compilationContext.Compilation, completionContext, cancellationToken);
            }
            catch (Exception e)
            {
                this.logger.LogError("Error with Completion in file {Uri} with {Context}. Underlying exception is: {Exception}", request.TextDocument.Uri, completionContext, e.ToString());
            }

            return new CompletionList(completions, isIncomplete: false);
        }

        public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken)
        {
            return this.completionProvider.Resolve(request, cancellationToken);
        }

        protected override CompletionRegistrationOptions CreateRegistrationOptions(CompletionCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = documentSelectorFactory.CreateForBicepAndParams(),
            AllCommitCharacters = new Container<string>(),
            ResolveProvider = true,
            TriggerCharacters = new Container<string>(":", " ", ".", "/", "'", "@", "{", "#", "?")
        };
    }
}
