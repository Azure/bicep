// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Azure.Deployments.Extensibility.Core.V2.Json;
using Bicep.Local.Extension.Protocol;

namespace Bicep.Local.Deploy.Extensibility
{
    public class LocalExtensionHost : ILocalExtensionHost
    {
        private readonly ILocalExtensionFactoryManager extensionFactoryManager;

        public LocalExtensionHost(ILocalExtensionFactoryManager extensionFactoryManager)
        {
            this.extensionFactoryManager = extensionFactoryManager;
        }

        public async Task<HttpResponseMessage> CallExtensibilityHost(LocalDeploymentEngineHost.ExtensionInfo extensionInfo, HttpContent content, CancellationToken cancellationToken)
        {
            var extension = await extensionFactoryManager.GetLocalExtensionAsync(extensionInfo.ExtensionName, extensionInfo.ExtensionVersion);
            var response = await CallExtension(extensionInfo.Method, extension, content, cancellationToken);

            // DeploymentEngine performs header validation and expects these two to always be set.
            response.Headers.Add("Location", "local");
            response.Headers.Add("Version", extensionInfo.ExtensionVersion);

            return response;
        }

        private async Task<HttpResponseMessage> CallExtension(string method, ILocalExtension provider, HttpContent content, CancellationToken cancellationToken)
        {
            switch (method)
            {
                case "createOrUpdate":
                    {
                        var resourceSpecification = await GetResourceSpecificationAsync(await content.ReadAsStreamAsync(cancellationToken), cancellationToken);
                        var extensionResponse = await provider.CreateOrUpdate(resourceSpecification, cancellationToken);

                        return await GetHttpResponseMessageAsync(extensionResponse, cancellationToken);
                    }
                case "delete":
                    {
                        var resourceReference = await GetResourceReferenceAsync(await content.ReadAsStreamAsync(cancellationToken), cancellationToken);
                        var extensionResponse = await provider.Delete(resourceReference, cancellationToken);

                        return await GetHttpResponseMessageAsync(extensionResponse, cancellationToken);
                    }
                case "get":
                    {
                        var resourceReference = await GetResourceReferenceAsync(await content.ReadAsStreamAsync(cancellationToken), cancellationToken);
                        var extensionResponse = await provider.Delete(resourceReference, cancellationToken);

                        return await GetHttpResponseMessageAsync(extensionResponse, cancellationToken);
                    }
                case "preview":
                    {
                        var resourceSpecification = await GetResourceSpecificationAsync(await content.ReadAsStreamAsync(cancellationToken), cancellationToken);
                        var extensionResponse = await provider.CreateOrUpdate(resourceSpecification, cancellationToken);

                        return await GetHttpResponseMessageAsync(extensionResponse, cancellationToken);
                    }
                default:
                    throw new NotImplementedException($"Unsupported method {method}");
            }
        }

        private async Task<Azure.Deployments.Extensibility.Core.V2.Models.ResourceSpecification> GetResourceSpecificationAsync(Stream stream, CancellationToken cancellationToken)
        => await DeserializeAsync(
                    stream,
                    JsonDefaults.SerializerContext.ResourceSpecification,
                    $"Deserializing '{nameof(global::Azure.Deployments.Extensibility.Core.V2.Models.ResourceSpecification)}' failed. Please ensure the request body contains a valid JSON object.",
                    cancellationToken);

        private async Task<Azure.Deployments.Extensibility.Core.V2.Models.ResourceReference> GetResourceReferenceAsync(Stream stream, CancellationToken cancellationToken)
            => await DeserializeAsync(
                        stream,
                        JsonDefaults.SerializerContext.ResourceReference,
                        $"Deserializing '{nameof(Azure.Deployments.Extensibility.Core.V2.Models.ResourceReference)}' failed. Please ensure the request body contains a valid JSON object.",
                        cancellationToken);

        private async Task<TEntity> DeserializeAsync<TEntity>(Stream stream, JsonTypeInfo<TEntity> typeInfo, string errorMessage, CancellationToken cancellationToken)
            => await JsonSerializer.DeserializeAsync(stream, typeInfo, cancellationToken) ?? throw new ArgumentNullException(errorMessage);

        private async Task<HttpResponseMessage> GetHttpResponseMessageAsync(LocalExtensionOperationResponse extensionResponse, CancellationToken cancellationToken)
        {
            if (extensionResponse.Resource is { } && extensionResponse.ErrorData is { })
            {
                throw new ArgumentException($"Setting '{nameof(LocalExtensibilityOperationResponse.ErrorData)}' and '{nameof(LocalExtensibilityOperationResponse.Resource)}' is not valid. Please make sure to set one of these properties.");
            }

            if (extensionResponse.Resource is not { } && extensionResponse.ErrorData is not { })
            {
                throw new ArgumentException($"'{nameof(LocalExtensibilityOperationResponse.ErrorData)}' and '{nameof(LocalExtensibilityOperationResponse.Resource)}' cannot be both empty. Please make sure to set one of these properties.");
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

            throw new UnreachableException($"Should not reach here, either '{nameof(LocalExtensibilityOperationResponse.ErrorData)}' or '{nameof(LocalExtensibilityOperationResponse.Resource)}' should have been set.");
        }
    }
}
