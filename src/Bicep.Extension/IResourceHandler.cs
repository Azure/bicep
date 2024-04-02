// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Bicep.Extension.Rpc;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;

namespace Bicep.Extension;

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
