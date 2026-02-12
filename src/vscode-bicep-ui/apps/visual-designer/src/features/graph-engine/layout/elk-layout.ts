// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ElkExtendedEdge, ElkNode } from "elkjs/lib/elk.bundled.js";
import type { AnimationPlaybackControlsWithThen } from "motion";
import type { PrimitiveAtom } from "jotai";
import type { Box } from "../../../utils/math/geometry";

import ELK from "elkjs/lib/elk.bundled.js";
import { getDefaultStore } from "jotai";
import { animate, transform } from "motion";
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

/** Duration (in seconds) of the spring animation when nodes move to new positions. */
const ANIMATION_DURATION_S = 0.6;

/**
 * Animate a node's boxAtom from its current position to a target position
 * using a spring animation.  Returns an awaitable animation control.
 */
function animateNodeTo(
  store: Store,
  boxAtom: PrimitiveAtom<Box>,
  targetX: number,
  targetY: number,
) {
  const box = store.get(boxAtom);
  const fromX = box.min.x;
  const fromY = box.min.y;

  const from = 0;
  const to = 100;
  const opts = { clamp: false };
  const xTransform = transform([from, to], [fromX, targetX], opts);
  const yTransform = transform([from, to], [fromY, targetY], opts);

  return animate(from, to, {
    type: "spring",
    duration: ANIMATION_DURATION_S,
    onUpdate: (latest) => {
      const x = xTransform(latest);
      const y = yTransform(latest);
      store.set(boxAtom, (b) => translateBox(b, x - b.min.x, y - b.min.y));
    },
  });
}

function applyElkLayout(
  store: Store,
  elkRoot: ElkNode,
  shouldAnimate: boolean,
  offsetX = 0,
  offsetY = 0,
  animations: AnimationPlaybackControlsWithThen[] = [],
): AnimationPlaybackControlsWithThen[] {
  const nodes = store.get(nodesAtom);

  for (const elkNode of elkRoot.children ?? []) {
    const node = nodes[elkNode.id];
    if (!node) continue;

    const x = (elkNode.x ?? 0) + offsetX;
    const y = (elkNode.y ?? 0) + offsetY;

    if (node.kind === "atomic") {
      if (shouldAnimate) {
        // Drive the spring animation directly on boxAtom and collect
        // the completion promise so callers can await all animations.
        animations.push(animateNodeTo(store, node.boxAtom, x, y));
      } else {
        // Place the node at the exact position immediately (no animation).
        const box = store.get(node.boxAtom);
        const dx = x - box.min.x;
        const dy = y - box.min.y;
        store.set(node.boxAtom, translateBox(box, dx, dy));
      }
    } else if (node.kind === "compound") {
      // For compound nodes, position their children relative to
      // the compound node's position. ELK gives children positions
      // relative to their parent.
      applyElkLayout(store, elkNode, shouldAnimate, x, y, animations);
    }
  }

  return animations;
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
 * The result of an ELK layout computation, ready to be applied to the store.
 */
export interface LayoutResult {
  elkRoot: ElkNode;
  offsetX: number;
  offsetY: number;
}

/**
 * Compute the ELK layout for the current graph without writing
 * anything to the store.  The returned {@link LayoutResult} can
 * be applied later via {@link applyLayout}, which allows the
 * caller to check for staleness between the async computation
 * and the synchronous store write.
 *
 * @param viewport When provided on the first layout, computes the
 *                 offset needed to center the graph in the viewport.
 *                 Subsequent layouts reuse the same offset automatically.
 */
export async function computeLayout(
  store: Store,
  viewport?: { width: number; height: number },
): Promise<LayoutResult> {
  const elkGraph = buildElkGraph(store);
  const elkRoot = await elk.layout(elkGraph);

  // Compute or reuse the offset so the graph stays centered.
  if (graphOffset === null && viewport) {
    const bbox = getElkBoundingBox(elkRoot);
    const graphCenterX = (bbox.minX + bbox.maxX) / 2;
    const graphCenterY = (bbox.minY + bbox.maxY) / 2;
    graphOffset = {
      x: viewport.width / 2 - graphCenterX,
      y: viewport.height / 2 - graphCenterY,
    };
  }

  return {
    elkRoot,
    offsetX: graphOffset?.x ?? 0,
    offsetY: graphOffset?.y ?? 0,
  };
}

/**
 * Synchronously start applying a previously computed layout to the store.
 * Returns a promise that resolves when all node animations have completed.
 *
 * @param animate When false, nodes snap to their final positions
 *                immediately (useful for the initial render).
 */
export async function applyLayout(store: Store, result: LayoutResult, animate = true): Promise<void> {
  const animations = applyElkLayout(store, result.elkRoot, animate, result.offsetX, result.offsetY);
  await Promise.all(animations);
}
