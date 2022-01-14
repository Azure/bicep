# Sample module

Sample description for test

## Parameters

| Name                           | Type           | Required | Description                         |
| :----------------------------- | :------------: | :------: | :---------------------------------- |
| `dnsPrefix`                    | `string`       | Yes      | The dns prefix                      |
| `linuxAdminUsername`           | `string`       | Yes      | The linux administrator username    |
| `sshRSAPublicKey`              | `string`       | Yes      | The RSA public key for SSH          |
| `servicePrincipalClientId`     | `string`       | Yes      | The service principal client ID     |
| `servicePrincipalClientSecret` | `secureString` | Yes      | The service principal client secret |
| `clusterName`                  | `string`       | No       | The cluster name                    |
| `location`                     | `string`       | No       | The deployment location             |
| `osDiskSizeGB`                 | `int`          | Yes      | The OS disk size (in GB)            |
| `agentCount`                   | `int`          | No       | The agent count                     |
| `agentVMSize`                  | `string`       | No       | The agent VM size                   |


## Outputs

| Name             | Type   | Description            |
| :--------------- | :----: | :--------------------- |
| controlPlaneFQDN | string | The control plane FQDN |


