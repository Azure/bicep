# Azure.Bicep.Testing.Framework

Test evaluated Bicep resources and outputs without deploying to Azure. This small
.NET 10 library wraps the Bicep CLI's snapshot JSON-RPC operation and works with
MSTest, xUnit, NUnit, and other .NET assertion and test libraries.

> **Experimental and not officially supported.** This package is maintained in the
> Bicep repository, but provides no support SLA or compatibility guarantees.
> Its API and snapshot behavior may change, including breaking changes, in any release.
> Report issues and feedback at https://github.com/Azure/bicep/issues.
> The package follows the repository's versioning; a stable-looking version number
> does not imply a stable or supported API.

## Getting started

```shell
dotnet add package Azure.Bicep.Testing.Framework
```

Create a Bicep file and a `.bicepparam` entry point alongside your tests:

`infra/main.bicep`:

```bicep
param allowPublicAccess bool

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'teststorage'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: allowPublicAccess
  }
}
```

`infra/main.bicepparam`:

```bicep
using 'main.bicep'

param allowPublicAccess = false
```

Copy your fixtures to the test output directory, preserving relative paths:

```xml
<ItemGroup>
  <None Update="infra\**\*" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

This MSTest example pins the CLI version for reproducible tests:

```csharp
using Bicep.RpcClient;
using Bicep.Testing.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class InfrastructureTests
{
    [TestMethod]
    public async Task Storage_disallows_public_access()
    {
        await using var session = await BicepTestHelper.CreateAsync(new BicepClientConfiguration
        {
            BicepVersion = "0.49.2",
        });
        var snapshot = await session.SnapshotAsync(new SnapshotOptions
        {
            FilePath = Path.Combine(AppContext.BaseDirectory, "infra", "main.bicepparam"),
            SubscriptionId = "00000000-0000-0000-0000-000000000001",
            ResourceGroup = "test-rg",
            Location = "eastus",
            DeploymentName = "offline-test",
        });

        Assert.IsEmpty(snapshot.Diagnostics, string.Join(Environment.NewLine, snapshot.Diagnostics));
        var storage = snapshot.RequireResource("Microsoft.Storage/storageAccounts", "teststorage");
        Assert.IsFalse(storage.Properties.GetProperty("allowBlobPublicAccess").GetBoolean());
        Assert.IsNotNull(storage.TryParseId());
        Assert.AreEqual(
            "/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/test-rg/providers/Microsoft.Storage/storageAccounts/teststorage",
            storage.Id);
    }
}
```

## CLI and session lifetime

`CreateAsync(configuration)` downloads the configured CLI if needed and caches
it under `~/.bicep/bin`. This installation can require network access. To run
without downloading a CLI, use an existing installation:

```csharp
using Bicep.RpcClient;
using Bicep.Testing.Framework;

await using var session = await BicepTestHelper.CreateAsync(new BicepClientConfiguration
{
    ExistingCliPath = Path.GetFullPath("tools/bicep.exe"),
});
```

Use the platform-appropriate executable path. The configuration overload also
supports the RPC client's transport and installation options. Alternatively,
`new BicepTestHelper(client)` accepts an existing `IBicepClient`.
**The helper owns and disposes that client.** Reuse a helper across snapshots
and dispose it at the end of the test or fixture lifetime. All asynchronous
operations accept a cancellation token.

Snapshot evaluation requires CLI 0.36.1 or later; `ManagementGroupId` requires
0.47.16 or later, and `ConfigPath` requires 0.48.0 or later. Use a current pinned
version for the latest snapshot fixes.

## Assertions and evaluation limits

- `PredictedResources` contains flattened resources, including resources from
  nested modules. `Properties`, `Sku`, `Tags`, `Identity`, `Zones`, `Plan`, and
  `ExtendedLocation` are `JsonElement`s; unknown fields are retained in
  `AdditionalProperties`.
- `Id`, `Name`, `Location`, `Kind`, and `ManagedBy` are nullable strings.
  Unresolved ARM values remain expressions such as `[parameters('name')]`.
  `TryParseId()` returns the parsed Azure `ResourceId` for a resolved ID, or
  `null` when the ID is absent, unresolved, or invalid.
- `ResourcesOfType(type)` compares types without case sensitivity.
  `RequireResource(id)` compares IDs without case sensitivity.
  `RequireResource(type, name)` compares the name case-sensitively.
  Both `RequireResource` overloads throw unless exactly one resource matches.
- `SnapshotHelpers.GetSnapshotValue(value, "/path/0/property")` reads a JSON
  Pointer and throws if the path is missing or invalid.
- `Outputs` maps output names to resolved JSON values.
- **Always assert on `Diagnostics`** before asserting on resources or outputs:
  evaluation can produce an incomplete snapshot. Compilation, transport, and
  malformed-response errors throw rather than returning an empty snapshot.
- Deployment metadata is optional; the CLI uses placeholder values when omitted.
  Set it explicitly when asserting on resource IDs, locations, or scope-dependent
  expressions. `ExternalInputs` can supply offline values for parameter-file
  external inputs.
- These are predictions, not live resource-provider responses. Runtime values
  may be unresolved or omitted. Azure-specific fields such as ID and name can be
  absent for extensible resources. Snapshots can contain sensitive parameter or
  output values; handle and persist them accordingly.

The library does not deploy resources, validate against live Azure, collect usage
telemetry, or implement an assertion library. CLI dependency restoration (for
example, registry modules) may still require network access. Pre-cache dependencies
and provide an existing CLI for fully disconnected runs.

PowerShell packaging and live deployment testing are outside this initial scope.

## Developing in the Bicep repository

The package is included in the solution's normal build, test, and NuGet packing
workflows. Run its tests (including snapshots against the locally built CLI) with:

```shell
dotnet test --project src/Bicep.Testing.Framework.Tests/Bicep.Testing.Framework.Tests.csproj --configuration Release
dotnet pack src/Bicep.Testing.Framework/Bicep.Testing.Framework.csproj --configuration Release
```
