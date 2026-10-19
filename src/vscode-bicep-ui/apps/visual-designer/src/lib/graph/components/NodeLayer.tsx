// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodeState } from "../atoms";

import { useAtomValue } from "jotai";
import { nodesByIdAtom } from "../atoms";
import { GraphNode } from "./GraphNode";

export interface NodeLayerProps {
  /** Nodes that have left the graph but are still fading out. */
  exitingNodes: readonly NodeState[];
  /** Nodes that fade in as they mount. */
  enteringNodeIds: ReadonlySet<string>;
}

export function NodeLayer({ exitingNodes, enteringNodeIds }: NodeLayerProps) {
  const nodesById = useAtomValue(nodesByIdAtom);

  return [
    ...Object.values(nodesById).map((node) => (
      <GraphNode key={node.id} node={node} isEntering={enteringNodeIds.has(node.id)} />
    )),
    ...exitingNodes.map((node) => <GraphNode key={node.id} node={node} isExiting />),
  ];
}
