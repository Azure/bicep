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
