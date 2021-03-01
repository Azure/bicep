# Deploying a bicep file

Bicep files can be directly deployed via the Az CLI or PowerShell Az module, so the standard deployment commands (i.e. `az deployment group create` or `New-AzResourceGroupDeployment`) will "just work" with a `.bicep` file.

## Deploy bicep file to a resource group

>**Note:** make sure you update th default value of the `name` parameter to be globally unique.

Via **Az CLI**:

```bash
az deployment group create -f ./main.bicep -g my-rg
```

Via **Azure PowerShell**:

```powershell
New-AzResourceGroupDeployment -TemplateFile ./main.bicep -ResourceGroupName my-rg
```

## Deploy with parameters

Our bicep file exposed two parameters that we can override (`location` and `name`)

**Note:** make sure you update your storage account `name` to be globally unique.

### Pass parameters on the command line

CLI:

```bash
az deployment group create -f ./main.bicep -g my-rg --parameters location=westus name=logstorage001
```

PowerShell:

```powershell
New-AzResourceGroupDeployment -TemplateFile ./main.bicep -ResourceGroupName my-rg -location westus -name logstorage001
```

### Use a local parameters JSON file

Bicep supports providing all of the parameters for a template via a JSON file. There is no new "bicep style" syntax for passing parameters. It is the same parameters file you would use for a template deployment. You can take a look at [this parameters tutorial](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-tutorial-use-parameter-file?tabs=azure-powershell) to learn how the parameters file is structured. Once you have your parameters file, you can pass it to the deployment command-line tools:

CLI:

```bash
az deployment group create -f ./main.bicep -g my-rg --parameters ./parameters.main.json
```

PowerShell:

```powershell
New-AzResourceGroupDeployment -TemplateFile ./main.bicep -ResourceGroupName my-rg -TemplateParameterFile ./parameters.main.json
```

## Deploy to non-resource group scopes

By default, Bicep files are set to be deployed to the resource group scope. However, you can deploy templates to any scope by specifying the `targetScope` property. See [Resource Scopes spec](../spec/resource-scopes.md) for details.

## Next steps

The next tutorial will walk you through the basics of using expressions like functions, string interpolation, and ternary operators in bicep.

[3 - Using expressions](./03-using-expressions.md)
