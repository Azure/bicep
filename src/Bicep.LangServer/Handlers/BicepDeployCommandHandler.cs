// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.Core.Semantics;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeployParams(string documentPath, string parameterFilePath, string id, string deploymentScope, string location, string template, string token, string expiresOnTimestamp, string deployId) : IRequest<string>;

    public class BicepDeployCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeployParams, string>
    {
        private readonly ICompilationManager compilationManager;
        private readonly IDeploymentCollectionProvider deploymentCollectionProvider;
        private readonly ITelemetryProvider telemetryProvider;

        public BicepDeployCommandHandler(ICompilationManager compilationManager, IDeploymentCollectionProvider deploymentCollectionProvider, ISerializer serializer, ITelemetryProvider telemetryProvider)
            : base(LangServerConstants.DeployCommand, serializer)
        {
            this.compilationManager = compilationManager;
            this.deploymentCollectionProvider = deploymentCollectionProvider;
            this.telemetryProvider = telemetryProvider;
        }

        public override async Task<string> Handle(BicepDeployParams request, CancellationToken cancellationToken)
        {
            PostDeployStartTelemetryEvent(request.deployId);

            var credential = new CredentialFromTokenAndTimeStamp(request.token, request.expiresOnTimestamp);
            var armClient = new ArmClient(credential);

            var missingParameters = FindMissingParameters(request.documentPath, request.parameterFilePath);

            (bool isSuccess, string deploymentOutput) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider,
                armClient,
                request.documentPath,
                request.template,
                request.parameterFilePath,
                request.id,
                request.deploymentScope,
                request.location);

            PostDeployResultTelemetryEvent(request.deployId, isSuccess);

            return deploymentOutput;
        }

        private IEnumerable<ParameterSymbol> FindMissingParameters(string documentPath, string parameterFilePath)
        {
            var documentUri = DocumentUri.FromFileSystemPath(documentPath);
            var compilationContext = compilationManager.GetCompilation(documentUri);

            if (compilationContext is null)
            {
                return Enumerable.Empty<ParameterSymbol>();
            }

            var semanticModel = compilationContext.Compilation.GetEntrypointSemanticModel();
            var parameterDeclarations = semanticModel.Root.ParameterDeclarations;

            if (string.IsNullOrEmpty(parameterFilePath))
            {
                return parameterDeclarations;
            }

            var parametersFileContents = File.ReadAllText(parameterFilePath);
            Dictionary<string, dynamic>? dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(parametersFileContents);

            if (dict is null)
            {
                return parameterDeclarations;
            }

            return parameterDeclarations.Where(x => !dict.ContainsKey(x.Name));
        }

        private void PostDeployStartTelemetryEvent(string deployId)
        {
            var telemetryEvent = BicepTelemetryEvent.CreateDeployStart(deployId);

            telemetryProvider.PostEvent(telemetryEvent);
        }

        private void PostDeployResultTelemetryEvent(string deployId, bool isSuccess)
        {
            var result = isSuccess ? BicepTelemetryEvent.Result.Succeeded : BicepTelemetryEvent.Result.Failed;
            var telemetryEvent = BicepTelemetryEvent.CreateDeployResult(deployId, result);

            telemetryProvider.PostEvent(telemetryEvent);
        }
    }
}
