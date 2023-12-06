// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Handlers
{
    [Method(BicepExternalSourceRequestHandler.BicepExternalSourceLspMethodName, Direction.ClientToServer)]
    public record BicepExternalSourceParams(
        string Target // The module reference to display sources for
    ) : IRequest<BicepExternalSourceResponse>;

    public record BicepExternalSourceResponse(string Content);

    /// <summary>
    /// Handles textDocument/bicepExternalSource LSP requests. These are sent by clients that are resolving contents of document URIs using the bicep-extsrc: scheme.
    /// The BicepDefinitionHandler returns such URIs when definitions are inside modules that reside in the local module cache.
    /// </summary>
    public class BicepExternalSourceRequestHandler : IJsonRpcRequestHandler<BicepExternalSourceParams, BicepExternalSourceResponse>
    {
        public const string BicepExternalSourceLspMethodName = "textDocument/bicepExternalSource";

        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IFileResolver fileResolver;

        public BicepExternalSourceRequestHandler(IModuleDispatcher moduleDispatcher, IFileResolver fileResolver)
        {
            this.moduleDispatcher = moduleDispatcher;
            this.fileResolver = fileResolver;
        }

        public Task<BicepExternalSourceResponse> Handle(BicepExternalSourceParams request, CancellationToken cancellationToken)
        {
            // If any of the following paths result in an exception being thrown (and surfaced client-side to the user),
            // it indicates a code defect client or server-side.
            // In normal operation, the user should never see them regardless of how malformed their code is.

            if (!moduleDispatcher.TryGetArtifactReference(ArtifactType.Module, request.Target, new Uri("file:///no-parent-file-is-available")).IsSuccess(out var moduleReference))
            {
                throw new InvalidOperationException(
                    $"The client specified an invalid module reference '{request.Target}'.");
            }

            if (!moduleReference.IsExternal)
            {
                throw new InvalidOperationException(
                    $"The specified module reference '{request.Target}' refers to a local module which is not supported by {BicepExternalSourceLspMethodName} requests.");
            }

            if (this.moduleDispatcher.GetArtifactRestoreStatus(moduleReference, out _) != ArtifactRestoreStatus.Succeeded)
            {
                throw new InvalidOperationException(
                    $"The module '{moduleReference.FullyQualifiedReference}' has not yet been successfully restored.");
            }

            if (!moduleDispatcher.TryGetLocalArtifactEntryPointUri(moduleReference).IsSuccess(out var uri))
            {
                throw new InvalidOperationException(
                    $"Unable to obtain the entry point URI for module '{moduleReference.FullyQualifiedReference}'.");
            }

            if (moduleDispatcher.TryGetModuleSources(moduleReference) is SourceArchive sourceArchive)
            {
                // TODO: For now, we just proffer the main source file
                var entrypointFile = sourceArchive.SourceFiles.Single(f => f.Path == sourceArchive.EntrypointRelativePath);
                return Task.FromResult(new BicepExternalSourceResponse(entrypointFile.Contents));
            }

            // No sources available, just retrieve the JSON source
            if (!this.fileResolver.TryRead(uri).IsSuccess(out var contents, out var failureBuilder))
            {
                var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;
                throw new InvalidOperationException($"Unable to read file '{uri}'. {message}");
            }

            return Task.FromResult(new BicepExternalSourceResponse(contents));
        }

        /// <summary>
        /// Creates a bicep-extsrc: URI for a given module's source file to give to the client to use as a document URI.  (Client will then
        ///   ask for us the source code via a textDocument/externalSource request).
        /// </summary>
        /// <param name="reference">The module reference</param>
        /// <param name="sourceArchive">The source archive for the module, if sources are available</param>
        /// <returns>A bicep-extsrc: URI</returns>
        public static Uri GetExternalSourceLinkUri(OciArtifactReference reference, SourceArchive? sourceArchive)
        {
            return new ExternalSourceReference(reference, sourceArchive).ToUri();
        }
    }
}
