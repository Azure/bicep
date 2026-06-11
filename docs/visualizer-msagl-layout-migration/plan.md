# Bicep Visualizer MSAGL Layout Migration Plan

Status: design and phased migration plan

## Sources And Current Anchors

- Automatic Graph Layout repository: https://github.com/microsoft/automatic-graph-layout
- Msagl NuGet package: https://www.nuget.org/packages/Msagl/
- Current VS Code visualizer host: `src/vscode-bicep/src/visualizer/view.ts`
- Current webview graph protocol types: `src/vscode-bicep-ui/apps/visual-designer/src/lib/messaging/messages.ts`
- Current client ELK layout: `src/vscode-bicep-ui/apps/visual-designer/src/features/layout/elk-layout.ts`
- Current LSP deployment graph handler: `src/Bicep.LangServer/Handlers/BicepDeploymentGraphHandler.cs`

## A) Architecture Overview

### Target Responsibilities

- React visualizer:
  - Owns rendering, interaction state, pan/zoom, selection, export, and animations.
  - Does not compute graph layout.
  - Holds the currently displayed graph: nodes, edges, and positions (the same state it already keeps today in the `lib/graph` atoms).
  - On a "graph may have changed" notification, asks the server for an update, sending the graph it currently displays, and applies the returned patches.
  - Keeps current interaction behavior during this migration. Future WYSIWYG drag/drop is a separate feature and does not affect this protocol.

- VS Code extension host:
  - Remains a thin bridge between webview messages and LSP requests.
  - Sends document/graph change notifications to the webview when the backing Bicep document or compilation changes.
  - Forwards visualizer requests/responses without deriving graph topology or layout.
  - Keeps VS Code-local commands such as reveal source range and open Problems panel.

- Language server:
  - Is the source of truth for graph topology, metadata, and layout derived from the Bicep compilation.
  - Rebuilds the canonical graph from the live compilation on each request (it already recompiles on every keystroke via `CompilationManager`).
  - Computes layout using the core MSAGL engine from the `Msagl` NuGet package.
  - Diffs the client's submitted graph against the freshly built graph and returns a list of patches, including initial load as patches from an empty client graph.
  - Is otherwise stateless per request: it does not track per-client revisions. It may keep an internal layout cache keyed by topology purely as an optimization.

- Shared schema/model layer:
  - Define a single visualizer protocol contract mirrored by C# LSP records and TypeScript interfaces.
  - Keep protocol definitions near `src/vscode-bicep/src/language/protocol.ts`, `src/vscode-bicep-ui/apps/visual-designer/src/lib/messaging/messages.ts`, and new C# records under the visualization feature folder (see below).
  - Add JSON serialization tests to keep C# and TypeScript contracts compatible.

### Language Server File Layout

The language server's flat `Handlers/` folder is already crowded, so all new server-side visualization code lives under a dedicated feature folder rather than as more loose files in `Handlers/`:

- `src/Bicep.LangServer/Features/Custom/Visualization/`

This folder holds the request/notification handlers, the graph service, the diff engine, the MSAGL layout engine, and the C# protocol records for this feature. The existing `Handlers/BicepDeploymentGraphHandler.cs` stays where it is for the compatibility path; new code is not added to `Handlers/`.

### MSAGL Placement And Invocation

- Add `Msagl` to central package management in `src/Directory.Packages.props`.
- Reference it from `src/Bicep.LangServer/Bicep.LangServer.csproj`.
- Introduce internal services under `Features/Custom/Visualization/`, conceptually shaped as:
  - `IVisualizerGraphService`: builds the canonical graph and computes diffs against a submitted client graph.
  - `IVisualizerLayoutEngine`: maps canonical graph to MSAGL geometry graph and returns node positions.
  - `MsaglVisualizerLayoutEngine`: invokes core MSAGL layout only; React still renders all visuals.
- Do not reference MSAGL from the React app or VS Code extension host.

