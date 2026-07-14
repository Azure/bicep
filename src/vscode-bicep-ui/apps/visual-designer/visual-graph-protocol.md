# Visual Graph Protocol

This document describes the server-driven visual graph protocol used by the Bicep visual designer.

The protocol is intentionally split into two phases:

1. Reconcile graph topology and metadata.
2. Render and measure nodes on the client, then request layout using actual node sizes.

This split exists because the language server cannot know the final rendered dimensions of React node cards before the webview renders them.

## Participants

- **Language server** builds the canonical graph from the live Bicep compilation, diffs topology and metadata, validates measured layout requests, and runs MSAGL.
- **VS Code extension host** forwards webview requests to language-server requests and forwards responses back. It does not compute topology or layout.
- **React webview** owns rendering, node measurement, pan/zoom, fit-view, and applying graph/layout patches.

## Message Flow

```mermaid
sequenceDiagram
    participant LS as Language server
    participant Ext as VS Code extension
    participant UI as React webview

    Note over LS,Ext: compilation or diagnostics update
    Ext-->>UI: documentDidChange

    alt request already in flight
        UI->>UI: mark dirty
    else idle
        UI->>Ext: getGraphUpdate(current topology)
        Ext->>LS: textDocument/visualGraphUpdate(current topology)
        LS->>LS: rebuild canonical graph and diff topology/metadata
        LS-->>Ext: topology/metadata patches
        Ext-->>UI: topology/metadata patches
        UI->>UI: apply patches and detect layout-affecting changes

        opt layout may be stale
            UI->>UI: render graph and measure node boxes
            UI->>UI: compare measured graph with last layout input
            alt measured layout input changed
                UI->>Ext: getGraphLayout(measured graph)
                Ext->>LS: textDocument/visualGraphLayout(measured graph)
                LS->>LS: rebuild canonical graph and validate measured topology
                alt topology still matches
                    LS->>LS: run MSAGL with measured node sizes
                    LS-->>Ext: ok + setNodeLayout patches
                    Ext-->>UI: ok + setNodeLayout patches
                    UI->>UI: apply layout patches and fit view
                else topology changed
                    LS-->>Ext: graphChanged
                    Ext-->>UI: graphChanged
                    UI->>UI: mark dirty and restart graph update
                end
            else measured layout input unchanged
                UI->>UI: keep existing positions
            end
        end

        opt dirty
            UI->>UI: request graph update again
        end
    end
```

## Graph Update

The graph update request reconciles topology and metadata only. It does not run layout.

```ts
interface GetGraphUpdateRequest {
  current: RenderedGraph | null;
}

interface GetGraphUpdateResponse {
  patches: GraphPatch[];
}
```

The extension forwards this as:

```ts
interface VisualGraphUpdateParams {
  textDocument: { uri: string };
  current: RenderedGraph | null;
}

interface VisualGraphUpdateResult {
  patches: GraphPatch[];
}
```

### Update Sequence

```mermaid
sequenceDiagram
    participant UI as React webview
    participant Ext as VS Code extension
    participant LS as Language server

    UI->>Ext: getGraphUpdate(current)
    Ext->>LS: textDocument/visualGraphUpdate(current)
    LS->>LS: build canonical graph
    LS->>LS: diff current topology/metadata against canonical graph
    LS-->>Ext: GraphPatch[]
    Ext-->>UI: GraphPatch[]
    UI->>UI: apply patches to client graph mirror
    UI->>UI: record whether patches may affect layout
```

## Client Layout Invalidation

The client decides whether layout may be stale while applying patches. This avoids making the server guess whether metadata changes affect rendered dimensions.

Layout-affecting patches:

- `clearGraph`
- `addNode`
- `removeNode`
- `addEdge`
- `removeEdge`
- `updateNode` when `type`, `isCollection`, or `hasChildren` changes

Non-layout-affecting patches:

- `updateNode` when only `hasError` changes
- `setErrorCount`
- `setNodeLayout`
- `setGraphBounds`

Notably, `hasError` does not trigger layout.

The server diffs node metadata per field and emits `updateNode` only when metadata actually changes. The client still compares each incoming field against the value it currently holds and treats the patch as layout-affecting only when a layout-relevant field (`type`, `isCollection`, or `hasChildren`) changed value. Source locations are resolved on demand and are not part of graph metadata, so whitespace-only edits do not produce node patches.

