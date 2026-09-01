# Comprehensive Module

Exercises every documentation feature | with multiline details.
Second line.

## Navigation

- [Resource Types](#resource-types)
- [Usage Examples](#usage-examples)
- [Parameters](#parameters)
- [Exported Types](#exported-types)
- [Exported Variables](#exported-variables)
- [Exported Functions](#exported-functions)
- [Outputs](#outputs)
- [Cross-referenced Modules](#cross-referenced-modules)

## Resource Types

| Resource Type | Existing |
| :-- | :-- |
| `Microsoft.Resources/resourceGroups@2024-03-01` | No |
| `Microsoft.Resources/resourceGroups@2024-03-01` | Yes |

## Usage Examples

### Example 1: _default_

Deploys the module with its default settings.

```bicep
targetScope = 'subscription'

metadata description = 'Deploys the module with its default settings.'

module example '../../main.bicep' = {
  name: 'example'
  params: {
    resourceGroupName: 'example-rg'
    secret: 'example'
  }
}
```

### Example 2: _restricted_

Exercises restricted network access.

```bicep
metadata description = 'Exercises restricted network access.'

module test '../../../main.bicep' = {
  name: 'test'
  params: {
    resourceGroupName: 'restricted-rg'
    secret: 'example'
    networkAccess: {
      kind: 'restricted'
      allowedCidrs: [
        '10.0.0.0/24'
      ]
    }
  }
}
```

## Parameters

| Name | Type | Required | Description |
| :-- | :-- | :-- | :-- |
| `enableTelemetry` | `bool` | No | Enables anonymous usage telemetry. |
| `location` | `string` | No | Deployment location. |
| `names` | `array` | No | Names assigned to the deployment. |
| `networkAccess` | `object` | No | Network access configuration. |
| `resourceGroupName` | `string` | Yes | Resource group \| name. Second line. |
| `retentionInDays` | `int` | No | Retention period in days. |
| `secret` | `securestring` | Yes | Secret used by the child module. |
| `settings` | `object` | No | Nested settings. |
| `tier` | `string` | No | Deployment tier. |

### `enableTelemetry`

- Default value: `false`

### `location`

- Default value: `deployment().location`

### `names`

- Default value:

```bicep
[
  'default'
]
```

- Min length: 1

- Max length: 5

### `networkAccess`

- Default value:

```bicep
{
  kind: 'public'
}
```

- Discriminator: `kind`
  - `public`:
    - `kind` (`string`), required
      - Allowed values: `public`
  - `restricted`:
    - `allowedCidrs` (`array`), required: Allowed CIDR ranges.
    - `kind` (`string`), required
      - Allowed values: `restricted`

### `resourceGroupName`

- Min length: 3

- Max length: 90

### `retentionInDays`

- Default value: `30`

- Min value: 1

- Max value: 365

### `secret`

- Secure: Yes

### `settings`

- Default value:

```bicep
{
  enabled: true
  labels: {
    environment: 'test'
  }
}
```

- Properties:
  - `enabled` (`bool`), required: Whether the feature is enabled.
  - `labels` (`object`), required: Labels applied to resources.
    - `environment` (`string`), required: Environment label.

### `tier`

- Default value: `'Standard'`

- Allowed values: `Premium`, `Standard`

## Exported Types

| Name | Type | Description |
| :-- | :-- | :-- |
| `networkAccessType` | `object` |  |
| `publicAccessType` | `object` |  |
| `restrictedAccessType` | `object` |  |
| `settingsType` | `object` | Nested module settings. |

### `networkAccessType`

- Discriminator: `kind`
  - `public`:
    - `kind` (`string`), required
      - Allowed values: `public`
  - `restricted`:
    - `allowedCidrs` (`array`), required: Allowed CIDR ranges.
    - `kind` (`string`), required
      - Allowed values: `restricted`

### `publicAccessType`

- Properties:
  - `kind` (`string`), required
    - Allowed values: `public`

### `restrictedAccessType`

- Properties:
  - `allowedCidrs` (`array`), required: Allowed CIDR ranges.
  - `kind` (`string`), required
    - Allowed values: `restricted`

### `settingsType`

- Properties:
  - `enabled` (`bool`), required: Whether the feature is enabled.
  - `labels` (`object`), required: Labels applied to resources.
    - `environment` (`string`), required: Environment label.

## Exported Variables

| Name | Type | Description |
| :-- | :-- | :-- |
| `defaultDeploymentPrefix` | `string` | The default deployment prefix. |

### `defaultDeploymentPrefix`

- Allowed values: `sample`

## Exported Functions

### `buildDisplayName`

Builds a display name.

Returns: `string`

| Name | Type | Description |
| :-- | :-- | :-- |
| `prefix` | `string` |  |

## Outputs

| Name | Type | Description |
| :-- | :-- | :-- |
| `existingResourceGroupId` | `string` | The existing resource group ID. |
| `resourceGroupId` | `string` | The deployed resource group ID. |
| `secureValue` | `securestring` | A secure output used to exercise output metadata. |

## Cross-referenced Modules

| Symbolic Name | Path | Description |
| :-- | :-- | :-- |
| `logging` | `modules/logging.bicep` | Configures diagnostic logging. |
