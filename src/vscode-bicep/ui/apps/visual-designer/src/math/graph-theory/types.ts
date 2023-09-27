export interface Node {
  id: string;
  compound?: boolean;
  parentId?: string;
  childIds?: string[];
}

export interface Edge {
  sourceId: string;
  targetId: string;
}

export interface Graph<TNode extends Node = Node, TEdge extends Edge = Edge> {
  nodesById: Record<string, TNode>;
  edges: TEdge[];
}
