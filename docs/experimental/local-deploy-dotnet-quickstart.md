# Creating a Local Extension with .NET

A quickstart guide to creating your own Bicep Local Extension using .NET.

This guide requires the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) and compatible Bicep CLI, VS Code extension, and extension SDK builds. If you are updating an existing extension, see [Authenticated extension hosts](./local-deploy.md#authenticated-extension-hosts).

Until compatible releases are available, this example references the extension SDK source directly. Use a Bicep checkout that supports authenticated extension hosts and CLI and VS Code extension builds from the same revision. See [installing build artifacts](../installing-nightly.md#manual) for setup instructions. Once a compatible SDK package is published, you can replace the source reference with that package.

## Project Scaffolding

1. Create a directory named `MyExtension` alongside your Bicep checkout directory, which must be named `bicep` for the relative path below. Inside `MyExtension`, create a project file named `MyExtension.csproj` with the following contents:
    ```xml
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <OutputType>Exe</OutputType>
        <RootNamespace>MyExtension</RootNamespace>
        <AssemblyName>bicep-ext-myextension</AssemblyName>
        <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
        <PublishSingleFile>true</PublishSingleFile>
        <SelfContained>true</SelfContained>
        <InvariantGlobalization>true</InvariantGlobalization>
        <TargetFramework>net10.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
      </PropertyGroup>
    
      <ItemGroup>
        <ProjectReference Include="..\bicep\src\Bicep.Local.Extension\Bicep.Local.Extension.csproj" />
      </ItemGroup>
    </Project>
    ```
1. Create a file named `Program.cs` with the following contents:
    ```csharp
    using Microsoft.AspNetCore.Builder;
    using Bicep.Local.Extension.Host.Extensions;
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
1. Create a file named `Models.cs` with the following contents:
    ```csharp
    using System.Text.Json.Serialization;
    using Azure.Bicep.Types.Concrete;
    using Bicep.Local.Extension.Types.Attributes;
    
    public enum OperationType
    {
        Uppercase,
        Lowercase,
        Reverse,
    }
    
    public class MyResourceIdentifiers
    {
        [TypeProperty("The resource name", ObjectTypePropertyFlags.Identifier | ObjectTypePropertyFlags.Required)]
        public required string Name { get; set; }
    }
    
    [ResourceType("MyResource")]
    public class MyResource : MyResourceIdentifiers
    {
        [TypeProperty("The resource operation type", ObjectTypePropertyFlags.Required)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OperationType? Operation { get; set; }
    
        [TypeProperty("The text output")]
        public string? Output { get; set; }
    }
    ```
1. Create a file under `Handlers/MyResourceHandler.cs` with the following contents:
    ```csharp
    using Bicep.Local.Extension.Host.Handlers;
    
    public class MyResourceHandler : TypedResourceHandler<MyResource, MyResourceIdentifiers>
    {
        protected override async Task<ResourceResponse> Preview(ResourceRequest request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
    
            return GetResponse(request);
        }
    
        protected override async Task<ResourceResponse> CreateOrUpdate(ResourceRequest request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            request.Properties.Output = request.Properties.Operation switch
            {
                OperationType.Uppercase => request.Properties.Name.ToUpperInvariant(),
                OperationType.Lowercase => request.Properties.Name.ToLowerInvariant(),
                OperationType.Reverse => new([.. request.Properties.Name.Reverse()]),
                _ => throw new InvalidOperationException(),
            };
    
            return GetResponse(request);
        }
    
        protected override MyResourceIdentifiers GetIdentifiers(MyResource properties)
            => new()
            {
                Name = properties.Name,
            };
    }
    ```

## Publishing your extension locally
1. Run the following to in the project directory to publish your extension to your local filesystem:
    ```sh
    dotnet publish --configuration release -r osx-arm64 .
    dotnet publish --configuration release -r linux-x64 .
    dotnet publish --configuration release -r win-x64 .
    
    bicep publish-extension --bin-osx-arm64 ./bin/release/osx-arm64/publish/bicep-ext-myextension --bin-linux-x64 ./bin/release/linux-x64/publish/bicep-ext-myextension --bin-win-x64 ./bin/release/win-x64/publish/bicep-ext-myextension.exe --target ./bin/bicep-ext-myextension --force
    ```

## Running your extension
1. Create a file named `bicepconfig.json` with the following contents:
    ```json
    {
      "experimentalFeaturesEnabled": {
        "localDeploy": true
      },
      "extensions": {
        "myextension": "./bin/bicep-ext-myextension"
      },
      "implicitExtensions": []
    }
    ```
1. Create a file named `main.bicep` with the following contents:
    ```bicep
    targetScope = 'local'
    extension myextension
    
    param inputText string
    
    resource foo 'MyResource' = {
      name: inputText
      operation: 'Reverse'
    }
    
    output outputText string = foo.output
    ```
1. Create a file named `main.bicepparam` with the following contents:
    ```bicep
    using 'main.bicep'
    
    param inputText = 'Please reverse me!'
    ```
1. Run the following:
    ```sh
    bicep local-deploy main.bicepparam
    ```

You should see the following output in your terminal:
```
% bicep local-deploy main.bicepparam
Output outputText: "!em esrever esaelP"
Resource foo (Create): Succeeded
Result: Succeeded
```
