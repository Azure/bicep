// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { EdgeAtomValue } from "../atoms/edges";

import { atom, useStore } from "jotai";
import { useEffect, useMemo, useRef } from "react";
import { useTheme } from "styled-components";
import { boxesOverlap, getBoxCenter, getBoxCenterSegmentIntersection } from "../../../utils/math";
import { nodesAtom } from "../atoms";

export function StraightEdge({ fromId, toId }: EdgeAtomValue) {
  const theme = useTheme();
  const ref = useRef<SVGPathElement>(null);
  const store = useStore();
  const edgeSegmentAtom = useMemo(
    () =>
      atom((get) => {
        const fromNode = get(nodesAtom)[fromId];
        const toNode = get(nodesAtom)[toId];

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
    <path
      ref={ref}
      fill="none"
      stroke={theme.edge.color}
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth={2}
      markerEnd="url(#line-arrow)"
    />
  );
}
