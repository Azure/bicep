// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.Tracing;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;

namespace Bicep.Cli.Rpc;

public class RpcServer
{
#pragma warning disable IDE1006
    public record CompileRequest(
        string path);

    public record CompileResponse(
        bool success,
        ImmutableArray<CompileResponseDiagnostic> diagnostics,
        string? contents);

    public record CompileResponseDiagnostic(
        int line,
        int @char,
        string level,
        string code,
        string message);

    public record ValidateRequest(
        string subscriptionId,
        string resourceGroup,
        string path);

    public record ValidateResponse(
        ValidateResponseError? error);

    public record ValidateResponseError(
        string code,
        string message,
        string? target,
        ValidateResponseError[]? details);
#pragma warning restore IDE1006

    private readonly BicepCompiler compiler;
    private readonly ITokenCredentialFactory credentialFactory;

    public RpcServer(BicepCompiler compiler, ITokenCredentialFactory credentialFactory)
    {
        this.compiler = compiler;
        this.credentialFactory = credentialFactory;
    }

    [JsonRpcMethod("bicep/compile", UseSingleObjectParameterDeserialization = true)]
    public async Task<CompileResponse> Compile(CompileRequest request, CancellationToken cancellationToken)
    {
        var inputUri = PathHelper.FilePathToFileUrl(request.path);
        if (!PathHelper.HasBicepExtension(inputUri) &&
            !PathHelper.HasBicepparamsExension(inputUri))
        {
            throw new InvalidOperationException("Cannot compile");
        }

        var compilation = await compiler.CreateCompilation(inputUri, new Workspace());
        var model = compilation.GetEntrypointSemanticModel();
        var diagnostics = GetDiagnostics(compilation).ToImmutableArray();

        if (model.HasErrors())
        {
            return new(false, diagnostics, null);
        }

        var writer = new StringWriter();
        if (PathHelper.HasBicepparamsExension(inputUri))
        {
            new ParametersEmitter(model).Emit(writer);
        }
        else
        {
            new TemplateEmitter(model).Emit(writer);
        }

        return new(true, diagnostics, writer.ToString());
    }

    [JsonRpcMethod("bicep/validate", UseSingleObjectParameterDeserialization = true)]
    public async Task<ValidateResponse> Validate(ValidateRequest request, CancellationToken cancellationToken)
    {
        var inputUri = PathHelper.FilePathToFileUrl(request.path);
        if (!PathHelper.HasBicepparamsExension(inputUri))
        {
            throw new InvalidOperationException("Cannot compile");
        }

        var compilation = await compiler.CreateCompilation(inputUri, new Workspace());
        var model = compilation.GetEntrypointSemanticModel();
        var diagnostics = GetDiagnostics(compilation).ToImmutableArray();

        if (model.HasErrors() ||
            !model.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel) ||
            usingModel is not SemanticModel bicepModel)
        {
            // TODO support ts & br
            throw new InvalidOperationException();
        }

        var paramsWriter = new StringWriter();
        new ParametersEmitter(model).Emit(paramsWriter);

        var templateWriter = new StringWriter();
        new TemplateEmitter(bicepModel).Emit(templateWriter);

        var configuration = model.Configuration;

        var parameters = JsonDocument.Parse(paramsWriter.ToString()).RootElement.GetProperty("parameters");

        var deploymentProperties = new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
        {
            Template = BinaryData.FromString(templateWriter.ToString()),
            Parameters = BinaryData.FromObjectAsJson(parameters),
        };

        var armDeploymentContent = new ArmDeploymentContent(deploymentProperties);

        var options = new ArmClientOptions();
        options.Diagnostics.ApplySharedResourceManagerSettings();
        options.Environment = new ArmEnvironment(configuration.Cloud.ResourceManagerEndpointUri, configuration.Cloud.AuthenticationScope);

        var credential = credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.CredentialOptions, configuration.Cloud.ActiveDirectoryAuthorityUri);
        var armClient = new ArmClient(credential, request.subscriptionId, options);

        var deploymentScope = $"/subscriptions/{request.subscriptionId}/resourceGroups/{request.resourceGroup}";
        var deploymentName = "validate-test";
        var deploymentId = ArmDeploymentResource.CreateResourceIdentifier(deploymentScope, deploymentName);
        var armDeployment = armClient.GetArmDeploymentResource(deploymentId);

        try
        {
            var response = await armDeployment.ValidateAsync(WaitUntil.Completed, armDeploymentContent, cancellationToken);

            return new(null);
        }
        catch (RequestFailedException ex) when (ex.GetRawResponse() is {} rawResponse)
        {
            var error = rawResponse.Content.ToString()
                .FromJson<JToken>()
                .TryGetProperty<JObject>("error");
                
            return new(GetErrorRecursive(error));
        }
    }

    private static ValidateResponseError GetErrorRecursive(JToken error)
    {
        var code = error.TryGetProperty<string>("code");
        var message = error.TryGetProperty<string>("message");
        var target = error.TryGetProperty<string>("target");
        var details = error.TryGetProperty<JArray>("details")?.Select(GetErrorRecursive).ToArray();

        return new(code, message, target, details);
    }

    private static IEnumerable<CompileResponseDiagnostic> GetDiagnostics(Compilation compilation)
    {
        foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
        {
            foreach (var diagnostic in diagnostics)
            {
                (int line, int character) = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, diagnostic.Span.Position);
                yield return new(line, character, diagnostic.Level.ToString(), diagnostic.Code, diagnostic.Message);
            }
        }
    }
}