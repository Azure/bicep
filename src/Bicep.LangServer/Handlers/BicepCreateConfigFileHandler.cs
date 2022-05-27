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

namespace Bicep.LanguageServer.Handlers
{
    [Method("bicep/createConfigFile", Direction.ClientToServer)]
    public class BicepCreateConfigParams : IRequest<bool>
    {
        public string? DestinationPath { get; init; }
    }

    /// <summary>
    /// Handles a request from the client to create a bicep configuration file
    /// </summary>
    public class BicepCreateConfigFileHandler : IJsonRpcRequestHandler<BicepCreateConfigParams, bool>
    {
        private readonly ILogger<BicepCreateConfigFileHandler> logger;

        public BicepCreateConfigFileHandler(ILogger<BicepCreateConfigFileHandler> logger)
        {
            this.logger = logger;
        }

        public async Task<bool> Handle(BicepCreateConfigParams request, CancellationToken cancellationToken)
        {
            string? destinationPath = request.DestinationPath;
            if (destinationPath is null)
            {
                throw new ArgumentException($"{nameof(destinationPath)} should not be null");
            }

            this.logger.LogTrace($"Writing new configuration file to {destinationPath}");
            string defaultBicepConfig = DefaultBicepConfigHelper.GetDefaultBicepConfig();
            await File.WriteAllTextAsync(destinationPath, defaultBicepConfig);
            return true;
        }
    }
}
