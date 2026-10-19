// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export interface PaintOrderNode {
  id: string;
  parentId: string | null;
}

/**
 * The order to paint nodes in, back to front, as an index per node ID.
 *
 * Nodes are absolutely positioned siblings, so containment has to be expressed by stacking: a container
 * is painted before its children, as nested elements would be. `raisedNodeId` is painted above its
 * siblings, and each of its ancestors above theirs, so the raised node and its subtree end up on top.
 * Sibling order is otherwise the order of `nodes`. A node whose parent is not in `nodes` is a root.
 */
export function computePaintOrder(nodes: Iterable<PaintOrderNode>, raisedNodeId: string | null): Map<string, number> {
  const parentById = new Map<string, string | null>();
  for (const { id, parentId } of nodes) {
    parentById.set(id, parentId);
  }

  const childrenByParent = new Map<string | null, string[]>();
  for (const [id, parentId] of parentById) {
    const scope = parentId !== null && parentById.has(parentId) ? parentId : null;
    const children = childrenByParent.get(scope);
    if (children) {
      children.push(id);
    } else {
      childrenByParent.set(scope, [id]);
    }
  }

  const raisedPath = new Set<string>();
  for (let id = raisedNodeId; id !== null && parentById.has(id); id = parentById.get(id) ?? null) {
    raisedPath.add(id);
  }

  const order = new Map<string, number>();
  const visit = (scope: string | null) => {
    const children = childrenByParent.get(scope) ?? [];
    const raised = children.find((id) => raisedPath.has(id));

    for (const id of raised === undefined ? children : [...children.filter((child) => child !== raised), raised]) {
      order.set(id, order.size);
      visit(id);
    }
  };
  visit(null);

  return order;
}

export function getNodeZIndex(paintOrder: ReadonlyMap<string, number>, nodeId: string): number {
  return 2 * (paintOrder.get(nodeId) ?? 0) + 2;
}

/**
 * A node an edge is stacked against: just below an endpoint, or just above the container that an exiting
 * endpoint is leaving.
 */
export interface EdgeStackAnchor {
  nodeId: string;
  placement: "below" | "above";
}

/** An edge with nothing to stack against is painted below every node. */
const BOTTOM_Z_INDEX = 0;

/**
 * The z-index of an edge: just below its higher endpoint, so an edge comes up with a node that is raised
 * above a container, and it never covers its own endpoints. Edges between nodes inside a container are
 * therefore painted over its frame. Each z-index sits between two nodes', so edges never tie with nodes.
 */
export function getEdgeZIndex(paintOrder: ReadonlyMap<string, number>, anchors: readonly EdgeStackAnchor[]): number {
  let zIndex = BOTTOM_Z_INDEX;

  for (const { nodeId, placement } of anchors) {
    if (paintOrder.has(nodeId)) {
      zIndex = Math.max(zIndex, getNodeZIndex(paintOrder, nodeId) + (placement === "below" ? -1 : 1));
    }
  }

  return zIndex;
}
