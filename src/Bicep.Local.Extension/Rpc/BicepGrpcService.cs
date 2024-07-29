// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Bicep.Local.Extension.Protocol;
using Google.Protobuf.Collections;
using Grpc.Core;
using Json.More;
using Microsoft.Extensions.Logging;

namespace Bicep.Local.Extension.Rpc
{
    public class BicepGrpcService : BicepExtension.BicepExtensionBase
    {
        private readonly ILogger<BicepGrpcService> logger;
        private readonly LocalExtensionDispatcher dispatcher;

        public BicepGrpcService(ILogger<BicepGrpcService> logger, LocalExtensionDispatcher dispatcher)
        {
            this.logger = logger;
            this.dispatcher = dispatcher;
        }

        public override Task<LocalExtensibilityOperationResponse> CreateOrUpdate(ResourceSpecification request, ServerCallContext context)
            => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Type).CreateOrUpdate(Convert(request), context.CancellationToken)));

        public override Task<LocalExtensibilityOperationResponse> Preview(ResourceSpecification request, ServerCallContext context)
            => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Type).Preview(Convert(request), context.CancellationToken)));

        public override Task<LocalExtensibilityOperationResponse> Get(ResourceReference request, ServerCallContext context)
            => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Type).Get(Convert(request), context.CancellationToken)));

        public override Task<LocalExtensibilityOperationResponse> Delete(ResourceReference request, ServerCallContext context)
            => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Type).Delete(Convert(request), context.CancellationToken)));

        public override Task<Empty> Ping(Empty request, ServerCallContext context)
            => Task.FromResult(new Empty());

        private Azure.Deployments.Extensibility.Core.V2.Models.ResourceSpecification Convert(ResourceSpecification request)
            => new(request.Type, request.ApiVersion, ToJsonObject(request.Properties, "Parsing extension properties failed. Please ensure is a valid JSON object."), GetExtensionConfig(request.Config));

        private Azure.Deployments.Extensibility.Core.V2.Models.ResourceReference Convert(ResourceReference request)
            => new(request.Type, request.ApiVersion, ToJsonObject(request.Identifiers, "Parsing extension identifiers failed. Please ensure is a valid JSON object."), GetExtensionConfig(request.Config));

        private JsonObject? GetExtensionConfig(string extensionConfig)
        {
            JsonObject? config = null;
            if (!string.IsNullOrEmpty(extensionConfig))
            {
                config = ToJsonObject(extensionConfig, "Parsing extension config failed. Please ensure is a valid JSON object.");
            }
            return config;
        }

        private JsonObject ToJsonObject(string json, string errorMessage)
            => JsonNode.Parse(json)?.AsObject() ?? throw new ArgumentNullException(errorMessage);

        private Resource? Convert(Azure.Deployments.Extensibility.Core.V2.Models.Resource? response)
            => response is null ? null :
                new()
                {
                    Identifiers = response.Identifiers.ToJsonString(),
                    Properties = response.Properties.ToJsonString(),
                    Status = response.Status,
                    Type = response.Type,
                    ApiVersion = response.ApiVersion,
                };

        private ErrorData? Convert(Azure.Deployments.Extensibility.Core.V2.Models.ErrorData? response)
        {
            if (response is null)
            {
                return null;
            }

            var errorData = new ErrorData()
            {
                Error = new Error()
                {
                    Code = response.Error.Code,
                    Message = response.Error.Message,
                    InnerError = response.Error.InnerError?.ToJsonString(),
                    Target = response.Error.Target?.ToString() ?? string.Empty,
                }
            };

            var errorDetails = Convert(response.Error.Details);
            if (errorDetails is not null)
            {
                errorData.Error.Details.AddRange(errorDetails);
            }
            return errorData;
        }

        private RepeatedField<ErrorDetail>? Convert(IList<Azure.Deployments.Extensibility.Core.V2.Models.ErrorDetail>? response)
        {
            if (response is null)
            {
                return null;
            }

            var list = new RepeatedField<ErrorDetail>();
            foreach (var item in response)
            {
                list.Add(Convert(item));
            }
            return list;
        }

        private ErrorDetail Convert(Azure.Deployments.Extensibility.Core.V2.Models.ErrorDetail response)
            => new()
            {
                Code = response.Code,
                Message = response.Message,
                Target = response.Target?.ToString() ?? string.Empty,
            };

        private LocalExtensibilityOperationResponse Convert(LocalExtensionOperationResponse response)
            => new()
            {
                ErrorData = Convert(response.ErrorData),
                Resource = Convert(response.Resource)
            };

        private static async Task<LocalExtensibilityOperationResponse> WrapExceptions(Func<Task<LocalExtensibilityOperationResponse>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                var response = new LocalExtensibilityOperationResponse
                {
                    Resource = null,
                    ErrorData = new ErrorData
                    {
                        Error = new Error
                        {
                            Message = $"Rpc request failed: {ex}",
                            Code = "RpcException",
                            Target = ""
                        }
                    }
                };

                return response;
            }
        }
    }
}
