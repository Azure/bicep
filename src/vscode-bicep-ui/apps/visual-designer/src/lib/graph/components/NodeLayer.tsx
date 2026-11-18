// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodeState } from "../atoms";

import { useAtomValue, useStore } from "jotai";
import { useLayoutEffect } from "react";
import { committedGraphVersionAtom, graphVersionAtom, nodesByIdAtom } from "../atoms";
import { GraphNode } from "./GraphNode";

export interface NodeLayerProps {
  /** Nodes that have left the graph but are still fading out. */
  exitingNodes: readonly NodeState[];
  /** Nodes that fade in as they mount. */
  enteringNodeIds: ReadonlySet<string>;
}

export function NodeLayer({ exitingNodes, enteringNodeIds }: NodeLayerProps) {
  const store = useStore();
  const nodesById = useAtomValue(nodesByIdAtom);
  const graphVersion = useAtomValue(graphVersionAtom);

  // Layout effects run children first, so by now every node in this commit has rendered and measured
  // itself. Nodes are memoized, so rendering this layer for a new version re-renders only changed nodes.
  useLayoutEffect(() => {
    store.set(committedGraphVersionAtom, graphVersion);
  }, [graphVersion, store]);

  return [
    ...Object.values(nodesById).map((node) => (
      <GraphNode key={node.id} node={node} isEntering={enteringNodeIds.has(node.id)} />
    )),
    ...exitingNodes.map((node) => <GraphNode key={node.id} node={node} isExiting />),
  ];
}
