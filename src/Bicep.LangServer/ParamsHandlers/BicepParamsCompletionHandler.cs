// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.ParamsHandlers
{
    public class BicepParamsCompletionHandler : CompletionHandlerBase
    {
        private readonly ILogger<BicepParamsCompletionHandler> logger;
        private readonly ICompletionProvider completionProvider;
        private readonly IFeatureProviderManager featureProviderManager;
        private readonly IParamsCompilationManager paramsCompilationManager;

        public BicepParamsCompletionHandler(ILogger<BicepParamsCompletionHandler> logger, ICompletionProvider completionProvider, IFeatureProviderManager featureProviderManager, IParamsCompilationManager paramsCompilationManager)
        {
            this.logger = logger;
            this.completionProvider = completionProvider;
            this.featureProviderManager = featureProviderManager;
            this.paramsCompilationManager = paramsCompilationManager;
        }

        public override Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var completions = Enumerable.Empty<CompletionItem>();
            var documentUri = request.TextDocument.Uri;

            if (featureProviderManager.GetFeatureProvider(documentUri.ToUri()).ParamsFilesEnabled)
            {
                var paramsCompilationContext = this.paramsCompilationManager.GetCompilation(documentUri);
                if (paramsCompilationContext is null)
                {
                    return Task.FromResult(new CompletionList());
                }

                int offset = PositionHelper.GetOffset(paramsCompilationContext.LineStarts, request.Position);
                var paramsCompletionContext = ParamsCompletionContext.Create(paramsCompilationContext, offset);
                try
                {
                    completions = this.completionProvider.GetFilteredParamsCompletions(paramsCompilationContext.ParamsSemanticModel, paramsCompletionContext);
                }
                catch (Exception e)
                {
                    this.logger.LogError("Error with Completion in file {Uri}. Underlying exception is: {Exception}", documentUri, e.ToString());
                }
            }

            return Task.FromResult(new CompletionList(completions, isIncomplete: false));
        }

        public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request);
        }

        protected override CompletionRegistrationOptions CreateRegistrationOptions(CompletionCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.CreateForParamsOnly(),
            AllCommitCharacters = new Container<string>(),
            ResolveProvider = false,
            TriggerCharacters = new Container<string>(":", " ", ".", "/", "'", "@", "{", "#")
        };
    }
}
