# Sample module

![Azure Public Test Date](https://azurequickstartsservice.blob.core.windows.net/badges/modules/Microsoft.Test/testModule/1.1/PublicLastTestDate.svg)
![Azure Public Test Result](https://azurequickstartsservice.blob.core.windows.net/badges/modules/Microsoft.Test/testModule/1.1/PublicDeployment.svg)

![Azure US Gov Last Test Date](https://azurequickstartsservice.blob.core.windows.net/badges/modules/Microsoft.Test/testModule/1.1/FairfaxLastTestDate.svg)
![Azure US Gov Last Test Result](https://azurequickstartsservice.blob.core.windows.net/badges/modules/Microsoft.Test/testModule/1.1/FairfaxDeployment.svg)

![Best Practice Check](https://azurequickstartsservice.blob.core.windows.net/badges/modules/Microsoft.Test/testModule/1.1/BestPracticeResult.svg)
![Cred Scan Check](https://azurequickstartsservice.blob.core.windows.net/badges/modules/Microsoft.Test/testModule/1.1/CredScanResult.svg)

[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-quickstart-templates%2Fmaster%2Fmodules%2FMicrosoft.Test%2FtestModule%2F1.1%2Fazuredeploy.json)
[![Deploy To Azure US Gov](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazuregov.svg?sanitize=true)](https://portal.azure.us/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-quickstart-templates%2Fmaster%2Fmodules%2FMicrosoft.Test%2FtestModule%2F1.1%2Fazuredeploy.json)
[![Visualize](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/visualizebutton.svg?sanitize=true)](http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-quickstart-templates%2Fmaster%2Fmodules%2FMicrosoft.Test%2FtestModule%2F1.1%2Fazuredeploy.json)

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


