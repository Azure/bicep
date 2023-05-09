// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.Semantics;
using Bicep.Core.Tracing;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Providers;
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

    public record BicepparamCompilationResult(bool isSuccess, string compilationResult);

    public class BicepDeploymentStartCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeploymentStartParams, BicepDeploymentStartResponse>
    {
        private readonly IDeploymentCollectionProvider deploymentCollectionProvider;
        private readonly IDeploymentOperationsCache deploymentOperationsCache;
        private readonly ITelemetryProvider telemetryProvider;
        private readonly BicepCompiler bicepCompiler;
        private readonly ICompilationManager compilationManager;
        private readonly IArmClientProvider armClientProvider;

        public BicepDeploymentStartCommandHandler(IDeploymentCollectionProvider deploymentCollectionProvider, IDeploymentOperationsCache deploymentOperationsCache, BicepCompiler bicepCompiler, ICompilationManager compilationManager, ISerializer serializer, ITelemetryProvider telemetryProvider, IArmClientProvider armClientProvider)
            : base(LangServerConstants.DeployStartCommand, serializer)
        {
            this.deploymentCollectionProvider = deploymentCollectionProvider;
            this.deploymentOperationsCache = deploymentOperationsCache;
            this.telemetryProvider = telemetryProvider;
            this.bicepCompiler = bicepCompiler;
            this.compilationManager = compilationManager;
            this.armClientProvider = armClientProvider;
        }

        public override async Task<BicepDeploymentStartResponse> Handle(BicepDeploymentStartParams request, CancellationToken cancellationToken)
        {
            PostDeployStartTelemetryEvent(request.deployId);

            var options = new ArmClientOptions();
            options.Diagnostics.ApplySharedResourceManagerSettings();
            options.Environment = new ArmEnvironment(new Uri(request.resourceManagerEndpointUrl), request.audience);

            var credential = new CredentialFromTokenAndTimeStamp(request.token, request.expiresOnTimestamp);
            var armClient = armClientProvider.createArmClient(credential, default, options);

            string? parametersFileJson = null;

            if (PathHelper.HasBicepparamsExension(DocumentUri.FromFileSystemPath(request.parametersFilePath).ToUri()))
            {
                //params file validation 
                if (request.parametersFileUpdateOption != ParametersFileUpdateOption.None)
                {
                    return new BicepDeploymentStartResponse(false, "Cannot create/overwrite/update parameter files when using a bicep parameters file", null);
                }

                if (request.updatedDeploymentParameters.Any())
                {
                    return new BicepDeploymentStartResponse(false, "Cannot update parameters for bicep parameter file", null);
                }

                //params file compilation
                var bicepparamCompilationResult = await TryCompileBicepparamFile(request.parametersFilePath);

                if (!bicepparamCompilationResult.isSuccess)
                {
                    return new BicepDeploymentStartResponse(false, bicepparamCompilationResult.compilationResult, null);
                }
                else
                {
                    parametersFileJson = bicepparamCompilationResult.compilationResult;
                }
            }
            else
            {
                if (request.parametersFileName is { })
                {
                    try
                    {
                        parametersFileJson = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                            request.documentPath,
                            request.parametersFileName,
                            request.parametersFilePath,
                            request.parametersFileUpdateOption,
                            request.updatedDeploymentParameters);
                    }
                    catch (Exception e)
                    {
                        return new BicepDeploymentStartResponse(false, e.Message, null);
                    }
                }
                else
                {
                    return new BicepDeploymentStartResponse(false, "ParametersFileName must be provided with JSON parameters file", null);
                }
            }

            //stringified json for params passed here 
            var bicepDeploymentStartResponse = await DeploymentHelper.StartDeploymentAsync(
                deploymentCollectionProvider,
                armClient,
                request.documentPath,
                request.template,
                request.id,
                request.deploymentScope,
                request.location,
                request.deployId,
                request.portalUrl,
                request.deploymentName,
                JsonElementFactory.CreateElement(parametersFileJson),
                deploymentOperationsCache);

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

        private async Task<BicepparamCompilationResult> TryCompileBicepparamFile(string parametersFilePath)
        {
            var documentUri = DocumentUri.FromFileSystemPath(parametersFilePath);
            var compilation = await new CompilationHelper(bicepCompiler, compilationManager).GetCompilation(documentUri);

            var paramsModel = compilation.GetEntrypointSemanticModel();

            var paramsOutputBuffer = new StringBuilder();
            using var paramsOutputWriter = new StringWriter(paramsOutputBuffer);

            var paramsEmitter = new ParametersEmitter(paramsModel);
            var paramsResult = paramsEmitter.Emit(paramsOutputWriter);

            if (paramsResult.Status == EmitStatus.Failed)
            {
                var fileDiagnosticPair = KeyValuePair.Create(compilation.SourceFileGrouping.EntryPoint, ImmutableArray.Create(paramsResult.Diagnostics.ToArray()));
                return new BicepparamCompilationResult(false, DiagnosticsHelper.GetDiagnosticsMessage(fileDiagnosticPair));
            }

            await paramsOutputWriter.FlushAsync();

            return new BicepparamCompilationResult(true, paramsOutputBuffer.ToString());
        }
    }
}