### How This Maps Onto The Existing App

The migration reuses the loop that already exists; it does not invent a new one.

- Today, `BicepVisualizerViewManager` installs a `handleDiagnostics` middleware that calls `view.render()` for every open visualizer whenever the language server publishes diagnostics. That is already the "the graph may have changed" trigger.
- Today, `BicepVisualizerView.render()` is debounced, gated by a `readyToRender` flag, pulls the full graph via `textDocument/deploymentGraph`, and posts it to the webview.
- Today, the webview's `useApplyDeploymentGraph` diffs the incoming full graph against the previous one (`isDeploymentGraphEqual`), preserves positions for surviving nodes, bumps `graphVersionAtom`, and `useAutoLayout` runs ELK inside a `useLayoutEffect` guarded by a `cancelled` flag so only the latest layout wins.

The new design changes three things and keeps the rest:

- The full-graph pull becomes a notification + an update request that carries the client's current graph and returns patches.
- The topology diff moves from the webview into the language server.
- ELK in the webview is replaced by MSAGL in the language server; the webview just applies the positions it receives.

### Proposed Data Flow

- Initial load:
  - Webview signals it is ready (the existing `ready` notification).
  - Webview sends an update request with an empty current graph.
  - Language server builds the graph, runs MSAGL, and returns `addNode`/`addEdge`/`setNodeLayout` patches that build the graph from empty.

- Incremental edits:
  - Compilation updates and the extension sends a `documentDidChange` notification (the same point where `handleDiagnostics` calls `render()` today).
  - Webview sends an update request carrying the graph it currently displays.
  - Language server rebuilds the graph, diffs it against the submitted graph, and returns the patches needed to reconcile the difference. If nothing changed, it returns no patches.

- Layout:
  - The server lays out with MSAGL only when topology (or node size/layout options) changed; for metadata-only changes it returns metadata patches and no position patches, so the client keeps its current positions.

- Future WYSIWYG drag/drop:
  - Out of scope for this migration and not part of this protocol. It can be added later as its own message without changing anything here.

## B) Protocol And Concurrency Design

### Message Sequence

```mermaid
sequenceDiagram
    participant LS as Language server
    participant Ext as VS Code extension
    participant UI as React visualizer

    Note over LS,Ext: compilation updates (edit, module restore, referenced file change)
    Ext-->>UI: documentDidChange
    alt a request is already in flight
        UI->>UI: mark dirty, wait for the in-flight response
    else idle
        UI->>Ext: getGraphUpdate(current nodes + edges)
        Ext->>LS: textDocument/visualizerGraphUpdate(current)
        LS->>LS: build graph from live compilation, diff vs current, lay out changed topology
        LS-->>Ext: patches (complete delta; empty if nothing changed)
        Ext-->>UI: patches
        UI->>UI: apply all patches; if dirty, send another request
    end
```

### Why There Are No Revision Tokens

The first draft of this design carried `graphRevision`, `layoutRevision`, a `topologyHash`, a `changeReason`, and a multi-status optimistic-concurrency negotiation. None of that is needed, because of two properties of this system:

- The update request already carries the client's current graph. The server does not need a token to know "what the client has" — the client sends exactly that. So the server can compute a precise diff with no shared version counter.
- Each response is a *complete* delta from the submitted graph to the server's current graph, not an incremental step that assumes a specific base. The only base a response assumes is the graph that was sent in its own request.

Together these mean responses do not have to be applied in order: if the client only ever applies the response to its newest request and discards older ones, the result is always correct. There is nothing to reconcile, so there are no conflict states.

A separate `layoutRevision` was meant to catch the case where the client holds new topology but stale positions. That cannot happen here: MSAGL layout is deterministic for a given topology, and the server always returns layout patches together with the topology change that requires them. The client can never end up with topology the server has not also laid out. The graph the client holds is therefore the only "version" the protocol needs, and the client already holds it.

