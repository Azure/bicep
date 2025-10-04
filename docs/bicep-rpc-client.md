# Bicep RPC Client

The Bicep RPC Client is a .NET library that provides a programmatic interface for interacting with
the Bicep CLI via JSON-RPC. This library enables you to call any Bicep CLI functionality from your
.NET applications without needing to shell out to the command line.

The RPC Client provides a unified and efficient interface for calling any version of Bicep
programmatically. It spawns a single executable to minimize cold-start delays for multiple Bicep
requests and supports caching of binaries under `~/.bicep/bin`. The library is distributed as a
lightweight NuGet package with minimal external dependencies and supports all JSON-RPC methods
available in the Bicep CLI, including support for concurrent operations.

## Prerequisites

Before using the Bicep RPC Client, ensure you have the .NET 9 SDK installed on your system. You
can download the latest .NET SDK from the [.NET download page][04].

To create a minimal console application that uses the Bicep RPC Client, create a new project file
with the following content:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Azure.Bicep.RpcClient" Version="0.38.5" />
  </ItemGroup>

</Project>
```

Alternatively, you can use the .NET CLI to create a new console application and add the package:

```bash
dotnet new console -n MyBicepApp
cd MyBicepApp
dotnet add package Azure.Bicep.RpcClient
```

## Getting started

To use the Bicep RPC Client, you need to create a client factory and initialize a client instance.
The factory handles downloading the Bicep CLI binary if needed and establishing the JSON-RPC
connection.

The following example demonstrates how to download, initialize, and use the Bicep RPC Client to
compile a Bicep file:

```csharp
using Bicep.RpcClient;

// Create a client factory with an HttpClient
var clientFactory = new BicepClientFactory(new HttpClient());

// Download and initialize the latest version of Bicep
using var client = await clientFactory.DownloadAndInitialize(
    new BicepClientConfiguration(), 
    cancellationToken);

// Compile a Bicep file
var result = await client.Compile(new CompileRequest("./main.bicep"));

if (result.Success)
{
    Console.WriteLine("Compilation successful!");
    Console.WriteLine(result.Contents);
}
else
{
    Console.WriteLine("Compilation failed:");
    foreach (var diagnostic in result.Diagnostics)
    {
        Console.WriteLine($"{diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}");
    }
}
```

### Using a specific Bicep version

By default, the client factory downloads and uses the latest version of Bicep. To use a specific
version instead, specify the version in the configuration. The following example shows how to
initialize the client with Bicep version 0.38.3:

```csharp
var clientFactory = new BicepClientFactory(new HttpClient());

using var client = await clientFactory.DownloadAndInitialize(
    new BicepClientConfiguration 
    { 
        BicepVersion = "0.38.3" 
    }, 
    cancellationToken);
```

### Using an existing Bicep installation

If you already have Bicep installed locally, you can initialize the client directly from the path
instead of downloading it. The following example shows how to initialize the client from an
existing installation:

```csharp
var clientFactory = new BicepClientFactory(new HttpClient());

using var client = await clientFactory.InitializeFromPath(
    "/path/to/bicep", 
    cancellationToken);
```

## Available operations

The Bicep RPC Client supports all major Bicep CLI operations through a consistent async API. Each
operation returns detailed results, including success status, diagnostics, and operation-specific
output.

### Compile

The `Compile` method compiles a Bicep file into an ARM template. The following example shows how
to compile a Bicep file and access the resulting ARM template JSON:

```csharp
var result = await client.Compile(new CompileRequest("./main.bicep"));

if (result.Success)
{
    // Access the compiled ARM template JSON
    var armTemplate = result.Contents;
}
```

### Compile parameters

The `CompileParams` method compiles a `.bicepparam` file into an ARM parameters file. You can
optionally provide parameter overrides as a dictionary. The following example shows how to compile
a parameters file:

```csharp
var result = await client.CompileParams(new CompileParamsRequest(
    "./main.bicepparam",
    new Dictionary<string, JsonNode>() // Optional parameter overrides
));

if (result.Success)
{
    var parameters = result.Parameters;
    var template = result.Template;
}
```

### Format

The `Format` method formats a Bicep file according to Bicep formatting standards. The following
example shows how to format a Bicep file and retrieve the formatted code:

```csharp
var result = await client.Format(new FormatRequest("./main.bicep"));

// Get the formatted Bicep code
var formattedCode = result.Contents;
```

### Get metadata

The `GetMetadata` method retrieves metadata about types, parameters, and outputs exported from a
Bicep file. This is useful for understanding the structure and exported symbols of a Bicep module.
The following example shows how to retrieve and display metadata:

```csharp
var result = await client.GetMetadata(new GetMetadataRequest("./main.bicep"));

