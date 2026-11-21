# Local orchestration viewer (experimental)

One local .NET 10 process serves React assets and SDK Streamable HTTP MCP.
The browser and existing Copilot chat call the same MCP tools; there are no
parallel browser REST endpoints. No cloud hosting, database, deployment on startup,
embedded editor, or embedded chat. Existing CLI and stdio MCP entry points are
unchanged. This is a proof of concept, not a production deployment controller.

## Run

Prerequisites: repository-pinned .NET SDK, Node.js 22.12+ with npm, PowerShell 7+.
From the repository root:

```sh
pwsh src/Bicep.Orchestration.Local/run.ps1
```

This installs locked frontend dependencies, typechecks/builds React, builds the
host, and runs it **in the foreground**. Open `http://127.0.0.1:5088`. The host
connects the viewer automatically, without an access token or login step.

Defaults:

- Workspace: current directory.
- Entry: `docs/experimental/orchestration-samples/basic/main.bicepparam`.
- Target: **JSON SIMULATION**, `.azure/local-demo-state.json`.
- No state file means empty observed topology; prediction does not create it.
- Compiler module restore may download the sample's public AVM dependencies.
  Simulation never calls ARM or starts external deployment extensions.

The viewer and its MCP tools always use JSON simulation, never real Azure.
There is no target switch or `set_target` tool, and `--real` is rejected.
Activity and topology refresh automatically every second in all four views.
Slow refreshes do not overlap; errors are displayed and retried on later ticks.
Refreshes pause during prediction/submission; during deployment they read the
latest completed-stack cache without blocking simulated stack execution.

Launch overrides (passed after `run.ps1`, or directly to the built DLL):

```sh
pwsh src/Bicep.Orchestration.Local/run.ps1 --port 5089 --state .azure/local-demo-other.json
# Fixed source/workspace chosen locally, never through an API:
pwsh src/Bicep.Orchestration.Local/run.ps1 --workspace /path/to/repo --entry path/to/main.bicepparam
```

Stop with Ctrl+C. Shutdown cancels active work, joins it, releases provider state
locks. Plans,
operations, pending approvals, and event history are in memory (up to 100 each), not durable.
After a crash, inspect simulation state before starting again.

For a faster subsequent launch after building:

```sh
dotnet src/Bicep.Orchestration.Local/bin/Debug/net10.0/Bicep.Orchestration.Local.dll
```

## Use

The header navigation switches between **Deploy**, **Approvals**, **Active Deployments**, and
**Service Health**. Switching views preserves the current plan, selectors, stack
selection, and operation; polling continues in every view.
The viewer uses local JSON simulation, not Azure deployments. Stage grouping is
illustrative; batching, health checks, and policy validation are not enforced.
The inspector is closed initially. Selecting a stack opens it; **Close** reclaims
the canvas without clearing the selection. **Inspect selected stack** reopens it.

Every matrix cell and the inspector show the current version from the observed
deployment stack's `version` tag. Add an optional `version` string to a stack
declaration (1–256 characters) to write this tag during deployment:

```bicep
stack app './app.bicepparam' = {
  version: '1.2.3'
  region: 'westus'
  deploy: 'always'
  inputs: {}
}
```

The tag is stored in both JSON simulation and Azure. Untagged stacks show
**Unknown**; absent stacks show **Not deployed**. Source edits and predictions
do not change the displayed current version. What-If reports managed resource
changes only, not the deployment stack itself or its version tag. A version-only
edit leaves resource predictions unchanged; approving and deploying still updates the tag.
Omitting `version` from a later deployment removes the tag.

The basic sample sets every stack's version from a `version` parameter
(default `1.0.0`). The Deployments RP sample uses `artifacts.version` for every
stack. Existing untagged simulation state needs a newly approved deployment to record it.

When the entry point provides `stageMappings` (directly or under `config`) as
ordered `{ name, regions }` groups, the matrix groups region rows under those
stage names. Unmapped regions appear under **Unassigned**. The Deployments RP
sample's `Batching` rule and parameter file are the authoring example; the local
viewer displays these stages but does not enforce rollout batching.

