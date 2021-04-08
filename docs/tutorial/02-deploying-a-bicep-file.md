# Deploying a Bicep file

Bicep files can be directly deployed via the Az CLI or PowerShell Az module, so the standard deployment commands (i.e. `az deployment group create` or `New-AzResourceGroupDeployment`) will "just work" with a `.bicep` file. You will need Az CLI version 2.20.0+ or PowerShell Az module version 5.6.0+.

## Deploy Bicep file to a resource group

**Az CLI**:

```bash
az deployment group create -f ./main.bicep -g my-rg
```

**Azure PowerShell**:

```powershell
New-AzResourceGroupDeployment -TemplateFile ./main.bicep -ResourceGroupName my-rg
```

>**Note:** make sure you update the default value of the `storageAccountName` parameter to be globally unique before deploying.

## Deploy with parameters

Our Bicep file exposed two parameters that we can be optionally overridden (`location` and `storageAccountName`) by passing new values at deployment time.

### Pass parameters on the command line

**Az CLI**:

```bash
az deployment group create -f ./main.bicep -g my-rg --parameters location=westus storageAccountName=uniquelogstorage001
```

**Azure PowerShell**:

```powershell
New-AzResourceGroupDeployment -TemplateFile ./main.bicep -ResourceGroupName my-rg -location westus -storageAccountName uniquelogstorage001
```

### Use a local parameters JSON file

Bicep supports providing all of the parameters for a template via a JSON file. There is no new "Bicep style" syntax for passing parameters. It is the same parameters file you would use for an ARM template deployment. You can take a look at [this parameters tutorial](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-tutorial-use-parameter-file?tabs=azure-powershell) to learn how the parameters file is structured. 

Once you have your parameters file, you can pass it to the deployment command-line tools:

**Az CLI**:

```bash
az deployment group create -f ./main.bicep -g my-rg --parameters ./parameters.json
```

**Azure PowerShell**:

```powershell
New-AzResourceGroupDeployment -TemplateFile ./main.bicep -ResourceGroupName my-rg -TemplateParameterFile ./parameters.json
```

## Deploy to non-resource group scopes

By default, Bicep files are set to be deployed to the resource group scope. You can deploy templates to any other scope in Azure (subscription, management group, or tenant) by specifying the `targetScope` property. See [Resource Scopes spec](../spec/resource-scopes.md) for details. Note that you will need to use the appropriate deployment command for the scope you are targeting. For example, if I set `targetScope` to `managementGroup`, I will need to use the Az CLI command `az deployment mg create ...`.

## Next steps

The next tutorial will walk you through the basics of using expressions like functions, string interpolation, and ternary operators in Bicep.

[3 - Using expressions](./03-using-expressions.md)
