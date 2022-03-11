// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LanguageServer.Handlers
{
    [Method("bicep/createConfigFile", Direction.ClientToServer)]
    public class BicepCreateConfigParams : IRequest<bool>
    {
        public DocumentUri? DestinationPath { get; init; }
    }

    /// <summary>
    /// Handles a request from the client to create a bicep configuration file
    /// </summary>
    public class BicepCreateConfigFileHandler : IJsonRpcRequestHandler<BicepCreateConfigParams, bool>
    {
        private readonly ILogger<BicepCreateConfigFileHandler> logger;
        private readonly ILanguageServerFacade server;

        public BicepCreateConfigFileHandler(ILogger<BicepCreateConfigFileHandler> logger, ILanguageServerFacade server)
        {
            this.logger = logger;
            this.server = server;
        }

        public async Task<bool> Handle(BicepCreateConfigParams request, CancellationToken cancellationToken)
        {
            string defaultBicepConfig = DefaultBicepConfigHelper.GetDefaultBicepConfig();
            string? destinationPath = request.DestinationPath?.GetFileSystemPath();
            if (destinationPath is null)
            {
                throw new ArgumentException($"{nameof(destinationPath)} should not be null");
            }

            this.logger.LogTrace($"Writing new configuration file to {destinationPath}");
            await File.WriteAllTextAsync(destinationPath, defaultBicepConfig);
            return true;
        }
    }
}
