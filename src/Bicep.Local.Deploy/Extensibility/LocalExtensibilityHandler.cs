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
    private record ExtensionKey(
        string Name,
        string Version);

    private Dictionary<ExtensionKey, LocalExtensibilityExtension> RegisteredExtensions = new();
    private readonly IModuleDispatcher moduleDispatcher;
    private readonly Func<Uri, Task<LocalExtensibilityExtension>> extensionFactory;

    public LocalExtensibilityHandler(IModuleDispatcher moduleDispatcher, Func<Uri, Task<LocalExtensibilityExtension>> extensionFactory)
    {
        this.moduleDispatcher = moduleDispatcher;
        this.extensionFactory = extensionFactory;
        // Built in provider for handling nested deployments
        RegisteredExtensions[new("LocalNested", "0.0.0")] = new AzExtensibilityExtension(this);
    }

    public async Task<ResourceResponseBody> CallExtensibilityHost(
        ExtensionInfo extensionInfo,
        HttpContent content,
        CancellationToken cancellationToken)
    {
        var extension = RegisteredExtensions[new(extensionInfo.ExtensionName, extensionInfo.ExtensionVersion)];

        return await CallExtension(extensionInfo.Method, extension, content, cancellationToken);
    }

    private async Task<ResourceResponseBody> CallExtension(
        string method,
        LocalExtensibilityExtension provider,
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

    private IEnumerable<(NamespaceType namespaceType, Uri binaryUri)> GetBinaryExtensions(Compilation compilation)
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

    public async Task InitializeExtensions(Compilation compilation)
    {
        var binaryExtensions = GetBinaryExtensions(compilation).DistinctBy(x => x.binaryUri);

        foreach (var (namespaceType, binaryUri) in binaryExtensions)
        {
            ExtensionKey providerKey = new(namespaceType.Settings.ArmTemplateProviderName, namespaceType.Settings.ArmTemplateProviderVersion);
            RegisteredExtensions[providerKey] = await extensionFactory(binaryUri);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(RegisteredExtensions.Values.Select(async provider =>
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