If a patch may affect layout, the client renders the updated graph, measures actual node boxes, builds a measured `RenderedGraph`, and compares it with the last measured graph that produced a layout. The client sends a layout request only when measured topology, sizes, or layout options changed.

```mermaid
flowchart TD
    A[Apply graph update patches] --> B{Any patch may affect layout?}
    B -- No --> C[Keep current positions]
    B -- Yes --> D[Render updated graph]
    D --> E[Measure actual node sizes]
    E --> F{Measured graph equals last layout input?}
    F -- Yes --> C
    F -- No --> G[Send measured layout request]
```

## Measured Layout

The layout request is sent only after the graph has rendered and node dimensions have been measured.

```ts
interface GetGraphLayoutRequest {
  current: RenderedGraph;
}

interface GetGraphLayoutResponse {
  status: "ok" | "graphChanged" | "layoutFailed";
  patches: GraphPatch[];
}
```

The extension forwards this as:

```ts
interface VisualGraphLayoutParams {
  textDocument: { uri: string };
  current: RenderedGraph;
  options?: VisualGraphLayoutOptions;
}

interface VisualGraphLayoutResult {
  status: "ok" | "graphChanged" | "layoutFailed";
  patches: GraphPatch[];
}
```

Successful layout responses contain `setNodeLayout` patches and, when available, one `setGraphBounds` patch used for fit-view.

### Layout Sequence

```mermaid
sequenceDiagram
    participant UI as React webview
    participant Ext as VS Code extension
    participant LS as Language server

    UI->>Ext: getGraphLayout(measured graph)
    Ext->>LS: textDocument/visualGraphLayout(measured graph)
    LS->>LS: rebuild canonical graph from live compilation
    LS->>LS: compare measured topology with canonical topology

    alt topology matches
        LS->>LS: run MSAGL with measured node sizes
        LS-->>Ext: ok + setNodeLayout patches
        Ext-->>UI: ok + setNodeLayout patches
        UI->>UI: apply positions and fit view
    else topology changed
        LS-->>Ext: graphChanged
        Ext-->>UI: graphChanged
        UI->>UI: mark dirty and request graph update
    else layout failed recoverably
        LS-->>Ext: layoutFailed
        Ext-->>UI: layoutFailed
        UI->>UI: keep existing positions
    end
```

## Rendered Graph

`RenderedGraph` carries topology plus measured node sizes. It intentionally does not send current positions back to the server.

```ts
interface RenderedGraph {
  nodes: RenderedGraphNode[];
  edges: RenderedGraphEdge[];
}

interface RenderedGraphNode {
  id: string;
  kind: "resource" | "module";
  parentId: string | null;
  type: string;
  isCollection: boolean;
  hasChildren: boolean;
  hasError: boolean;
  width: number;
  height: number;
}

interface RenderedGraphEdge {
  id: string;
  sourceId: string;
  targetId: string;
}
```

## Patch Shape

```ts
type GraphPatch =
  | { op: "clearGraph" }
  | { op: "addNode"; node: GraphNode }
  | { op: "removeNode"; nodeId: string }
  | { op: "updateNode"; nodeId: string; changes: GraphNodeChanges }
  | { op: "addEdge"; edge: GraphEdge }
  | { op: "removeEdge"; edgeId: string }
  | { op: "setNodeLayout"; nodeId: string; layout: NodeLayout }
  | { op: "setGraphBounds"; bounds: GraphBounds }
  | { op: "setErrorCount"; errorCount: number };
```

## Concurrency Rules

Each visualizer keeps one in-flight visual graph request at a time. A visual graph request is either a graph update request or a layout request.

```mermaid
stateDiagram-v2
    [*] --> Idle
    Idle --> Updating: documentDidChange
    Updating --> Measuring: patches may affect layout
    Updating --> Idle: no layout needed
    Measuring --> Layouting: measured graph changed
    Measuring --> Idle: measured graph unchanged
    Layouting --> Idle: ok or layoutFailed
    Layouting --> Updating: graphChanged or dirty
    Updating --> Updating: dirty after response
```

If `documentDidChange` arrives while a request is in flight, the client sets a dirty flag. When the current request finishes, the client sends a fresh graph update if dirty is set.

The server remains stateless per request. It validates each measured layout request against the current live compilation instead of tracking graph revisions.
