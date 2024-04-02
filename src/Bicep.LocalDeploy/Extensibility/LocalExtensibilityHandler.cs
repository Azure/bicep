// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Messages;
using Bicep.Core.Semantics.Namespaces;
using Bicep.LocalDeploy.Namespaces;

namespace Azure.Bicep.LocalDeploy.Extensibility;

public class LocalExtensibilityHandler
{
    private record ProviderKey(
        string Name,
        string Version);

    private ConcurrentDictionary<ProviderKey, Func<IExtensibilityProvider>> RegisteredProviders = new();

    private LocalExtensibilityHandler()
    {
    }

    private void Register(string name, string version, Func<IExtensibilityProvider> providerFactory)
    {
        RegisteredProviders[new(name, version)] = providerFactory;
    }

    private async Task<ExtensibilityOperationResponse> CallProvider(string method, IExtensibilityProvider provider, ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        switch (method)
        {
            case "get":
                return await provider.Get(request, cancellationToken);
            case "delete":
                return await provider.Delete(request, cancellationToken);
            case "save":
                return await provider.Save(request, cancellationToken);
            case "previewSave":
                return await provider.PreviewSave(request, cancellationToken);
            default:
                throw new NotImplementedException($"Unsupported method {method}");
        }
    }

    public async Task<ExtensibilityOperationResponse> CallExtensibilityHost(
        string method,
        ExtensibilityOperationRequest request,
        CancellationToken cancellationToken)
    {
        var providerKey = new ProviderKey(request.Import.Provider, request.Import.Version);
        var provider = RegisteredProviders.TryGetValue(providerKey, out var providerFactory)
            ? providerFactory()
            : throw new NotImplementedException($"Unrecognized provider {request.Import.Provider}, version {request.Import.Version}");

        return await CallProvider(method, provider, request, cancellationToken);
    }

    public static LocalExtensibilityHandler Build(Action<string> logAction)
    {
        var handler = new LocalExtensibilityHandler();

        handler.Register("LocalNested", "0.0.0", () => new AzExtensibilityProvider(handler));
        handler.Register(UtilsNamespaceType.Settings.ArmTemplateProviderName, UtilsNamespaceType.Settings.ArmTemplateProviderVersion, () => new UtilsExtensibilityProvider(logAction));
        handler.Register(K8sNamespaceType.Settings.ArmTemplateProviderName, K8sNamespaceType.Settings.ArmTemplateProviderVersion, () => new K8sExtensibilityProvider());
        handler.Register(GithubNamespaceType.Settings.ArmTemplateProviderName, GithubNamespaceType.Settings.ArmTemplateProviderVersion, () => new GithubExtensibilityProvider());

        return handler;
    }
}
