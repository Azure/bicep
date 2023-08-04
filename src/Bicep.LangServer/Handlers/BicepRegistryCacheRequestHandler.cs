// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Handlers
{
    [Method(BicepRegistryCacheRequestHandler.BicepCacheLspMethod, Direction.ClientToServer)]
    public record BicepRegistryCacheParams(
        TextDocumentIdentifier TextDocument, // The bicep file which contains a reference to the target module
        string Target                        // The module reference to display sources for
    ) : ITextDocumentIdentifierParams, IRequest<BicepRegistryCacheResponse>;

    public record BicepRegistryCacheResponse(string Content);

    /// <summary>
    /// Handles textDocument/bicepCache LSP requests. These are sent by clients that are resolving contents of document URIs using the bicep-cache:// scheme.
    /// The BicepDefinitionHandler returns such URIs when definitions are inside modules that reside in the local module cache.
    /// </summary>
    public class BicepRegistryCacheRequestHandler : IJsonRpcRequestHandler<BicepRegistryCacheParams, BicepRegistryCacheResponse>
    {
        public const string BicepCacheLspMethod = "textDocument/bicepCache";

        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IFileResolver fileResolver;
        private readonly IFeatureProviderFactory featureProviderFactory;

        public BicepRegistryCacheRequestHandler(IModuleDispatcher moduleDispatcher, IFileResolver fileResolver, IFeatureProviderFactory featureProviderFactory)
        {
            this.moduleDispatcher = moduleDispatcher;
            this.fileResolver = fileResolver;
            this.featureProviderFactory = featureProviderFactory;
        }

        public Task<BicepRegistryCacheResponse> Handle(BicepRegistryCacheParams request, CancellationToken cancellationToken)
        {
            // If any of the following paths result in an exception being thrown (and surfaced client-side to the user),
            // it indicates a code defect client or server-side.
            // In normal operation, the user should never see them regardless of how malformed their code is.

            var parentBicepUri = request.TextDocument.Uri.ToUri();
            if (!moduleDispatcher.TryGetModuleReference(request.Target, parentBicepUri, out var moduleReference, out _))
            {
                throw new InvalidOperationException($"The client specified an invalid module reference '{request.Target}'.");
            }

            if (!moduleReference.IsExternal)
            {
                throw new InvalidOperationException($"The specified module reference '{request.Target}' refers to a local module which is not supported by {BicepCacheLspMethod} requests.");
            }

            if (this.moduleDispatcher.GetModuleRestoreStatus(moduleReference, out _) != ModuleRestoreStatus.Succeeded)
            {
                throw new InvalidOperationException($"The module '{moduleReference.FullyQualifiedReference}' has not yet been successfully restored.");
            }

            if (!moduleDispatcher.TryGetLocalModuleEntryPointUri(moduleReference, out var uri, out _))
            {
                throw new InvalidOperationException($"Unable to obtain the entry point URI for module '{moduleReference.FullyQualifiedReference}'.");
            }

            var featureProvider = featureProviderFactory.GetFeatureProvider(parentBicepUri);
            if (featureProvider.PublishSourceEnabled && moduleDispatcher.TryGetModuleSources(moduleReference, out var sourceArchive))
            {
                using var sources = sourceArchive;

                // TODO: For now, we just proffer the main file
                var entrypointFile = sources.GetSourceFiles().Single(f => f.Metadata.Path == sourceArchive.GetEntrypointPath());
                return Task.FromResult(new BicepRegistryCacheResponse(entrypointFile.Contents));
            }

            // No sources available, just retrieve the JSON source
            if (!this.fileResolver.TryRead(uri, out var contents, out var failureBuilder))
            {
                var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;
                throw new InvalidOperationException($"Unable to read file '{uri}'. {message}");
            }

            return Task.FromResult(new BicepRegistryCacheResponse(contents));
        }
    }
}
