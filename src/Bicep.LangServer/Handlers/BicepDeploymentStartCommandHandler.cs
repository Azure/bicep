// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Tracing;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepUpdatedDeploymentParameter(string name, string value, bool isSecure, ParameterType? parameterType);

    public record BicepDeploymentStartParams(
        string documentPath,
        string parametersFilePath,
        string id,
        string deploymentScope,
        string location,
        string template,
        string token,
        string expiresOnTimestamp,
        string deployId,
        string deploymentName,
        string portalUrl,
        bool parametersFileExists,
        string? parametersFileName, 
        ParametersFileUpdateOption parametersFileUpdateOption,
        List<BicepUpdatedDeploymentParameter> updatedDeploymentParameters, 
        string resourceManagerEndpointUrl,
        string audience) : IRequest<string>;

    public record BicepDeploymentStartResponse(bool isSuccess, string outputMessage, string? viewDeploymentInPortalMessage);

    public class BicepDeploymentStartCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeploymentStartParams, BicepDeploymentStartResponse>
    {
        private readonly IDeploymentCollectionProvider deploymentCollectionProvider;
        private readonly IDeploymentOperationsCache deploymentOperationsCache;
        private readonly ITelemetryProvider telemetryProvider;
        private readonly BicepCompiler bicepCompiler;
        private readonly ICompilationManager compilationManager;


        public BicepDeploymentStartCommandHandler(IDeploymentCollectionProvider deploymentCollectionProvider, IDeploymentOperationsCache deploymentOperationsCache, BicepCompiler bicepCompiler, ICompilationManager compilationManager, ISerializer serializer, ITelemetryProvider telemetryProvider)
            : base(LangServerConstants.DeployStartCommand, serializer)
        {
            this.deploymentCollectionProvider = deploymentCollectionProvider;
            this.deploymentOperationsCache = deploymentOperationsCache;
            this.telemetryProvider = telemetryProvider;
            this.bicepCompiler = bicepCompiler;
            this.compilationManager = compilationManager;
        }

        public override async Task<BicepDeploymentStartResponse> Handle(BicepDeploymentStartParams request, CancellationToken cancellationToken)
        {
            PostDeployStartTelemetryEvent(request.deployId);

            var options = new ArmClientOptions();
            options.Diagnostics.ApplySharedResourceManagerSettings();
            options.Environment = new ArmEnvironment(new Uri(request.resourceManagerEndpointUrl), request.audience);

            var credential = new CredentialFromTokenAndTimeStamp(request.token, request.expiresOnTimestamp);
            var armClient = new ArmClient(credential, default, options);

            string? bicepparamJsonOutput = null;

            if(request.parametersFilePath.EndsWith(".bicepparam"))
            {
                //params file validation 
                if(request.parametersFileUpdateOption != ParametersFileUpdateOption.None)
                {
                    return new BicepDeploymentStartResponse(false, "Can not create/overwrite/update parameter files when using a bicep parameters file", null);
                }

                if(request.updatedDeploymentParameters.Any())
                {
                    return new BicepDeploymentStartResponse(false, "Can not update parameters for bicep parameter file", null);
                }

                //params file compilation
                var documentUri = DocumentUri.FromFileSystemPath(request.parametersFilePath);
                var compilation = await new CompilationHelper(bicepCompiler, compilationManager).GetCompilation(documentUri);
                
                var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile()
                .FirstOrDefault(x => x.Key.FileUri == documentUri.ToUri());
                
                if (diagnosticsByFile.Value.Any(x => x.Level == DiagnosticLevel.Error))
                {
                   return new BicepDeploymentStartResponse(false, "Bicep build failed. Please fix below errors:\n" + DiagnosticsHelper.GetDiagnosticsMessage(diagnosticsByFile), null);
                };
                
                bicepparamJsonOutput = GetParamsJsonOutput(compilation);
            } 
            
            var bicepDeploymentStartResponse = await DeploymentHelper.StartDeploymentAsync(
                deploymentCollectionProvider,
                armClient,
                request.documentPath,
                request.template,
                request.parametersFilePath,
                request.id,
                request.deploymentScope,
                request.location,
                request.deployId,
                request.parametersFileName,
                request.parametersFileUpdateOption,
                request.updatedDeploymentParameters,
                request.portalUrl,
                request.deploymentName,
                deploymentOperationsCache,
                bicepparamJsonOutput);

            PostDeployStartResultTelemetryEvent(request.deployId, bicepDeploymentStartResponse.isSuccess);

            return bicepDeploymentStartResponse;
        }

        private void PostDeployStartTelemetryEvent(string deployId)
        {
            var telemetryEvent = BicepTelemetryEvent.CreateDeployStart(deployId);

            telemetryProvider.PostEvent(telemetryEvent);
        }

        private void PostDeployStartResultTelemetryEvent(string deployId, bool isSuccess)
        {
            var telemetryEvent = BicepTelemetryEvent.CreateDeployStartOrWaitForCompletionResult(
                TelemetryConstants.EventNames.DeployStartResult,
                deployId,
                isSuccess);

            telemetryProvider.PostEvent(telemetryEvent);
        }

        private string GetParamsJsonOutput(Compilation compilation)
        {
            var paramsModel = compilation.GetEntrypointSemanticModel();

            var paramsOutputBuffer = new StringBuilder();
            using var paramsOutputWriter = new StringWriter(paramsOutputBuffer);

            var paramsEmitter = new ParametersEmitter(paramsModel);
            var paramsResult = paramsEmitter.Emit(paramsOutputWriter);

            paramsOutputWriter.Flush();
            
            return paramsOutputBuffer.ToString();
        }
    }
}
