// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Bicep.Local.Extension.Rpc;

namespace Bicep.Local.Extension.Protocol;

public record ResourceRequestBody(
    JsonObject? Config,
    string Type,
    JsonObject Properties,
    string? ApiVersion);

public record ResourceReferenceRequestBody(
    JsonObject Identifiers,
    JsonObject? Config,
    string Type,
    string? ApiVersion);

public record ResourceResponseBody(
    ErrorPayload? Error,
    JsonObject Identifiers,
    string Type,
    string Status,
    JsonObject Properties);

public record ErrorPayload(
    string Code,
    string Target,
    string Message,
    ErrorDetail[]? Details,
    JsonObject? InnerError);

public record ErrorDetail(
    string Code,
    string Target,
    string Message);

public interface IGenericResourceHandler
{
    Task<ResourceResponseBody> CreateOrUpdate(
        ResourceRequestBody request,
        CancellationToken cancellationToken);

    Task<ResourceResponseBody> Preview(
        ResourceRequestBody request,
        CancellationToken cancellationToken);

    Task<ResourceResponseBody> Get(
        ResourceReferenceRequestBody request,
        CancellationToken cancellationToken);

    Task<ResourceResponseBody> Delete(
        ResourceReferenceRequestBody request,
        CancellationToken cancellationToken);
}

public interface IResourceHandler : IGenericResourceHandler
{
    string ResourceType { get; }
}
