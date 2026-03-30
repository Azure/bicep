# Step 4: Wire Up the LSP Data Source via Shared Messaging

## Goal

Replace the hardcoded test data in the visual designer's `App.tsx` with real deployment graph data from the Bicep language server, using the `@vscode-bicep-ui/messaging` shared messaging library.

## Background

### Data source: `textDocument/deploymentGraph`

The Bicep language server exposes a custom LSP request `textDocument/deploymentGraph` that returns:

```typescript
interface DeploymentGraph {
  nodes: DeploymentGraphNode[];
  edges: DeploymentGraphEdge[];
  errorCount: number;
}

interface DeploymentGraphNode {
  id: string;           // e.g. "myModule::vmResource" (:: delimited hierarchy)
  type: string;         // e.g. "Microsoft.Compute/virtualMachines" or "<module>"
  isCollection: boolean;
  range: Range;         // source range in the .bicep file
  hasChildren: boolean; // modules with child resources
  hasError: boolean;
  filePath: string;     // URI of the source file
}

interface DeploymentGraphEdge {
  sourceId: string;
  targetId: string;
}

interface Range {
  start: Position;
  end: Position;
}

interface Position {
  line: number;
  character: number;
}
```

### Node ID hierarchy

Node IDs use `::` as a hierarchy delimiter:
- `"vmResource"` — top-level resource
- `"myModule::vmResource"` — resource inside a module
- `"outerModule::innerModule::storage"` — nested modules

The parent is derived by popping the last `::` segment:
- `"myModule::vmResource"` → parent = `"myModule"`, symbol = `"vmResource"`
- `"vmResource"` → parent = `undefined` (top-level), symbol = `"vmResource"`

Nodes with `hasChildren: true` and type `"<module>"` are compound nodes (containers).

### Shared messaging library

The `@vscode-bicep-ui/messaging` package provides:

- **`WebviewMessageChannelProvider`** — React context provider that creates a `WebviewMessageChannel`
- **`useWebviewNotification(method, callback)`** — Subscribe to notifications from the extension host
- **`useWebviewMessageChannel()`** — Get the raw channel for sending notifications/requests
- **`WebviewMessageChannel`** — Handles message routing: request/response (with UUID correlation) and notifications (fire-and-forget)

The visual designer already has `@vscode-bicep-ui/messaging` as a dependency.

## Tasks

### 4.1 Define the message contract

Create a shared types file that defines the methods and payloads used between the extension host and the visual designer webview.

**File**: `src/vscode-bicep-ui/apps/visual-designer/src/messages.ts`

```typescript
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// ── Notification: Extension → Webview ──
// Sent when the deployment graph changes (initial load + on every file edit)
export const DEPLOYMENT_GRAPH_NOTIFICATION = "deploymentGraph";

export interface DeploymentGraphPayload {
  documentPath: string;
  deploymentGraph: DeploymentGraph | null;
}

export interface DeploymentGraph {
  nodes: DeploymentGraphNode[];
  edges: DeploymentGraphEdge[];
  errorCount: number;
}

export interface DeploymentGraphNode {
  id: string;
  type: string;
  isCollection: boolean;
  range: Range;
  hasChildren: boolean;
  hasError: boolean;
  filePath: string;
}

export interface DeploymentGraphEdge {
  sourceId: string;
  targetId: string;
}

export interface Range {
  start: Position;
  end: Position;
}

export interface Position {
  line: number;
  character: number;
}

// ── Notification: Webview → Extension ──
// Sent when the user wants to navigate to a source range
export const REVEAL_FILE_RANGE_NOTIFICATION = "revealFileRange";

export interface RevealFileRangePayload {
  filePath: string;
  range: Range;
}

// ── Notification: Webview → Extension ──
// Sent when the webview has initialized and is ready to receive data
export const READY_NOTIFICATION = "ready";
```

### 4.2 Create the deployment graph → Jotai atoms mapping

Create a hook that receives `DeploymentGraph` data and maps it to the Jotai atom model.

**File**: `src/vscode-bicep-ui/apps/visual-designer/src/hooks/useDeploymentGraph.ts`

```typescript
import { useSetAtom } from "jotai";
import { useCallback } from "react";
import {
  addAtomicNodeAtom,
  addCompoundNodeAtom,
  addEdgeAtom,
  nodesAtom,
  edgesAtom,
} from "../features/graph-engine/atoms";
import type { DeploymentGraph, DeploymentGraphNode } from "../messages";

interface NodeData {
  symbolicName: string;
  resourceType: string;
  isCollection: boolean;
  hasError: boolean;
  range: Range;
  filePath: string;
}

interface ModuleData {
  symbolicName: string;
  path: string;
  isCollection: boolean;
  hasError: boolean;
  range: Range;
  filePath: string;
}
```

