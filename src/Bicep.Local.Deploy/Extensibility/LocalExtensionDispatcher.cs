// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Engine.ExtensibilityV2.Contract.Models;
using Azure.Deployments.Engine.Instrumentation;
using Azure.Deployments.Engine.Workers;
using Azure.Deployments.Extensibility.Core.V2.Json;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph.ArtifactReferences;
using Bicep.Core.TypeSystem.Types;
using Bicep.IO.Abstraction;
using Bicep.Local.Deploy.Azure;
using Bicep.Local.Deploy.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using IAsyncDisposable = System.IAsyncDisposable;


namespace Bicep.Local.Deploy.Extensibility;

public class LocalExtensionDispatcher : IAsyncDisposable
{
    private record ExtensionKey(
        string Name,
        string Version);

    private Dictionary<ExtensionKey, ILocalExtension> RegisteredExtensions = new();
    private readonly ILocalExtensionFactory localExtensionFactory;
    private readonly WorkerJobDispatcherClient jobDispatcher;
    private readonly LocalDeploymentEngine localDeploymentEngine;

    public LocalExtensionDispatcher(
        IConfigurationManager configurationManager,
        ILocalExtensionFactory localExtensionFactory,
        IArmDeploymentProvider armDeploymentProvider)
    {
        var services = new ServiceCollection()
            .RegisterLocalDeployServices(this)
            .BuildServiceProvider();

        this.localExtensionFactory = localExtensionFactory;
        this.localDeploymentEngine = services.GetRequiredService<LocalDeploymentEngine>();
        this.jobDispatcher = services.GetRequiredService<WorkerJobDispatcherClient>();
        // Built in extension for handling nested deployments
        RegisteredExtensions[new("LocalNested", "0.0.0")] = new NestedDeploymentExtension(armDeploymentProvider, localDeploymentEngine, configurationManager);
    }

    public async Task<HttpResponseMessage> CallExtensibilityHost(
        LocalDeploymentEngineHost.ExtensionInfo extensionInfo,
        HttpContent content,
        CancellationToken cancellationToken)
    {
        var extension = RegisteredExtensions[new(extensionInfo.ExtensionName, extensionInfo.ExtensionVersion)];

        var response = await CallExtension(extensionInfo.Method, extension, content, cancellationToken);

        // DeploymentEngine performs header validation and expects these two to always be set.
        response.Headers.Add("Location", "local");
        response.Headers.Add("Version", extensionInfo.ExtensionVersion);

        return response;
    }

    private async Task<HttpResponseMessage> CallExtension(
        string method,
        ILocalExtension extensionHost,
        HttpContent content,
        CancellationToken cancellationToken)
    {
        switch (method)
        {
            case "createOrUpdate":
                {
                    var resourceSpecification = await GetResourceSpecificationAsync(await content.ReadAsStreamAsync(cancellationToken), cancellationToken);
                    var extensionResponse = await extensionHost.CreateOrUpdate(resourceSpecification, cancellationToken);

                    return await GetHttpResponseMessageAsync(extensionResponse, cancellationToken);
                }
            case "delete":
                {
                    var resourceReference = await GetResourceReferenceAsync(await content.ReadAsStreamAsync(cancellationToken), cancellationToken);
                    var extensionResponse = await extensionHost.Delete(resourceReference, cancellationToken);

                    return await GetHttpResponseMessageAsync(extensionResponse, cancellationToken);
                }
            case "get":
                {
                    var resourceReference = await GetResourceReferenceAsync(await content.ReadAsStreamAsync(cancellationToken), cancellationToken);
                    var extensionResponse = await extensionHost.Get(resourceReference, cancellationToken);

                    return await GetHttpResponseMessageAsync(extensionResponse, cancellationToken);
                }
            case "preview":
                {
                    var resourceSpecification = await GetResourceSpecificationAsync(await content.ReadAsStreamAsync(cancellationToken), cancellationToken);
                    var extensionResponse = await extensionHost.Preview(resourceSpecification, cancellationToken);

                    return await GetHttpResponseMessageAsync(extensionResponse, cancellationToken);
                }
            default:
                throw new NotImplementedException($"Unsupported method {method}");
        }
    }

    private async Task<ResourceSpecification> GetResourceSpecificationAsync(Stream stream, CancellationToken cancellationToken)
        => await DeserializeAsync(
                    stream,
                    JsonDefaults.SerializerContext.ResourceSpecification,
                    $"Deserializing '{nameof(ResourceSpecification)}' failed. Please ensure the request body contains a valid JSON object.",
                    cancellationToken);

