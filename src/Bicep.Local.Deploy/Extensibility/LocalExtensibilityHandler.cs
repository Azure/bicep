// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Azure.Deployments.Engine.Host.Azure.ExtensibilityV2.Contract.Models;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Messages;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Microsoft.WindowsAzure.ResourceStack.Common.Utilities;
using IAsyncDisposable = System.IAsyncDisposable;

namespace Bicep.Local.Deploy.Extensibility;

public class LocalExtensibilityHandler : IAsyncDisposable
{
    private record ProviderKey(
        string Name,
        string Version);

    private Dictionary<ProviderKey, LocalExtensibilityProvider> RegisteredProviders = new();
    private readonly IModuleDispatcher moduleDispatcher;
    private readonly Func<Uri, Task<LocalExtensibilityProvider>> providerFactory;

    public LocalExtensibilityHandler(IModuleDispatcher moduleDispatcher, Func<Uri, Task<LocalExtensibilityProvider>> providerFactory)
    {
        this.moduleDispatcher = moduleDispatcher;
        this.providerFactory = providerFactory;
        // Built in provider for handling nested deployments
        RegisteredProviders[new("LocalNested", "0.0.0")] = new AzExtensibilityProvider(this);
    }

    public async Task<ResourceResponseBody> CallExtensibilityHost(
        string extensionName,
        string extensionVersion,
        string method,
        HttpContent content,
        CancellationToken cancellationToken)
    {
        var provider = RegisteredProviders[new(extensionName, extensionVersion)];

        return await CallProvider(method, provider, content, cancellationToken);
    }

    private async Task<ResourceResponseBody> CallProvider(
        string method,
        LocalExtensibilityProvider provider,
        HttpContent content,
        CancellationToken cancellationToken)
    {
        return method switch
        {
            "get" => await provider.Get(await content.ReadAsAsync<ResourceReferenceRequestBody>(cancellationToken), cancellationToken),
            "delete" => await provider.Delete(await content.ReadAsAsync<ResourceReferenceRequestBody >(cancellationToken), cancellationToken),
            "createOrUpdate" => await provider.CreateOrUpdate(await content.ReadAsAsync<ResourceRequestBody>(cancellationToken), cancellationToken),
            "preview" => await provider.Preview(await content.ReadAsAsync<ResourceRequestBody>(cancellationToken), cancellationToken),
            _ => throw new NotImplementedException($"Unsupported method {method}"),
        };
    }

    private IEnumerable<(NamespaceType namespaceType, Uri binaryUri)> GetBinaryProviders(Compilation compilation)
    {
        var namespaceTypes = compilation.GetAllBicepModels()
            .Select(x => x.Root.NamespaceResolver)
            .SelectMany(x => x.GetNamespaceNames().Select(x.TryGetNamespace))
            .WhereNotNull();

        foreach (var namespaceType in namespaceTypes)
        {
            if (namespaceType.Artifact is { } artifact &&
                moduleDispatcher.TryGetProviderBinary(artifact) is { } binaryUri)
            {
                yield return (namespaceType, binaryUri);
            }
        }
    }

    public async Task InitializeProviders(Compilation compilation)
    {
        var binaryProviders = GetBinaryProviders(compilation).DistinctBy(x => x.binaryUri);

        foreach (var (namespaceType, binaryUri) in binaryProviders)
        {
            ProviderKey providerKey = new(namespaceType.Settings.ArmTemplateProviderName, namespaceType.Settings.ArmTemplateProviderVersion);
            RegisteredProviders[providerKey] = await providerFactory(binaryUri);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(RegisteredProviders.Values.Select(async provider =>
        {
            try
            {
                await provider.DisposeAsync();
            }
            catch
            {
                // TODO: handle errors shutting down processes gracefully
            }
        }));
    }
}
