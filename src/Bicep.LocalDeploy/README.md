# Local-only Bicep

## Pre-requisites
Install the CLI & VSCode extension:
- (Mac/Linux)
   ```powershell
   bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh) --branch ant/localdeploy
   bash <(curl -Ls https://aka.ms/bicep/nightly-vsix.sh) --branch ant/localdeploy
   ```
- (Windows)
   ```powershell
   iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) --branch ant/localdeploy"
   iex "& { $(irm https://aka.ms/bicep/nightly-vsix.ps1) --branch ant/localdeploy"
   ```

## Samples
### Local Kubernetes
If you have Docker installed, this can be obtained by [Enabling Kubernetes Support](https://docs.docker.com/desktop/kubernetes/)

Deploy using "local-deploy" Bicep CLI command:
```sh
export LOCAL_KUBECONFIG=$(cat ~/.kube/config | base64)

bicep local-deploy src/Bicep.LocalDeploy/Samples/Kubernetes/bicep-on-k8s.bicepparam
```

This will run the [bicep-on-k8s](https://github.com/anthony-c-martin/bicep-on-k8s) service locally - test it out by submitting a POST request:
```sh
curl -X POST http://localhost:80/build \
  -H 'Content-Type: application/json' \
  -d '{"bicepContents": "param foo string"}'
```

### "Wait" functionality
```sh
bicep local-deploy src/Bicep.LocalDeploy/Samples/Utils/wait.bicepparam
```

### "Assert" functionality
```sh
bicep local-deploy src/Bicep.LocalDeploy/Samples/Utils/assert.bicepparam
```

## Caveats
* Only extensible resources are supported - there is no support for Az.