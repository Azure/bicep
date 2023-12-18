# Local-only Bicep

## Samples
### Local Kubernetes
If you have Docker installed, this can be obtained by [Enabling Kubernetes Support](https://docs.docker.com/desktop/kubernetes/)

Deploy using "local-deploy" Bicep CLI command:
- (Mac/Linux)
    ```sh
    export LOCAL_KUBECONFIG=$(cat ~/.kube/config | base64)
    ./bicep-osx-arm64 local-deploy samples/Kubernetes/voting-app.bicepparam
    ```
- (Windows)
    ```powershell
    $env:LOCAL_KUBECONFIG = [convert]::ToBase64String([System.IO.File]::ReadAllBytes("$env:USERPROFILE\.kube\config"))
    ./bicep-win-x64 local-deploy samples/Kubernetes/voting-app.bicepparam
    ```

#### Voting App
Param file: `samples/Kubernetes/voting-app.bicepparam`

This will run the [Voting App Sample](https://github.com/Azure-Samples/azure-voting-app-redis) locally.

After deploying, test it out by navigating to [http://localhost](http://localhost) in a browser.

Cleanup:
```sh
kubectl delete deployment azure-vote-back
kubectl delete deployment azure-vote-front
kubectl delete service azure-vote-back
kubectl delete service azure-vote-front
```

#### Bicep Compilation Service
Param file: `samples/Kubernetes/bicep-on-k8s.bicepparam`

This will run the [bicep-on-k8s](https://github.com/anthony-c-martin/bicep-on-k8s) service locally.

Test it out by submitting a POST request:
```sh
curl -X POST http://localhost:80/build -H 'Content-Type: application/json' -d '{"bicepContents": "param foo string"}'
```

Cleanup:
```sh
kubectl delete deployment bicepbuild 
kubectl delete service bicepbuild
```

### "Wait" functionality
```sh
bicep local-deploy samples/Utils/wait.bicepparam
```

### "Assert" functionality
```sh
bicep local-deploy samples/Utils/assert.bicepparam
```

## Caveats
* Only extensible resources are supported - there is no support for Az.