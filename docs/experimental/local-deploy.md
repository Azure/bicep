# Using Local Deploy (Experimental!)

## What is it?
Bicep Local Deploy can be used to author Bicep files which use Bicep extensions that are designed to run fully locally, without the need for an Azure connection.

Some examples of experimental extensions that have been created:
* [GitHub](https://github.com/anthony-c-martin/bicep-ext-github): Manage GitHub resources.
* [Local](https://github.com/anthony-c-martin/bicep-ext-local): Run bash or powershell scripts locally.
* [Http](https://github.com/anthony-c-martin/bicep-ext-http): Make HTTP requests.
* [KeyVault data plane](https://github.com/anthony-c-martin/bicep-ext-keyvault): Manage KeyVault data plane operations (secrets, certificates etc).
* [Kubernetes](https://github.com/anthony-c-martin/bicep-ext-kubernetes): Manage Kubernetes resources directly.

These extensions can be combined as you wish - for example, you could:
* Read kubernetes config using a bash script and deploy Kubernetes resources with the kubernetes extension
* Fetch secrets from KeyVault and upload them as GitHub secrets.

This feature is currently experimental while we collect feedback.

## Using

### Interactive (Deploy Pane)
1. Open a `.bicepparam` file in your editor.
1. Press the Deployment Pane button visible in the top right of your editor window.

    ![](../images/localdeploy-deploypane-button.png)
1. Press the `Deploy` buttons to deploy locally.

    ![](../images/localdeploy-deploypane-ui.png)

### Via CLI
1. Run:
    ```sh
    bicep local-deploy <path_to_bicepparam_file>
    ```

## Building your own extension
Use one of the example repositories linked above as a starting point for creating your own extension.

More detailed information for implementation guidance will be added here shortly.

## Limitations
1. Currently, it is not possible to mix and match Local and Azure resources in a single deployment. Please raise an issue if you would like to see this implemented.
1. Code signing for the proof-of-concept extensions has not been implemented, meaning you may run into errors running the samples on a Mac.

## Raising bugs or feature requests
Please raise bug reports or feature requests under [Bicep Issues](https://github.com/Azure/bicep/issues) as usual.