The mapping logic:

1. **Clear existing state**: Set `nodesAtom` to `{}` and `edgesAtom` to `[]`.
2. **Identify compound vs atomic nodes**: Nodes with `hasChildren: true` and type `"<module>"` are compound. All others are atomic.
3. **Build parent-child relationships**: For each node, parse the `::` delimited ID to determine the parent compound node.
4. **Add compound nodes first**: Create compound nodes with their child IDs (collected from step 3).
5. **Add atomic nodes**: Create atomic nodes with initial positions at `(0, 0)` (positions will be computed by ELK layout in Step 3).
6. **Add edges**: Map each `DeploymentGraphEdge` to a Jotai edge atom.
7. **Trigger layout**: After all nodes are added, schedule an ELK layout pass.

**Key implementation detail**: The `addCompoundNodeAtom` requires child IDs at creation time. But `addAtomicNodeAtom` also needs to exist for compound node's derived `boxAtom` to compute correctly. Order of operations:

1. First, add all atomic nodes (they have independent `boxAtom`)
2. Then, add compound nodes with their collected child IDs
3. Then, add edges

```typescript
export function useApplyDeploymentGraph() {
  const setNodesAtom = useSetAtom(nodesAtom);
  const setEdgesAtom = useSetAtom(edgesAtom);
  const addAtomicNode = useSetAtom(addAtomicNodeAtom);
  const addCompoundNode = useSetAtom(addCompoundNodeAtom);
  const addEdge = useSetAtom(addEdgeAtom);

  return useCallback((graph: DeploymentGraph | null) => {
    // Clear existing state
    setNodesAtom({});
    setEdgesAtom([]);

    if (!graph || graph.nodes.length === 0) {
      return;
    }

    // Classify nodes
    const compoundNodeIds = new Set<string>();
    const parentChildMap = new Map<string, string[]>();  // parentId → childIds[]

    for (const node of graph.nodes) {
      if (node.hasChildren) {
        compoundNodeIds.add(node.id);
        parentChildMap.set(node.id, []);
      }
    }

    // Build parent-child relationships from :: delimited IDs
    for (const node of graph.nodes) {
      const segments = node.id.split("::");
      if (segments.length > 1) {
        const parentId = segments.slice(0, -1).join("::");
        if (parentChildMap.has(parentId)) {
          parentChildMap.get(parentId)!.push(node.id);
        }
      }
    }

    // Phase 1: Add all atomic nodes (position at 0,0 — ELK will fix)
    for (const node of graph.nodes) {
      if (!compoundNodeIds.has(node.id)) {
        const symbol = node.id.split("::").pop()!;
        addAtomicNode(node.id, { x: 0, y: 0 }, {
          symbolicName: symbol,
          resourceType: node.type,
          isCollection: node.isCollection,
          hasError: node.hasError,
          range: node.range,
          filePath: node.filePath,
        });
      }
    }

    // Phase 2: Add compound nodes
    for (const node of graph.nodes) {
      if (compoundNodeIds.has(node.id)) {
        const symbol = node.id.split("::").pop()!;
        const childIds = parentChildMap.get(node.id) ?? [];
        addCompoundNode(node.id, childIds, {
          symbolicName: symbol,
          path: node.filePath,
          isCollection: node.isCollection,
          hasError: node.hasError,
          range: node.range,
          filePath: node.filePath,
        });
      }
    }

    // Phase 3: Add edges
    for (const edge of graph.edges) {
      addEdge(
        `${edge.sourceId}>${edge.targetId}`,
        edge.sourceId,
        edge.targetId,
      );
    }

    // Phase 4: Schedule ELK layout (after DOM measurement)
    // (Uses the layout infrastructure from Step 3)
  }, [setNodesAtom, setEdgesAtom, addAtomicNode, addCompoundNode, addEdge]);
}
```

### 4.3 Wire up the messaging in `App.tsx`

Replace the hardcoded test data with the messaging integration:

