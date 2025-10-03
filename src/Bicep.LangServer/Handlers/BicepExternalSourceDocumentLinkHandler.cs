// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Reactive;
using Azure.Deployments.Core.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;
using static Bicep.LanguageServer.Telemetry.BicepTelemetryEvent;

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
    public class BicepExternalSourceDocumentLinkHandler(IModuleDispatcher ModuleDispatcher, ILanguageServerFacade Server, ITelemetryProvider TelemetryProvider, ISourceFileFactory sourceFileFactory)
        : DocumentLinkHandlerBase<ExternalSourceDocumentLinkData>
    {
        protected override Task<DocumentLinkContainer<ExternalSourceDocumentLinkData>> HandleParams(DocumentLinkParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var links = GetDocumentLinks(sourceFileFactory, request, cancellationToken);
            return Task.FromResult(new DocumentLinkContainer<ExternalSourceDocumentLinkData>(links));
        }

        protected override async Task<DocumentLink<ExternalSourceDocumentLinkData>> HandleResolve(DocumentLink<ExternalSourceDocumentLinkData> request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await ResolveDocumentLink(request, ModuleDispatcher, sourceFileFactory, Server, TelemetryProvider);
        }

        protected override DocumentLinkRegistrationOptions CreateRegistrationOptions(DocumentLinkCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = TextDocumentSelector.ForScheme(LangServerConstants.ExternalSourceFileScheme),
            ResolveProvider = true,
        };

        public static IEnumerable<DocumentLink<ExternalSourceDocumentLinkData>> GetDocumentLinks(ISourceFileFactory sourceFileFactory, DocumentLinkParams request, CancellationToken cancellationToken)
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
                    var dummyFile = sourceFileFactory.CreateDummyArtifactReferencingFile();
                    if (!currentDocumentReference.ToArtifactReference(dummyFile).IsSuccess(out var currentDocumentArtifact, out var message))
                    {
                        Trace.WriteLine(message);
                        yield break;
                    }

                    if (!currentDocumentArtifact.TryLoadSourceArchive().IsSuccess(out var currentDocumentSourceArchive, out var ex))
                    {
                        Trace.WriteLine(ex.Message);
                        yield break;
                    }

                    foreach (var nestedLink in currentDocumentSourceArchive.FindDocumentLinks(currentDocumentRelativeFile))
                    {
                        var targetFileInfo = currentDocumentSourceArchive.FindSourceFile(nestedLink.Target);
                        var linkToRawCompiledJson = new ExternalSourceReference(request.TextDocument.Uri)
                            .WithRequestForSourceFile(targetFileInfo.Metadata.Path).ToUri().ToString();

                        // Does this nested link have a pointer to its artifact so we can try restoring it and get the source?
                        if (targetFileInfo.Metadata.ArtifactAddress?.ArtifactId is { } sourceId)
                        {
                            // Yes, it's an external module with source.  We won't set the target now - we'll wait until the user clicks on it to resolve it, to give us a chance to restore the module.
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

        public static async Task<DocumentLink<ExternalSourceDocumentLinkData>> ResolveDocumentLink(
            DocumentLink<ExternalSourceDocumentLinkData> request,
            IModuleDispatcher moduleDispatcher,
            ISourceFileFactory sourceFileFactory,
            ILanguageServerFacade server,
            ITelemetryProvider telemetryProvider)
        {
            Trace.WriteLine($"{nameof(BicepExternalSourceDocumentLinkHandler)}: Resolving external source document link: {request.Data.TargetArtifactId}");

            var data = request.Data;

            var dummyFile = sourceFileFactory.CreateDummyArtifactReferencingFile();
            if (!OciArtifactReference.TryParse(dummyFile, ArtifactType.Module, null, data.TargetArtifactId).IsSuccess(out var targetArtifactReference, out var error))
            {
                server.Window.ShowWarning($"Unable to parse the module source ID '{data.TargetArtifactId}': {error(DiagnosticBuilder.ForDocumentStart()).Message}");
                telemetryProvider.PostEvent(ExternalSourceDocLinkClickFailure("TryParseModule"));
                return GetAlternateLink();
            }

            var restoreStatus = moduleDispatcher.GetArtifactRestoreStatus(targetArtifactReference, out var errorBuilder);
            var errorMessage = errorBuilder?.Invoke(ForDocumentStart()).Message;
            Trace.WriteLineIf(errorMessage is { }, $"Restore status: {errorMessage})");
            if (restoreStatus == ArtifactRestoreStatus.Unknown)
            {
                // We haven't tried restoring this module yet. Let's try it now.
                Trace.WriteLine($"Attempting to restore module {targetArtifactReference.FullyQualifiedReference}");
                await moduleDispatcher.RestoreArtifacts(new[] { targetArtifactReference }, forceRestore: false);

                restoreStatus = moduleDispatcher.GetArtifactRestoreStatus(targetArtifactReference, out errorBuilder);
                errorMessage = errorBuilder?.Invoke(ForDocumentStart()).Message;
                Trace.WriteLineIf(errorMessage is { }, $"New restore status: {errorMessage})");

                if (restoreStatus != ArtifactRestoreStatus.Succeeded)
                {
                    var diagnostic = errorBuilder?.Invoke(DiagnosticBuilder.ForDocumentStart());
                    var restoreMessage = diagnostic?.Message ?? "Unknown error";
                    server.Window.ShowWarning($"Unable to restore module {targetArtifactReference.FullyQualifiedReference}: {restoreMessage}");
                    telemetryProvider.PostEvent(ExternalSourceDocLinkClickFailure("unableToRestore", diagnostic?.Code));
                    return GetAlternateLink();
                }
            }
            else if (restoreStatus == ArtifactRestoreStatus.Failed)
            {
                server.Window.ShowWarning("Restore previously failed. Force module restore or restart to try again.");
                telemetryProvider.PostEvent(ExternalSourceDocLinkClickFailure("restorePrevFailed"));
                return GetAlternateLink();
            }

            // If we get here, the module *should* have sources available (since we are going through delayed resolution), so show a message if we can't for some reason
            if (!targetArtifactReference.TryLoadSourceArchive().IsSuccess(out var sourceArchive, out var ex))
            {
                server.Window.ShowWarning($"Unable to retrieve source code for module {targetArtifactReference.FullyQualifiedReference}. {ex.Message}");
                telemetryProvider.PostEvent(ExternalSourceDocLinkClickFailure("tryGetModuleSources", ex.Message));
                return GetAlternateLink();
            }

            var registryType = targetArtifactReference.FullyQualifiedReference.StartsWithOrdinalInsensitively("br:mcr.microsoft.com/") ? ModuleRegistryType.MCR : ModuleRegistryType.ACR;
            telemetryProvider.PostEvent(ExternalSourceDocLinkClickSuccess(ExternalSourceRequestType.BicepEntrypoint, registryType));

            return request with
            {
                Target = new ExternalSourceReference(targetArtifactReference, sourceArchive).ToUri().ToString()
            };

            DocumentLink<ExternalSourceDocumentLinkData> GetAlternateLink() => request with
            {
                Target = data.CompiledJsonLink
            };
        }
    }
}
