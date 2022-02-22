// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Bicep.LanguageServer.Deploy;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDeployCommandHandler : ExecuteTypedResponseCommandHandlerBase<string, string, string, string, string, string, string>
    {
        private readonly ITokenCredentialFactory credentialFactory;
        private readonly IConfigurationManager configurationManager;

        public BicepDeployCommandHandler(
            ISerializer serializer,
            IConfigurationManager configurationManager,
            ITokenCredentialFactory credentialFactory)
            : base(LanguageConstants.Deploy, serializer)
        {
            this.credentialFactory = credentialFactory;
            this.configurationManager = configurationManager;
        }

        public override async Task<string> Handle(
            string bicepFilePath,
            string parameterFilePath,
            string id,
            string scope,
            string location,
            string template,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bicepFilePath))
            {
                throw new ArgumentException("Invalid input file");
            }

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var configuration = configurationManager.GetConfiguration(documentUri.ToUri());

            var credential = this.credentialFactory.CreateChain(ImmutableArray.Create(CredentialType.VisualStudioCode), configuration.Cloud.ActiveDirectoryAuthorityUri);
            ArmClient armClient = new ArmClient(credential);

            string deploymentOutput = await DeploymentHelper.CreateDeployment(armClient, template, parameterFilePath, id, scope, location);

            return deploymentOutput;
        }
    }
}
