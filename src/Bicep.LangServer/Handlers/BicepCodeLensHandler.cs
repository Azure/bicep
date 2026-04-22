// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;
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
        private readonly IFileExplorer fileExplorer;
        private readonly ISourceFileFactory sourceFileFactory;
        private readonly DocumentSelectorFactory documentSelectorFactory;

        public BicepCodeLensHandler(IFileExplorer fileExplorer, ISourceFileFactory sourceFileFactory, DocumentSelectorFactory documentSelectorFactory)
        {
            this.fileExplorer = fileExplorer;
            this.sourceFileFactory = sourceFileFactory;
            this.documentSelectorFactory = documentSelectorFactory;
        }

        public override Task<CodeLensContainer?> Handle(CodeLensParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lenses = ExternalSourceCodeLensProvider.GetCodeLenses(fileExplorer, sourceFileFactory, request, cancellationToken);

            return Task.FromResult<CodeLensContainer?>(new CodeLensContainer(lenses));
        }

        public override Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override CodeLensRegistrationOptions CreateRegistrationOptions(CodeLensCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = new(
                documentSelectorFactory.CreateForAllSupportedLangIds()
                .Concat(TextDocumentFilter.ForScheme(LangServerConstants.ExternalSourceFileScheme))),
            ResolveProvider = false
        };
    }
}