```tsx
import { WebviewMessageChannelProvider, useWebviewNotification, useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useApplyDeploymentGraph } from "./hooks/useDeploymentGraph";
import { DEPLOYMENT_GRAPH_NOTIFICATION, READY_NOTIFICATION, DeploymentGraphPayload } from "./messages";

function VisualDesignerApp() {
  const applyGraph = useApplyDeploymentGraph();
  const messageChannel = useWebviewMessageChannel();

  // Send READY notification on mount
  useEffect(() => {
    messageChannel.sendNotification({
      method: READY_NOTIFICATION,
    });
  }, [messageChannel]);

  // Listen for deployment graph updates
  useWebviewNotification(
    DEPLOYMENT_GRAPH_NOTIFICATION,
    useCallback((params: unknown) => {
      const payload = params as DeploymentGraphPayload;
      applyGraph(payload.deploymentGraph);
    }, [applyGraph]),
  );

  return (
    <PanZoomProvider>
      <$ControlBarContainer>
        <GraphControlBar />
      </$ControlBarContainer>
      <Canvas>
        <Graph />
      </Canvas>
      {/* StatusBar will be added in Step 6 */}
    </PanZoomProvider>
  );
}

export function App() {
  return (
    <WebviewMessageChannelProvider>
      <VisualDesignerApp />
    </WebviewMessageChannelProvider>
  );
}
```

### 4.4 Update `ResourceDeclaration` and `ModuleDeclaration` data types

Update the declaration components to accept the data shape from the deployment graph:

**`ResourceDeclaration.tsx`** — Update `data` interface:

```typescript
export interface ResourceDeclarationProps {
  id: string;
  data: {
    symbolicName: string;
    resourceType: string;
    isCollection?: boolean;
    hasError?: boolean;
    range?: Range;
    filePath?: string;
  };
}
```

**`ModuleDeclaration.tsx`** — Update `data` interface:

```typescript
export interface ModuleDeclarationProps {
  id: string;
  data: {
    symbolicName: string;
    path: string;
    isCollection?: boolean;
    hasError?: boolean;
    range?: Range;
    filePath?: string;
  };
}
```

### 4.5 Handle graph updates (incremental re-renders)

The extension host sends a new `DEPLOYMENT_GRAPH` notification every time the Bicep file changes (triggered by the `handleDiagnostics` middleware). Each notification contains the **full** graph — not a diff.

The simplest approach: on each notification, clear the entire Jotai state and rebuild. This is what the old visualizer does (Cytoscape replaces all elements on each update via `cy.json({ elements })`).

**Consideration**: Clearing and rebuilding causes a full re-layout and visual reset. To mitigate jarring jumps:
- Store node positions before clearing
- After rebuilding, check if node IDs match and restore positions for nodes that didn't change
- Only run ELK layout if the graph structure actually changed

This optimization can be deferred — start with the simple clear-and-rebuild approach.

### 4.6 Keep dev mode working

The visual designer needs to work standalone for development (via `npm run dev`). When not running inside a VS Code webview, `acquireVsCodeApi()` is not available.

The `WebviewMessageChannelProvider` from `@vscode-bicep-ui/messaging` calls `acquireVsCodeApi()` in the `WebviewMessageChannel` constructor. For dev mode, either:

1. **Conditionally wrap**: Only use `WebviewMessageChannelProvider` when running in VS Code (detect via `typeof acquireVsCodeApi !== 'undefined'`).
2. **Mock the channel**: Keep the hardcoded test data as a fallback when `acquireVsCodeApi` is not available.
3. **Use the playground's mock**: `@vscode-elements/webview-playground` may provide a mock `acquireVsCodeApi`.

Recommended approach: conditionally render. In dev mode, use the hardcoded data; in production (VS Code webview), use the real messaging:

```tsx
const isDev = typeof acquireVsCodeApi === 'undefined';

export function App() {
  if (isDev) {
    return <DevApp />;  // uses hardcoded test data
  }

  return (
    <WebviewMessageChannelProvider>
      <VisualDesignerApp />
    </WebviewMessageChannelProvider>
  );
}
```

## Verification

1. **Unit test the mapping function**: Create a test file that calls `useApplyDeploymentGraph` with sample `DeploymentGraph` payloads and verifies the correct Jotai atoms are created:
   - Test with a flat graph (no modules)
   - Test with nested modules (compound nodes)
   - Test with edges
   - Test with null/empty graph
   - Test with `isCollection` and `hasError` flags
2. **Dev mode**: Run `npm run dev` — should still render the hardcoded test data.
3. **Integration test** (after Step 5): Open a `.bicep` file with various resources and modules, and verify the graph renders correctly.

## Notes

- The `@vscode-bicep-ui/messaging` library does NOT have an extension-host-side counterpart. The extension host will need to handle the `WebviewRequestMessage`/`WebviewNotificationMessage` format directly (see Step 5).
- The `READY` notification is conceptually the same as the old `READY_MESSAGE` — it signals to the extension host that the webview is initialized and ready to receive data.
- The full round-trip: Webview sends `ready` → Extension host calls `textDocument/deploymentGraph` LSP request → Extension host sends `deploymentGraph` notification → Webview receives and renders.
