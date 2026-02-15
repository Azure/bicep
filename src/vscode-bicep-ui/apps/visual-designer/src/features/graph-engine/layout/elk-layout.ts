// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ElkExtendedEdge, ElkNode } from "elkjs/lib/elk.bundled.js";

import ELK from "elkjs/lib/elk.bundled.js";
import { getDefaultStore } from "jotai";
import { edgesAtom, nodesAtom } from "../atoms";

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

function applyElkLayout(store: Store, elkRoot: ElkNode, offsetX = 0, offsetY = 0): void {
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

export async function runLayout(store: Store): Promise<void> {
  const elkGraph = buildElkGraph(store);
  const layoutResult = await elk.layout(elkGraph);
  applyElkLayout(store, layoutResult);
}
