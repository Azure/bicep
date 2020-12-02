# Decompiling an ARM Template
 
> Requires Bicep CLI v0.2.59 or later
 
The Bicep CLI provides the ability to decompile any existing ARM Template to a `.bicep` file, using the `decompile` command:
```sh
bicep decompile "path/to/file.json"
```

You can use this command to get to a starting point for Bicep authoring. Note that because there is no guaranteed conversion from JSON to Bicep, decompilation may fail, or you may be left with errors/warnings in the generated Bicep file to fix up. See [Limitations](#limiations) for some details of what is not currently possible.

You can also use the "Decompile" button in the [Bicep Playground](https://aka.ms/bicepdemo)

## Exporting a resource group
You can pass an exported template directly to the `bicep decompile` command to effectively export a resource group to a `.bicep` file.

### Azure CLI
The following will create a file named 'main.bicep' in the current directory:
```sh
az group export --name "your_resource_group_name" > main.json
bicep decompile main.json
```
### Azure PowerShell
The following will create a file named 'main.bicep' in the current directory:
```powershell
Export-AzResourceGroup -ResourceGroupName "your_resource_group_name" -Path ./main.json
bicep decompile main.json
```

### Azure Portal
See [Export Template](https://aka.ms/armexport) for guidance. Use `bicep decompile <filename>` on the downloaded file.


## Current Limitations
The following are temporary limiations on the `bicep decompile` command:
* Templates using copy loops or conditionals cannot be decompiled
* Nested templates can only be decompiled if they are using ['inner' expression evaluation scope](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/linked-templates#expression-evaluation-scope-in-nested-templates).
* The 'scope' property is unsupported for extension resource declarations.