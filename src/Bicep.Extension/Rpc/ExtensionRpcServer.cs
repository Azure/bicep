// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json.Serialization;
using StreamJsonRpc;

namespace Bicep.Extension.Rpc;

public class ExtensionRpcServer : IExtensionRpcProtocol
{
    private readonly ResourceDispatcher dispatcher;

    public ExtensionRpcServer(ResourceDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    public static IJsonRpcMessageHandler CreateMessageHandler(Stream inputStream, Stream outputStream)
    {
        var formatter = new JsonMessageFormatter();
        formatter.JsonSerializer.ContractResolver = new CamelCasePropertyNamesContractResolver();

        return new HeaderDelimitedMessageHandler(inputStream, outputStream, formatter);
    }
    
    public Task<ExtensibilityOperationResponse> Save(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
        => WrapExceptions(async () => await dispatcher.GetHandler(request.Resource.Type).Save(request, cancellationToken));

    public Task<ExtensibilityOperationResponse> PreviewSave(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
        => WrapExceptions(async () => await dispatcher.GetHandler(request.Resource.Type).PreviewSave(request, cancellationToken));

    public Task<ExtensibilityOperationResponse> Get(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
        => WrapExceptions(async () => await dispatcher.GetHandler(request.Resource.Type).Get(request, cancellationToken));

    public Task<ExtensibilityOperationResponse> Delete(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
        => WrapExceptions(async () => await dispatcher.GetHandler(request.Resource.Type).Delete(request, cancellationToken));

    private static async Task<ExtensibilityOperationResponse> WrapExceptions(Func<Task<ExtensibilityOperationResponse>> func)
    {
        try 
        {
            return await func();
        }
        catch (Exception ex)
        {
            return new ExtensibilityOperationResponse(
                null,
                null,
                [new("RpcException", $"Rpc request failed: {ex}", "")]);
        }
    }
}
