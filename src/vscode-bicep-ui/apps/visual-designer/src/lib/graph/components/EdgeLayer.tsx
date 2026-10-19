// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { EdgeAtomValue, NodeState } from "../atoms";

import { useAtomValue } from "jotai";
import { useContext, useMemo } from "react";
import { styled } from "styled-components";
import { edgesAtom, nodesByIdAtom } from "../atoms";
import { GraphContext } from "../context/GraphContext";
import { getContainerEdgesZIndex, ROOT_EDGES_Z_INDEX } from "../paint-order";
import { StraightEdge } from "./StraightEdge";

const $ScopeSvg = styled.svg.attrs<{ $zIndex: number }>(({ $zIndex }) => ({ style: { zIndex: $zIndex } }))`
  overflow: visible;
  position: absolute;
  pointer-events: none;
`;

interface DrawnEdge {
  id: string;
  from: NodeState;
  to: NodeState;
  isEntering: boolean;
  isExiting: boolean;
}

interface EdgeScope {
  /** The container both endpoints sit in, or null for edges between root nodes or across containers. */
  parentId: string | null;
  edges: DrawnEdge[];
}

function groupByScope(edges: readonly DrawnEdge[]): EdgeScope[] {
  const edgesByScope = new Map<string | null, DrawnEdge[]>();

  for (const edge of edges) {
    // An exiting edge stays in its scope, so it fades out over the container its nodes are leaving.
    const parentId = edge.from.parentId === edge.to.parentId ? edge.from.parentId : null;
    const scopeEdges = edgesByScope.get(parentId);

    if (scopeEdges) {
      scopeEdges.push(edge);
    } else {
      edgesByScope.set(parentId, [edge]);
    }
  }

  return [...edgesByScope].map(([parentId, scopeEdges]) => ({ parentId, edges: scopeEdges }));
}

export interface EdgeLayerProps {
  exitingNodes: readonly NodeState[];
  exitingEdges: readonly EdgeAtomValue[];
  /** An edge to a node fading in fades in with it. */
  enteringNodeIds: ReadonlySet<string>;
}

/**
 * Draws every edge.
 *
 * Nodes are absolutely positioned siblings stacked by z-index, and one SVG occupies one place in that
 * stack, so edges are drawn in one SVG per scope: those between root nodes below every node, and those
 * inside a container just above it -- over its frame, under its children -- following it as the paint
 * order changes.
 */
export function EdgeLayer({ exitingNodes, exitingEdges, enteringNodeIds }: EdgeLayerProps) {
  const edges = useAtomValue(edgesAtom);
  const nodesById = useAtomValue(nodesByIdAtom);
  const { paintOrderAtom } = useContext(GraphContext);
  const paintOrder = useAtomValue(paintOrderAtom);

  // The edge elements are created once per graph change, so restacking re-renders no edges.
  const scopes = useMemo(() => {
    const exitingById = new Map(exitingNodes.map((node) => [node.id, node]));
    const resolve = (id: string) => nodesById[id] ?? exitingById.get(id);
    const drawnEdges = [
      ...edges.map((edge) => ({ edge, isExiting: false })),
      ...exitingEdges.map((edge) => ({ edge, isExiting: true })),
    ].flatMap(({ edge, isExiting }) => {
      const from = resolve(edge.fromId);
      const to = resolve(edge.toId);
      const isEntering = enteringNodeIds.has(edge.fromId) || enteringNodeIds.has(edge.toId);
      return from && to ? [{ id: edge.id, from, to, isEntering, isExiting }] : [];
    });

    return groupByScope(drawnEdges).map(({ parentId, edges: scopeEdges }) => ({
      parentId,
      children: scopeEdges.map(({ id, from, to, isEntering, isExiting }) => (
        <StraightEdge key={id} from={from} to={to} isEntering={isEntering} isExiting={isExiting} />
      )),
    }));
  }, [edges, enteringNodeIds, exitingEdges, exitingNodes, nodesById]);

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
