// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue } from "jotai";
import { useMemo } from "react";
import { styled } from "styled-components";
import { edgesAtom } from "../atoms/edges";
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
 * Inner edges sit above compound nodes but below atomic nodes (z-index: 1).
 */
export function OuterEdgeLayer() {
  return <FilteredEdgeLayer zIndex={-1} inner={false} />;
}

export function InnerEdgeLayer() {
  return <FilteredEdgeLayer zIndex={1} inner={true} />;
}

function FilteredEdgeLayer({ zIndex, inner }: { zIndex: number; inner: boolean }) {
  const edges = useAtomValue(edgesAtom);

  const filtered = useMemo(() => {
    // Use the :: id convention: an edge is "inner" if both endpoints
    // share the same compound parent prefix.
    return edges.filter((edge) => {
      const fromParent = edge.fromId.includes("::") ? edge.fromId.split("::").slice(0, -1).join("::") : null;
      const toParent = edge.toId.includes("::") ? edge.toId.split("::").slice(0, -1).join("::") : null;
      const isInner = fromParent !== null && toParent !== null && fromParent === toParent;
      return inner ? isInner : !isInner;
    });
  }, [edges, inner]);

  if (filtered.length === 0) {
    return null;
  }

  return (
    <$Svg $zIndex={zIndex}>
      <EdgeMarkerDefs />
      {filtered.map((edge) => (
        <StraightEdge key={edge.id} {...edge} />
      ))}
    </$Svg>
  );
}
