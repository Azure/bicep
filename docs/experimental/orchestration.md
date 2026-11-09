# Orchestration Mode

## What is it?

Orchestration mode is an experimental Bicep feature for defining more complex orchestration of multi-step deployment workflows. Instead of deploying Azure resources directly like standard Bicep templates, it uses Deployment Stacks to break your applications into distinct components with defined lifecycles.

## Try it out
### Nightly Builds
To preview this, use the following scripts to install a [nightly build](https://github.com/Azure/bicep/blob/main/docs/installing-nightly.md):

* Mac/Linux
    ```sh
    # Bicep VSCode Extension
    bash <(curl -Ls https://aka.ms/bicep/nightly-vsix.sh) --branch ant/orchestrator_demo
    # Bicep CLI
    bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh) --branch ant/orchestrator_demo
    ```
* Windows
    ```powershell
    # Bicep VSCode Extension
    iex "& { $(irm https://aka.ms/bicep/nightly-vsix.ps1) } -Branch ant/orchestrator_demo"
    # Bicep CLI
    iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) } -Branch ant/orchestrator_demo"
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
    bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh) --branch ant/orchestrator_demo
    git clone --single-branch --branch ant/orchestrator_demo https://github.com/Azure/bicep.git bicep-orchestrator --depth 1
    ```
* Windows
    ```powershell
    iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) } -Branch ant/orchestrator_demo"
    git clone --single-branch --branch ant/orchestrator_demo https://github.com/Azure/bicep.git bicep-orchestrator --depth 1
    ```

The following samples demonstrate orchestration mode capabilities:

### Basic Multi-Region Application Deployment

To run this sample:
```sh
~/.azure/bin/bicep orchestrate deploy ./bicep-orchestrator/docs/experimental/orchestration-samples/basic/main.bicepparam
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

## Selecting Stacks by Label

Stacks can declare optional string-valued labels. Labels describe the topology; they are not Azure resource tags. A label named `region` does not set the deployment location: keep the stack's top-level `region` property.

```bicep
stack app './app.bicepparam' = {
  region: 'westus'
  labels: {
    kind: 'frontend'
    region: 'westus'
  }
  inputs: {}
  requires: [cluster]
}
```

Use `--label` with any orchestration command:

```sh
bicep orchestrate deploy main.bicepparam --label kind=frontend --label region=westus,eastus
bicep orchestrate what-if main.bicepparam --label kind=frontend --label region=westus
bicep orchestrate teardown main.bicepparam --label kind=frontend --label region=westus
```

* Different keys are ANDed; comma-separated values for a key are ORed. Repeating a key merges its accepted values.
* Keys and values are case-sensitive. Missing labels do not match. Invalid selectors and selections matching no active stacks fail before Azure operations begin.
* Labels may use parameter and loop expressions, but must be evaluable before deployment. Each collection instance is matched independently; conditions and empty collections are respected.
* Deploy and what-if include all transitive prerequisites of matching stacks, even when those prerequisites do not match the labels. A whole collection in `requires` includes all its active instances. Prerequisites are not skipped based on previous deployments.
* Filtered teardown includes only matching stacks, not their prerequisites. It fails if an unselected active stack depends on a selected stack. Selected dependents are deleted before their prerequisites; a deletion failure stops further batches.
* Selection and teardown safety use the topology in the supplied source and parameters, not historical deployment state. Stacks absent from that topology cannot be detected.
* Without `--label`, commands operate on all active stacks. What-if visits prerequisites before dependents; teardown deletes dependents before prerequisites.

## Local Simulation

Set `BICEP_ORCHESTRATION_SIMULATION_STATE` to a JSON file path to simulate orchestration Azure operations:

```sh
export BICEP_ORCHESTRATION_SIMULATION_STATE="$PWD/simulation-state.json"
bicep orchestrate deploy main.bicepparam --label region=westus
bicep orchestrate what-if main.bicepparam --label region=westus
bicep orchestrate teardown main.bicepparam --label region=westus
unset BICEP_ORCHESTRATION_SIMULATION_STATE
```

When the variable is unset, commands use Azure normally. An empty value is an error. Simulation is limited to resource-group stacks and inline nested deployments. It uses the deployment snapshot engine to predict resources and outputs from the template, parameters, and deployment scope metadata, and uses the same orchestration dependencies and label selection. The snapshot engine expands loops, conditions, and nested deployments; nested deployment wrapper resources are not persisted. What-if reads current resources from local state, compares them with predictions using the snapshot differ, and renders resource and property-level changes with the same formatter as Azure what-if (`+` create, `~` modify, `-` delete, `=` unchanged), without writing state. Detached resources are left unchanged and omitted from the report. Teardown and resources removed from a stack honor its delete/detach configuration.

Provider selection and state-file lifetime are managed by the ARM deployment provider factory. The orchestration command uses `IArmDeploymentProvider` for deployment, what-if, and teardown without checking whether ARM is simulated. The selected provider is shared with nested deployments for the invocation; its state-file lock is released on success or failure.

State is a flat JSON array of resource objects, each with an `id` property. IDs are matched case-insensitively and must be unique. An absent file starts as an empty array (`[]`); its parent directory must exist. Writes replace the file atomically, and a companion `.lock` file prevents concurrent simulation processes from updating the same state. Snapshot diagnostics or unresolved resource IDs fail the stack without committing its resource changes, although already-created resource groups and successful stacks remain.

Azure-generated values are **not invented**. Azure-dependent expressions (such as `reference()` and `listKeys()`) are preserved as unresolved expressions in predicted properties and outputs. Local state is used for comparisons and lifecycle tracking, not to evaluate reference or list responses. For example, a predicted resource can contain:

```json
[
  {
    "id": "/subscriptions/example/resourceGroups/apps/providers/Microsoft.Resources/tags/default",
    "name": "default",
    "type": "Microsoft.Resources/tags",
    "apiVersion": "2021-04-01",
    "properties": {
      "tags": {
        "endpoint": "[reference('/subscriptions/example/resourceGroups/apps/providers/Microsoft.App/containerApps/app', '2024-03-01').configuration.ingress.fqdn]"
      }
    }
  }
]
```

Deploy replaces each managed resource's local record with its snapshot prediction. Existing resources are not managed or deleted. Simulation does not emulate Azure validation, deny assignments, generated credentials, or service-specific behavior; unresolved outputs passed to downstream stacks remain expressions, not real Azure results. External extensions and linked templates are unsupported. Module restore may still access registries; use cached modules with `--no-restore` for offline runs.

Stack parameter processing errors are reported with their underlying cause before any resource-group or stack state is written. Object-valued `stack.setting` inputs support property access in parameter assignments.

**Security:** state stores plaintext properties, parameters, and outputs, potentially including secrets. Keep it outside source control. On Unix, generated state files are owner-readable/writable only.

## Current Work
This section outlines the various different features we want to build, along with the current status.

### Validation & End-to-end
#### Goals
* It should be possible to break complex service infrastructure into multiple independent components Bicep, with high-quality static validation.
* It should be possible to run CLI commands to deploy, validate, and teardown this infrastructure.

#### Status
* Basic syntax is functional
* Orchestration works in `bicep orchestrate deploy`, `bicep orchestrate what-if` and `bicep orchestrate teardown` commands
* Further refinements needed to improve syntax and validation

### Modeling Dependencies
#### Goals
* It should be possible to model dependencies between components, and have the orchestration adhere to these rules when determining the order to roll out changes.
* It should be possible to request the deployment of a single component, and have the orchestrator reason about the platform state to understand which pre-requisites need to be deployed.

#### Status
* `requires` models individual stack, indexed stack, and whole-collection dependencies.
* Labels select subsets, with transitive prerequisite inclusion for deploy and what-if.
* Reasoning about previously deployed platform state is not yet implemented.

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