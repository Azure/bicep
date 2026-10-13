// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { EdgeAtomValue } from "../atoms/edges";

import { useAtomValue } from "jotai";
import { useMemo } from "react";
import { styled } from "styled-components";
import { edgesAtom } from "../atoms/edges";
import { focusedNodeIdAtom, getNodeZIndex, getParentId } from "../atoms/nodes";
import { StraightEdge } from "./StraightEdge";

/** Edges between top-level nodes sit below every node. */
const ROOT_SCOPE_Z_INDEX = -1;

const $ScopeSvg = styled.svg.attrs<{ $zIndex: number }>(({ $zIndex }) => ({ style: { zIndex: $zIndex } }))`
  overflow: visible;
  position: absolute;
  pointer-events: none;
`;

interface EdgeScope {
  /** The compound node both endpoints sit in, or null for top-level edges. */
  parentId: string | null;
  edges: EdgeAtomValue[];
}

function groupByScope(edges: EdgeAtomValue[]): EdgeScope[] {
  const edgesByScope = new Map<string | null, EdgeAtomValue[]>();

  for (const edge of edges) {
    const fromParentId = getParentId(edge.fromId);
    const parentId = fromParentId === getParentId(edge.toId) ? fromParentId : null;
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
 * stack, so edges are drawn in one SVG per scope: those between top-level nodes below every node, and
 * those inside a compound just above it -- over its frame, under its children -- following it as focus
 * restacks the graph.
 */
export function EdgeLayer() {
  const edges = useAtomValue(edgesAtom);
  const focusedNodeId = useAtomValue(focusedNodeIdAtom);

  // The edge elements are created once per edge change, so restacking on focus re-renders no edges.
  const scopes = useMemo(
    () =>
      groupByScope(edges).map(({ parentId, edges: scopeEdges }) => ({
        parentId,
        children: scopeEdges.map((edge) => <StraightEdge key={edge.id} {...edge} />),
      })),
    [edges],
  );

  return scopes.map(({ parentId, children }) => (
    <$ScopeSvg
      // Node IDs are never empty, so the root scope's key cannot collide with a compound's.
      key={parentId ?? ""}
      $zIndex={parentId === null ? ROOT_SCOPE_Z_INDEX : getNodeZIndex(parentId, "compound", focusedNodeId) + 1}
    >
      {children}
    </$ScopeSvg>
  ));
}
