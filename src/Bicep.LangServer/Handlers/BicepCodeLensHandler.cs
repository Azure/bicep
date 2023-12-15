// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Registry;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    // Provides code lenses for a range in a Bicep document
    public class BicepCodeLensHandler : CodeLensHandlerBase
    {
        private readonly IClientCapabilitiesProvider clientCapabilitiesProvider;
        private readonly ICompilationManager compilationManager;
        private readonly IModuleDispatcher moduleDispatcher;

        public BicepCodeLensHandler(ICompilationManager compilationManager, IClientCapabilitiesProvider clientCapabilitiesProvider, IModuleDispatcher moduleDispatcher)
        {
            this.clientCapabilitiesProvider = clientCapabilitiesProvider;
            this.compilationManager = compilationManager;
            this.moduleDispatcher = moduleDispatcher;
        }

        public override Task<CodeLensContainer?> Handle(CodeLensParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lenses = ExternalSourceCodeLensProvider.GetCodeLenses(moduleDispatcher, request, cancellationToken);

            return Task.FromResult<CodeLensContainer?>(new CodeLensContainer(lenses));
        }

        public override Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override CodeLensRegistrationOptions CreateRegistrationOptions(CodeLensCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector =new( 
                DocumentSelectorFactory.AllSupportedLangIds
                .Concat(TextDocumentFilter.ForScheme(LangServerConstants.ExternalSourceFileScheme))),
            ResolveProvider = false
        };
    }
}
