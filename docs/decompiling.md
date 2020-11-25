# Decompiling an ARM Template
 
> Requires Bicep CLI v0.2.59 or later
 
The Bicep CLI provides the ability to decompile any existing ARM Template to a `.bicep` file, using the `decompile` command:
```sh
bicep decompile "path/to/file.json"
```

You can use this command to get to a starting point for Bicep authoring. Note that because there is no guaranteed conversion from JSON to Bicep, decompilation may fail, or you may be left with errors/warnings in the generated Bicep file to fix up. See [Limitations](#limiations) for some details of what is not currently possible.

## Exporting a resource group
You can pass an exported template directly to the `bicep decompile` command to effectively export a resource group to a `.bicep` file.

### Azure CLI
```sh
az group export --name "your_resource_group_name" > main.json
bicep decompile main.json
```

### Azure PowerShell
```powershell
Export-AzResourceGroup -ResourceGroupName "your_resource_group_name" -Path ./main.json
bicep decompile main.json
```

## Limitations
* Templates using copy loops or conditionals cannot be decompiled
* Nested templates cannot be decompiled
* Cross-scope linked templates cannot be decompiled
