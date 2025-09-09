# Orchestration Mode

## Using this
### Nightly Builds
To preview this, use the following scripts to install a [nightly build](https://github.com/Azure/bicep/blob/main/docs/installing-nightly.md):

* Mac/Linux
    ```sh
    # Bicep VSCode Extension
    bash <(curl -Ls https://aka.ms/bicep/nightly-vsix.sh) --branch orchestrator
    # Bicep CLI
    bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh) --branch orchestrator
    ```
* Windows
    ```powershell
    # Bicep VSCode Extension
    iex "& { $(irm https://aka.ms/bicep/nightly-vsix.ps1) } -Branch orchestrator"
    # Bicep CLI
    iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) } -Branch orchestrator"
    ```
### Bicep changes
The changes are behind an experimental feature flag, so you will also need to ensure that you have a `bicepconfig.json` file with the following:
```json
{
  "experimentalFeaturesEnabled": {
    "orchestration": true
  }
}
```

To enable "orchestration mode" for a particular Bicep file, add the following to the top:
```bicep
targetScope = 'orchestrator'
```

## Samples

See [samples](./samples) for some usage examples.

## Notes

See [notes.md](./notes.md) for rough notes on this feature.
