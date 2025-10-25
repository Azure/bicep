# Orchestration Mode

## What is it?

Orchestration mode is an experimental Bicep feature for defining more complex orchestration of multi-step deployment workflows. Instead of deploying Azure resources directly like standard Bicep templates, it uses Deployment Stacks to break your applications into distinct components with defined lifecycles.

## Try it out
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
    "deployCommands": true,
    "orchestration": true
  }
}
```

To enable "orchestration mode" for a particular Bicep file, add the following to the top:
```bicep
targetScope = 'orchestrator'
```

## Samples

### Pre-requisites
The following commands will download the POC Nightly Bicep CLI and install it to ~/.azure/bin/bicep, download the samples to ./bicep-orchestrator.
* Mac/Linux
    ```sh
    bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh) --branch orchestrator
    git clone --single-branch --branch orchestrator https://github.com/Azure/bicep.git bicep-orchestrator --depth 1
    ```
* Windows
    ```powershell
    iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) } -Branch orchestrator"
    git clone --single-branch --branch orchestrator https://github.com/Azure/bicep.git bicep-orchestrator --depth 1
    ```

The following samples demonstrate orchestration mode capabilities:

### Basic Multi-Region Application Deployment

To run this sample:
```sh
~/.azure/bin/bicep deploy ./bicep-orchestrator/docs/experimental/orchestration-samples/basic/main.bicepparam
```

**Location**: [`basic/main.bicepparam`](./orchestration-samples/basic/main.bicepparam)

This sample deploys a containerized application across multiple Azure regions with global load balancing.

#### Components:

1. **Main Orchestrator** [`basic/main.bicep`](./orchestration-samples/basic/main.bicep):
   * Multi-region deployment using stack arrays
   * Deployment batching with configurable wait times
   * Dependency management between deployment phases
   * Conditional logic based on deployment mode (`hotfix` vs `standard`)

2. **Regional Cluster Infrastructure** [`basic/cluster/main.bicep`](./orchestration-samples/basic/cluster/main.bicep):
   * Azure Container Apps managed environments per region

3. **Application Deployment** [`basic/clusterApp/main.bicep`](./orchestration-samples/basic/clusterApp/main.bicep):
   * Containerized applications deployed to each regional cluster

4. **Global Services** [`basic/global/main.bicep`](./orchestration-samples/basic/global/main.bicep):
   * Azure Front Door with WAF and global load balancing

## Current Work
This section outlines the various different features we want to build, along with the current status.

### Validation & End-to-end
#### Goals
* It should be possible to break complex service infrastructure into multiple independent components Bicep, with high-quality static validation.
* It should be possible to run CLI commands to deploy, validate, and teardown this infrastructure.

#### Status
* Basic syntax is functional
* Orchestration works in `bicep deploy`, `bicep what-if` and `bicep teardown` commands
* Further refinements needed to improve syntax and validation

### Modeling Dependencies
#### Goals
* It should be possible to model dependencies between components, and have the orchestration adhere to these rules when determining the order to roll out changes.
* It should be possible to request the deployment of a single component, and have the orchestrator reason about the platform state to understand which pre-requisites need to be deployed.

#### Status
* Not started

### Lifecycle Rules
#### Goals
* It should be possible to define the following rules for each component:
    * Always deploy during an orchestrated rollout
    * Deploy if version changed
    * Deploy if changes detected (template/parameters hash)

#### Status
* Not started

### Health Checks
#### Goals
* It should be possible to publish and run custom code to define how to monitor health of a particular component.
* This code should be executed automatically after deploying, and health information of the component should be visible.

#### Status
* Not started

### Rollback Rules
#### Goals
* It should be possible to define automatic rollback of a component if a health check fails.
* It should be possible to define sequencing of rollback if needed - e.g. whether to roll back the full infra, or just a particular component.

#### Status
* Not started

### Bake Time
#### Goals
* It should be possible to define a period of waiting between components in different stages.
* It should be possible to check on the health state of the previous stage once the bake time is completed, before deciding whether to advance.

#### Status
* Not started

### Approval Gates
#### Goals
* It should be possible to define a workflow where orchestration will wait at a particular point for a manual approval to continue.

#### Status
* Not started

## Out of scope
### Application Lifecycle Management
#### Goals
* It should be possible to upload application code binaries, as well as just deploying infra.

### Hosting as a Service
#### Goals
* It should be possible to invoke the orchestrator via a service (similar to the deployment engine), rather than depend on a local binary. We will use the Bicep CLI to prove out the experience.