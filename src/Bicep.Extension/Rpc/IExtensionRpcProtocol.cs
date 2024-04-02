// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;

namespace Bicep.Extension.Rpc;

public record ExtensibilityOperationRequest(
    ExtensibleImportData Import,
    ExtensibleResourceData Resource);

public record ExtensibilityOperationResponse(
    ExtensibleResourceData? Resource,
    ExtensibleResourceMetadata? ResourceMetadata,
    ImmutableArray<ExtensibilityError>? Errors);

public record ExtensibleImportData(
    string Provider,
    string Version,
    JObject? Config);

public record ExtensibleResourceData(
    string Type,
    JObject? Properties);

public record ExtensibleResourceMetadata(
    ImmutableArray<string>? ReadOnlyProperties,
    ImmutableArray<string>? ImmutableProperties,
    ImmutableArray<string>? DynamicProperties);

public record ExtensibilityError(
    string Code,
    string Message,
    string Target);
/*
[JsonSerializable(typeof(ExtensibilityOperationRequest))]
[JsonSerializable(typeof(ExtensibilityOperationResponse))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class ExtensibilitySerializerContext : JsonSerializerContext { }
*/
public interface IExtensionRpcProtocol
{
    [JsonRpcMethod("extension/save", UseSingleObjectParameterDeserialization = true)]
    Task<ExtensibilityOperationResponse> Save(ExtensibilityOperationRequest request, CancellationToken cancellationToken);

    [JsonRpcMethod("extension/previewSave", UseSingleObjectParameterDeserialization = true)]
    Task<ExtensibilityOperationResponse> PreviewSave(ExtensibilityOperationRequest request, CancellationToken cancellationToken);

    [JsonRpcMethod("extension/get", UseSingleObjectParameterDeserialization = true)]
    Task<ExtensibilityOperationResponse> Get(ExtensibilityOperationRequest request, CancellationToken cancellationToken);

    [JsonRpcMethod("extension/delete", UseSingleObjectParameterDeserialization = true)]
    Task<ExtensibilityOperationResponse> Delete(ExtensibilityOperationRequest request, CancellationToken cancellationToken);
}