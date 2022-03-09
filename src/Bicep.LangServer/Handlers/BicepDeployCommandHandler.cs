// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Bicep.LanguageServer.Deploy;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    [Method(BicepDeployCommandHandler.BicepDeployLspMethod, Direction.ClientToServer)]
    public record BicepDeployParams(TextDocumentIdentifier textDocument, string parameterFilePath, string id, string deploymentScope, string location, string template) : IRequest<string>;

    public class BicepDeployCommandHandler : IJsonRpcRequestHandler<BicepDeployParams, string>
    {
        public const string BicepDeployLspMethod = "bicep/deploy";

        private readonly ITokenCredentialFactory credentialFactory;
        private readonly IConfigurationManager configurationManager;
        private readonly IDeploymentCollectionProvider deploymentCollectionProvider;

        public BicepDeployCommandHandler(
            IConfigurationManager configurationManager,
            ITokenCredentialFactory credentialFactory,
            IDeploymentCollectionProvider deploymentCollectionProvider)
        {
            this.credentialFactory = credentialFactory;
            this.configurationManager = configurationManager;
            this.deploymentCollectionProvider = deploymentCollectionProvider;
        }

        public async Task<string> Handle(BicepDeployParams request, CancellationToken cancellationToken)
        {
            DocumentUri documentUri = request.textDocument.Uri;
            var configuration = configurationManager.GetConfiguration(documentUri.ToUri());
            var credential = this.credentialFactory.CreateChain(ImmutableArray.Create(CredentialType.VisualStudioCode), configuration.Cloud.ActiveDirectoryAuthorityUri);
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
