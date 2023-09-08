# Sample module

Sample description

## Details

The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog.
The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog.
The quick brown fox jumps over the lazy dog.

## Parameters

| Name                           | Type            | Required | Description                         |
| :----------------------------- | :-------------: | :------: | :---------------------------------- |
| `foo`                          | `null | string` | No       | Test nullable parameter types       |
| `dnsPrefix`                    | `string`        | Yes      | The dns prefix                      |
| `linuxAdminUsername`           | `string`        | Yes      | The linux administrator username    |
| `sshRSAPublicKey`              | `string`        | Yes      | The RSA public key for SSH          |
| `servicePrincipalClientId`     | `string`        | Yes      | The service principal client ID     |
| `servicePrincipalClientSecret` | `securestring`  | Yes      | The service principal client secret |
| `clusterName`                  | `string`        | No       | The cluster name                    |
| `location`                     | `string`        | No       | The deployment location             |
| `osDiskSizeGB`                 | `int`           | Yes      |                                     |
| `agentCount`                   | `int`           | No       | The agent count                     |
| `agentVMSize`                  | `string`        | No       | The agent VM size                   |

## Outputs

| Name               | Type     | Description                  |
| :----------------- | :------: | :--------------------------- |
| `controlPlaneFQDN` | `string` | The control plane FQDN       |
| `osDiskSizeGB`     | `int`    | Override default describtion |

## Examples

### Example 1

```bicep
module mod 'br/public:test/testmodule:1.1.1' = {
  name: 'mod'
  params: {
    dnsPrefix: ''
    linuxAdminUsername: ''
    sshRSAPublicKey: ''
    servicePrincipalClientId: ''
    servicePrincipalClientSecret: ''
    osDiskSizeGB: 1
  }
}
```