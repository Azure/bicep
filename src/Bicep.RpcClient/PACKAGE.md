# Azure Bicep RPC Client

A .NET library for programmatically interacting with the [Bicep CLI](https://github.com/Azure/bicep)
via JSON-RPC.

> **Note:** While this package is publicly available on nuget.org, it is not officially supported. Breaking changes may be introduced at any time.

## Getting started

### Install the package

```bash
dotnet add package Azure.Bicep.RpcClient
```

### Initialize the client

The `BicepClientFactory` handles downloading the Bicep CLI (if needed) and establishing a JSON-RPC
connection. By default, binaries are cached under `~/.bicep/bin`.

```csharp
using Bicep.RpcClient;

var factory = new BicepClientFactory();

// Download (or reuse) the latest Bicep CLI and connect
using var bicep = await factory.Initialize(BicepClientConfiguration.Default);
```

### Pin a specific Bicep version

```csharp
using var bicep = await factory.Initialize(new() {
    BicepVersion = "0.38.5"
});
```

### Use an existing Bicep installation

```csharp
using var bicep = await factory.Initialize(new() {
    ExistingCliPath = "/usr/local/bin/bicep"
});
```

## Available operations

All methods are async, accept an optional `CancellationToken`, and communicate with the Bicep CLI
over JSON-RPC.

### Compile

Compiles a `.bicep` file into an ARM template JSON string.

```csharp
var result = await bicep.Compile(new("./main.bicep"));

if (result.Success)
{
    // result.Contents contains the ARM template JSON
    Console.WriteLine(result.Contents);
}
```

### CompileParams

Compiles a `.bicepparam` file into ARM deployment parameters. You can optionally override parameter
values.

```csharp
var result = await bicep.CompileParams(new(
    "./main.bicepparam",
    new Dictionary<string, JsonNode>()));

if (result.Success)
{
    Console.WriteLine(result.Parameters); // ARM parameters JSON
    Console.WriteLine(result.Template);   // ARM template JSON (if resolvable)
    // result.TemplateSpecId is set when the params file references a template spec
}
```

### Format

Formats a Bicep file according to the standard Bicep formatting rules.
Requires Bicep CLI **0.37.1** or later.

```csharp
var result = await bicep.Format(new("./main.bicep"));

File.WriteAllText("./main.bicep", result.Contents);
```

### GetMetadata

Retrieves parameters, outputs, exports, and file-level metadata from a Bicep file.

```csharp
var result = await bicep.GetMetadata(new("./main.bicep"));

foreach (var param in result.Parameters)
{
    Console.WriteLine($"param {param.Name}: {param.Type?.Name} — {param.Description}");
}

foreach (var output in result.Outputs)
{
    Console.WriteLine($"output {output.Name}: {output.Type?.Name}");
}

foreach (var export in result.Exports)
{
    Console.WriteLine($"@export() {export.Kind} {export.Name}");
}

foreach (var meta in result.Metadata)
{
    Console.WriteLine($"metadata {meta.Name} = '{meta.Value}'");
}
```

### GetFileReferences

Returns all file paths referenced by a Bicep file — modules, loaded files, and the file itself.

```csharp
var result = await bicep.GetFileReferences(
    new("./main.bicep"));

foreach (var path in result.FilePaths)
{
    Console.WriteLine(path);
}
```

### GetDeploymentGraph

Returns the resource dependency graph for a Bicep file, useful for visualization.

```csharp
var result = await bicep.GetDeploymentGraph(
    new("./main.bicep"));

foreach (var node in result.Nodes)
{
    var existing = node.IsExisting ? " (existing)" : "";
    Console.WriteLine($"  {node.Name}: {node.Type}{existing}");
}

foreach (var edge in result.Edges)
{
    Console.WriteLine($"  {edge.Source} -> {edge.Target}");
}
```

### GetSnapshot

Generates a snapshot of a `.bicepparam` file with Azure deployment context.
Requires Bicep CLI **0.36.1** or later.

```csharp
var result = await bicep.GetSnapshot(new(
    "./main.bicepparam",
    new(
        TenantId: null,
        SubscriptionId: "00000000-0000-0000-0000-000000000000",
        ResourceGroup: "my-rg",
        Location: "eastus",
        DeploymentName: "my-deployment"),
    ExternalInputs: null));

Console.WriteLine(result.Snapshot);
```

### GetVersion

Returns the version of the connected Bicep CLI. The result is cached for subsequent calls.

```csharp
var version = await bicep.GetVersion();
Console.WriteLine($"Bicep CLI version: {version}");
```