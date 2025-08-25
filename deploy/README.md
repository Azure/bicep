## Goals

1. Allow users to model all properties of the deployment (including scope, stacks configuration) in one place
2. Add support for "destroy" functionality for cleaning up a Stack resource

## Demo

https://github.com/user-attachments/assets/da64f3a3-3c11-411b-94b6-6324f79af107

## Intalling

- Mac/Linux
    ```sh
    bash <(curl -Ls https://aka.ms/bicep/nightly-vsix.sh) --branch ant/poc_cli
    bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh) --branch ant/poc_cli
    ```

- Windows
    ```powershell
    iex "& { $(irm https://aka.ms/bicep/nightly-vsix.ps1) } -Branch ant/poc_cli"
    iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) } -Branch ant/poc_cli"
    ```

## Running Samples

### Script

Pre-reqs:
1. Download samples [here](https://download-directory.github.io/?url=https%3A%2F%2Fgithub.com%2FAzure%2Fbicep%2Ftree%2Fant%2Fpoc_cli%2Fdeploy%2Fsamples)
1. Unzip
1. Cd to the unzipped directory

#### CLI args
```sh
# what-if
bicep what-if --arg-subscription-id d08e1a72-8180-4ed3-8125-9dff7376b0bd --arg-resource-group ant-test script/main.bicepparam

# deploy
bicep deploy --arg-subscription-id d08e1a72-8180-4ed3-8125-9dff7376b0bd --arg-resource-group ant-test script/main.bicepparam
```

#### Env vars
Linux/Mac
```sh
export AZURE_SUBSCRIPTION_ID=d08e1a72-8180-4ed3-8125-9dff7376b0bd
export AZURE_RESOURCE_GROUP=ant-test

# what-if
bicep what-if script/env_vars.bicepparam

# deploy
bicep deploy script/env_vars.bicepparam
```

Windows
```powershell
$env:AZURE_SUBSCRIPTION_ID = "d08e1a72-8180-4ed3-8125-9dff7376b0bd"
$env:AZURE_RESOURCE_GROUP = "ant-test"

# what-if
bicep what-if script/env_vars.bicepparam

# deploy
bicep deploy script/env_vars.bicepparam
```

#### Stacks
Linux/Mac
```sh
export AZURE_SUBSCRIPTION_ID=d08e1a72-8180-4ed3-8125-9dff7376b0bd
export AZURE_RESOURCE_GROUP=ant-test

# what-if not currently supported

# deploy
bicep deploy script/stack.bicepparam

# destroy
bicep destroy script/stack.bicepparam
```

Windows
```powershell
$env:AZURE_SUBSCRIPTION_ID = "d08e1a72-8180-4ed3-8125-9dff7376b0bd"
$env:AZURE_RESOURCE_GROUP = "ant-test"

# what-if not currently supported

# deploy
bicep deploy script/stack.bicepparam

# destroy
bicep destroy script/stack.bicepparam
```