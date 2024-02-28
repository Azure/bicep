// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
#nullable disable // The generated code is not yet nullable-aware, this is a temporary fix for that
    public partial record Asdfg : IHandlerIdentity
    {
        public string TargetArtifactId { get; init; }

        public Asdfg(string targetArtifactId)
        {
            TargetArtifactId = targetArtifactId;
        }
    }

#nullable restore

    /// <summary>
    /// This handles the case where the document is a source file from an external module, and we've been asked to return nested links within it (to files local to that module or to other external modules)
    /// </summary>
    public class BicepExternalSourceDocumentLinkHandler(IModuleDispatcher ModuleDispatcher)
        : DocumentLinkHandlerBase<Asdfg>
    {
        protected override Task<DocumentLinkContainer<Asdfg>> HandleParams(DocumentLinkParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var links = GetDocumentLinks(ModuleDispatcher, request, cancellationToken);
            return Task.FromResult(new DocumentLinkContainer<Asdfg>(links));
        }

        protected override async Task<DocumentLink<Asdfg>> HandleResolve(DocumentLink<Asdfg> request, CancellationToken cancellationToken)
        {
            return await ResolveDocumentLink(request, cancellationToken);
        }

        protected override DocumentLinkRegistrationOptions CreateRegistrationOptions(DocumentLinkCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = TextDocumentSelector.ForScheme(LangServerConstants.ExternalSourceFileScheme),
            ResolveProvider = true,
            WorkDoneProgress = true,
        };

        public static IEnumerable<DocumentLink<Asdfg>> GetDocumentLinks(IModuleDispatcher moduleDispatcher, DocumentLinkParams request, CancellationToken cancellationToken)
        {
            var currentDocument = request.TextDocument;

            if (currentDocument.Uri.Scheme == LangServerConstants.ExternalSourceFileScheme)
            {
                ExternalSourceReference? currentDocumentReference;
                try
                {
                    currentDocumentReference = new ExternalSourceReference(currentDocument.Uri);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"There was an error retrieving source code for this module: {ex.Message}");
                    yield break;
                }


                var currentDocumentRelativeFile = currentDocumentReference.RequestedFile;
                if (currentDocumentRelativeFile is { })
                {
                    if (!currentDocumentReference.ToArtifactReference().IsSuccess(out var currentDocumentArtifact, out var message))
                    {
                        Trace.WriteLine(message);
                        yield break;
                    }

                    if (!moduleDispatcher.TryGetModuleSources(currentDocumentArtifact).IsSuccess(out var currentDocumentSourceArchive, out var ex))
                    {
                        Trace.WriteLine(ex.Message);
                        yield break;
                    }

                    if (currentDocumentSourceArchive.DocumentLinks.TryGetValue(currentDocumentRelativeFile, out var nestedLinks))
                    {
                        foreach (var nestedLink in nestedLinks)
                        {
                            // Does this nested link have a pointer to its artifact so we can try restoring it and get the source?
                            var targetFileInfo = currentDocumentSourceArchive.FindExpectedSourceFile(nestedLink.Target);
                            if (targetFileInfo.SourceArtifactId is { } && targetFileInfo.SourceArtifactId.StartsWith(OciArtifactReferenceFacts.SchemeWithColon)) //asdfg test ignore if not "br:"
                            {
                                // Yes, it's an external module with source.  We won't set the target now - we'll wait until the user clicks on it to resolve it.
                                var sourceId = targetFileInfo.SourceArtifactId.Substring(OciArtifactReferenceFacts.SchemeWithColon.Length);
                                yield return new DocumentLink<Asdfg>()
                                {
                                    Range = nestedLink.Range.ToRange(),
                                    Data = new Asdfg(sourceId),
                                };
                            }

                            yield return new DocumentLink()
                            {
                                // This is a link to a file that we don't have source for, so we'll just display the main.json file
                                Range = nestedLink.Range.ToRange(),
                                Target = new ExternalSourceReference(request.TextDocument.Uri)
                                    .WithRequestForSourceFile(targetFileInfo.Path).ToUri().ToString(),
                            };
                        }
                    }
                }
            }
        }

        public async Task<DocumentLink<Asdfg>> ResolveDocumentLink(DocumentLink<Asdfg> request, CancellationToken cancellationToken)
        {
            Trace.WriteLine($"{nameof(BicepExternalSourceDocumentLinkHandler)}: Resolving document link: {request.Data.TargetArtifactId}");

            var data = request.Data;

            if (!OciArtifactReference.TryParseModule(data.TargetArtifactId).IsSuccess(out var targetArtifactReference, out var error))
            {
                Trace.WriteLine(error(DiagnosticBuilder.ForDocumentStart()).Message); //asdfg
                return request;
            }

            var restoreStatus = ModuleDispatcher.GetArtifactRestoreStatus(targetArtifactReference, out var errorBuilder);
            if (restoreStatus != ArtifactRestoreStatus.Succeeded)
            {
                var errorMessage = errorBuilder is { } ? errorBuilder(DiagnosticBuilder.ForDocumentStart()).Message : "The module has not yet been successfully restored.";

                Trace.WriteLine($"  The module has not yet been successfully restored. {errorMessage}"); //asdfg

                // Attempt to restore the module
                if (!await ModuleDispatcher.RestoreModules(new[] { targetArtifactReference }, forceRestore: true))
                {
                    ModuleDispatcher.GetArtifactRestoreStatus(targetArtifactReference, out errorBuilder);
                    errorMessage = errorBuilder is { } ? errorBuilder(DiagnosticBuilder.ForDocumentStart()).Message : "Unknown error.";
                    throw new InvalidOperationException($"Unable to restore module {targetArtifactReference.FullyQualifiedReference}: {errorMessage}");
                }
            }

            if (!ModuleDispatcher.TryGetModuleSources(targetArtifactReference).IsSuccess(out var sourceArchive, out var ex))
            {
                Trace.WriteLine(ex.Message);
                throw ex; //asdfg
            }

            return request with
            {
                Target = new ExternalSourceReference(targetArtifactReference, sourceArchive).ToUri().ToString()
            };
        }
    }
}
