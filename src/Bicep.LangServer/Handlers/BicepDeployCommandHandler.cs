// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.LanguageServer.Deploy;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeployParams(string parameterFilePath, string id, string deploymentScope, string location, string template, string token, string expiresOnTimestamp) : IRequest<string>;

    public class BicepDeployCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeployParams, string>
    {
        private readonly IDeploymentCollectionProvider deploymentCollectionProvider;

        public BicepDeployCommandHandler(IDeploymentCollectionProvider deploymentCollectionProvider, ISerializer serializer)
            : base(LangServerConstants.DeployCommand, serializer)
        {
            this.deploymentCollectionProvider = deploymentCollectionProvider;
        }

        public override async Task<string> Handle(BicepDeployParams request, CancellationToken cancellationToken)
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
