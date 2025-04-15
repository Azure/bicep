// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.LanguageServer.Providers;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepEditConfigParams : IRequest<bool>
    {
        public string? DestinationPath { get; init; }
    }

    /// <summary>
    /// Handles a request from the client to edit a bicep configuration file
    /// </summary>
    /// <remarks>
    /// Using ExecuteTypedResponseCommandHandlerBase instead of IJsonRpcRequestHandler because IJsonRpcRequestHandler will throw "Content modified" if text changes are detected, and for this command
    /// that is expected.
    /// </remarks>
    public class BicepEditConfigFileHandler : ExecuteTypedResponseCommandHandlerBase<BicepEditConfigParams, bool>
    {
        private readonly ILogger<BicepEditConfigFileHandler> logger;
        private readonly IClientCapabilitiesProvider clientCapabilitiesProvider;
        private readonly ILanguageServerFacade server;

        public BicepEditConfigFileHandler(ILanguageServerFacade server, IClientCapabilitiesProvider clientCapabilitiesProvider, ILogger<BicepEditConfigFileHandler> logger, ISerializer serializer)
            : base(LangServerConstants.EditConfigFile, serializer)
        {
            this.clientCapabilitiesProvider = clientCapabilitiesProvider;
            this.server = server;
            this.logger = logger;
        }

        public override async Task<bool> Handle(BicepEditConfigParams request, CancellationToken cancellationToken)
        {
            string? destinationPath = request.DestinationPath;
            if (destinationPath is null)
            {
                throw new ArgumentException($"{nameof(destinationPath)} should not be null");
            }

            this.logger.LogTrace($"Writing new configuration file to {destinationPath}");
            string defaultBicepConfig = DefaultBicepConfigHelper.GetDefaultBicepConfig();
            await File.WriteAllTextAsync(destinationPath, defaultBicepConfig);

            await BicepEditLinterRuleCommandHandler.AddAndSelectRuleLevel(server, clientCapabilitiesProvider, destinationPath, DefaultBicepConfigHelper.DefaultRuleCode);
            return true;
        }
    }
}
