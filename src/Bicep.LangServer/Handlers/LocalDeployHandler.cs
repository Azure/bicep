// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Bicep.LocalDeploy;
using Azure.Bicep.LocalDeploy.Extensibility;
using Bicep.Core.Emit;
using Bicep.Core.Semantics.Namespaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LanguageServer.Handlers;

[Method("bicep/localDeploy", Direction.ClientToServer)]
public record LocalDeployRequest(TextDocumentIdentifier TextDocument)
    : ITextDocumentIdentifierParams, IRequest<LocalDeployResponse>;

public record LocalDeploymentContent(
    string ProvisioningState);

public record LocalDeploymentOperationContent(
    string ResourceName,
    string ProvisioningState);

public record LocalDeployResponse(
    LocalDeploymentContent Deployment,
    ImmutableArray<LocalDeploymentOperationContent> Operations);

public class LocalDeployHandler : IJsonRpcRequestHandler<LocalDeployRequest, LocalDeployResponse>
{
    private readonly ICompilationManager compilationManager;

    public LocalDeployHandler(ICompilationManager compilationManager)
    {
        this.compilationManager = compilationManager;
    }

    public async Task<LocalDeployResponse> Handle(LocalDeployRequest request, CancellationToken cancellationToken)
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

        using var paramsWriter = new StringWriter();
        new ParametersEmitter(paramsModel).Emit(paramsWriter);
        var parametersString = paramsWriter.ToString();

        using var templateWriter = new StringWriter();
        new TemplateEmitter(paramsModel.Compilation, usingModel).Emit(templateWriter);
        var templateString = templateWriter.ToString();

        var extensibilityHandler = new LocalExtensibilityHandler();
        extensibilityHandler.Register(UtilsNamespaceType.Settings.ArmTemplateProviderName, UtilsNamespaceType.Settings.ArmTemplateProviderVersion, () => new UtilsExtensibilityProvider());
        extensibilityHandler.Register(K8sNamespaceType.Settings.ArmTemplateProviderName, K8sNamespaceType.Settings.ArmTemplateProviderVersion, () => new K8sExtensibilityProvider());

        var result = await LocalDeployment.Deploy(extensibilityHandler, templateString, parametersString, cancellationToken);

        return FromResult(result);
    }

    private static LocalDeployResponse FromResult(LocalDeployment.Result result)
    {
        LocalDeploymentContent deployment = new(
            result.Deployment.Properties.ProvisioningState?.ToString() ?? "");

        var operations = result.Operations.Select(operation => new LocalDeploymentOperationContent(
            operation.Properties.TargetResource.SymbolicName,
            operation.Properties.ProvisioningState.ToString()));

        return new(deployment, operations.ToImmutableArray());
    }
}