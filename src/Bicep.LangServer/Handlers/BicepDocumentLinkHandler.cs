// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDocumentLinkHandler : DocumentLinkHandlerBase
    {
        private readonly IModuleDispatcher moduleDispatcher;

        public BicepDocumentLinkHandler(IModuleDispatcher moduleDispatcher)
        {
            this.moduleDispatcher = moduleDispatcher;
        }

        public override Task<DocumentLinkContainer?> Handle(DocumentLinkParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var links = GetDocumentLinks(moduleDispatcher, request, cancellationToken);
            return Task.FromResult<DocumentLinkContainer?>(new DocumentLinkContainer(links));
        }

        public override Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        protected override DocumentLinkRegistrationOptions CreateRegistrationOptions(DocumentLinkCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = TextDocumentSelector.ForScheme(LangServerConstants.ExternalSourceFileScheme)
        };

        public static IEnumerable<DocumentLink> GetDocumentLinks(IModuleDispatcher moduleDispatcher, DocumentLinkParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (request.TextDocument.Uri.Scheme == LangServerConstants.ExternalSourceFileScheme)
            {
                ExternalSourceReference? externalReference = null;
                try
                {
                    externalReference = new ExternalSourceReference(request.TextDocument.Uri);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"(Experimental) There was an error retrieving source code for this module: {ex.Message}");
                    yield break;
                }

                if (externalReference.RequestedFile is not null)
                {
                    if (!externalReference.ToArtifactReference().IsSuccess(out var artifactReference, out var message))
                    {
                        Trace.WriteLine(message);
                        yield break;
                    }

                    if (!moduleDispatcher.TryGetModuleSources(artifactReference).IsSuccess(out var sourceArchive, out var ex))
                    {
                        Trace.WriteLine(ex.Message);
                        yield break;
                    }

                    sourceArchive.FindExpectedSourceFile(externalReference.RequestedFile);
                    if (sourceArchive.DocumentLinks.TryGetValue(externalReference.RequestedFile, out var links))
                    {
                        foreach (var link in links)
                        {
                            yield return new DocumentLink()
                            {
                                Range = link.Range.ToRange(),
                                Target = new ExternalSourceReference(request.TextDocument.Uri)
                                    .WithRequestForSourceFile(link.Target).ToUri().ToString()
                            };
                        }
                    }
                }
            }
        }
    }
}
