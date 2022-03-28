// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.LanguageServer.Deploy;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    [Method(BicepDeployCommandHandler.MethodName, Direction.ClientToServer)]
    public record BicepDeployParams(TextDocumentIdentifier textDocument, string parameterFilePath, string id, string deploymentScope, string location, string template, string token, string expiresOnTimestamp) : IRequest<string>;

    public class BicepDeployCommandHandler : IJsonRpcRequestHandler<BicepDeployParams, string>
    {
        public const string MethodName = "bicep/deploy";

        private readonly IDeploymentCollectionProvider deploymentCollectionProvider;

        public BicepDeployCommandHandler(IDeploymentCollectionProvider deploymentCollectionProvider)
        {
            this.deploymentCollectionProvider = deploymentCollectionProvider;
        }

        public async Task<string> Handle(BicepDeployParams request, CancellationToken cancellationToken)
        {
            var credential = new CredentialFromTokenAndTimeStamp(request.token, request.expiresOnTimestamp);
            var armClient = new ArmClient(credential);

            string deploymentOutput = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider,
                armClient,
                request.template,
                request.parameterFilePath,
                request.id,
                request.deploymentScope,
                request.location);

            return deploymentOutput;
        }
    }
}
