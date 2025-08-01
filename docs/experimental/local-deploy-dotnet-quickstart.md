# Creating a Local Extension with .NET

A quickstart guide to creating your own Bicep Local Extension using .NET.

## Project Scaffolding

This guide assumes you have the .NET 9 SDK installed locally.

1. Run the following commands to create a new console app:
    ```sh
    dotnet new console
    dotnet add package Azure.Bicep.Local.Extension
    ```
1. Create a file named `Program.cs` with the following contents:
    ```csharp
    using Microsoft.AspNetCore.Builder;
    using Bicep.Local.Extension.Host.Extensions;
    using Bicep.Extension.Github.Handlers;
    using Microsoft.Extensions.DependencyInjection;
    
    var builder = WebApplication.CreateBuilder();
    
    builder.AddBicepExtensionHost(args);
    builder.Services
        .AddBicepExtension(
            name: "MyExtension",
            version: "0.0.1",
            isSingleton: true,
            typeAssembly: typeof(Program).Assembly)
        .WithResourceHandler<MyResourceHandler>();
    
    var app = builder.Build();

    app.MapBicepExtension();
    
    await app.RunAsync();
    ```
1. Create a handler named `MyResourceHandler.cs` with the following contents:
    ```csharp
    
    ```
