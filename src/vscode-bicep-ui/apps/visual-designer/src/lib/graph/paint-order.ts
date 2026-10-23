// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "@/lib/math";

import { boxesOverlap } from "@/lib/math";

export interface PaintOrderNode {
  id: string;
  parentId: string | null;
  /** Whether the node is drawn around children. */
  isCompound: boolean;
  /** Where the node is going, or null before it is measured. */
  targetBox: Box | null;
}

/** The z-indices of the edges between the children of one parent (the root when null). */
interface EdgeLayers {
  /** Edges between nodes that sit on no sibling: just above the parent, below all of its children. */
  below: number;
  /** Edges of a node that sits on a sibling compound node: just above those compound nodes, below the rest. */
  sittingOnCompound: number | null;
}

export interface PaintOrder {
  /** Each node's z-index. */
  readonly nodeZIndices: ReadonlyMap<string, number>;
  /** The edge layers of each parent with children, and of the root under the key null. */
  readonly edgeLayers: ReadonlyMap<string | null, EdgeLayers>;
  /** Nodes that sit on a sibling compound node, so their edges are drawn above it. */
  readonly nodesOnCompounds: ReadonlySet<string>;
}

/** An edge, stacked among the children of `parentId` (the root when null). Edges join siblings. */
export interface EdgeStacking {
  parentId: string | null;
  endpointIds: readonly string[];
}

/** The z-index of anything not in the paint order: below every node. */
const BOTTOM_Z_INDEX = 0;

/**
 * The order to paint nodes and edges in, back to front.
 *
 * Nodes are absolutely positioned siblings, so containment is expressed by stacking, as nested elements would
 * be: a parent is painted first, then the edges between its children, then each child with its descendants.
 * Edges are therefore below every node in their parent, and above the parent itself.
 *
 * Among siblings, `raisedNodeId` is painted last, and each of its ancestors last among theirs. A node sits on a
 * sibling compound node when their target boxes overlap and it is drawn above that node: it is atomic, or it comes
 * later in sibling order. Compound nodes that something sits on are painted before their other siblings, and the
 * edges of the nodes sitting on them just above them, so those edges cross the compound node but stay below every
 * other node. A node whose parent is not in `nodes` is a root.
 */
export function computePaintOrder(nodes: Iterable<PaintOrderNode>, raisedNodeId: string | null): PaintOrder {
  const nodesById = new Map<string, PaintOrderNode>();
  for (const node of nodes) {
    nodesById.set(node.id, node);
  }

  const childIdsByParent = new Map<string | null, string[]>();
  for (const { id, parentId } of nodesById.values()) {
    const parent = parentId !== null && nodesById.has(parentId) ? parentId : null;
    const childIds = childIdsByParent.get(parent);
    if (childIds) {
      childIds.push(id);
    } else {
      childIdsByParent.set(parent, [id]);
    }
  }

  const raisedPath = new Set<string>();
  for (let id = raisedNodeId; id !== null && nodesById.has(id); id = nodesById.get(id)?.parentId ?? null) {
    raisedPath.add(id);
  }

  const nodeZIndices = new Map<string, number>();
  const edgeLayers = new Map<string | null, EdgeLayers>();
  const nodesOnCompounds = new Set<string>();
  // Even z-indices, leaving the odd one above each node free for edges leaving it (see `getEdgeZIndex`).
  let nextZIndex = 2;
  const take = () => {
    const zIndex = nextZIndex;
    nextZIndex += 2;
    return zIndex;
  };

  const paintChildren = (parentId: string | null) => {
    const childIds = childIdsByParent.get(parentId) ?? [];
    const raised = childIds.find((id) => raisedPath.has(id));
    const siblings = raised === undefined ? childIds : [...childIds.filter((id) => id !== raised), raised];
    const occupied = new Set<string>();

    siblings.forEach((compoundId, compoundIndex) => {
      const compound = nodesById.get(compoundId);
      const compoundBox = compound?.isCompound ? compound.targetBox : null;
      if (!compoundBox) {
        return;
      }

      siblings.forEach((id, index) => {
        const sibling = nodesById.get(id);
        if (
          id !== compoundId &&
          sibling?.targetBox &&
          (!sibling.isCompound || index > compoundIndex) &&
          boxesOverlap(sibling.targetBox, compoundBox)
        ) {
          occupied.add(compoundId);
          nodesOnCompounds.add(id);
        }
      });
    });

    const below = take();
    const occupiedFirst = siblings.filter((id) => occupied.has(id));
    occupiedFirst.forEach(paint);
    const sittingOnCompound = occupiedFirst.length > 0 ? take() : null;
    edgeLayers.set(parentId, { below, sittingOnCompound });
    siblings.filter((id) => !occupied.has(id)).forEach(paint);
  };

  function paint(id: string) {
    nodeZIndices.set(id, take());
    if (childIdsByParent.has(id)) {
      paintChildren(id);
    }
  }

  paintChildren(null);

  return { nodeZIndices, edgeLayers, nodesOnCompounds };
}

export function getNodeZIndex(paintOrder: PaintOrder, nodeId: string): number | null {
  return paintOrder.nodeZIndices.get(nodeId) ?? null;
}

/**
 * The z-index of an edge among the children of its parent: below all of them, unless an endpoint sits on a sibling
 * compound node. An edge whose parent has no children left, such as one fading out of a container that closed, is
 * painted just above that parent.
 */
export function getEdgeZIndex(paintOrder: PaintOrder, { parentId, endpointIds }: EdgeStacking): number {
  const layers = paintOrder.edgeLayers.get(parentId);

  if (layers) {
    const isSittingOnCompound = endpointIds.some((id) => paintOrder.nodesOnCompounds.has(id));
    return isSittingOnCompound && layers.sittingOnCompound !== null ? layers.sittingOnCompound : layers.below;
  }

  const parentZIndex = parentId === null ? null : getNodeZIndex(paintOrder, parentId);
  return parentZIndex === null ? BOTTOM_Z_INDEX : parentZIndex + 1;
}
