// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Nodes;
using Bicep.Local.Extension.Host;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types.Attributes;
using Moq;

namespace Bicep.Local.Deploy.IntegrationTests;

public class MockResourceHandler : TypedResourceHandler<MockResourceHandler.Properties, MockResourceHandler.Identifiers, MockResourceHandler.Config>
{
    [ResourceType("apps/Deployment", "v1")]
    public class Properties
    {
        public required Metadata Metadata { get; set; }

        public required JsonObject Spec { get; set; }
    }

    public class Metadata
    {
        public required string Name { get; set; }

        public string? Namespace { get; set; }
    }

    public class Identifiers
    {
        public required Metadata Metadata { get; set; }
    }

    public class Config
    {
        public required string KubeConfig { get; set; }
        public string? Namespace { get; set; }
    }

    public Func<ResourceRequest, CancellationToken, ResourceResponse>? OnCreateOrUpdate { get; set; }

    protected override async Task<ResourceResponse> CreateOrUpdate(ResourceRequest request, CancellationToken cancellationToken)
    {
        if (OnCreateOrUpdate is null)
        {
            return await base.CreateOrUpdate(request, cancellationToken);
        }

        return OnCreateOrUpdate(request, cancellationToken);
    }

    public Func<ResourceRequest, CancellationToken, ResourceResponse>? OnPreview { get; set; }

    protected override async Task<ResourceResponse> Preview(ResourceRequest request, CancellationToken cancellationToken)
    {
        if (OnPreview is null)
        {
            return await base.Preview(request, cancellationToken);
        }

        return OnPreview(request, cancellationToken);
    }

    public Func<ReferenceRequest, CancellationToken, ResourceResponse>? OnGet { get; set; }

    protected override async Task<ResourceResponse> Get(ReferenceRequest reference, CancellationToken cancellationToken)
    {
        if (OnGet is null)
        {
            return await base.Get(reference, cancellationToken);
        }

        return OnGet(reference, cancellationToken);
    }

    public Func<ReferenceRequest, CancellationToken, ResourceResponse>? OnDelete { get; set; }

    protected override async Task<ResourceResponse> Delete(ReferenceRequest reference, CancellationToken cancellationToken)
    {
        if (OnDelete is { })
        {
            return OnDelete(reference, cancellationToken);
        }

        return await base.Delete(reference, cancellationToken);
    }

    protected override Identifiers GetIdentifiers(Properties properties)
        => new()
        {
            Metadata = properties.Metadata
        };
}