1. **Source:** edit Bicep in VS Code; use existing Copilot chat there.
2. **Predict (Deploy):** predict changes against the automatically refreshed topology, optionally using
   **Add filter** to select a label and search its distinct values. Choose one or
   more values and **Apply filter**. Filter chips can be edited or removed;
   **Clear filters** selects all stacks. Cancel or Escape discards draft edits.
   The supported operator is **Equals**: values within one label are OR'd, and
   different label filters are AND'd (for example `kind=app region=eastus,westus`).
   Prerequisites are included. Filters select deployment scope; they do not hide
   matrix cells. Available labels and values come from the full topology, not
   the visible region rows. Shared MCP plans populate the same filter chips.
   Region filtering is a visual filter, not an execution selector.
3. **Inspect:** Deploy and both monitoring views use a compact matrix grouping stacks by region (rows) and declaration
   (columns, with loop indexes combined as `[*]`). Columns are not execution
   stages; exact dependency links are shown in the inspector. Multiple instances
   in the same region and declaration remain individually selectable.
   The matrix scrolls with sticky headers; no sample-specific groups are inferred.
   Observed resources are separate from the desired topology.
   Predicted changes are always shown in Deploy; unaffected cells remain as muted context, and
   stacks outside the predicted plan have a dashed border and explain that they are not in the
   deployment scope. Selecting one explains how to adjust the filters and predict again.
   The prediction summary reports the number of resource changes across planned stacks, with
   create/modify/delete counts and unchanged results. Ignored, unsupported, or other uncertain
   results are called out separately rather than presented as a clean prediction.
   The plan distinguishes stacks selected directly by labels from prerequisites, and identifies
   the in-plan stacks that directly require each prerequisite. These reasons are captured in the
   plan and remain available during approval, even if source or topology subsequently changes.
   After predicting, **Hide stacks outside deployment scope** limits the Deploy view to the
   predicted stacks, including prerequisites, in both layouts. It is a visual filter only;
   it does not change the plan, and resets when the prediction is cleared.
   Switch from **Regions** to **Dependencies** for a zoomed-out diagram grouped by stack
   declaration. Arrows point from prerequisites to dependents; a grouped arrow means at least
   one instance has that dependency, not that every instance does. Select a group and then an
   instance to inspect its exact dependencies, status, version, and resources. The relationship
   list provides a text alternative to the diagram. Group details and the stack inspector link
   to the `requires` expression in local VS Code when present.
   Links refer to the current source, not a stored copy of an earlier plan.
   Select cells for prerequisite links, managed resource IDs, and property-level
   delta inspection. Create, modify, delete, and no-change are distinct from
   execution status. Predicted creates, modifications, and deletions use green,
   amber, and red accents respectively; labels still identify each change. A cell
   with multiple types uses the most prominent accent: delete, then modify, then create.
   Click a stack label in the inspector to add it to the label
   selectors, preserving other keys and replacing the value for the same key.
   Editing selectors invalidates the displayed plan; predict again before apply.
   Type in the **Region** column header to filter rows by a case-insensitive
   substring; clear the text to show all regions. The filter persists across
   views and is visual only: it does not change deployment scope or the plan.
4. **Submit:** **Submit deployment** validates and queues the exact predicted
   plan, then opens **Approvals**. Submission does not approve, deploy, create an
   operation, or write simulation state. Multiple plans can await approval; later
   predictions do not replace submitted plans.
5. **Approvals:** review the submitted source, scope, stack list, and captured
   What-If details. **Approve & deploy** explicitly approves that exact plan,
   revalidates source/parameters/configuration, target and observed state, then
   starts execution. **Reject plan** removes it without deployment. A plan can
   execute only once, must first be submitted, and cannot execute concurrently
   with another deployment. Stale plans remain pending so they can be rejected;
   predict and submit a new plan after an edit or drift. This local prototype
   does not enforce separate submitter/approver identities or roles.
