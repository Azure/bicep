// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { EdgeAtomValue } from "@/lib/graph/atoms/edges";

import { atom, useStore } from "jotai";
import { useEffect, useMemo, useRef } from "react";
import { styled, useTheme } from "styled-components";
import { nodesByIdAtom } from "@/lib/graph/atoms";
import { boxesOverlap, getBoxCenter, getBoxCenterSegmentIntersection } from "@/lib/utils/math";

const $EdgePath = styled.path`
  transition: stroke 180ms ease;
`;

export function StraightEdge({ fromId, toId }: EdgeAtomValue) {
  const theme = useTheme();
  const ref = useRef<SVGPathElement>(null);
  const store = useStore();
  const edgeSegmentAtom = useMemo(
    () =>
      atom((get) => {
        const fromNode = get(nodesByIdAtom)[fromId];
        const toNode = get(nodesByIdAtom)[toId];

        if (!fromNode || !toNode) {
          return {};
        }

        const fromBox = get(fromNode.boxAtom);
        const toBox = get(toNode.boxAtom);

        if (boxesOverlap(fromBox, toBox)) {
          return {};
        }

        const fromCenter = getBoxCenter(fromBox);
        const toCenter = getBoxCenter(toBox);

        return {
          from: getBoxCenterSegmentIntersection(fromBox, toCenter),
          to: getBoxCenterSegmentIntersection(toBox, fromCenter),
        };
      }),
    [fromId, toId],
  );

  useEffect(() => {
    const onEdgeSegmentUpdate = () => {
      if (!ref.current) {
        return;
      }

      const { from, to } = store.get(edgeSegmentAtom);

      if (!from || !to) {
        ref.current.removeAttribute("d");
      } else if (isNaN(from.x) || isNaN(from.y) || isNaN(to.x) || isNaN(to.y)) {
        ref.current.removeAttribute("d");
      } else {
        ref.current.setAttribute("d", `M ${from.x} ${from.y} L ${to.x} ${to.y}`);
      }
    };

    onEdgeSegmentUpdate();

    return store.sub(edgeSegmentAtom, () => onEdgeSegmentUpdate());
  }, [store, edgeSegmentAtom]);

  return (
    <$EdgePath
      ref={ref}
      fill="none"
      stroke={theme.edge.color}
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth={1.5}
      markerEnd="url(#line-arrow)"
    />
  );
}
