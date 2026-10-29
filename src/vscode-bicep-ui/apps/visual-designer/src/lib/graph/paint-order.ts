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

/** An edge between two sibling nodes. */
export interface PaintOrderEdge {
  fromId: string;
  toId: string;
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
 * `raisedNodeIds` are the nodes the user raised, from least to most recently raised, as windows are stacked. A
 * raised node is painted above its siblings, each of its ancestors above theirs, and a node raised later above one
 * raised earlier. The other siblings keep their order, below every raised one.
 *
 * A node sits on a sibling compound node when their target boxes overlap and it is drawn above that node: it was
 * raised more recently, or neither was raised and it is atomic or later in sibling order. So a raised compound node,
 * such as a module the user just opened, is drawn above everything it overlaps, while an atomic node that nobody
 * raised is drawn above a compound node that nobody raised. The edges of a node sitting on a compound node are painted
 * just above it, so they cross the compound node, and below the nodes they join. All nodes share one stacking context,
 * so that lifts the nodes sitting on a compound node, and the siblings joined to them by an edge, above it. Every
 * other sibling stays where it was drawn: one drawn below a compound node it overlaps stays below it, along with
 * whatever is drawn below that sibling and overlaps it. A node whose parent is not in `nodes` is a root.
 */
export function computePaintOrder(
  nodes: Iterable<PaintOrderNode>,
  edges: Iterable<PaintOrderEdge>,
  raisedNodeIds: readonly string[],
): PaintOrder {
  const nodesById = new Map<string, PaintOrderNode>();
  for (const node of nodes) {
    nodesById.set(node.id, node);
  }

  const neighborIds = new Map<string, string[]>();
  const link = (id: string, neighborId: string) => {
    const ids = neighborIds.get(id);
    if (ids) {
      ids.push(neighborId);
    } else {
      neighborIds.set(id, [neighborId]);
    }
  };
  for (const { fromId, toId } of edges) {
    link(fromId, toId);
    link(toId, fromId);
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

  // How recently each node, or a node inside it, was raised: 0 when never, and higher when more recently.
  const ranks = new Map<string, number>();
  raisedNodeIds.forEach((raisedId, index) => {
    for (let id: string | null = raisedId; id !== null && nodesById.has(id); id = nodesById.get(id)?.parentId ?? null) {
      ranks.set(id, index + 1);
    }
  });
  const rank = (id: string) => ranks.get(id) ?? 0;

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
    // Sorting is stable, so siblings nobody raised keep their order.
    const siblings = [...(childIdsByParent.get(parentId) ?? [])].sort((left, right) => rank(left) - rank(right));
    const indices = new Map(siblings.map((id, index) => [id, index]));
    const index = (id: string) => indices.get(id) ?? -1;
    const isCompound = (id: string) => nodesById.get(id)?.isCompound === true;
    const overlap = (left: string, right: string) => {
      const leftBox = nodesById.get(left)?.targetBox;
      const rightBox = nodesById.get(right)?.targetBox;
      return !!leftBox && !!rightBox && boxesOverlap(leftBox, rightBox);
    };
    // Whether `id` is drawn above `compoundId` where they overlap, before anything is lifted.
    const isAboveCompound = (id: string, compoundId: string) =>
      rank(id) > rank(compoundId) ||
      (rank(id) === rank(compoundId) && (!isCompound(id) || index(id) > index(compoundId)));

    const occupied = new Set<string>();
    for (const compoundId of siblings.filter(isCompound)) {
      for (const id of siblings) {
        if (id !== compoundId && isAboveCompound(id, compoundId) && overlap(id, compoundId)) {
          occupied.add(compoundId);
          nodesOnCompounds.add(id);
        }
      }
    }

    // The nodes sitting on a compound node and their neighbors are lifted above it, so their edges can cross it.
    const lifted = new Set<string>();
    for (const id of siblings) {
      if (nodesOnCompounds.has(id)) {
        for (const liftedId of [id, ...(neighborIds.get(id) ?? [])]) {
          if (indices.has(liftedId) && !occupied.has(liftedId)) {
            lifted.add(liftedId);
          }
        }
      }
    }

    // What stays below the occupied compound nodes: each sibling drawn below one of them that overlaps it.
    const sunk = new Set(occupied);
    for (let changed = true; changed; ) {
      changed = false;
      for (const id of siblings) {
        if (sunk.has(id) || lifted.has(id)) {
          continue;
        }

        const isBelowSunk = [...sunk].some(
          (sunkId) =>
            (occupied.has(sunkId) ? !isAboveCompound(id, sunkId) : index(id) < index(sunkId)) && overlap(id, sunkId),
        );
        if (isBelowSunk) {
          sunk.add(id);
          changed = true;
        }
      }
    }

    const below = take();
    // Sunk siblings were drawn below every occupied compound node they overlap, so sibling order keeps them below.
    siblings.filter((id) => sunk.has(id)).forEach(paint);
    const sittingOnCompound = occupied.size > 0 ? take() : null;
    edgeLayers.set(parentId, { below, sittingOnCompound });
    siblings.filter((id) => !sunk.has(id)).forEach(paint);
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
