// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Bicep.Local.Extension.Rpc;

namespace Bicep.Local.Extension.Protocol;

public record Resource(
    string Type,
    string? ApiVersion,
    string? Status,
    JsonObject Identifiers,
    JsonObject? Config,
    JsonObject Properties);

public record ResourceSpecification(
    string Type,
    string? ApiVersion,
    JsonObject Properties,
    JsonObject? Config);

public record ResourceReference(
    string Type,
    string? ApiVersion,
    JsonObject Identifiers,
    JsonObject? Config);

public record ErrorData(
    Error Error);

public record Error(
    string Code,
    string Target,
    string Message,
    ErrorDetail[]? Details,
    JsonObject? InnerError);

public record ErrorDetail(
    string Code,
    string Target,
    string Message);

public record LocalExtensibilityOperationResponse(
    Resource? Resource,
    ErrorData? ErrorData);

public interface IGenericResourceHandler
{
    Task<LocalExtensibilityOperationResponse> CreateOrUpdate(
        ResourceSpecification request,
        CancellationToken cancellationToken);

    Task<LocalExtensibilityOperationResponse> Preview(
        ResourceSpecification request,
        CancellationToken cancellationToken);

    Task<LocalExtensibilityOperationResponse> Get(
        ResourceReference request,
        CancellationToken cancellationToken);

    Task<LocalExtensibilityOperationResponse> Delete(
        ResourceReference request,
        CancellationToken cancellationToken);
}

public interface IResourceHandler : IGenericResourceHandler
{
    string ResourceType { get; }
}
