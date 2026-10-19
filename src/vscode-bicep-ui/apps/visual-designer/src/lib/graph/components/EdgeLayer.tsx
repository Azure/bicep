// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { EdgeAtomValue } from "../atoms/edges";
import type { NodeState } from "../atoms/nodes";

import { useAtomValue } from "jotai";
import { useContext, useMemo } from "react";
import { styled } from "styled-components";
import { edgesAtom } from "../atoms/edges";
import { nodesByIdAtom } from "../atoms/nodes";
import { GraphContext } from "../context/GraphContext";
import { getContainerEdgesZIndex, ROOT_EDGES_Z_INDEX } from "../paint-order";
import { StraightEdge } from "./StraightEdge";

const $ScopeSvg = styled.svg.attrs<{ $zIndex: number }>(({ $zIndex }) => ({ style: { zIndex: $zIndex } }))`
  overflow: visible;
  position: absolute;
  pointer-events: none;
`;

interface EdgeScope {
  /** The container both endpoints sit in, or null for edges between root nodes or across containers. */
  parentId: string | null;
  edges: EdgeAtomValue[];
}

function groupByScope(edges: EdgeAtomValue[], nodesById: Readonly<Record<string, NodeState>>): EdgeScope[] {
  const edgesByScope = new Map<string | null, EdgeAtomValue[]>();

  for (const edge of edges) {
    const fromParentId = nodesById[edge.fromId]?.parentId ?? null;
    const parentId = fromParentId === (nodesById[edge.toId]?.parentId ?? null) ? fromParentId : null;
    const scopeEdges = edgesByScope.get(parentId);

    if (scopeEdges) {
      scopeEdges.push(edge);
    } else {
      edgesByScope.set(parentId, [edge]);
    }
  }

  return [...edgesByScope].map(([parentId, scopeEdges]) => ({ parentId, edges: scopeEdges }));
}

/**
 * Draws every edge.
 *
 * Nodes are absolutely positioned siblings stacked by z-index, and one SVG occupies one place in that
 * stack, so edges are drawn in one SVG per scope: those between root nodes below every node, and those
 * inside a container just above it -- over its frame, under its children -- following it as the paint
 * order changes.
 */
export function EdgeLayer() {
  const edges = useAtomValue(edgesAtom);
  const nodesById = useAtomValue(nodesByIdAtom);
  const { paintOrderAtom } = useContext(GraphContext);
  const paintOrder = useAtomValue(paintOrderAtom);

  // The edge elements are created once per edge change, so restacking re-renders no edges.
  const scopes = useMemo(
    () =>
      groupByScope(edges, nodesById).map(({ parentId, edges: scopeEdges }) => ({
        parentId,
        children: scopeEdges.map((edge) => <StraightEdge key={edge.id} {...edge} />),
      })),
    [edges, nodesById],
  );

  return scopes.map(({ parentId, children }) => (
    <$ScopeSvg
      // Node IDs are never empty, so the root scope's key cannot collide with a container's.
      key={parentId ?? ""}
      $zIndex={parentId === null ? ROOT_EDGES_Z_INDEX : getContainerEdgesZIndex(paintOrder, parentId)}
    >
      {children}
    </$ScopeSvg>
  ));
}
