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
        using var factory = new PooledBicepClientFactory();
        var helper = new BicepTestHelper(factory);
        var configuration = new BicepClientConfiguration
        {
            BicepVersion = "0.49.2",
        };
        var snapshot = await helper.SnapshotAsync(configuration, new SnapshotOptions
        {
            FilePath = Path.Combine(AppContext.BaseDirectory, "infra", "main.bicepparam"),
            SubscriptionId = "00000000-0000-0000-0000-000000000001",
            ResourceGroup = "test-rg",
            Location = "eastus",
            DeploymentName = "offline-test",
        });

        Assert.IsEmpty(snapshot.Diagnostics, string.Join(Environment.NewLine, snapshot.Diagnostics));
        var storage = snapshot.GetArmResourceOfType("Microsoft.Storage/storageAccounts")
            .Single(resource => resource.Name == "teststorage");
        Assert.IsFalse(storage.GetRequiredBoolProperty("/properties/allowBlobPublicAccess"));
        Assert.AreEqual(
            "/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/test-rg/providers/Microsoft.Storage/storageAccounts/teststorage",
            storage.GetRequiredStringProperty("/id"));
    }
}
```

## CLI and factory lifetime

`SnapshotAsync(configuration, options)` uses the supplied `IBicepClientFactory`
to acquire a client for each call and disposes that client when the call completes,
including on failure. The default factories download the configured CLI if needed and cache
it under `~/.bicep/bin`. This installation can require network access. To run
without downloading a CLI, use an existing installation:

```csharp
using Bicep.RpcClient;
using Bicep.Testing.Framework;

using var factory = new PooledBicepClientFactory();
var helper = new BicepTestHelper(factory);
var snapshot = await helper.SnapshotAsync(new BicepClientConfiguration
{
    ExistingCliPath = Path.GetFullPath("tools/bicep.exe"),
}, new SnapshotOptions
{
    FilePath = Path.GetFullPath("infra/main.bicepparam"),
});
```

Use the platform-appropriate executable path. The configuration also supports
the RPC client's transport and installation options. A helper can evaluate
different CLI versions side by side by passing different configurations to
`SnapshotAsync`. Use `BicepClientFactory` for a new client per snapshot, or
`PooledBicepClientFactory` to reuse clients by configuration.
**The caller owns the factory.** Dispose a pooled factory at the end of the test
or fixture lifetime; the helper itself does not require disposal. All asynchronous
operations accept a cancellation token.

Snapshot evaluation requires CLI 0.36.1 or later; `ManagementGroupId` requires
0.47.16 or later, and `ConfigPath` requires 0.48.0 or later. Use a current pinned
version for the latest snapshot fixes.

## Assertions and evaluation limits

- `PredictedResources` contains flattened resources, including resources from
  nested modules. Each `SnapshotResource.Body` is the complete evaluated
  `JsonElement`, preserving all resource fields. Unresolved ARM values remain
  expressions such as `[parameters('name')]`.
- Resources with parseable ARM IDs are represented as `ArmSnapshotResource`.
  Its `Id` is an Azure Deployments `ResourceId`; `Type` and `Name` are derived
  from that ID. Missing, invalid, non-string, and unresolved IDs leave resources
  as generic `SnapshotResource` instances. This can include placeholder IDs when
  deployment context is omitted.
- `GetArmResources()` selects only `ArmSnapshotResource` instances.
  `GetArmResourceOfType(type)` additionally filters by case-insensitive fully
  qualified type. Use LINQ's `Single`, `Where`, or `GroupBy` for name, ID, or
  cross-resource assertions. Generic resources remain accessible through
  `PredictedResources`.
- `resource.GetProperty("/properties/path/0")` reads a case-sensitive RFC 6901
  JSON Pointer. The empty pointer selects the whole body; `~0` escapes `~` and
  `~1` escapes `/` in property names. Malformed pointers throw `ArgumentException`;
  missing paths throw `KeyNotFoundException`. JSON null is returned as a value.
- `GetStringProperty`, `GetIntProperty`, and `GetBoolProperty` return the
  corresponding nullable value. They return null only for JSON null, not for
  missing properties. Wrong JSON types throw `InvalidOperationException`.
  Integers must fit in a 32-bit signed integer; fractional and out-of-range
  numbers throw `FormatException`. Strings are not coerced into numbers or booleans.
- `GetRequiredStringProperty`, `GetRequiredIntProperty`, and
  `GetRequiredBoolProperty` use the same JSON Pointer and conversion rules,
  but additionally throw `JsonException` for JSON null.
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