foreach (var export in result.Exports)
{
    Console.WriteLine($"Exported {export.Kind}: {export.Name}");
    Console.WriteLine($"Description: {export.Description}");
}

foreach (var param in result.Parameters)
{
    Console.WriteLine($"Parameter: {param.Name}");
}
```

### Get file references

The `GetFileReferences` method returns all file references, including modules and extensions, used
by a Bicep file. The following example shows how to retrieve the list of referenced files:

```csharp
var result = await client.GetFileReferences(new GetFileReferencesRequest("./main.bicep"));

foreach (var filePath in result.FilePaths)
{
    Console.WriteLine($"Referenced file: {filePath}");
}
```

### Get deployment graph

The `GetDeploymentGraph` method returns the deployment graph for a Bicep file. This graph
represents the resource dependencies and is primarily used for visualization purposes. The
following example shows how to retrieve the deployment graph:

```csharp
var result = await client.GetDeploymentGraph(new GetDeploymentGraphRequest("./main.bicep"));

// Process deployment graph data
```

### Get snapshot

The `GetSnapshot` method generates a snapshot of a Bicep parameters file with deployment metadata.
This is useful for understanding what resources would be deployed with the given parameters. The
following example shows how to generate a snapshot:

```csharp
var result = await client.GetSnapshot(new GetSnapshotRequest(
    "./main.bicepparam",
    new GetSnapshotRequest.MetadataDefinition(
        TenantId: null,
        SubscriptionId: "00000000-0000-0000-0000-000000000000",
        ResourceGroup: "my-rg",
        Location: "eastus",
        DeploymentName: "my-deployment"
    ),
    null // Optional external inputs
));

// Access the snapshot
var snapshot = result.Snapshot;
```

### Get version

The `GetVersion` method retrieves the version of the Bicep CLI that the client is using. The
following example shows how to get and display the version:

```csharp
var version = await client.GetVersion();
Console.WriteLine($"Bicep version: {version}");
```

## Configuration options

The `BicepClientConfiguration` class provides several configuration options that control how the
client downloads and initializes the Bicep CLI. You can specify the version, installation path,
target platform, and architecture. The following example demonstrates all available configuration
options:

```csharp
var config = new BicepClientConfiguration
{
    // Version of Bicep to download (format: "x.y.z")
    // If not specified, downloads the latest version
    BicepVersion = "0.38.3",
    
    // Custom installation path for Bicep binaries
    // Default: ~/.bicep/bin
    InstallPath = "/custom/path/to/bicep",
    
    // Target OS platform (for cross-platform scenarios)
    // Default: Current OS
    OsPlatform = OSPlatform.Linux,
    
    // Target architecture (for cross-architecture scenarios)
    // Default: Current architecture
    Architecture = Architecture.X64
};

using var client = await clientFactory.DownloadAndInitialize(config, cancellationToken);
```

## Error handling

When Bicep operations fail, the RPC Client provides detailed diagnostic information through the
`Diagnostics` property of the response. Each diagnostic includes the severity level, error code,
message, and location information. The following example shows how to handle and display
diagnostics:

```csharp
var result = await client.Compile(new CompileRequest("./main.bicep"));

if (!result.Success)
{
    foreach (var diagnostic in result.Diagnostics)
    {
        Console.WriteLine($"[{diagnostic.Level}] {diagnostic.Code}");
        Console.WriteLine($"  Message: {diagnostic.Message}");
        Console.WriteLine($"  Location: Line {diagnostic.Range.Start.Line}, Char {diagnostic.Range.Start.Char}");
        Console.WriteLine($"  Source: {diagnostic.Source}");
    }
}
```

## Best practices

When working with the Bicep RPC Client, following these best practices helps ensure efficient
resource usage and optimal performance.

### Dispose of clients

Always dispose of `IBicepClient` instances when you're done using them to ensure proper cleanup of
resources. The client manages a running Bicep CLI process that needs to be terminated correctly.
The following example shows the recommended disposal pattern:

```csharp
using var client = await clientFactory.DownloadAndInitialize(config, cancellationToken);

// Use the client...

// Automatically disposed when leaving the using block
```

### Reuse clients

For multiple operations, reuse the same client instance instead of creating a new one for each
operation. Creating a new client for each operation incurs the overhead of starting a new Bicep
CLI process. The following example shows how to reuse a client for multiple operations:

```csharp
using var client = await clientFactory.DownloadAndInitialize(config, cancellationToken);

// Perform multiple operations with the same client
var result1 = await client.Compile(new CompileRequest("./file1.bicep"));
var result2 = await client.Compile(new CompileRequest("./file2.bicep"));
var result3 = await client.Format(new FormatRequest("./file3.bicep"));
```

### Handle cancellation

All client methods support cancellation through the `CancellationToken` parameter. Use cancellation
tokens to provide a way to stop long-running operations or to implement timeout behavior. The
following example shows how to implement a timeout using cancellation tokens:

```csharp
var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromMinutes(5));