`changeReason` is dropped for the same reason the existing `handleDiagnostics` middleware does not carry one: the notification only means "the graph may have changed, ask for an update." The server recomputes from the live compilation regardless of why it changed.

### Protocol Spec

Three messages. The webview is already bound to a single document, so the document URI is the only routing information required.

```ts
// Extension -> webview. "The graph may have changed; request an update when ready."
// Fired where BicepVisualizerViewManager.handleDiagnostics currently calls view.render().
interface DocumentDidChangeNotification {
  method: "visualizer/documentDidChange";
  params: { documentUri: string };
}

// Webview -> extension -> language server. Carries the graph the webview currently displays.
// `current` is null (or empty) on first load.
interface GetGraphUpdateRequest {
  method: "visualizer/getGraphUpdate";
  params: {
    textDocument: { uri: string };
    current: {
      nodes: ClientGraphNode[];
      edges: ClientGraphEdge[];
    } | null;
  };
}

// Language server -> extension -> webview.
// A complete delta transforming `current` into the server's latest graph.
// An empty `patches` array means nothing changed.
interface GetGraphUpdateResponse {
  patches: GraphPatch[];
}
```

`ClientGraphNode` and `ClientGraphEdge` are the minimal identity the server needs to diff and to decide whether layout must run: node `id`, `kind`, `parentId`, layout-affecting `size`, and edge `id`/`sourceId`/`targetId`. The client does not send positions back; the server does not need them to compute the next layout, and omitting them keeps the request small.

### Concurrency: Single In-Flight Request

