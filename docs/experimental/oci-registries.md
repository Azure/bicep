# Using OCI Registries (Experimental!)

> [!NOTE]
> This feature is currently experimental while we collect feedback.

## What is it?

By default, Bicep publishes and restores modules and extensions using the Azure Container Registry (ACR) SDK. The `ociEnabled` experimental feature lets Bicep publish to and restore from **any** OCI-compliant registry, such as GitHub Container Registry (GHCR), Docker Hub, or a self-hosted registry (e.g. Harbor), using the [ORAS](https://oras.land/) transport and standard Docker credentials.

ACR registries continue to use the Azure SDK and Azure credentials as before; other registries use the ORAS transport.

## How to enable it?

Add the feature flag to your `bicepconfig.json`:

```json
{
  "experimentalFeaturesEnabled": {
    "ociEnabled": true
  }
}
```

## Publishing

Publishing and restoring generally require the target registry to be [trusted](#trusted-registries) and, for private registries, that you are [authenticated](#authentication).

Publish a **module** to any OCI registry with `bicep publish`:

```sh
bicep publish ./main.bicep --target br:ghcr.io/myorg/modules/storage:v1
```

Publish an **extension** with `bicep publish-extension` (see [Using the `publish-extension` command](./publish-extension-command.md)):

```sh
bicep publish-extension ./index.json --target br:ghcr.io/myorg/extensions/myext:v1
```

## Consuming

Reference a published module the same way you would an ACR module. Bicep restores it from the registry on compile:

```bicep
module storage 'br:ghcr.io/myorg/modules/storage:v1' = {
  name: 'storage'
  params: {
    // ...
  }
}
```

## Trusted registries

For security, Bicep only connects to (and sends credentials to) registries on a trusted allowlist. Everything else is rejected before any network call is made, so a reference to an untrusted host can never leak your credentials.

The following registries are trusted by default and cannot be removed:

- `*.azurecr.io`, `*.azurecr.cn`, `*.azurecr.us` (Azure Container Registry)
- `mcr.microsoft.com`, `mcr.azure.cn`
- `ghcr.io` (GitHub Container Registry)

> [!WARNING]
> Only add registries that you own or trust. Bicep sends your credentials to any registry on this allowlist, so adding an attacker-controlled or untrusted host could allow your registry credentials to be stolen.

To allow additional registries, set the `BICEP_TRUSTED_REGISTRIES` environment variable to a comma- (or semicolon-) separated list of hostnames. Entries may be exact hostnames or `*.suffix` wildcards:

```sh
# Mac/Linux
export BICEP_TRUSTED_REGISTRIES="harbor.contoso.com,*.contoso.io"
```

```powershell
# Windows (PowerShell)
$env:BICEP_TRUSTED_REGISTRIES = "harbor.contoso.com,*.contoso.io"
```

## Authentication

- **ACR registries** (`*.azurecr.io`, etc.) use your Azure credentials, following the `cloud.credentialPrecedence` setting in `bicepconfig.json`. No extra configuration is required.
- **Other registries** use Docker credentials. The simplest option is to log in with the Docker CLI, which writes credentials that Bicep reads from your Docker `config.json`:

  ```sh
  docker login ghcr.io
  ```

  Bicep resolves credentials per-host from `config.json` (the `auths`, `credHelpers`, and `credsStore` sections), so credentials for one registry are never sent to another.

- **CI pipelines** can instead supply credentials via environment variables. `DOCKER_USERNAME`/`DOCKER_PASSWORD` are only used when `DOCKER_REGISTRY` matches the target registry, ensuring they are never sent to an unexpected host:

  ```sh
  export DOCKER_REGISTRY="ghcr.io"
  export DOCKER_USERNAME="my-user"
  export DOCKER_PASSWORD="$MY_TOKEN"
  ```

Anonymous (unauthenticated) access is used automatically when no credentials are found, which is enough to restore public modules.

## Raising bugs or feature requests

Please raise bug reports or feature requests under [Bicep Issues](https://github.com/Azure/bicep/issues) as usual.