    private async Task<ResourceReference> GetResourceReferenceAsync(Stream stream, CancellationToken cancellationToken)
        => await DeserializeAsync(
                    stream,
                    JsonDefaults.SerializerContext.ResourceReference,
                    $"Deserializing '{nameof(ResourceReference)}' failed. Please ensure the request body contains a valid JSON object.",
                    cancellationToken);

    private async Task<TEntity> DeserializeAsync<TEntity>(Stream stream, JsonTypeInfo<TEntity> typeInfo, string errorMessage, CancellationToken cancellationToken)
        => await JsonSerializer.DeserializeAsync(stream, typeInfo, cancellationToken) ?? throw new ArgumentNullException(errorMessage);

    private async Task<HttpResponseMessage> GetHttpResponseMessageAsync(LocalExtensionOperationResponse extensionResponse, CancellationToken cancellationToken)
    {
        if (extensionResponse.Resource is { } && extensionResponse.ErrorData is { })
        {
            throw new ArgumentException($"Setting '{nameof(LocalExtensionOperationResponse.ErrorData)}' and '{nameof(LocalExtensionOperationResponse.Resource)}' is not valid. Please make sure to set one of these properties.");
        }

        if (extensionResponse.Resource is not { } && extensionResponse.ErrorData is not { })
        {
            throw new ArgumentException($"'{nameof(LocalExtensionOperationResponse.ErrorData)}' and '{nameof(LocalExtensionOperationResponse.Resource)}' cannot be both empty. Please make sure to set one of these properties.");
        }

        var memoryStream = new MemoryStream();
        if (extensionResponse.ErrorData is { })
        {
            await JsonSerializer.SerializeAsync(memoryStream, extensionResponse.ErrorData, JsonDefaults.SerializerContext.ErrorData, cancellationToken);
            memoryStream.Position = 0;
            var streamContent = new StreamContent(memoryStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {
                Content = streamContent
            };
        }
        else if (extensionResponse.Resource is { })
        {
            await JsonSerializer.SerializeAsync(memoryStream, extensionResponse.Resource, JsonDefaults.SerializerContext.Resource, cancellationToken);
            memoryStream.Position = 0;
            var streamContent = new StreamContent(memoryStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = streamContent
            };
        }

        throw new UnreachableException($"Should not reach here, either '{nameof(LocalExtensionOperationResponse.ErrorData)}' or '{nameof(LocalExtensionOperationResponse.Resource)}' should have been set.");
    }

    private IEnumerable<(NamespaceType namespaceType, IOUri binaryUri)> GetBinaryExtensions(Compilation compilation)
    {
        var namespaceTypes = compilation.GetAllBicepModels()
            .Select(x => x.Root.NamespaceResolver)
            .SelectMany(x => x.GetNamespaceNames().Select(x.TryGetNamespace))
            .WhereNotNull();

        foreach (var namespaceType in namespaceTypes)
        {
            if (namespaceType.ExtensionArtifactReference is IExtensionArtifactReference extensionArtifactReference)
            {
                var extensionArtifact = extensionArtifactReference.ResolveExtensionArtifact();

                if (extensionArtifact.BinaryFile.Exists())
                {
                    yield return (namespaceType, extensionArtifact.BinaryFile.Uri);
                }

            }
        }
    }

    public async Task InitializeExtensions(Compilation compilation)
    {
        var binaryExtensions = GetBinaryExtensions(compilation).DistinctBy(x => x.binaryUri);

        foreach (var (namespaceType, binaryUri) in binaryExtensions)
        {
            ExtensionKey extensionKey = new(namespaceType.Settings.TemplateExtensionName, namespaceType.Settings.TemplateExtensionVersion);
            RegisteredExtensions[extensionKey] = await localExtensionFactory.Start(binaryUri);
        }
    }

    public async Task<LocalDeploymentResult> Deploy(string templateString, string parametersString, CancellationToken cancellationToken)
    {
        var name = Guid.NewGuid().ToString();
        await localDeploymentEngine.StartDeployment(name, templateString, parametersString, cancellationToken);

        var result = await localDeploymentEngine.CheckDeployment(name);
        while (result.Deployment.Properties.ProvisioningState?.IsTerminal() != true)
        {
            await Task.Delay(20, cancellationToken);
            result = await localDeploymentEngine.CheckDeployment(name);
        }

        return result;
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(RegisteredExtensions.Values.Select(async extension =>
        {
            try
            {
                await extension.DisposeAsync();
            }
            catch
            {
                // TODO: handle errors shutting down processes gracefully
            }
        }));
        jobDispatcher.Dispose();
    }
}
