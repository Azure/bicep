// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Resolution;
using Bicep.LanguageServer.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using LspClientCapabilities = OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.ClientCapabilities;
using LspCodeLens = OmniSharp.Extensions.LanguageServer.Protocol.Models.CodeLens;

namespace Bicep.LanguageServer.Features.Language.CodeLens
{
    // Provides code lenses for a range in a Bicep document
    public class BicepCodeLensHandler : CodeLensHandlerBase
    {
        private readonly IExternalArtifactContextFactory externalArtifactContextFactory;
        private readonly DocumentSelectorFactory documentSelectorFactory;

        public BicepCodeLensHandler(IExternalArtifactContextFactory externalArtifactContextFactory, DocumentSelectorFactory documentSelectorFactory)
        {
            this.externalArtifactContextFactory = externalArtifactContextFactory;
            this.documentSelectorFactory = documentSelectorFactory;
        }

        public override Task<CodeLensContainer?> Handle(CodeLensParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lenses = ExternalSourceCodeLensProvider.GetCodeLenses(externalArtifactContextFactory, request, cancellationToken);

            return Task.FromResult<CodeLensContainer?>(new CodeLensContainer(lenses));
        }

        public override Task<LspCodeLens> Handle(LspCodeLens request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override CodeLensRegistrationOptions CreateRegistrationOptions(CodeLensCapability capability, LspClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = new(
                documentSelectorFactory.CreateForAllSupportedLangIds()
                .Concat(TextDocumentFilter.ForScheme(LangServerConstants.ExternalSourceFileScheme))),
            ResolveProvider = false
        };
    }
}
