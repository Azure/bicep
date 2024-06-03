// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace Bicep.Local.Extension.Protocol;

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
    JsonObject? Config);

public record ExtensibleResourceData(
    string Type,
    JsonObject? Properties);

public record ExtensibleResourceMetadata(
    ImmutableArray<string>? ReadOnlyProperties,
    ImmutableArray<string>? ImmutableProperties,
    ImmutableArray<string>? DynamicProperties);

public record ExtensibilityError(
    string Code,
    string Message,
    string Target);

public interface IGenericResourceHandler
{
    Task<ExtensibilityOperationResponse> Save(
        ExtensibilityOperationRequest request,
        CancellationToken cancellationToken);

    Task<ExtensibilityOperationResponse> PreviewSave(
        ExtensibilityOperationRequest request,
        CancellationToken cancellationToken);

    Task<ExtensibilityOperationResponse> Get(
        ExtensibilityOperationRequest request,
        CancellationToken cancellationToken);

    Task<ExtensibilityOperationResponse> Delete(
        ExtensibilityOperationRequest request,
        CancellationToken cancellationToken);
}

public interface IResourceHandler : IGenericResourceHandler
{
    string ResourceType { get; }
}
