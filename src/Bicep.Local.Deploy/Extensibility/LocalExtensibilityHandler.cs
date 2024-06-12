// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Messages;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Types;
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

    private async Task<ExtensibilityOperationResponse> CallProvider(string method, IExtensibilityProvider provider, ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return method switch
        {
            "get" => await provider.Get(request, cancellationToken),
            "delete" => await provider.Delete(request, cancellationToken),
            "save" => await provider.Save(request, cancellationToken),
            "previewSave" => await provider.PreviewSave(request, cancellationToken),
            _ => throw new NotImplementedException($"Unsupported method {method}"),
        };
    }

    public async Task<ExtensibilityOperationResponse> CallExtensibilityHost(
        string method,
        ExtensibilityOperationRequest request,
        CancellationToken cancellationToken)
    {
        var provider = RegisteredProviders[new(request.Import.Provider, request.Import.Version)];

        return await CallProvider(method, provider, request, cancellationToken);
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
