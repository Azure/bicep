// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Host.TypeDefinitionBuilder;
using Bicep.Local.Extension.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Local.Extension.Host.Extensions;
public static class IServiceCollectionExtensions

{
    private static IDictionary<string, string> ArgumentMappings = new Dictionary<string, string>()
    {
        { "-d", "describe" },
        { "--describe", "describe" },

        { "-s", "socket" },
        { "--socket", "socket" },

        { "-t", "http" },
        { "--http", "http" },

        { "-p", "pipe" },
        { "--pipe", "pipe" }
    };


    private static bool IsTracingEnabled
        => bool.TryParse(Environment.GetEnvironmentVariable("BICEP_TRACING_ENABLED"), out var isEnabled) && isEnabled;

    public static IServiceCollection AddBicepExtensionServices(this IServiceCollection services
                                                                , Func<TypeFactory, ObjectType, TypeSettings> typeSettings
                                                                , Action<TypeFactory, Dictionary<string, ObjectTypeProperty>>? typeConfiguration = null)
    {
        var typeFactory = new TypeFactory([]);
        var configuration = new Dictionary<string, ObjectTypeProperty>();

        if (typeSettings is null)
        {
            throw new ArgumentNullException(nameof(typeSettings));
        }

        if (typeConfiguration is not null)
        {
            typeConfiguration(typeFactory, configuration);
        }

        ObjectType configurationType = typeFactory.Create(() => new ObjectType("configuration", configuration, null));

        TypeSettings settings = typeSettings(typeFactory, configurationType);

        services.AddSingleton(settings)
            .AddSingleton(typeFactory)
            .AddSingleton<IResourceHandlerFactory, ResourceHandlerFactory>()
            .AddSingleton<ITypeSpecGenerator, TypeDefinitionGenerator>()
            .AddSingleton<ITypeProvider, TypeProvider>()    
            .AddSingleton<IDictionary<Type, Func<TypeBase>>>(sp => new Dictionary<Type, Func<TypeBase>>
                {
                    { typeof(string), () => new StringType() },
                    { typeof(bool), () => new BooleanType() },
                    { typeof(int), () => new IntegerType() }
                }.ToImmutableDictionary());

        services.AddGrpc();
        services.AddGrpcReflection();
        return services;
    }

    public static async Task RunBicepExtensionAsync(this WebApplication? app)
    {
        if (app is null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        if (app.Configuration.GetValue<bool>("describe"))
        {
            var typeSpecGenerator = app.Services.GetRequiredService<ITypeSpecGenerator>();
            var spec = typeSpecGenerator.GenerateBicepResourceTypes();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
            };

            var stdout = Console.Out;
            await stdout.WriteLineAsync(JsonSerializer.Serialize(
                spec,
                jsonOptions));
        }
        else
        {
            await app.RunAsync();
        }
    }

    public static WebApplicationBuilder AddBicepExtensionHost(this WebApplicationBuilder builder, string[] args)
    {
        if (IsTracingEnabled)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
        }

        builder.Configuration.AddCommandLine(args, ArgumentMappings);

        builder.WebHost.ConfigureKestrel((context, options) =>
        {
            (string? Socket, string? Pipe, int Http) connectionOptions = (context.Configuration.GetValue<string>("socket"),
                                                                          context.Configuration.GetValue<string>("pipe"),
                                                                          context.Configuration.GetValue<int>("http", 5000));

            switch (connectionOptions)
            {
                case { Socket: { }, Pipe: null }:
                    options.ListenUnixSocket(connectionOptions.Socket, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    break;
                case { Socket: null, Pipe: { } }:
                    options.ListenNamedPipe(connectionOptions.Pipe, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    break;
                default:
                    options.ListenLocalhost(connectionOptions.Http, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    break;
            }
        });

        return builder;
    }

    public static WebApplication MapBicepDispatcher<TDispatcher>(this WebApplication app)
        where TDispatcher : BicepExtension.BicepExtensionBase
    {
        app.MapGrpcService<TDispatcher>();

        if (app.Environment.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }

        return app;
    }

    public static IServiceCollection AddBicepResourceHandler<T>(this IServiceCollection services)
        where T : class, Handlers.IResourceHandler
    {
        var resourceHandler = typeof(T);

        // check that only one handler is registered for the given resource type
        if (resourceHandler.TryGetTypedResourceHandlerInterface(out var baseInterface) && baseInterface is not null)
        {
            var resourceType = baseInterface.GetGenericArguments()[0];

            var existingHandler = services
                .Where(st => st.ServiceType.IsAssignableFrom(typeof(IResourceHandler<>)))
                .Select(t =>
                {
                    var implementationType = t.IsKeyedService ? t.KeyedImplementationType : t.ImplementationType;

                    if (implementationType?.TryGetTypedResourceHandlerInterface(out Type? typedInterface) == true && typedInterface is not null)
                    {
                        var genericType = typedInterface.GetGenericArguments()[0];
                        return genericType;
                    }

                    return null;
                })
                .OfType<Type>()
                .FirstOrDefault(et => et == resourceType);

            if (existingHandler is not null)
            {
                throw new InvalidOperationException($"A handler [`{existingHandler.FullName}`] is already registered for type [`{resourceType.FullName}`]");
            }

        }

        services.AddSingleton<IResourceHandler, T>();

        return services;
    }

}

