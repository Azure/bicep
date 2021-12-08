// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Handlers
{
    [Method(BicepRegistryCacheRequestHandler.BicepCacheLspMethod, Direction.ClientToServer)]
    public record BicepRegistryCacheParams(TextDocumentIdentifier TextDocument, string Target) : ITextDocumentIdentifierParams, IRequest<BicepRegistryCacheResponse>;

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

        private readonly IConfigurationManager configurationManager;

        public BicepRegistryCacheRequestHandler(IModuleDispatcher moduleDispatcher, IFileResolver fileResolver, IConfigurationManager configurationManager)
        {
            this.moduleDispatcher = moduleDispatcher;
            this.fileResolver = fileResolver;
            this.configurationManager = configurationManager;
        }

        public Task<BicepRegistryCacheResponse> Handle(BicepRegistryCacheParams request, CancellationToken cancellationToken)
        {
            // If any of the following paths result in an exception being thrown (and surfaced client-side to the user),
            // it indicates a code defect client or server-side.
            // In normal operation, the user should never see them regardless of how malformed their code is.            

            var configuration = this.configurationManager.GetConfiguration(request.TextDocument.Uri.ToUri());
            var moduleReference = this.moduleDispatcher.TryGetModuleReference(request.Target, configuration, out _) ?? throw new InvalidOperationException($"The client specified an invalid module reference '{request.Target}'.");

            if (!moduleReference.IsExternal)
            {
                throw new InvalidOperationException($"The specified module reference '{request.Target}' refers to a local module which is not supported by {BicepCacheLspMethod} requests.");
            }

            if (this.moduleDispatcher.GetModuleRestoreStatus(moduleReference, configuration, out _) != ModuleRestoreStatus.Succeeded)
            {
                throw new InvalidOperationException($"The module '{moduleReference.FullyQualifiedReference}' has not yet been successfully restored.");
            }

            var uri = this.moduleDispatcher.TryGetLocalModuleEntryPointUri(null, moduleReference, configuration, out _) ?? throw new InvalidOperationException($"Unable to obtain the entry point URI for module '{moduleReference.FullyQualifiedReference}'.");
            if (!this.fileResolver.TryRead(uri, out var contents, out var failureBuilder))
            {
                var message = failureBuilder(DiagnosticBuilder.ForPosition(new TextSpan(0, 0))).Message;
                throw new InvalidOperationException($"Unable to read file '{uri}'. {message}");
            }

            return Task.FromResult(new BicepRegistryCacheResponse(contents));
        }
    }
}
