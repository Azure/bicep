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

/** Edges between root nodes are painted below every node. */
export const ROOT_EDGES_Z_INDEX = -1;

export function getNodeZIndex(paintOrder: ReadonlyMap<string, number>, nodeId: string): number {
  return 2 * (paintOrder.get(nodeId) ?? 0) + 2;
}

/** Edges inside a container are painted over its frame and under its children. */
export function getContainerEdgesZIndex(paintOrder: ReadonlyMap<string, number>, containerId: string): number {
  return getNodeZIndex(paintOrder, containerId) + 1;
}
