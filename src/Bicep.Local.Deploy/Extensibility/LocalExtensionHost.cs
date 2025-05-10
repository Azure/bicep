// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Azure.Deployments.Extensibility.Messages;

namespace Bicep.Local.Deploy.Extensibility;

public record LocalExtensionOperationResponse(Resource? Resource, ErrorData? ErrorData);

[JsonSerializable(typeof(LocalExtensionOperationResponse))]
public partial class LocalExtensionOperationResponseContext : JsonSerializerContext { }

public static class LocalExtensionOperationResponseJsonDefaults
{
    public readonly static JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public readonly static LocalExtensionOperationResponseContext SerializerContext = new(SerializerOptions);
}

public abstract class LocalExtensionHost : IAsyncDisposable
{
    public abstract Task<LocalExtensionOperationResponse> Delete(ResourceReference request, CancellationToken cancellationToken);

    public abstract Task<LocalExtensionOperationResponse> Get(ResourceReference request, CancellationToken cancellationToken);

    public abstract Task<LocalExtensionOperationResponse> Preview(ResourceSpecification request, CancellationToken cancellationToken);

    public abstract Task<LocalExtensionOperationResponse> CreateOrUpdate(ResourceSpecification request, CancellationToken cancellationToken);

    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
