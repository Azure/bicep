// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Extensions.DependencyInjection;

public interface IBicepExtensionBuilder
{
    /// <summary>
    /// Gets the associated service collection.
    /// </summary>
    IServiceCollection Services { get; }
}
