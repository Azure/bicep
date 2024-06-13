// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.ErrorResponses;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Types;
using Bicep.LanguageServer.CompilationManager;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Extensibility;
using MediatR;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace Bicep.LanguageServer.Handlers;

[Method("bicep/localDeploy", Direction.ClientToServer)]
public record LocalDeployRequest(TextDocumentIdentifier TextDocument)
    : ITextDocumentIdentifierParams, IRequest<LocalDeployResponse>;

public record LocalDeploymentContent(
    string ProvisioningState,
    ImmutableDictionary<string, JToken> Outputs,
    LocalDeploymentOperationError? Error);

public record LocalDeploymentOperationError(
    string Code,
    string Message,
    string Target);

public record LocalDeploymentOperationContent(
    string ResourceName,
    string ProvisioningState,
    LocalDeploymentOperationError? Error);

public record LocalDeployResponse(
    LocalDeploymentContent Deployment,
    ImmutableArray<LocalDeploymentOperationContent> Operations);

public class LocalDeployHandler : IJsonRpcRequestHandler<LocalDeployRequest, LocalDeployResponse>
{
    private readonly IModuleDispatcher moduleDispatcher;
    private readonly ICompilationManager compilationManager;
    private readonly ILanguageServerFacade server;

    public LocalDeployHandler(IModuleDispatcher moduleDispatcher, ICompilationManager compilationManager, ILanguageServerFacade server)
    {
        this.moduleDispatcher = moduleDispatcher;
        this.compilationManager = compilationManager;
        this.server = server;
    }

    public async Task<LocalDeployResponse> Handle(LocalDeployRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (this.compilationManager.GetCompilation(request.TextDocument.Uri) is not { } context)
            {
                throw new InvalidOperationException("Failed to find active compilation.");
            }

            var paramsModel = context.Compilation.GetEntrypointSemanticModel();
            //Failure scenario is ignored since a diagnostic for it would be emitted during semantic analysis
            if (paramsModel.HasErrors() ||
                !paramsModel.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
            {
                throw new InvalidOperationException("Bicep file had errors.");
            }

            var parameters = context.Compilation.Emitter.Parameters();
            if (parameters.Parameters is not { } parametersString ||
                parameters.Template?.Template is not { } templateString)
            {
                throw new InvalidOperationException("Bicep file had errors.");
            }

            await using LocalExtensibilityHandler extensibilityHandler = new(moduleDispatcher, GrpcExtensibilityProvider.Start);
            await extensibilityHandler.InitializeProviders(context.Compilation);

            var result = await LocalDeployment.Deploy(extensibilityHandler, templateString, parametersString, cancellationToken);

            return FromResult(result);
        }
        catch (Exception ex)
        {
            server.Window.LogError($"Unhandled exception during local deployment: {ex}");
            return new(
                new("Failed", ImmutableDictionary<string, JToken>.Empty, new("UnhandledException", ex.Message, "")),
                []
            );
        }
    }

    private static LocalDeploymentOperationContent FromOperation(DeploymentOperationDefinition operation)
    {
        var result = operation.Properties.StatusMessage.TryFromJToken<OperationResult>();
        var error = result?.Error?.Message.TryFromJson<ErrorResponseMessage>()?.Error;
        var operationError = error is { } ? new LocalDeploymentOperationError(error.Code, error.Message, error.Target) : null;

        return new LocalDeploymentOperationContent(
            operation.Properties.TargetResource.SymbolicName,
            operation.Properties.ProvisioningState.ToString(),
            operationError);
    }

    private static LocalDeployResponse FromResult(LocalDeployment.Result result)
    {
        var deployError = result.Deployment.Properties.Error is { } error ?
            new LocalDeploymentOperationError(error.Code, error.Message, error.Target) : null;


        LocalDeploymentContent deployment = new(
            result.Deployment.Properties.ProvisioningState.ToString() ?? "Failed",
            result.Deployment.Properties.Outputs?.ToImmutableDictionary(x => x.Key, x => x.Value.Value) ?? ImmutableDictionary<string, JToken>.Empty,
            deployError);

        var operations = result.Operations.Select(FromOperation).ToImmutableArray();

        return new(deployment, operations);
    }
}
