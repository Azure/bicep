import type { Graph, Node } from "./types";

export function getAncestors<TNode extends Node>(graph: Graph<TNode>, node: TNode): TNode[] {
  const ancestors = [];

  while (node.parentId) {
    ancestors.push(graph.nodesById[node.parentId]);
  }

  return ancestors;
}

export function getChildren<TNode extends Node>(graph: Graph<TNode>, node: TNode): TNode[] {
  const children = [];

  if (node.childIds) {
    for (const childId of node.childIds ?? []) {
      children.push(graph.nodesById[childId]);
    }
  }

  return children;
}

export function getDescendants<TNode extends Node>(graph: Graph<TNode>, node: TNode): TNode[] {
  const descendants = new Set<TNode>();
  const descendantIds = node.childIds ?? [];

  while (descendantIds.length > 0) {
    node = graph.nodesById[descendantIds.shift() as string];

    if (!descendants.has(node)) {
      descendants.add(node);
      descendantIds.push(...(node.childIds ?? []));
    }
  }

  return Array.from(descendants);
}