This replaces the optimistic-concurrency machinery with the convergence pattern the app already uses (`useAutoLayout`'s `cancelled` guard plus the debounced `render`).

Per open visualizer the webview keeps:

- one in-flight `getGraphUpdate` request, and
- a `dirty` flag.

Rules:

- On `documentDidChange`: if a request is in flight, set `dirty`; otherwise send a request with the currently displayed graph.
- On response: apply all patches (the response's base is the graph that was sent, which is still what is displayed, because nothing else mutates the displayed graph between request and response). Then, if `dirty`, clear it and send a fresh request with the now-current graph.
- A short debounce (the existing render debounce, ~75-150 ms) coalesces bursts of notifications into a single request.

This guarantees that when a response is applied its base equals the current displayed graph, so stale application is impossible without any version checks. If the webview additionally tags requests with a local monotonic sequence number, it can also defensively ignore a late response from a superseded request — but the single-in-flight rule already prevents that situation from arising.

### Forward Compatibility: A Writable Webview

The single-in-flight rule is sufficient *only while the webview is a pure reader*: the document is edited solely in the text editor, the server is the sole author of the graph, and the webview just applies what it is told. That is the entire scope of this migration.

It stops being sufficient once the webview can also *author* edits — the planned drag-n-drop "create resource" feature, where a webview gesture becomes a text edit on the `.bicep` document. Then two writers (the text editor and the webview) race against the same document, and a server response computed against a pre-edit document could clobber or flicker the user's in-progress change. Ordering edits at that point will need a document version on the messages (the LSP `TextDocument.version` the server already tracks via `BicepCompilationManager`), and possibly a different edit-application path (e.g. `workspace/applyEdit`).

We intentionally do **not** design that handshake now — it would be speculative, and its shape will be clearer when drag-n-drop is actually built. The point to record is only that the protocol can absorb it additively (an optional `version` field on the three messages) without reintroducing `graphRevision`/`layoutRevision`. We will revisit the protocol when implementing drag-n-drop.

### Edge Cases

- No graph yet / compilation has errors that prevent a graph: server returns a single `clearGraph` (or empty patches if already empty). The next `documentDidChange` reconciles.
- Client submits nodes the server no longer has: the diff naturally emits `removeNode`/`removeEdge` for them. This is a normal outcome, not a conflict.
- Document closed or renamed: handled by the existing view lifecycle in `view.ts`; no in-flight response is applied to a disposed view.
- Recoverable MSAGL failure: server returns topology/metadata patches without layout, keeping prior positions for surviving nodes, and logs a recoverable error. The view stays usable.
- Non-recoverable failure: extension falls back to the existing `textDocument/deploymentGraph` path, which remains available behind the feature flag during migration.

## C) Graph Model And Patch Format

### Canonical Graph Representation

```ts
interface VisualizerGraph {
  documentUri: string;
  errorCount: number;
  nodes: GraphNode[];
  edges: GraphEdge[];
}

interface GraphNode {
  id: string;
  kind: "resource" | "module" | "extension" | "unknown";
  parentId?: string;
  type: string;
  symbolName: string;
  isCollection: boolean;
  hasChildren: boolean;
  hasError: boolean;
  source: {
    filePath: string | null;
    range: Range;
  };
  visual: {
    size: { width: number; height: number };
    styleKey?: string;
  };
  layout?: NodeLayout;
  metadata?: Record<string, unknown>;
}

interface GraphEdge {
  id: string;
  sourceId: string;
  targetId: string;
  kind: "dependency" | "containment" | "implicit";
  metadata?: Record<string, unknown>;
}

interface NodeLayout {
  x: number;
  y: number;
  width: number;
  height: number;
}
```

Edges carry no route geometry. The current visualizer draws straight edges entirely on the client — `StraightEdge.tsx` takes the two node boxes, draws a segment between their centers, and clips it to the box boundaries — so it never consumes layout-engine edge routes (bend points/splines). Passing server routes would add protocol surface and a `setEdgeRoute` patch for geometry the client recomputes anyway, and which would otherwise need re-syncing every time a node moves. If curved or orthogonal routing is ever wanted, add `route` then.

Ports are not required for parity with the current visualizer, which uses node-to-node dependency edges. Add ports later only if edge attachment points become meaningful for sub-resources, resource properties, or grouped module boundaries.

### Patch Format

The response is a plain ordered list of typed patches — no wrapper, no revisions, no patch-set identity. A typed list is preferred over generic JSON Patch because the graph has domain-specific invariants (containment ordering, edge endpoint validity) that typed patches make explicit.

```ts
type GraphPatch =
  | { op: "clearGraph" }
  | { op: "addNode"; node: GraphNode }
  | { op: "removeNode"; nodeId: string }
  | { op: "updateNode"; nodeId: string; changes: Partial<GraphNode> }
  | { op: "addEdge"; edge: GraphEdge }
  | { op: "removeEdge"; edgeId: string }
  | { op: "updateEdge"; edgeId: string; changes: Partial<GraphEdge> }
  | { op: "setNodeLayout"; nodeId: string; layout: NodeLayout }
  | { op: "setErrorCount"; errorCount: number };
```

The entire list for one response is applied as a unit: the client applies all patches, then re-renders. Because the list is a complete delta from the graph the client submitted, there is no per-patch precondition to check.

### Patch Ordering

The server returns patches already ordered so they apply cleanly against the submitted graph:

- Remove edges.
- Remove nodes deepest-first.
- Add/update nodes.
- Add/update edges.
- Apply node layout.
- Set error count.

Initial load is just the build-from-empty case of this same ordering: `addNode`, `addEdge`, then layout patches.

### Layout-Only Versus Topology Patches

The server decides per request what to include:

- Topology changed: include add/remove/update node and edge patches plus `setNodeLayout` for the new and affected nodes.
- Topology unchanged, only metadata changed (error state, source ranges, file paths): include only `updateNode`/`setErrorCount` and omit all position patches, so the client keeps its current positions and nothing reflows.
- Nothing changed: return an empty list.

The client never has to reason about this. It applies whatever positions it receives and leaves the rest untouched, which is exactly what the existing position-preserving diff in `use-deployment-graph.ts` already does.

### Future Drag-And-Drop Support

Drag/drop is out of scope for this migration and is not part of this protocol. The only forward-looking requirement is that nothing here blocks it: a future WYSIWYG editor mode can add its own webview→server message for client-authored position overrides without changing the messages above. That is a separate design when the feature is actually built.

## D) Layout Behavior With MSAGL

### Layout Triggers And Topology Changes

Run MSAGL when:

- Node set changes.
- Edge set changes.
- Parent/child containment changes.
- Node kind changes between module/resource/unknown.
- Layout-affecting node size changes.
- Layout options change.
- User invokes reset layout.

Do not run MSAGL when:

- Only source ranges change.
- Only file paths change.
- Diagnostics count changes without affecting node visual size.
- The submitted topology already matches the freshly built topology.

### MSAGL Constraints And Options

- Use core MSAGL geometry graph APIs from the `Msagl` package, not Drawing, WPF, or WinForms viewer packages.
- Model Bicep resources/modules as MSAGL nodes with deterministic sizes.
- Model dependencies as directed MSAGL edges.
- Use a layered/Sugiyama-style top-to-bottom layout to match current ELK intent:
  - Direction: top to bottom.
  - Stable layer ordering by source order and node ID.
  - Tunable node separation, rank/layer separation, and component separation.
  - Edges are still modeled in MSAGL so they influence layering and node placement, but the client draws the straight edges itself. Use MSAGL's straight-line edge routing mode (`StraightLineEdges`) so layout does not spend time computing spline routes we discard; only node positions are read out.
- Represent modules/compound nodes as clusters if core MSAGL cluster support is adequate.
- If cluster behavior is not good enough initially, lay out nested module graphs independently and compose them into parent bounding boxes in the server service.

### Stable Layouts And Minimal Movement

- Deterministic input ordering: sort nodes and edges by canonical ID.
- Preserve previous positions for surviving nodes:
  - Use previous layout as initial geometry where MSAGL supports it.
  - Anchor unchanged connected components by translating new layout to previous centroid.
  - For localized additions, place new nodes near dependency neighbors before running layout.
- Avoid viewport-dependent layout. Server returns graph-coordinate positions and bounds; React owns pan/zoom and fit-view.
- Keep layout options stable across sessions unless explicitly changed.

### Performance And Incremental Layout

- Cache layout server-internally, keyed by document URI, an internal topology hash, node size profile, and layout options. This is an optimization invisible to the protocol.
- Cancel obsolete layouts aggressively.
- Add telemetry for node count, edge count, layout duration, patch count, conflict count, and fallback count.
- Set a large-graph budget:
  - Under budget: full MSAGL layout.
  - Near budget: layout only changed connected components when possible.
  - Over budget or timeout: return topology patches with previous surviving positions and a recoverable layout error; retry after idle or manual reset.
- Keep the language server responsive by running layout work asynchronously and observing cancellation between graph build, MSAGL invocation, and patch generation.

## E) Migration Strategy And Backward Compatibility

- Keep existing `textDocument/deploymentGraph` and `deploymentGraph` notification until the new path is proven.
- Add a feature flag, for example `bicep.visualizer.serverLayout.enabled`, default off initially.
- Add protocol capabilities:
  - React advertises patch ops and server-layout support.
  - Extension/language server falls back when the new request is unsupported.
- Phase migration:
  - New protocol scaffolding can run while React still uses ELK.
  - Server can compute MSAGL layouts in shadow mode while React ignores them.
  - Compare MSAGL and current ELK path with telemetry and selected visual tests.
  - Enable server layout for insiders/dev flag.
  - Remove ELK only after parity, performance, and fallback confidence.
- Keep UI functional throughout:
  - Existing full graph notification remains available.
  - React can continue applying topology updates through the current store.
  - If server layout fails, React keeps the last known layout and shows updated metadata/topology where safe.

## F) Check-In / PR Plan

### Phase 1: Protocol Contracts And Flags

- Goal and scope:
  - Add protocol types, capability handshake, and feature flag with no behavior change.
- Files/areas likely touched:
  - `src/vscode-bicep/src/language/protocol.ts`
  - `src/vscode-bicep-ui/apps/visual-designer/src/lib/messaging/messages.ts`
  - C# protocol records under `src/Bicep.LangServer/Features/Custom/Visualization`
- Test plan:
  - TypeScript unit tests for message channel request/response.
  - C# serialization tests for request/response records.
- Acceptance criteria:
  - Old visualizer path still works.
  - New messages serialize round-trip.
  - Feature flag defaults to existing behavior.

### Phase 2: Document Change Notification Loop

- Goal and scope:
  - Replace direct full-graph push under flag with notify-then-request flow, still returning current full graph converted to patches from empty/current state.
- Files/areas likely touched:
  - `src/vscode-bicep/src/visualizer/view.ts`
  - `src/vscode-bicep/src/visualizer/viewManager.ts`
  - `src/vscode-bicep-ui/apps/visual-designer/src/App.tsx`
- Test plan:
  - Webview messaging tests.
  - Extension tests for ready/change/request sequencing.
- Acceptance criteria:
  - Rapid edits produce latest-request-wins behavior.
  - Stale responses are ignored.
  - Old full-graph path remains available.

### Phase 3: Server Graph Service

- Goal and scope:
  - Introduce a language-server graph service that builds the canonical graph from the live compilation and diffs it against a submitted client graph; no MSAGL yet.
- Files/areas likely touched:
  - New request/notification handler and graph service under `src/Bicep.LangServer/Features/Custom/Visualization`
  - `src/Bicep.LangServer/BicepCompilationManager.cs`
  - `src/Bicep.LangServer/CompilationManager/CompilationContext.cs`
- Test plan:
  - LSP integration tests for diff correctness.
  - Range-only edit tests (expect metadata-only patches).
  - Topology change tests (expect add/remove patches).
  - Empty-submission initial-load tests.
- Acceptance criteria:
  - Topology-identical edits return no position patches.
  - Topology changes return the expected add/remove/update patches.
  - Referenced file changes are reflected in the rebuilt graph.

### Phase 4: Patch Diff Engine

- Goal and scope:
  - Return typed patch sets instead of full graph objects.
- Files/areas likely touched:
  - Diff service under `src/Bicep.LangServer/Features/Custom/Visualization`
  - `src/vscode-bicep-ui/apps/visual-designer/src/lib/messaging/use-deployment-graph.ts`
  - Graph atoms under `src/vscode-bicep-ui/apps/visual-designer/src/lib/graph`
- Test plan:
  - Unit tests for add/remove/update ordering.
  - Whole-list application tests.
  - Empty-list no-op tests.
  - Metadata-only (no position patches) tests.
- Acceptance criteria:
  - Current visualizer behavior can be reproduced by patch application while ELK still performs layout.

### Phase 5: MSAGL Adapter In Shadow Mode

- Goal and scope:
  - Add `Msagl`, implement layout engine, and compute layout behind a flag without applying it by default.
- Files/areas likely touched:
  - `src/Directory.Packages.props`
  - `src/Bicep.LangServer/Bicep.LangServer.csproj`
  - Layout engine under `src/Bicep.LangServer/Features/Custom/Visualization`
  - `src/Bicep.LangServer/IServiceCollectionExtensions.cs`
- Test plan:
  - Unit tests for MSAGL input mapping.
  - Deterministic output shape tests.
  - Performance smoke tests for representative graphs.
- Acceptance criteria:
  - Server returns layout patches in shadow mode.
  - Failures are recoverable and logged.
  - No client behavior changes when the feature flag is off.

### Phase 6: React Applies Server Layout

- Goal and scope:
  - Under feature flag, React applies `setNodeLayout`; disable ELK auto-layout for server-layout sessions.
- Files/areas likely touched:
  - `src/vscode-bicep-ui/apps/visual-designer/src/features/layout/use-auto-layout.ts`
  - `src/vscode-bicep-ui/apps/visual-designer/src/features/layout/elk-layout.ts`
  - Graph components and edge layer under `src/vscode-bicep-ui/apps/visual-designer/src/lib/graph`
- Test plan:
  - Vitest for patch application.
  - Playwright visualizer e2e for initial load, edit, delete, module graph, and fit view.
- Acceptance criteria:
  - No client layout runs when the flag is on.
  - Graph remains stable through rapid edits.
  - Fit view still works from server-provided bounds/layout.

### Phase 7: Single-Flight Hardening And Telemetry

- Goal and scope:
  - Add the single-in-flight + dirty-flag loop, request cancellation, debounce, layout timing, and fallback telemetry.
- Files/areas likely touched:
  - Extension visualizer bridge.
  - LSP graph service.
  - Telemetry/event plumbing.
- Test plan:
  - Integration tests simulating rapid edits and bursts of notifications.
  - Cancellation tests for long layout.
  - Telemetry shape tests where available.
- Acceptance criteria:
  - Only the latest request's response mutates the UI.
  - Bursts of edits collapse to a single in-flight request plus one follow-up.
  - Layout telemetry is emitted with bounded cardinality.

### Phase 8: Reserved For Future WYSIWYG

- Goal and scope:
  - Out of scope for this migration. Placeholder for a future editor mode that submits client-authored position overrides via its own message. No work planned here.
- Acceptance criteria:
  - The messages defined in this plan do not need to change to add that feature later.

### Phase 9: Default-On And ELK Removal

- Goal and scope:
  - Enable server layout by default after validation; remove ELK dependency and dead code.
- Files/areas likely touched:
  - `src/vscode-bicep-ui/apps/visual-designer/package.json`
  - `src/vscode-bicep-ui/package-lock.json`
  - ELK layout files under `src/vscode-bicep-ui/apps/visual-designer/src/features/layout`
- Test plan:
  - Full VS Code UI build.
  - Visualizer e2e tests.
  - LSP integration tests.
  - Performance telemetry review.
- Acceptance criteria:
  - No `elkjs` dependency remains.
  - Server layout is default.
  - Old protocol fallback is retained for one release if needed.

## G) Risks And Mitigations

- Performance regressions in the language server:
  - Mitigate with cancellation, cache by topology hash, time budgets, telemetry, and non-blocking async layout work.

- UI flicker or node jumps:
  - Mitigate with whole-list patch application, stable node IDs, deterministic ordering, and preserving previous positions for surviving nodes.

- Version drift between webview, extension, and language server:
  - Mitigate with a protocol version/capability handshake and fallback to the old `textDocument/deploymentGraph` path during migration.

- Patch complexity:
  - Mitigate with typed patch ops, strict ordering, whole-list application, and tests for every operation.

- Node size mismatch between server and React:
  - Mitigate by defining deterministic visual size in canonical graph, keeping layout independent of DOM measurement, and adding visual regression tests.

- MSAGL compound/module layout gaps:
  - Mitigate by validating cluster support early; if needed, compose nested module layouts manually before deeper cluster investment.

- Drag reconciliation complexity:
  - Out of scope for this migration; deferred to the future drag-n-drop work, which will revisit the protocol (see Forward Compatibility).

- Large graphs:
  - Mitigate with layout budgets, cached layouts, component-level recomputation, and graceful fallback to last-known positions.

## H) Progress Tracking

This migration ships as a sequence of small PRs. Phases in section F are units of *work*, not strictly units of *review* — most phases are one PR, but a couple split and one is not a PR at all. Keep this section as the living checklist: update the status and link the PR as each phase merges. Phases are ordered so that every PR keeps `main` shippable — the old ELK path stays the default until Phase 9.

Legend: `[ ]` not started, `[~]` in progress, `[x]` done.

| # | Phase | Status | PR | Notes |
|---|-------|--------|----|-------|
| 1 | Protocol contracts and flags | [x] | 49b011acf | Types + feature flag landed; capability handshake deferred (flag is the gate). Node kind trimmed to resource/module; edges carry no kind. |
| 2 | Document change notification loop | [x] | #19792 | Notify-then-request flow behind the flag; old full-graph path intact. Extension forwards `documentDidChange`→webview, webview pulls via `getGraphUpdate`→`textDocument/visualGraphUpdate`; single in-flight + dirty loop; patches applied to a client graph mirror, then translated to `DeploymentGraph` so ELK still lays out. Dev playground gains a server-layout toggle + throwaway TS differ. **Reordered after 3–4** so it forwards the real LSP request instead of a throwaway TS diff. |
| 3 | Server graph service | [x] | #19757 | Canonical graph built from compilation (`VisualGraphBuilder`, mirrors `BicepDeploymentGraphHandler`); no MSAGL. Combined with Phase 4 into one PR. Types renamed `Visualizer*`→`Visual*`. |
| 4 | Patch diff engine | [x] | #19757 | `VisualGraphDiffer` returns `GraphPatch[]`; handler registered (shadow mode, not flag-gated); ELK still lays out on the client. Combined with Phase 3. |
| 5 | MSAGL adapter (shadow mode) | [ ] | — | Add `Msagl`, compute layout behind the flag, not applied by default. |
| 6 | React applies server layout | [ ] | — | Apply `setNodeLayout`; disable ELK for server-layout sessions. |
| 7 | Single-flight hardening + telemetry | [ ] | — | In-flight + dirty loop, cancellation, debounce, metrics. |
| 8 | Reserved for future WYSIWYG | [ ] | — | No work planned; placeholder only. |
| 9 | Default-on and ELK removal | [ ] | — | Flip default, remove `elkjs`, keep old-path fallback one release. |

### How Phases Map To PRs

Expect roughly **7–8 PRs**, not nine, because phases are not strictly one-to-one with PRs:

- **One PR each (5):** Phase 1, Phase 2, Phase 5, Phase 6, Phase 9.
- **Phase 3 + 4 are the core change** and stay separate PRs; either may split further (e.g. canonical graph builder separate from the diff algorithm) if a single PR gets too large to review — so this is 2–3 PRs.
- **Phase 7** (single-flight hardening + telemetry) folds into Phase 6 and only becomes its own PR if Phase 6 would otherwise be too big.
- **Phase 8 is not a PR.** It is a reserved placeholder for future WYSIWYG work; no code lands for it in this migration.

Indicative PR sequence:

1. Protocol contracts + flag (Phase 1)
2. Notification/request loop (Phase 2)
3. Canonical graph builder (Phase 3)
4. Diff engine → patches (Phase 4)
5. MSAGL adapter, shadow mode (Phase 5)
6. React applies server layout (Phase 6)
7. Single-flight hardening + telemetry (Phase 7, optional split from 6)
8. Default-on + ELK removal (Phase 9)

The guiding rule is the Per-PR Definition of Done below: split wherever a PR would otherwise be too large to review, rather than forcing PR boundaries to match phase boundaries.

### Sequencing And Dependencies

- Phases 1–4 are additive and flag-gated; they can merge while the visualizer still uses ELK.
- Phase 5 depends on 3–4 (needs the canonical graph and patch engine).
- Phase 6 depends on 5 and is the first phase where users with the flag on see MSAGL layout.
- Phase 7 can overlap with 6 but should land before enabling the flag widely.
- Phase 9 is gated on parity, performance, and fallback confidence from 6–7.

### Per-PR Definition Of Done

- Meets the phase's acceptance criteria in section F.
- `main` remains shippable with the feature flag in its default state.
- New baselines (if any) are inspected and explained in the PR.
- This table is updated (status + PR link) in the same PR.
