// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using static Bicep.LanguageServer.Telemetry.BicepTelemetryEvent;

namespace Bicep.LanguageServer.Handlers
{
    [Method(BicepExternalSourceRequestHandler.BicepExternalSourceLspMethodName, Direction.ClientToServer)]
    public record BicepExternalSourceParams(
        string Target, // The module reference to display sources for
        string? requestedSourceFile = null // The relative source path of the file in the module to get source for (main.json if null))
    ) : IRequest<BicepExternalSourceResponse>;

    public record BicepExternalSourceResponse(string? Content, string? Error = null);

    /// <summary>
    /// Handles textDocument/bicepExternalSource LSP requests. These are sent by clients that are resolving contents of document URIs using the bicep-extsrc: scheme.
    /// The BicepDefinitionHandler returns such URIs when definitions are inside modules that reside in the local module cache.
    /// </summary>
    public class BicepExternalSourceRequestHandler : IJsonRpcRequestHandler<BicepExternalSourceParams, BicepExternalSourceResponse>
    {
        public const string BicepExternalSourceLspMethodName = "textDocument/bicepExternalSource";

        private readonly IModuleDispatcher moduleDispatcher;
        private readonly ITelemetryProvider telemetryProvider;
        private readonly ISourceFileFactory sourceFileFactory;

        public BicepExternalSourceRequestHandler(
            IModuleDispatcher moduleDispatcher,
            ITelemetryProvider telemetryProvider,
            ISourceFileFactory sourceFileFactory)
        {
            this.moduleDispatcher = moduleDispatcher;
            this.telemetryProvider = telemetryProvider;
            this.sourceFileFactory = sourceFileFactory;
        }

        public Task<BicepExternalSourceResponse> Handle(BicepExternalSourceParams request, CancellationToken cancellationToken)
        {
            // If any of the following paths results in an exception being thrown (and surfaced client-side to the user),
            // it indicates a code defect client or server-side.
            // In normal operation, the user should never see them regardless of how malformed their code is.
            var dummyReferencingFile = this.sourceFileFactory.CreateDummyArtifactReferencingFile();
            if (!moduleDispatcher.TryGetArtifactReference(dummyReferencingFile, ArtifactType.Module, request.Target).IsSuccess(out var moduleReference))
            {
                telemetryProvider.PostEvent(ExternalSourceRequestFailure(nameof(moduleDispatcher.TryGetArtifactReference)));
                return Task.FromResult(new BicepExternalSourceResponse(null,
                    $"The client specified an invalid module reference '{request.Target}'."));
            }

            if (moduleReference is not OciArtifactReference ociModuleReference)
            {
                telemetryProvider.PostEvent(ExternalSourceRequestFailure("localNotSupported"));
                return Task.FromResult(new BicepExternalSourceResponse(null,
                    $"The specified module reference '{request.Target}' refers to a local module which is not supported by {BicepExternalSourceLspMethodName} requests."));
            }

            if (!moduleDispatcher.TryGetLocalArtifactEntryPointFileHandle(moduleReference).IsSuccess())
            {
                telemetryProvider.PostEvent(ExternalSourceRequestFailure(nameof(moduleDispatcher.TryGetLocalArtifactEntryPointFileHandle)));
                return Task.FromResult(new BicepExternalSourceResponse(null,
                    $"Unable to obtain the entry point URI for module '{moduleReference.FullyQualifiedReference}'."));
            }

            var success = ociModuleReference.TryLoadSourceArchive().IsSuccess(out var sourceArchive, out var ex);

            if (request.requestedSourceFile is { })
            {
                if (success)
                {
                    Debug.Assert(sourceArchive is { });
                    var requestedFile = sourceArchive.FindSourceFile(request.requestedSourceFile);
                    telemetryProvider.PostEvent(CreateSuccessTelemetry(sourceArchive, request.requestedSourceFile));
                    return Task.FromResult(new BicepExternalSourceResponse(requestedFile.Contents));
                }
                else if (ex is SourceNotAvailableException)
                {
                    // Fall through
                }
                else
                {
                    Debug.Assert(ex is { });
                    telemetryProvider.PostEvent(ExternalSourceRequestFailure($"TryGetModuleSources: {ex.GetType().Name}"));
                    return Task.FromResult(new BicepExternalSourceResponse(null, ex.Message));
                }
            }

            // No sources available, or specifically requesting the compiled main.json (requestedSourceFile=null).
            // Return the compiled JSON (main.json).
            if (!ociModuleReference.ModuleMainTemplateFile.TryReadAllText().IsSuccess(out var contents, out var failureBuilder))
            {
                var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;
                return Task.FromResult(new BicepExternalSourceResponse(null, $"Unable to read file '{ociModuleReference.ModuleMainTemplateFile.Uri}'. {message}"));
            }

            telemetryProvider.PostEvent(CreateSuccessTelemetry(sourceArchive, request.requestedSourceFile));
            return Task.FromResult(new BicepExternalSourceResponse(contents));
        }

        /// <summary>
        /// Creates a bicep-extsrc: URI for a given module's source file to give to the client to use as a document URI.  (Client will then
        ///   ask for us the source code via a textDocument/externalSource request).
        /// </summary>
        /// <param name="reference">The module reference</param>
        /// <param name="sourceArchive">The source archive for the module, if sources are available</param>
        /// <returns>A bicep-extsrc: URI</returns>
        public static Uri GetRegistryModuleSourceLinkUri(OciArtifactReference reference, SourceArchive? sourceArchive)
        {
            return new ExternalSourceReference(reference, sourceArchive).ToUri();
        }

        public static Uri GetTemplateSpecSourceLinkUri(TemplateSpecModuleReference reference)
        {
            var uriBuilder = new UriBuilder($"{LangServerConstants.ExternalSourceFileScheme}:{Uri.EscapeDataString(reference.FullyQualifiedReference)}")
            {
                Query = Uri.EscapeDataString(reference.FullyQualifiedReference),
            };

            return uriBuilder.Uri;
        }

        private BicepTelemetryEvent CreateSuccessTelemetry(SourceArchive? sourceArchive, string? requestedSourceFile)
        {
            return ExternalSourceRequestSuccess(
                hasSource: sourceArchive is not null,
                archiveFilesCount: sourceArchive?.SourceFileCount ?? 0,
                fileExtension: Path.GetExtension(requestedSourceFile) ?? (requestedSourceFile is null ? ".json" : string.Empty),
                requestType: requestedSourceFile switch
                {
                    null => ExternalSourceRequestType.CompiledJson,
                    string when sourceArchive is null => ExternalSourceRequestType.Unknown, // shouldn't happen
                    string file when StringComparer.Ordinal.Equals(file, sourceArchive.EntrypointRelativePath) => ExternalSourceRequestType.BicepEntrypoint,
                    string file when file.Contains("<root>") => ExternalSourceRequestType.NestedExternal,
                    _ => ExternalSourceRequestType.Local
                }
            );
        }
    }
}
