// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ElkExtendedEdge, ElkNode } from "elkjs/lib/elk.bundled.js";

import ELK from "elkjs/lib/elk.bundled.js";
import { getDefaultStore } from "jotai";
import { translateBox } from "../../../utils/math";
import { nodesAtom, edgesAtom } from "../atoms";

type Store = ReturnType<typeof getDefaultStore>;

const elk = new ELK();

const ELK_OPTIONS: Record<string, string> = {
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

function buildElkGraph(store: Store): ElkNode {
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
  function toElkNode(node: (typeof nodes)[string]): ElkNode {
    if (!node) {
      return { id: "unknown" };
    }

    const box = store.get(node.boxAtom);
    const width = Math.max(box.max.x - box.min.x, 1);
    const height = Math.max(box.max.y - box.min.y, 1);

    if (node.kind === "compound") {
      const childIds = store.get(node.childIdsAtom);
      const children = childIds
        .map((id) => nodes[id])
        .filter((child): child is NonNullable<typeof child> => child !== undefined)
        .map(toElkNode);

      // Don't pass explicit width/height for compound nodes — ELK computes them
      // based on children + padding. Pass padding so ELK reserves space for the label.
      return {
        id: node.id,
        children,
        layoutOptions: {
          ...ELK_OPTIONS,
          "elk.padding": "[top=50,left=40,bottom=40,right=40]",
        },
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

function applyElkLayout(store: Store, elkRoot: ElkNode, animate: boolean, offsetX = 0, offsetY = 0): void {
  const nodes = store.get(nodesAtom);

  for (const elkNode of elkRoot.children ?? []) {
    const node = nodes[elkNode.id];
    if (!node) continue;

    const x = (elkNode.x ?? 0) + offsetX;
    const y = (elkNode.y ?? 0) + offsetY;

    if (node.kind === "atomic") {
      if (animate) {
        // Setting originAtom triggers spring animation in AtomicNode
        store.set(node.originAtom, { x, y });
      } else {
        // Place the node at the exact position immediately (no animation).
        // Set boxAtom first so min matches origin, then set originAtom.
        const box = store.get(node.boxAtom);
        const dx = x - box.min.x;
        const dy = y - box.min.y;
        store.set(node.boxAtom, translateBox(box, dx, dy));
        store.set(node.originAtom, { x, y });
      }
    } else if (node.kind === "compound") {
      // For compound nodes, position their children relative to
      // the compound node's position. ELK gives children positions
      // relative to their parent.
      applyElkLayout(store, elkNode, animate, x, y);
    }
  }
}

/**
 * Persistent offset applied to all ELK-computed positions so the
 * graph appears centered in the viewport. Set on the first layout
 * and reused on subsequent layouts to keep the graph in place.
 */
let graphOffset: { x: number; y: number } | null = null;

/**
 * Compute the bounding box of top-level ELK nodes from the layout result.
 */
function getElkBoundingBox(elkRoot: ElkNode): { minX: number; minY: number; maxX: number; maxY: number } {
  let minX = Infinity;
  let minY = Infinity;
  let maxX = -Infinity;
  let maxY = -Infinity;

  for (const child of elkRoot.children ?? []) {
    const x = child.x ?? 0;
    const y = child.y ?? 0;
    const w = child.width ?? 0;
    const h = child.height ?? 0;
    minX = Math.min(minX, x);
    minY = Math.min(minY, y);
    maxX = Math.max(maxX, x + w);
    maxY = Math.max(maxY, y + h);
  }

  return { minX, minY, maxX, maxY };
}

/**
 * Run ELK layout on the current graph.
 * @param animate When false, nodes snap to their final positions immediately
 *                (useful for the initial render). Defaults to true.
 * @param viewport When provided on the first layout, computes the offset
 *                 needed to center the graph in the viewport. Subsequent
 *                 layouts reuse the same offset automatically.
 */
export async function runLayout(
  store: Store,
  animate = true,
  viewport?: { width: number; height: number },
): Promise<void> {
  const elkGraph = buildElkGraph(store);
  const layoutResult = await elk.layout(elkGraph);

  // Compute or reuse the offset so the graph stays centered.
  if (graphOffset === null && viewport) {
    const bbox = getElkBoundingBox(layoutResult);
    const graphCenterX = (bbox.minX + bbox.maxX) / 2;
    const graphCenterY = (bbox.minY + bbox.maxY) / 2;
    graphOffset = {
      x: viewport.width / 2 - graphCenterX,
      y: viewport.height / 2 - graphCenterY,
    };
  }

  const ox = graphOffset?.x ?? 0;
  const oy = graphOffset?.y ?? 0;

  applyElkLayout(store, layoutResult, animate, ox, oy);
}
