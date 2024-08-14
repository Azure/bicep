// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Azure.Deployments.Extensibility.Messages;

namespace Bicep.Local.Deploy.Extensibility;

public record LocalExtensibilityOperationResponse(Resource? Resource, ErrorData? ErrorData);

[JsonSerializable(typeof(LocalExtensibilityOperationResponse))]
public partial class LocalExtensibilityOperationResponseContext : JsonSerializerContext { }

public static class LocalExtensibilityOperationResponseJsonDefaults
{
    public readonly static JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public readonly static LocalExtensibilityOperationResponseContext SerializerContext = new(SerializerOptions);
}

public abstract class LocalExtensibilityHost : IAsyncDisposable
{
    public abstract Task<LocalExtensibilityOperationResponse> Delete(ResourceReference request, CancellationToken cancellationToken);

    public abstract Task<LocalExtensibilityOperationResponse> Get(ResourceReference request, CancellationToken cancellationToken);

    public abstract Task<LocalExtensibilityOperationResponse> Preview(ResourceSpecification request, CancellationToken cancellationToken);

    public abstract Task<LocalExtensibilityOperationResponse> CreateOrUpdate(ResourceSpecification request, CancellationToken cancellationToken);

    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
