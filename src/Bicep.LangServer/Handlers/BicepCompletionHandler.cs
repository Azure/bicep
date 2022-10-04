// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Features;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
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
        private readonly IFeatureProviderFactory featureProviderFactory;

        public BicepCompletionHandler(ILogger<BicepCompletionHandler> logger, ICompilationManager compilationManager, ICompletionProvider completionProvider, IFeatureProviderFactory featureProviderFactory)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.completionProvider = completionProvider;
            this.featureProviderFactory = featureProviderFactory;
        }

        public override Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var completions = Enumerable.Empty<CompletionItem>();

            var compilationContext = this.compilationManager.GetCompilation(request.TextDocument.Uri);
            if (compilationContext is null)
            {
                return Task.FromResult(new CompletionList());
            }

            int offset = PositionHelper.GetOffset(compilationContext.LineStarts, request.Position);
            var completionContext = BicepCompletionContext.Create(featureProviderFactory.GetFeatureProvider(request.TextDocument.Uri.ToUri()), compilationContext.Compilation, offset);

            try
            {
                completions = this.completionProvider.GetFilteredCompletions(compilationContext.Compilation, completionContext);
            }
            catch (Exception e)
            {
                this.logger.LogError("Error with Completion in file {Uri} with {Context}. Underlying exception is: {Exception}", request.TextDocument.Uri, completionContext, e.ToString());
            }

            return Task.FromResult(new CompletionList(completions, isIncomplete: false));
        }

        public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request);
        }

        protected override CompletionRegistrationOptions CreateRegistrationOptions(CompletionCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.Create(),
            AllCommitCharacters = new Container<string>(),
            ResolveProvider = false,
            TriggerCharacters = new Container<string>(":", " ", ".", "/", "'", "@", "{", "#")
        };
    }
}