6. **Active Deployments:** matrix cells show stack execution status. The timeline
   shows the latest shared operation ID. `get_operation` polling reports
   queued/running/terminal **stack** transitions, never fabricated per-resource
   progress. Topology is refreshed at successful stack completion. A subsequent
   prediction reports no changes. In JSON simulation, each stack remains Running
   for a random 5,000–10,000 milliseconds before applying its predicted state;
   independent stacks run concurrently and dependents wait for prerequisites.
   Running badges show a small spinner, with animation disabled for reduced motion.
   **Hide stacks outside deployment scope** filters both topology layouts to the
   latest operation's submitted stack scope, including prerequisites, even while
   stacks are queued or after it completes. It does not change execution or the
   timeline. Stacks no longer present in the current topology are reported separately.
   The toggle starts off for each new operation.
   When connected to an older host, the toggle becomes available once a stack
   starts, after the complete scope has been queued.
   These cancellable delays apply only to the local viewer, not CLI simulation
   or Azure deployment providers. On failure, failed stacks are marked Failed and unstarted
   stacks Skipped instead of remaining Queued; shutdown marks unfinished stacks
   Canceled.
7. **Service Health:** matrix cells show observed managed-resource counts.
   Select a cell to inspect resource IDs. Counts and resource IDs are observations,
   **not health assessments**.
   The view explicitly identifies simulated state when using JSON simulation;
   The topology updates automatically every second. Health checks are not implemented.

The sample has orchestration rule declarations, but this viewer does **not**
implement declarative approval rules, health checks, bake times, or rollout
validation gates. Manual plan approval is separate from these orchestration rules.
Prediction is not proof of deployment safety; unresolved prediction fails
explicitly rather than producing a misleading clean plan.

## Existing Copilot chat via MCP

Add a local HTTP server to VS Code's MCP configuration (do not replace the
existing stdio server). No authentication headers or prompted inputs are needed:

```json
{
  "servers": {
    "bicep-local-orchestration": {
      "type": "http",
      "url": "http://127.0.0.1:5088/mcp"
    }
  }
}
```

Remove saved token inputs and Authorization headers from older configurations.

The browser uses the official MCP TypeScript client and calls the same tools as
Copilot. It polls `get_activity` so plans and operations started by Copilot
appear in the same matrix and timeline without a separate chat surface.
`get_activity` reports the simulation host's instance and revision even when
topology observation fails.

| Tool | Arguments | Behavior |
|---|---|---|
| `get_topology` | `expectedTargetInstance`, `expectedTargetRevision` (optional together) | Desired dependencies + provider observations; reject a changed target when the expected target is supplied |
| `get_activity` | none | Current target, latest draft plan and operation, and pending approvals with submission timestamps |
| `plan_deployment` | `selectors: string[]` (optional), `expectedTargetInstance`, `expectedTargetRevision` (optional together) | Read-only plan and ID; reject a changed target when the expected target is supplied |
| `submit_deployment` | `planId` | Validate and queue a predicted plan for approval; no deployment |
| `approve_deployment` | `planId`, `approved: boolean` | Require a submitted plan and explicit user approval; revalidate freshness and return an operation ID |
| `reject_deployment` | `planId` | Remove a pending plan without deployment |
| `get_operation` | `operationId` | Authoritative status + ordered events |

Pass both target fields from `get_activity` when making target-sensitive
topology or prediction calls to reject requests for a previous host instance
after a restart. All calls use simulation regardless of the supplied fields.

Explicit approval is not an authorization boundary: any local client
can explicitly approve execution. Never configure an agent to auto-approve
`approve_deployment`; inspect and approve the exact submitted plan before execution.

## MCP and local HTTP boundary

The only application protocol endpoint is `/mcp`; there are no `/api/*` REST
endpoints. Root and static asset requests only serve the viewer. MCP calls do
not require authentication.
Only `Host: 127.0.0.1:<port>` and absent or exactly matching same-origin `Origin`
are accepted; no CORS, cookies or filesystem browsing API.
Use **127.0.0.1**, not `localhost`. Assets contain no credentials.

