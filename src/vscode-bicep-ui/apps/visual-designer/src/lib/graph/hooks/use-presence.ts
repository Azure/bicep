// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { EdgeAtomValue, NodeState } from "../atoms";

import { useAtomValue } from "jotai";
import { useEffect, useState } from "react";
import { edgesAtom, nodesByIdAtom } from "../atoms";
import { NODE_EXIT_TRANSITION } from "../motion";

type NodesById = Readonly<Record<string, NodeState>>;

export interface Presence {
  /** Nodes that left the graph from inside a container that remains, while they fade out. */
  exitingNodes: readonly NodeState[];
  /** Edges that left the graph with an exiting node, while they fade out. */
  exitingEdges: readonly EdgeAtomValue[];
  /** Nodes that appeared inside a container that was not one before, so they fade in. */
  enteringNodeIds: ReadonlySet<string>;
}

const NO_PRESENCE: Presence = { exitingNodes: [], exitingEdges: [], enteringNodeIds: new Set() };

/** The nearest ancestor of `node`, looked up in `lookup`, that is also in `nodes`. */
function findAncestorIn(node: NodeState, lookup: NodesById, nodes: NodesById): string | null {
  for (let id = node.parentId; id !== null; id = lookup[id]?.parentId ?? null) {
    if (nodes[id]) {
      return id;
    }
  }

  return null;
}

export function derivePresence(
  previousNodes: NodesById,
  previousEdges: readonly EdgeAtomValue[],
  nodes: NodesById,
  edges: readonly EdgeAtomValue[],
): Presence {
  const previousContainerIds = new Set(
    Object.values(previousNodes).flatMap((node) => (node.parentId === null ? [] : [node.parentId])),
  );

  const exitingNodes = Object.values(previousNodes).filter(
    (node) => !nodes[node.id] && findAncestorIn(node, previousNodes, nodes) !== null,
  );

  const enteringNodeIds = new Set(
    Object.values(nodes)
      .filter((node) => {
        if (previousNodes[node.id]) {
          return false;
        }

        const ancestorId = findAncestorIn(node, nodes, previousNodes);
        return ancestorId !== null && !previousContainerIds.has(ancestorId);
      })
      .map((node) => node.id),
  );

  const exitingIds = new Set(exitingNodes.map((node) => node.id));
  const edgeIds = new Set(edges.map((edge) => edge.id));
  const exitingEdges = previousEdges.filter(
    (edge) => !edgeIds.has(edge.id) && (exitingIds.has(edge.fromId) || exitingIds.has(edge.toId)),
  );

  return { exitingNodes, exitingEdges, enteringNodeIds };
}

/**
 * Which nodes and edges fade in or out as the graph changes, so a container's descendants fade as it
 * opens or closes rather than popping. Other nodes appear and disappear at once, as do all of them when
 * motion is reduced.
 */
export function usePresence(reduceMotion: boolean): Presence {
  const nodes = useAtomValue(nodesByIdAtom);
  const edges = useAtomValue(edgesAtom);
  const [tracked, setTracked] = useState({ nodes, edges, presence: NO_PRESENCE });
  let presence = tracked.presence;

  if (tracked.nodes !== nodes || tracked.edges !== edges) {
    presence = reduceMotion ? NO_PRESENCE : derivePresence(tracked.nodes, tracked.edges, nodes, edges);
    setTracked({ nodes, edges, presence });
  }

  useEffect(() => {
    const { exitingNodes, exitingEdges } = tracked.presence;
    if (exitingNodes.length === 0 && exitingEdges.length === 0) {
      return;
    }

    const timer = window.setTimeout(
      () =>
        setTracked((current) =>
          current.presence === tracked.presence
            ? { ...current, presence: { ...current.presence, exitingNodes: [], exitingEdges: [] } }
            : current,
        ),
      NODE_EXIT_TRANSITION.duration * 1000 + 50,
    );

    return () => window.clearTimeout(timer);
  }, [tracked.presence]);

  return presence;
}
