// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Local.Extension.Host;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class BicepExtensionServiceCollectionExtensions
{
    /// <summary>
    /// Adds Bicep extension services and gRPC endpoints to the specified service collection.
    /// </summary>
    /// <remarks>This method registers required services for Bicep extension functionality, including resource
    /// handlers and gRPC reflection. Call this method during application startup to enable Bicep extension
    /// features.</remarks>
    /// <param name="services">The service collection to which the Bicep extension and related services will be added. Cannot be null.</param>
    /// <returns>An <see cref="IBicepExtensionBuilder"/> that can be used to further configure the Bicep extension.</returns>
    public static IBicepExtensionBuilder AddBicepExtension(this IServiceCollection services)
    {
        services.AddSingleton<IResourceHandlerCollection, ResourceHandlerCollection>();

        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
        });
        services.AddGrpcReflection();
        
        return new DefaultBicepExtensionBuilder(services);
    }
}
