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
    /// Configures the dependency injection container with core Bicep extension services and type definitions.
    /// </summary>
    /// <example>
    /// <code>
    /// services.AddBicepExtension(
    ///     name: "MyCompany.KubernetesExtension",
    ///     version: "1.0.0",
    ///     isSingleton: true,
    ///     typeConfiguration: (typeFactory, config) => {
    ///         var stringType = typeFactory.Create(() => new StringType());
    ///         config["apiUrl"] = new ObjectTypeProperty(
    ///             typeFactory.GetReference(stringType), 
    ///             ObjectTypePropertyFlags.Required, 
    ///             "The Kubernetes API server URL");
    ///     });
    /// </code>
    /// </example>
    public static IBicepExtensionBuilder AddBicepExtension(
        this IServiceCollection services,
        string name,
        string version,
        bool isSingleton,
        Assembly typeAssembly,
        Type? configurationType = null)
    {
        var configuration = new Dictionary<string, ObjectTypeProperty>();

        services.AddSingleton<ITypeProvider>(new TypeProvider([typeAssembly]));
        services.AddSingleton<ITypeDefinitionBuilder>(sp => new TypeDefinitionBuilder(
            name,
            version,
            isSingleton,
            configurationType,
            sp.GetRequiredService<ITypeProvider>()));

        services.AddSingleton<IResourceHandlerCollection, ResourceHandlerCollection>();

        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
        });
        services.AddGrpcReflection();

        return new DefaultBicepExtensionBuilder(services);
    }
}
