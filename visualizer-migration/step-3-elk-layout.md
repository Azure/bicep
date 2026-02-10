# Step 3: Integrate ELK.js Auto-Layout

## Goal

Add automatic graph layout to the new visual designer using ELK.js, so nodes are arranged in a readable hierarchical layout instead of requiring manual positioning. This is critical for replicating the old visualizer's behavior where graphs are automatically laid out on load and can be re-laid out via the "Reset Layout" button.

## Background

### Old visualizer ELK usage

The old visualizer uses `cytoscape-elk` (a Cytoscape.js adapter for ELK) with these options:

```typescript
elk: {
  algorithm: "layered",
  "layered.layering.strategy": "INTERACTIVE",
  "layered.nodePlacement.bk.fixedAlignment": "BALANCED",
  "layered.cycleBreaking.strategy": "DEPTH_FIRST",
  "elk.direction": "DOWN",
  "elk.aspectRatio": 2.5,
  "spacing.nodeNodeBetweenLayers": 80,
  "spacing.baseValue": 120,
  "spacing.componentComponent": 100,
}
```

The layout runs:
1. On initial load (after elements are set)
2. When the user clicks "Reset Layout"
3. With `fit: true` and `animate: true` (800ms, `cubic-bezier(0.33, 1, 0.68, 1)`)

Cytoscape-elk handles passing node dimensions to ELK and mapping results back to Cytoscape positions. We need to replicate this plumbing manually.

### New visual designer state model

- Each atomic node has an `originAtom` (initial/target position) and a `boxAtom` (current bounding box)
- Compound nodes have a derived `boxAtom` computed from children + padding
- Setting `originAtom` to a new value triggers a spring animation in `AtomicNode.tsx` that interpolates the node from its current position to the new origin
- The `GraphControlBar` "Reset Layout" button currently just re-sets each `originAtom` to its current value (triggering a no-op animation)

### Integration approach

