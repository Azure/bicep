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
    /// Adds the Bicep extension to the specified service collection, enabling support for resource handling and gRPC
    /// services within the application.
    /// </summary>
    /// <remarks>This method registers a singleton implementation of <see cref="IResourceHandlerCollection"/>
    /// and configures gRPC services with detailed error reporting and reflection enabled. Call this method during
    /// application startup to enable Bicep extension features.</remarks>
    /// <param name="services">The service collection to which the Bicep extension and related services will be added. Cannot be null.</param>
    /// <returns>An instance of <see cref="IBicepExtensionBuilder"/> that can be used to further configure the Bicep extension.</returns>
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

    [Obsolete("""
    Use the fluent configuration API instead:
    services
        .AddBicepExtension()
        .WithDefaults("MyExtension", "1.0.0", isSingleton: true)
        .WithTypeAssemblies([typeof(MyResource).Assembly])
        .WithConfigurationType<MyConfig>()
    """, error: false)]
    public static IBicepExtensionBuilder AddBicepExtension(
        this IServiceCollection services,
        string name,
        string version,
        bool isSingleton,
        Assembly typeAssembly,
        Type? configurationType = null)
    {
        var builder = services
                       .AddBicepExtension()
                       .WithDefaults(name, version, isSingleton)
                       .WithTypeAssemblies([typeAssembly]);

        if(configurationType is not null)
        {
            builder.WithConfigurationType(configurationType);
        }

        return builder;
    }        


}
