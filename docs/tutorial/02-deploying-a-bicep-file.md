# Deploying a bicep file

Bicep files **cannot** yet be directly deployed via the Az CLI or PowerShell Az module. You must first compile the bicep file with `bicep build` then deploy via deployment commands (`az deployment group create` or `New-AzResourceGroupDeployment`).

## Compile your bicep file and deploy the template to a resource group

Let's start by compiling and deploying the `main.bicep` file that we've been working with. I can do this via Azure PowerShell or Az CLI. For Powershell be sure to have the Az module installed to follow this tutorial by running `Install-Module -Name Az -AllowClobber` for [CLI download the correct version](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)

**Note:** make sure you update your storage account `name` to be globally unique.

Via **Azure PowerShell**:

```powershell
bicep build ./main.bicep # generates main.json
New-AzResourceGroup -Name my-rg -Location eastus # optional - create resource group 'my-rg'
New-AzResourceGroupDeployment -TemplateFile ./main.json -ResourceGroupName my-rg
```

Via **Az CLI**:

```bash
bicep build ./main.bicep # generates main.json
az group create -n my-rg -l eastus # optional - create resource group 'my-rg'
az deployment group create -f ./main.json -g my-rg
```

## Deploy with parameters

In our 0.2 release, there is **no** new "bicep-style" of authoring a parameters file. Since the compiled bicep file is a standard JSON ARM Template, parameters can be passed to the template in the same ways you are likely already used to.

Our bicep file exposed two parameters that we can override (`location` and `name`)

**Note:** make sure you update your storage account `name` to be globally unique.

### Pass parameters on the command line

CLI:

```bash
az deployment group create -f ./main.json -g my-rg --parameters location=westus name=logstorage001
```

PowerShell:

```powershell
New-AzResourceGroupDeployment -TemplateFile ./main.json -ResourceGroupName my-rg -location westus -name logstorage001
```

### Use a local parameters JSON file

ARM Templates support providing all of the parameters for a template via a JSON file. You can take a look at [this parameters tutorial](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-tutorial-use-parameter-file?tabs=azure-powershell) to learn how the parameters file is structured. Once you have your parameters file, you can pass it to the deployment command-line tools:

CLI:

```bash
az deployment group create -f ./main.json -g my-rg --parameters ./parameters.main.json
```

PowerShell:

```powershell
New-AzResourceGroupDeployment -TemplateFile ./main.json -ResourceGroupName my-rg -TemplateParameterFile ./parameters.main.json
```

## Deploy to non-resource group scopes

Bicep will compile a template that uses the resource group `$schema` by default:

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"
  ...
}
```

However, you can deploy this template to any scope with the [correct command](https://docs.microsoft.com/azure/azure-resource-manager/templates/deploy-to-subscription). Note that this is a temporary workaround until Bicep has better support for deploying resources to any scope in Azure.

## Next steps

The next tutorial will walk you through the basics of using expressions like functions, string interpolation, and ternary operators in bicep.

[3 - Using expressions](./03-using-expressions.md)