Use `elkjs` directly (not through `cytoscape-elk`, since we don't use Cytoscape anymore). ELK takes a hierarchical graph structure as input and outputs node positions. We then write those positions to each node's `originAtom`, which triggers the existing spring animations.

## Tasks

### 3.1 Add `elkjs` dependency

Add `elkjs` to the visual designer's dependencies:

```bash
cd src/vscode-bicep-ui/apps/visual-designer
npm install elkjs
```

This adds `elkjs` to `package.json`. The `elkjs` package includes a WebAssembly build that runs the ELK Java layout algorithms in the browser.

### 3.2 Create the layout utility module

Create a new file:

**File**: `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/layout/elk-layout.ts`

This module should:

1. **Convert Jotai state → ELK input**: Read `nodesAtom` and `edgesAtom` from the Jotai store, build an `ElkNode` tree (ELK's input format).
2. **Run ELK layout**: Call `elk.layout(graph)` with the appropriate options.
3. **Map ELK output → Jotai state**: Write computed `(x, y)` positions back to each atomic node's `originAtom`.

```typescript
import ELK, { ElkNode, ElkExtendedEdge } from "elkjs/lib/elk.bundled.js";
import { getDefaultStore } from "jotai";
import { nodesAtom, edgesAtom } from "../atoms";
import type { NodeState, AtomicNodeState, CompoundNodeState } from "../atoms/nodes";
import type { EdgeAtomValue } from "../atoms/edges";

const elk = new ELK();

const ELK_OPTIONS = {
  "elk.algorithm": "layered",
  "elk.direction": "DOWN",
  "elk.aspectRatio": "2.5",
  "elk.layered.layering.strategy": "INTERACTIVE",
  "elk.layered.nodePlacement.bk.fixedAlignment": "BALANCED",
  "elk.layered.cycleBreaking.strategy": "DEPTH_FIRST",
  "elk.spacing.nodeNode": "120",
  "elk.layered.spacing.nodeNodeBetweenLayers": "80",
  "elk.spacing.componentComponent": "100",
};
```

### 3.3 Handle the ELK input format

ELK expects a hierarchical `ElkNode` structure:

```typescript
interface ElkNode {
  id: string;
  width?: number;
  height?: number;
  x?: number;
  y?: number;
  children?: ElkNode[];
  edges?: ElkExtendedEdge[];
  layoutOptions?: Record<string, string>;
}
```

The conversion logic:

1. **Identify root vs child**: Compound nodes contain child IDs (stored in `childIdsAtom`). Build a parent lookup: for each compound node, mark its children.
2. **Build ELK hierarchy**:
   - Root `ElkNode` has `children` = all top-level nodes (not contained in any compound node)
   - Each compound node's `children` = its child atomic nodes
   - Each node needs `width` and `height` — read from the node's `boxAtom` (`max.x - min.x`, `max.y - min.y`)
3. **Edges**: Flatten all edges as `ElkExtendedEdge` objects with `sources: [fromId]`, `targets: [toId]`, placed at the appropriate level (root level for edges connecting top-level nodes)

```typescript
function buildElkGraph(
  store: ReturnType<typeof getDefaultStore>,
): ElkNode {
  const nodes = store.get(nodesAtom);
  const edges = store.get(edgesAtom);

  // Build parent lookup: childId → parentId
  const childToParent = new Map<string, string>();
  for (const node of Object.values(nodes)) {
    if (node.kind === "compound") {
      const childIds = store.get(node.childIdsAtom);
      for (const childId of childIds) {
        childToParent.set(childId, node.id);
      }
    }
  }

  // Convert a node to an ElkNode
  function toElkNode(node: NodeState): ElkNode {
    const box = store.get(node.boxAtom);
    const width = Math.max(box.max.x - box.min.x, 1);
    const height = Math.max(box.max.y - box.min.y, 1);

    if (node.kind === "compound") {
      const childIds = store.get(node.childIdsAtom);
      const children = childIds
        .map((id) => nodes[id])
        .filter(Boolean)
        .map(toElkNode);

      return {
        id: node.id,
        children,
        layoutOptions: ELK_OPTIONS,
      };
    }

    return { id: node.id, width, height };
  }

  // Build top-level children (nodes not contained by any compound)
  const topLevelNodes = Object.values(nodes)
    .filter((n) => !childToParent.has(n.id))
    .map(toElkNode);

  // All edges go at root level (ELK handles cross-hierarchy edges)
  const elkEdges: ElkExtendedEdge[] = edges.map((edge) => ({
    id: edge.id,
    sources: [edge.fromId],
    targets: [edge.toId],
  }));

  return {
    id: "root",
    children: topLevelNodes,
    edges: elkEdges,
    layoutOptions: ELK_OPTIONS,
  };
}
```

### 3.4 Apply ELK output to Jotai atoms

After ELK computes positions, write them back:

```typescript
function applyElkLayout(
  store: ReturnType<typeof getDefaultStore>,
  elkRoot: ElkNode,
  offsetX = 0,
  offsetY = 0,
): void {
  const nodes = store.get(nodesAtom);

  for (const elkNode of elkRoot.children ?? []) {
    const node = nodes[elkNode.id];
    if (!node) continue;

    const x = (elkNode.x ?? 0) + offsetX;
    const y = (elkNode.y ?? 0) + offsetY;

    if (node.kind === "atomic") {
      // Setting originAtom triggers spring animation in AtomicNode
      store.set(node.originAtom, { x, y });
    } else if (node.kind === "compound") {
      // For compound nodes, position their children relative to
      // the compound node's position. ELK gives children positions
      // relative to their parent.
      applyElkLayout(store, elkNode, x, y);
    }
  }
}
```

### 3.5 Expose the `runLayout` function

```typescript
export async function runLayout(store: ReturnType<typeof getDefaultStore>): Promise<void> {
  const elkGraph = buildElkGraph(store);
  const layoutResult = await elk.layout(elkGraph);
  applyElkLayout(store, layoutResult);
}
```

### 3.6 Handle the chicken-and-egg problem: node dimensions

ELK needs node dimensions (`width`, `height`) to compute layout. But the new visual designer determines dimensions by rendering the React component and measuring it (via `useResizeObserver` in `AtomicNode.tsx`).

**Solution — two-phase approach**:

1. **Phase 1 — Initial render**: Nodes render at temporary positions (e.g., all at `(0, 0)` or stacked). The `useResizeObserver` and `useLayoutEffect` in `AtomicNode.tsx` measure actual dimensions and update `boxAtom.max`.
2. **Phase 2 — Layout**: After all nodes have been measured (wait one frame via `requestAnimationFrame` or a small timeout), call `runLayout()`. This reads measured dimensions from `boxAtom`, computes ELK layout, and writes positions to `originAtom`, triggering the spring animation.

This can be orchestrated in the component that manages the graph lifecycle (currently `App.tsx`, later the data hook from Step 4):

```typescript
// After adding all nodes:
requestAnimationFrame(() => {
  requestAnimationFrame(async () => {
    // By now, all nodes have been measured via useLayoutEffect
    await runLayout(store);
  });
});
```

A more robust approach: add a `layoutRequestAtom` that tracks when a layout is needed, and a `useEffect` in the `Graph` component that watches it.

### 3.7 Wire up the "Reset Layout" button in `GraphControlBar`

Update `GraphControlBar.tsx` to call `runLayout` on the "Reset Layout" button click:

```typescript
import { runLayout } from "../../graph-engine/layout/elk-layout";

// Inside GraphControlBar:
const handleResetLayout = useAtomCallback(
  useCallback(async (_get, _set) => {
    const store = getDefaultStore();
    await runLayout(store);
  }, []),
);
```

Replace the current `resetLayout` callback which just re-sets `originAtom`:

```tsx
<$ControlButton onClick={handleResetLayout} title="Reset Layout" aria-label="Reset Layout">
  <Codicon name="type-hierarchy-sub" size={16} />
</$ControlButton>
```

### 3.8 Add auto-layout on initial graph load

When nodes are first added to the graph, automatically run layout after they've been measured. This ensures the graph appears in a readable arrangement rather than piled at (0,0).

Create a hook or utility that watches for changes to `nodesAtom` and triggers layout:

```typescript
// In App.tsx or a dedicated hook:
useEffect(() => {
  // Wait for DOM measurement
  const frame1 = requestAnimationFrame(() => {
    const frame2 = requestAnimationFrame(async () => {
      await runLayout(store);
      // Optionally fit the view after layout
    });

    return () => cancelAnimationFrame(frame2);
  });

  return () => cancelAnimationFrame(frame1);
}, [/* trigger on graph data change */]);
```

### 3.9 (Optional) Auto-fit after layout

After layout completes, call the `fitView` logic from `GraphControlBar` to center and scale the graph in the viewport:

```typescript
// After runLayout(), compute bounding box and call transform()
```

This is already implemented in `GraphControlBar.fitView` — extract it into a reusable utility so it can be called programmatically.

## File Structure After This Step

```
src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/
  layout/
    elk-layout.ts          ← NEW: ELK integration (buildElkGraph, applyElkLayout, runLayout)
  atoms/
    ...                    ← unchanged
  components/
    ...                    ← unchanged
```

## Verification

1. Run `npm run build` in `src/vscode-bicep-ui` — should build with no errors.
2. Run `npm run dev` in the visual designer. The hardcoded test data (`A`, `B`, `C`, `D`, `E` nodes) should appear in a clean layered arrangement (top-to-bottom) instead of their hardcoded positions.
3. Drag nodes out of position, then click "Reset Layout" — nodes should animate back to ELK-computed positions.
4. Test with various graph topologies:
   - Linear chain (A → B → C → D)
   - Diamond (A → B, A → C, B → D, C → D)
   - Compound node with children
   - Disconnected components
5. Verify compound nodes properly contain their children with appropriate padding.
6. Verify the graph fits the viewport after initial layout.

## Notes

- **ELK bundle size**: `elkjs/lib/elk.bundled.js` is ~600KB gzipped. This is acceptable for a webview. If size becomes a concern, consider running ELK on the extension host side and sending computed positions to the webview.
- **ELK worker mode**: `elkjs` supports running in a Web Worker (`elkjs/lib/elk-worker.js`) for non-blocking layout. This can be added later if layout computation causes UI jank for large graphs.
- **Compound node dimensions**: For compound nodes, ELK computes the dimensions based on children + padding. We should NOT pass explicit `width`/`height` for compound nodes — ELK will compute these. However, we need to pass the compound node's padding configuration so ELK knows how much space to reserve for the label area (`padding.top: 50` in the current config).
- **Edge routing**: The old visualizer uses Cytoscape's bezier edge routing. The new visualizer uses straight edges with box-intersection clipping. ELK can compute edge routing (bend points), but sticking with straight edges is simpler and looks fine. Edge routing can be enhanced later.
