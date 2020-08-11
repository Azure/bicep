# Setup your bicep development environment

## Install the CLI and VS Code extension

To write and compile bicep files, you will need the cross-platform **bicep cli**, and in order to get the best authoring experience you can install the **bicep vs code extension**. Follow the [installation instructions](../installing.md) to get both of these installed, then return here to verify the installation and proceed with the tutorial.

## Validate the install bicep CLI

Validate that the cli is running by creating a blank file `main.arm` and then running:

```bash
bicep build main.arm
```

You should get an output json file of the same name in your current directory -- in this case `main.json`. It should be a skeleton ARM JSON template:

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {},
  "functions": [],
  "variables": {},
  "resources": [],
  "outputs": {}
}
```

## Verify the Bicep VS Code extension (Language service)

Open the `main.arm` file in VS code. If the extension is installed, you should see syntax highlighting working and you should see the language in the lower right hand corner of the VS code window change to `bicep`.

## Next steps

In the next tutorial, we will start working with the basic bicep syntax:

[1 - Working with a basic bicep file](./01-simple-template.md)
