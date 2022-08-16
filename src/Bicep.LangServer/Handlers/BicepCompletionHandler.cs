// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
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
        private readonly IFeatureProvider featureProvider;
        private readonly IParamsCompilationManager paramsCompilationManager;

        public BicepCompletionHandler(ILogger<BicepCompletionHandler> logger, ICompilationManager compilationManager, ICompletionProvider completionProvider, IFeatureProvider featureProvider, IParamsCompilationManager paramsCompilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.completionProvider = completionProvider;
            this.featureProvider = featureProvider;
            this.paramsCompilationManager = paramsCompilationManager;
        }

        public override Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var completions = Enumerable.Empty<CompletionItem>();

            if (featureProvider.ParamsFilesEnabled && PathHelper.HasBicepparamsExension(request.TextDocument.Uri.ToUri()))
            {
                var paramsCompilationContext = this.paramsCompilationManager.GetCompilation(request.TextDocument.Uri); 
                if (paramsCompilationContext == null)
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
                    this.logger.LogError("Error with Completion in file {Uri}. Underlying exception is: {Exception}", request.TextDocument.Uri, e.ToString());
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
