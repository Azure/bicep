# Local-only Bicep
This document explains how to set up the experimental local-only deployment support for 3rd party extensibility providers, without a dependency on Azure.

Here's an example of deploying to a local kubernetes cluster, logging, and executing a bash script via terminal:

<img width="815" alt="image" src="https://github.com/anthony-c-martin/bicep/assets/38542602/7e0c353f-7d9b-4fd0-9468-bf877680a3e0">

Here's an example of using VSCode to dpeloy to a local kubernetes cluster:

https://github.com/anthony-c-martin/bicep/assets/38542602/b9450f54-7272-418b-8c5a-9c62f122d2b4

## Installing
### Mac/Linux
```sh
# install the CLI to ~/.azure/bin/bicep
bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh) --repo anthony-c-martin/bicep
# install the VSCode Extension
bash <(curl -Ls https://aka.ms/bicep/nightly-vsix.sh) --repo anthony-c-martin/bicep
```

### Windows
```sh
# install the CLI to ~/.azure/bin/bicep
iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) } -Repo anthony-c-martin/bicep"
# install the VSCode Extension
iex "& { $(irm https://aka.ms/bicep/nightly-vsix.ps1) } -Repo anthony-c-martin/bicep"
```

## Running Samples
* Copy the full [samples](../src/Bicep.LocalDeploy/samples) folder locally. You can use [this tool](https://download-directory.github.io/?url=https%3A%2F%2Fgithub.com%2Fanthony-c-martin%2Fbicep%2Ftree%2Fmain%2Fsrc%2FBicep.LocalDeploy%2Fsamples) to download it as a zip file.
* For testing with the Kubernetes provider, you will need access to a cluster configured in your kubeconfig file. If you have Docker installed, this can be obtained by [Enabling Kubernetes Support](https://docs.docker.com/desktop/kubernetes/).

### Via CLI
Replace `<path_to_bicepparam>` with the path to the `.bicepparam` file you wish to deploy.
```sh
~/.azure/bin/bicep local-deploy <path_to_bicepparam>
```

### Via VSCode
Open the [Deployment Pane](./deploy-ui.md) for a `.bicepparam` file you wish to deploy, and press the "Deploy" button.

## Utils Samples
### Bash/PowerShell script execution ([samples/utils/script.bicepparam](../src/Bicep.LocalDeploy/samples/utils/script.bicepparam))
Execute a bash or powershell script as part of a local deployment.

### "Wait" functionality ([samples/utils/wait.bicepparam](../src/Bicep.LocalDeploy/samples/utils/wait.bicepparam))
Introduce a sleep for a given number of milliseconds in your deployment.

### Logging functionality ([samples/utils/log.bicepparam](../src/Bicep.LocalDeploy/samples/utils/log.bicepparam))
Write a log to console during your deployment to help debug.

### Assertion functionality ([samples/utils/assert.bicepparam](../src/Bicep.LocalDeploy/samples/utils/assert.bicepparam))
Fail the deployment if a certain condition is false.

## Kubernetes Samples
### Voting App ([samples/kubernetes/voting-app.bicepparam](../src/Bicep.LocalDeploy/samples/kubernetes/voting-app.bicepparam))

This will run the [Voting App Sample](https://github.com/Azure-Samples/azure-voting-app-redis) locally.

After deploying, test it out by navigating to [http://localhost](http://localhost) in a browser.

Cleanup:
```sh
kubectl delete deployment azure-vote-back
kubectl delete deployment azure-vote-front
kubectl delete service azure-vote-back
kubectl delete service azure-vote-front
```

### Simple web server ([samples/kubernetes/echo-server.bicepparam](../src/Bicep.LocalDeploy/samples/kubernetes/echo-server.bicepparam))

This will run the [echo-server](https://ealenn.github.io/Echo-Server/) service locally.

Test it out by submitting a request:
```sh
curl -I localhost:8080
```

Cleanup:
```sh
kubectl delete deployment echo-server 
kubectl delete service echo-server
```

## GitHub Samples
### Fetch Repo information ([samples/github/repo.bicepparam](../src/Bicep.LocalDeploy/samples/github/repo.bicepparam))

Fetches a repo + contributor from GitHub.

## Contributing new providers or types
I'm happy to take contributions to extend experimental providers or add new ones. Generally, this will require the following:
* A namespace type for each provider. For example, [here's](../src/Bicep.LocalDeploy/Namespaces/GithubNamespaceType.cs) how Github types are defined (search for `Repository` and `Collaborator` to see the examples). This is necessary for editor support (validation, completions etc).
* An extensibility provider. For example, [here's](../src/Bicep.LocalDeploy/Extensibility/GithubExtensibilityProvider.cs) how GitHub runtime behavior is defined (search for `Repository` and `Collaborator` to see the examples). This is necessary for actually being able to read or deploy resources.
* If adding a new extensibility provider, you must register it in the `Build()` method [here](../src/Bicep.LocalDeploy/Extensibility/LocalExtensibilityHandler.cs).

## Caveats
* There is currently no support for deploying Azure resources. Theoretically there's no reason why this can't work, I just haven't had the time to build it.
