# Hello-world sample
This bicep sample is the canonical hello world.

## Code description
This bicep file takes a `yourName` parameter and adds that to a `hello` variable and
returns the concatenated string as an ARM output.

## Deploy process
The command to deploy this bicep file is:

### PowerShell
``` PowerShell
New-AzResourceGroupDeployment -ResourceGroupName myRG -TemplateFile .\main.bicep
```

### Azure CLI
``` Bash
az deployment group create --resource-group myRG --template-file ./main.bicep
```


## Output
The output of the deployment should be:

### PowerShell
``` Powershell
PS C:\bicep\docs\examples\000\01-hello-world>$deployment = New-AzResourceGroupDeployment -ResourceGroupName myRG -TemplateFile .\main.bicep -TemplateParameterObject @{ yourName="Chris" }
PS C:\bicep\docs\examples\000\01-hello-world>$deployment.Outputs.helloWorld.value
Hello World! - Hi Chris
```

### Azure CLI
``` Bash
/c/bicep/docs/examples/000/01-hello-world (main)
az deployment group create --resource-group myRG --template-file ./main.bicep  --parameters yourName=Chris --query properties.outputs.helloWorld.value
"Hello World! - Hi Chris"

```

## Full Execution Samples
Full execution samples are included for the Hello World only.

### PowerShell
``` PowerShell
PS C:\bicep\docs\examples\000\01-hello-world> New-AzResourceGroupDeployment -ResourceGroupName myRG -TemplateFile .\main.bicep

cmdlet New-AzResourceGroupDeployment at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
yourName: Chris

DeploymentName          : main
ResourceGroupName       : myRG
ProvisioningState       : Succeeded
Timestamp               : 3/25/2021 6:49:39 PM
Mode                    : Incremental
TemplateLink            :
Parameters              :
                          Name             Type                       Value
                          ===============  =========================  ==========
                          yourName         String                     Chris

Outputs                 :
                          Name             Type                       Value
                          ===============  =========================  ==========
                          helloWorld       String                     Hello World! - Hi Chris

DeploymentDebugLogLevel :
```

### Azure CLI
``` Bash
/c/bicep/docs/examples/000/01-hello-world (main)
$ az deployment group create --resource-group myRG --template-file ./main.bicep
Please provide string value for 'yourName' (? for help): Chris
{
  "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/myRG/providers/Microsoft.Resources/deployments/main",
  "location": null,
  "name": "main",
  "properties": {
    "correlationId": "00000000-0000-0000-0000-000000000000",
    "debugSetting": null,
    "dependencies": [],
    "duration": "PT1.4204367S",
    "error": null,
    "mode": "Incremental",
    "onErrorDeployment": null,
    "outputResources": [],
    "outputs": {
      "helloWorld": {
        "type": "String",
        "value": "Hello World! - Hi Chris"
      }
    },
    "parameters": {
      "yourName": {
        "type": "String",
        "value": "Chris"
      }
    },
    "parametersLink": null,
    "providers": [],
    "provisioningState": "Succeeded",
    "templateHash": "3401454988920660610",
    "templateLink": null,
    "timestamp": "2021-03-25T18:50:59.790800+00:00",
    "validatedResources": null
  },
  "resourceGroup": "bicepTesting",
  "tags": null,
  "type": "Microsoft.Resources/deployments"
}

```
