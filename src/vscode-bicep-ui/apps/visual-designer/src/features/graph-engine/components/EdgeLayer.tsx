// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue } from "jotai";
import { useMemo } from "react";
import { styled } from "styled-components";
import { edgesAtom } from "../atoms/edges";
import { focusedNodeIdAtom, getNodeZIndex } from "../atoms/nodes";
import { EdgeMarkerDefs } from "./EdgeMarkerDefs";
import { StraightEdge } from "./StraightEdge";

const $Svg = styled.svg<{ $zIndex: number }>`
  overflow: visible;
  position: absolute;
  pointer-events: none;
  z-index: ${({ $zIndex }) => $zIndex};
`;

/**
 * Outer edges sit below all nodes (z-index: -1).
 * Inner edges sit above their parent compound but below its atomic children,
 * with a per-group z-index that tracks the compound's focus elevation.
 */
export function OuterEdgeLayer() {
  const edges = useAtomValue(edgesAtom);

  const filtered = useMemo(() => {
    return edges.filter((edge) => {
      const fromParent = edge.fromId.includes("::") ? edge.fromId.split("::").slice(0, -1).join("::") : null;
      const toParent = edge.toId.includes("::") ? edge.toId.split("::").slice(0, -1).join("::") : null;
      return !(fromParent !== null && toParent !== null && fromParent === toParent);
    });
  }, [edges]);

  if (filtered.length === 0) {
    return null;
  }

  return (
    <$Svg $zIndex={-1}>
      <EdgeMarkerDefs />
      {filtered.map((edge) => (
        <StraightEdge key={edge.id} {...edge} />
      ))}
    </$Svg>
  );
}

export function InnerEdgeLayer() {
  const edges = useAtomValue(edgesAtom);
  const focusedNodeId = useAtomValue(focusedNodeIdAtom);

  // Group inner edges by their shared parent compound ID so each
  // group can be rendered at the correct z-index relative to its
  // parent compound (which may be elevated due to focus).
  const groups = useMemo(() => {
    const map = new Map<string, typeof edges>();

    for (const edge of edges) {
      const fromParent = edge.fromId.includes("::") ? edge.fromId.split("::").slice(0, -1).join("::") : null;
      const toParent = edge.toId.includes("::") ? edge.toId.split("::").slice(0, -1).join("::") : null;

      if (fromParent !== null && toParent !== null && fromParent === toParent) {
        let group = map.get(fromParent);
        if (!group) {
          group = [];
          map.set(fromParent, group);
        }
        group.push(edge);
      }
    }

    return map;
  }, [edges]);

  if (groups.size === 0) {
    return null;
  }

  return (
    <>
      {[...groups.entries()].map(([parentId, groupEdges]) => {
        const parentZ = getNodeZIndex(parentId, "compound", focusedNodeId);
        return (
          <$Svg key={parentId} $zIndex={parentZ + 1}>
            <EdgeMarkerDefs />
            {groupEdges.map((edge) => (
              <StraightEdge key={edge.id} {...edge} />
            ))}
          </$Svg>
        );
      })}
    </>
  );
}
