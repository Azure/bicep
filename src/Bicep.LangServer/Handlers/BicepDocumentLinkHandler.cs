// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Reactive;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.LanguageServer.Handlers
{
#nullable disable // The generated code is not yet nullable-aware, this disables #nullable for the generated code to fix that

    public partial record ExternalSourceDocumentLinkData : IHandlerIdentity
    {
        public string TargetArtifactId { get; init; }
        // A link to the main.json file in the outer module's source.tgz file, in case we can't get to the nested module's source
        public string CompiledJsonLink { get; init; }

        public ExternalSourceDocumentLinkData(string targetArtifactId, string compiledJsonLink)
        {
            TargetArtifactId = targetArtifactId;
            this.CompiledJsonLink = compiledJsonLink;
        }
    }

#nullable restore

    /// <summary>
    /// This handles the case where the document is a source file from an external module, and we've been asked to return nested links within it (to files local to that module or to other external modules)
    /// </summary>
    public class BicepExternalSourceDocumentLinkHandler(IModuleDispatcher ModuleDispatcher, ILanguageServerFacade server)
        : DocumentLinkHandlerBase<ExternalSourceDocumentLinkData>
    {
        protected override Task<DocumentLinkContainer<ExternalSourceDocumentLinkData>> HandleParams(DocumentLinkParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var links = GetDocumentLinks(ModuleDispatcher, request, cancellationToken);
            return Task.FromResult(new DocumentLinkContainer<ExternalSourceDocumentLinkData>(links));
        }

        protected override async Task<DocumentLink<ExternalSourceDocumentLinkData>> HandleResolve(DocumentLink<ExternalSourceDocumentLinkData> request, CancellationToken cancellationToken)
        {
            return await ResolveDocumentLink(request, cancellationToken);
        }

        protected override DocumentLinkRegistrationOptions CreateRegistrationOptions(DocumentLinkCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = TextDocumentSelector.ForScheme(LangServerConstants.ExternalSourceFileScheme),
            ResolveProvider = true,
        };

        public static IEnumerable<DocumentLink<ExternalSourceDocumentLinkData>> GetDocumentLinks(IModuleDispatcher moduleDispatcher, DocumentLinkParams request, CancellationToken cancellationToken)
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
                            var targetFileInfo = currentDocumentSourceArchive.FindExpectedSourceFile(nestedLink.Target);
                            var linkToRawCompiledJson = new ExternalSourceReference(request.TextDocument.Uri)
                                .WithRequestForSourceFile(targetFileInfo.Path).ToUri().ToString();

                            // Does this nested link have a pointer to its artifact so we can try restoring it and get the source?
                            if (targetFileInfo.SourceArtifactId is { } && targetFileInfo.SourceArtifactId.StartsWith(OciArtifactReferenceFacts.SchemeWithColon))
                            {
                                // Yes, it's an external module with source.  We won't set the target now - we'll wait until the user clicks on it to resolve it, to give us a chance to restore the module.
                                var sourceId = targetFileInfo.SourceArtifactId.Substring(OciArtifactReferenceFacts.SchemeWithColon.Length);
                                yield return new DocumentLink<ExternalSourceDocumentLinkData>()
                                {
                                    Range = nestedLink.Range.ToRange(),
                                    Data = new ExternalSourceDocumentLinkData(sourceId, linkToRawCompiledJson)
                                };
                            }
                            else
                            {
                                yield return new DocumentLink()
                                {
                                    // This is a link to a file that we don't have source for, so we'll just display the main.json file
                                    Range = nestedLink.Range.ToRange(),
                                    Target = linkToRawCompiledJson
                                };
                            }
                        }
                    }
                }
            }
        }

        public async Task<DocumentLink<ExternalSourceDocumentLinkData>> ResolveDocumentLink(DocumentLink<ExternalSourceDocumentLinkData> request, CancellationToken cancellationToken)
        {
            Trace.WriteLine($"{nameof(BicepExternalSourceDocumentLinkHandler)}: Resolving external source document link: {request.Data.TargetArtifactId}");

            var data = request.Data;

            if (!OciArtifactReference.TryParseModule(data.TargetArtifactId).IsSuccess(out var targetArtifactReference, out var error))
            {
                ShowMessage($"Unable to parse the module source ID '{data.TargetArtifactId}': {error(DiagnosticBuilder.ForDocumentStart()).Message}");
                return GetAlternateLink();
            }

            var restoreStatus = ModuleDispatcher.GetArtifactRestoreStatus(targetArtifactReference, out var errorBuilder);
            var errorMessage = errorBuilder?.Invoke(DiagnosticBuilder.ForDocumentStart()).Message;
            Trace.WriteLineIf(errorMessage is { }, $"Restore status: {errorMessage})");
            if (restoreStatus == ArtifactRestoreStatus.Unknown)
            {
                // We haven't tried restoring this module yet. Let's try it now.

                Trace.WriteLine($"Attempting to restore module {targetArtifactReference.FullyQualifiedReference}");
                if (!await ModuleDispatcher.RestoreArtifacts(new[] { targetArtifactReference }, forceRestore: false))
                {
                    ModuleDispatcher.GetArtifactRestoreStatus(targetArtifactReference, out errorBuilder);
                    var restoreMessage = errorBuilder?.Invoke(DiagnosticBuilder.ForDocumentStart()).Message ?? "Unknown error";
                    ShowMessage($"Unable to restore module {targetArtifactReference.FullyQualifiedReference}: {errorMessage}");
                    return GetAlternateLink();
                }
            }
            else if (restoreStatus == ArtifactRestoreStatus.Failed)
            {
                Trace.WriteLine("Restore previously failed. Force module restore or restart to try again.");
                return GetAlternateLink();
            }

            // If we get here, the module *should* have sources available (since we are going through delayed resolution), so show a message if we can't for some reason
            if (!ModuleDispatcher.TryGetModuleSources(targetArtifactReference).IsSuccess(out var sourceArchive, out var ex))
            {
                ShowMessage($"Unable to retrieve source code for module {targetArtifactReference.FullyQualifiedReference}. {ex.Message}");
                return GetAlternateLink();
            }

            return request with
                {
                    Target = new ExternalSourceReference(targetArtifactReference, sourceArchive).ToUri().ToString()
                };

            DocumentLink<ExternalSourceDocumentLinkData> GetAlternateLink() => request with
                {
                    Target = data.CompiledJsonLink
                };
        }

        private void ShowMessage(string message)
        {
            server.Window.ShowWarning(message);
        }
    }
}