try
{
    using var client = await clientFactory.DownloadAndInitialize(config, cts.Token);
    var result = await client.Compile(new CompileRequest("./main.bicep"), cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was cancelled");
}
```

### Cache Bicep binaries

By default, the RPC Client caches downloaded Bicep binaries in `~/.bicep/bin`. When you request a
specific version that has already been downloaded, the client uses the cached binary instead of
downloading it again. The following example demonstrates how the caching behavior works:

```csharp
// First call: downloads Bicep
using var client1 = await clientFactory.DownloadAndInitialize(
    new BicepClientConfiguration { BicepVersion = "0.38.3" }, 
    cancellationToken);

// Subsequent calls: uses cached binary
using var client2 = await clientFactory.DownloadAndInitialize(
    new BicepClientConfiguration { BicepVersion = "0.38.3" }, 
    cancellationToken);
```

## Advanced scenarios

The Bicep RPC Client supports several advanced scenarios for specialized use cases.

### Cross-platform compilation

You can download and use Bicep binaries for different platforms and architectures than the one
you're running on. This is useful for build systems that need to prepare artifacts for different
target environments. The following example shows how to download a Linux binary from Windows:

```csharp
var clientFactory = new BicepClientFactory(new HttpClient());

// Download Linux x64 binary even if running on Windows
using var client = await clientFactory.DownloadAndInitialize(
    new BicepClientConfiguration 
    { 
        OsPlatform = OSPlatform.Linux,
        Architecture = Architecture.X64
    }, 
    cancellationToken);
```

### Parameter overrides

When compiling `.bicepparam` files, you can provide parameter overrides programmatically without
modifying the parameters file. This is useful for dynamically changing parameter values based on
runtime conditions. The following example shows how to override parameters:

```csharp
var parameterOverrides = new Dictionary<string, JsonNode>
{
    ["location"] = JsonValue.Create("westus"),
    ["vmSize"] = JsonValue.Create("Standard_D2s_v3")
};

var result = await client.CompileParams(new CompileParamsRequest(
    "./main.bicepparam",
    parameterOverrides
));
```

### Concurrent operations

The RPC Client supports concurrent operations through its async API. You can compile multiple
files concurrently using the same client instance. The following example shows how to compile
multiple Bicep files in parallel:

```csharp
var tasks = new List<Task<CompileResponse>>();

using var client = await clientFactory.DownloadAndInitialize(config, cancellationToken);

// Compile multiple files concurrently
foreach (var file in bicepFiles)
{
    tasks.Add(client.Compile(new CompileRequest(file)));
}

var results = await Task.WhenAll(tasks);
```

## Troubleshooting

This section covers common issues you might encounter when using the Bicep RPC Client and how to
resolve them.

### Version format

Bicep versions must be specified in the format `x.y.z` where `x`, `y`, and `z` are integers. Do
not include the `v` prefix that appears in release tags. The following example shows the correct
and incorrect formats:

```csharp
// ❌ Incorrect
new BicepClientConfiguration { BicepVersion = "v0.38.3" }

// ✅ Correct
new BicepClientConfiguration { BicepVersion = "0.38.3" }
```

### File not found

If you receive a `FileNotFoundException` when using `InitializeFromPath`, ensure the path to the
Bicep CLI is correct, and the file exists. The following example shows how to verify the file
exists before initializing the client:

```csharp
var bicepPath = "/path/to/bicep";
if (!File.Exists(bicepPath))
{
    throw new FileNotFoundException($"Bicep CLI not found at: {bicepPath}");
}

using var client = await clientFactory.InitializeFromPath(bicepPath, cancellationToken);
```

### Network issues

If you experience issues downloading Bicep binaries, verify that you have network connectivity and
that the Bicep download URLs are accessible from your environment. The client downloads binaries
from the following locations:

- Download URL format: `https://downloads.bicep.azure.com/<version>/bicep-<os>-<arch>`
- Latest version API: `https://downloads.bicep.azure.com/releases/latest`

If you're behind a corporate firewall or proxy, ensure these URLs are accessible. You may need to
configure your `HttpClient` with appropriate proxy settings before passing it to the
`BicepClientFactory`.

## See also

- [Bicep documentation][01]
- [Bicep CLI JSON-RPC documentation][02]
- [Bicep GitHub repository][03]
- [.NET download page][04]

<!-- Link reference definitions -->
[01]: https://aka.ms/bicep
[02]: https://learn.microsoft.com/azure/azure-resource-manager/bicep/bicep-cli?tabs=bicep-cli#jsonrpc
[03]: https://github.com/Azure/bicep
[04]: https://dotnet.microsoft.com/download