| Method | Path | Behavior |
|---|---|---|
| MCP Streamable HTTP | `/mcp` | All topology, activity, prediction, apply, and operation calls |
| GET | `/` and static asset paths | Serve the viewer only |

403 means disallowed Host/Origin; tool failures are returned as MCP tool errors.
Provider exception payloads and source/resource bodies are not written to public
logs. Inspection responses and simulation
state **can contain sensitive resource properties**: use non-secret sample inputs
and protect local files. All local clients are trusted and can inspect state or
approve simulated deployments.
Loopback and Host/Origin checks do not protect against other local processes.

## Implementation and limitations

- `Bicep.Orchestration.Local` directly references the CLI assembly to reuse
  `OrchestrationSelection`, `SimulatedArmDeploymentProvider`, `SnapshotHelper` /
  `SnapshotDiffer`, and `LocalExtensionDispatcher`. No CLI output parsing, no
  duplicate Bicep evaluation/deployment engine.
- `IArmDeploymentProvider.ObserveStack` is a read-only contract. Simulation reads
  the flat state array and stack-managed resource IDs. ARM reads the stack and
  each managed resource; only a missing stack (404) becomes empty observation.
  Incomplete observation is an error, not "healthy" or "not deployed".
- Simulation plans fingerprint the entire state file, including unmanaged
  imports. State locks are operation-scoped, not host-lifetime. Reads during
  execution return the latest completed-stack cache.
- Deletions **within a configured stack** follow its `actionOnUnmanage`.
  Removed stack declarations are not automatically discovered or torn down.
  There is no teardown API. Unknown/orphan stacks are not subscription-inventoried.
- Simulation currently supports resource-group scopes and what the existing
  snapshot evaluator can resolve; it is not an ARM emulator. Inputs requiring
  unsupported external values fail instead of being mocked.
- This host is simulation-only. Existing CLI deployment providers are unchanged;
  no viewer or MCP action can select real Azure.
- Failures have sanitized public status, not full ARM exception details.
  Operations can partially succeed; cancellation/termination does not roll back
  completed stacks. Re-observe and predict before retrying.
- Public SDK versions are aligned at **2.3.0**: AspNetCore 2.3.0 declares an
  exact dependency on ModelContextProtocol 2.3.0. The previous AspNetCore 1.2.0
  central version was not mixed with the existing 2.3.0 runtime or downgraded.
- UI uses the Clawpilot light/dark CSS tokens and Segoe UI typography. It is a
  minimal graph inspector, not a source editor or cloud management portal.

## Validation

From the repository root:

```sh
npm --prefix src/Bicep.Orchestration.Local/web ci
npm --prefix src/Bicep.Orchestration.Local/web run build
dotnet build src/Bicep.Orchestration.Local/Bicep.Orchestration.Local.csproj --nologo -v:q
npm --prefix src/Bicep.Orchestration.Local/web exec -- playwright install chromium
npm --prefix src/Bicep.Orchestration.Local/web test
dotnet test --project src/Bicep.Orchestration.Local.UnitTests/Bicep.Orchestration.Local.UnitTests.csproj
dotnet test --project src/Bicep.Cli.IntegrationTests/Bicep.Cli.IntegrationTests.csproj --filter FullyQualifiedName~OrchestrationSimulationTests
dotnet test --project src/Bicep.Cli.UnitTests/Bicep.Cli.UnitTests.csproj --filter FullyQualifiedName~OrchestrationSelectionTests
```

Browser tests copy the sample into temporary workspaces, launch child hosts, and
stop them afterward. They exercise the MCP transport and the actual UI:
connect → predict → inspect → apply → monitor → no-change → matrix filters/layout.
Polling tests cover one-second automatic refresh, slow-response serialization,
error recovery, and simulation-only tools. Progress tests cover the 5–10 second
delay, cancellation before persistence, and provider polling failures.
No real Azure requests or provisioning are performed.
