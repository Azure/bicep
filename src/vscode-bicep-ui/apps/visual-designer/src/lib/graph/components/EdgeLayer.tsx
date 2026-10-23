// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { EdgeAtomValue, NodeState } from "../atoms";
import type { EdgeStacking } from "../paint-order";

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
 * Draws every edge, stacked among the children of its endpoints' parent (see `getEdgeZIndex`). An edge that is
 * fading out with its nodes is stacked by the nearest ancestor still shown, such as a container that closed.
 */
export function EdgeLayer({ exitingNodes, exitingEdges, enteringNodeIds }: EdgeLayerProps) {
  const edges = useAtomValue(edgesAtom);
  const nodesById = useAtomValue(nodesByIdAtom);

  return useMemo(() => {
    const exitingById = new Map(exitingNodes.map((node) => [node.id, node]));
    const resolve = (id: string) => nodesById[id] ?? exitingById.get(id);
    const getShownParentId = (node: NodeState): string | null => {
      for (let id = node.parentId; id !== null; id = resolve(id)?.parentId ?? null) {
        if (nodesById[id]) {
          return id;
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

      // Edges join siblings, so both endpoints share a parent.
      const stacking: EdgeStacking = { parentId: getShownParentId(from), endpointIds: [from.id, to.id] };
      const isEntering = enteringNodeIds.has(edge.fromId) || enteringNodeIds.has(edge.toId);

      return [
        <StraightEdge
          key={edge.id}
          id={edge.id}
          from={from}
          to={to}
          stacking={stacking}
          isEntering={isEntering}
          isExiting={isExiting}
        />,
      ];
    });
  }, [edges, enteringNodeIds, exitingEdges, exitingNodes, nodesById]);
}
