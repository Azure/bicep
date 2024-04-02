// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Messages;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Types;
using Bicep.LocalDeploy.Namespaces;
using Microsoft.VisualStudio.Threading;
using IAsyncDisposable = System.IAsyncDisposable;

namespace Azure.Bicep.LocalDeploy.Extensibility;

public class LocalExtensibilityHandler : IAsyncDisposable
{
    private record ProviderKey(
        string Name,
        string Version);

    private Dictionary<ProviderKey, AsyncLazy<LocalExtensibilityProvider>> RegisteredProviders = new();

    private LocalExtensibilityHandler()
    {
    }

    private void RegisterAsync(string name, string version, Func<Task<LocalExtensibilityProvider>> providerFactory)
    {
#pragma warning disable VSTHRD012 // Provide JoinableTaskFactory where allowed
        RegisteredProviders.TryAdd(new(name, version), new AsyncLazy<LocalExtensibilityProvider>(providerFactory));
#pragma warning restore VSTHRD012 // Provide JoinableTaskFactory where allowed
    }

    private void Register(string name, string version, Func<LocalExtensibilityProvider> providerFactory)
        => RegisterAsync(name, version, () => Task.FromResult(providerFactory()));

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
        LocalExtensibilityProvider provider;
        try {
            // TOOD use fully qualified reference to guarantee uniqueness
            provider = await RegisteredProviders[new(request.Import.Provider, request.Import.Version)]
                .GetValueAsync(cancellationToken);
        } catch (Exception ex) {
            return new(
                null,
                null,
                [new("ProviderLoadFailed", $"Failed to launch provider: {ex.Message}", request.Import.Provider)]);
        }

        return await CallProvider(method, provider, request, cancellationToken);
    }

    public static LocalExtensibilityHandler Build(IReadOnlyDictionary<Uri, NamespaceType> binaryProviders)
    {
        var handler = new LocalExtensibilityHandler();

        handler.Register("LocalNested", "0.0.0", () => new AzExtensibilityProvider(handler));
        foreach (var (binaryUri, namespaceType) in binaryProviders)
        {
            handler.RegisterAsync(
                namespaceType.Settings.ArmTemplateProviderName,
                namespaceType.Settings.ArmTemplateProviderVersion,
                async () => await JsonRpcExtensibilityProvider.Start(binaryUri));
        }

        return handler;
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(RegisteredProviders.Values.Select(async x => {
            try {
                if (x.IsValueCreated)
                {
                    var value = await x.GetValueAsync();
                    await value.DisposeAsync();
                }
            }
            catch
            {
                // ignore
            }
        }));
    }
}
