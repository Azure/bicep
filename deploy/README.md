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

#### Deploy
```sh
bicep deploy --arg-subscription-id d08e1a72-8180-4ed3-8125-9dff7376b0bd --arg-resource-group ant-test deploy/samples/script/main.bicepparam
```

#### WhatIf
```sh
bicep what-if --arg-subscription-id d08e1a72-8180-4ed3-8125-9dff7376b0bd --arg-resource-group ant-test deploy/samples/script/main.bicepparam
```