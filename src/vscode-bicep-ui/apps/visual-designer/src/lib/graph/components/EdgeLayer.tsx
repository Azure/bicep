// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { EdgeAtomValue, NodeState } from "../atoms";
import type { EdgeStackAnchor } from "../paint-order";

import { useAtomValue } from "jotai";
import { useMemo } from "react";
import { edgesAtom, nodesByIdAtom } from "../atoms";
import { StraightEdge } from "./StraightEdge";

export interface EdgeLayerProps {
  exitingNodes: readonly NodeState[];
  exitingEdges: readonly EdgeAtomValue[];
  /** An edge to a node fading in fades in with it. */
  enteringNodeIds: ReadonlySet<string>;
}

/**
 * Draws every edge, each stacked just below its higher endpoint (see `getEdgeZIndex`). An edge that is
 * fading out with its nodes is stacked above the container they are leaving.
 */
export function EdgeLayer({ exitingNodes, exitingEdges, enteringNodeIds }: EdgeLayerProps) {
  const edges = useAtomValue(edgesAtom);
  const nodesById = useAtomValue(nodesByIdAtom);

  return useMemo(() => {
    const exitingById = new Map(exitingNodes.map((node) => [node.id, node]));
    const resolve = (id: string) => nodesById[id] ?? exitingById.get(id);
    const getAnchor = (node: NodeState): EdgeStackAnchor | null => {
      if (nodesById[node.id]) {
        return { nodeId: node.id, placement: "below" };
      }

      for (let id = node.parentId; id !== null; id = resolve(id)?.parentId ?? null) {
        if (nodesById[id]) {
          return { nodeId: id, placement: "above" };
        }
      }

      return null;
    };

    return [
      ...edges.map((edge) => ({ edge, isExiting: false })),
      ...exitingEdges.map((edge) => ({ edge, isExiting: true })),
    ].flatMap(({ edge, isExiting }) => {
      const from = resolve(edge.fromId);
      const to = resolve(edge.toId);
      if (!from || !to) {
        return [];
      }

      const anchors = [getAnchor(from), getAnchor(to)].filter((anchor) => anchor !== null);
      const isEntering = enteringNodeIds.has(edge.fromId) || enteringNodeIds.has(edge.toId);

      return [
        <StraightEdge
          key={edge.id}
          id={edge.id}
          from={from}
          to={to}
          stackAnchors={anchors}
          isEntering={isEntering}
          isExiting={isExiting}
        />,
      ];
    });
  }, [edges, enteringNodeIds, exitingEdges, exitingNodes, nodesById]);
}
