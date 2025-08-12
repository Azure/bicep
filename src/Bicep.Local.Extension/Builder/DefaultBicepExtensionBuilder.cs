// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Extensions.DependencyInjection;

internal sealed class DefaultBicepExtensionBuilder : IBicepExtensionBuilder
{
    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    public DefaultBicepExtensionBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
