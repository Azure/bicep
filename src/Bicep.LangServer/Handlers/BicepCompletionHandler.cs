﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepCompletionHandler : CompletionHandler
    {
        private readonly ILogger<BicepCompletionHandler> logger;
        private readonly ICompilationManager compilationManager;
        private readonly ICompletionProvider completionProvider;

        public BicepCompletionHandler(ILogger<BicepCompletionHandler> logger, ICompilationManager compilationManager, ICompletionProvider completionProvider)
            : base(CreateRegistrationOptions())
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.completionProvider = completionProvider;
        }

        public override Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var compilationContext = this.compilationManager.GetCompilation(request.TextDocument.Uri);
            if (compilationContext == null)
            {
                return Task.FromResult(new CompletionList());
            }

            int offset = PositionHelper.GetOffset(compilationContext.LineStarts, request.Position);
            var completionContext = BicepCompletionContext.Create(compilationContext.Compilation, offset);
            var completions = Enumerable.Empty<CompletionItem>();
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

        private static CompletionRegistrationOptions CreateRegistrationOptions() => new CompletionRegistrationOptions
        {
            DocumentSelector = DocumentSelectorFactory.Create(),
            AllCommitCharacters = new Container<string>(),
            ResolveProvider = false,
            TriggerCharacters = new Container<string>(":", " ", ".", "/", "'", "@")
        };
    }
}
