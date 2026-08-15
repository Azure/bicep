# Storage Module

Creates a storage account with example telemetry and diagnostics settings.

## Navigation

- [Resource Types](#resource-types)
- [Usage Examples](#usage-examples)
- [Parameters](#parameters)
- [Exported Functions](#exported-functions)
- [Outputs](#outputs)
- [Cross-referenced Modules](#cross-referenced-modules)
- [Data Collection](#data-collection)

## Resource Types

| Resource Type | Existing |
| :-- | :-- |
| `Microsoft.Network/virtualNetworks@2023-09-01` | Yes |
| `Microsoft.Storage/storageAccounts@2023-01-01` | No |

## Usage Examples

### Example 1: _default_

Deploys the module with default settings.

```bicep
// Deploys the module with default settings.
module example '../../main.bicep' = {
  name: 'example'
}
```

## Parameters

| Name | Type | Required | Description |
| :-- | :-- | :-- | :-- |
| `adminPassword` | `securestring` | Yes | Administrator password for the jumpbox. |
| `enableTelemetry` | `bool` | No | Enables usage telemetry for this module. |
| `location` | `string` | No | Azure region for the resources. |
| `networkRule` | `object` | No | Network rule configuration for the storage account. |
| `retentionInDays` | `int` | No | Number of days to retain diagnostic logs. |
| `skuName` | `string` | No | Storage account SKU name. |
| `storageAccountName` | `string` | Yes | Name of the storage account. |

### `adminPassword`

- Secure: Yes

### `enableTelemetry`

- Default value: `true`

### `location`

- Default value: `'westus'`

### `networkRule`

- Default value:

```bicep
{
  type: 'allowAll'
}
```

- Discriminator: `type`
  - `allowAll`:
    - `type` (`string`), required
      - Allowed values: `allowAll`
  - `ipRestricted`:
    - `allowedIpRanges` (`array`), required: Allowed IP ranges in CIDR notation.
    - `type` (`string`), required
      - Allowed values: `ipRestricted`

### `retentionInDays`

- Default value: `30`

- Min value: 1

- Max value: 365

### `skuName`

- Default value: `'Standard_LRS'`

- Allowed values: `Standard_GRS`, `Standard_LRS`

### `storageAccountName`

- Min length: 3

- Max length: 24

## Exported Functions

### `buildTags`

Builds a resource tag object from an environment name.

Returns: `object`

| Name | Type | Description |
| :-- | :-- | :-- |
| `environmentName` | `string` |  |

## Outputs

| Name | Type | Description |
| :-- | :-- | :-- |
| `storageAccountId` | `string` | The resource ID of the storage account. |

## Cross-referenced Modules

| Symbolic Name | Path | Description |
| :-- | :-- | :-- |
| `logging` | `modules/logging.bicep` |  |

## Data Collection

This module uses the `enableTelemetry` parameter to report anonymized module usage to Microsoft, in support of continued investment in the Bicep and Azure Verified Modules ecosystems. No resource-specific data is collected.
