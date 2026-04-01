// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Sessions;

namespace Bicep.Core.Registry.Providers;

/// <summary>
/// Represents a strategy for handling registry interactions for a specific host or group of hosts.
/// </summary>
public interface IRegistryProvider
{
    /// <summary>
    /// Gets the unique provider name (for example, "acr" or "generic").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the provider priority. Higher priority providers win if multiple <see cref="CanHandle"/> checks succeed.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Determines whether the provider can handle the specified registry host.
    /// </summary>
    bool CanHandle(string registry);

    /// <summary>
    /// Gets an OCI registry transport implementation for the specified registry host.
    /// Implementations may return a shared singleton or construct a fresh transport per request.
    /// </summary>
    IOciRegistryTransport GetTransport(string registry);

    /// <summary>
    /// Creates a registry session capable of performing push/pull operations for the specified reference.
    /// </summary>
    IRegistrySession CreateSession(RegistryRef reference, RegistryProviderContext context);
}
