// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { EdgeAtomValue } from "../atoms/edges";

import { atom, useAtomValue, useStore } from "jotai";
import { motion } from "motion/react";
import { useEffect, useMemo, useRef } from "react";
import { styled, useTheme } from "styled-components";
import { boxesOverlap, getBoxCenter, getBoxCenterSegmentIntersection } from "@/lib/math";
import { fadedNodeIdsAtom, nodesByIdAtom } from "../atoms";

const EDGE_FADE_TRANSITION = { duration: 0.1, ease: "easeOut" } as const;

const $EdgePath = styled(motion.path)`
  transition: stroke 180ms ease;
`;

export function StraightEdge({ fromId, toId }: EdgeAtomValue) {
  const theme = useTheme();
  const ref = useRef<SVGPathElement>(null);
  const store = useStore();
  const fadedNodeIds = useAtomValue(fadedNodeIdsAtom);
  const isFaded = fadedNodeIds.has(fromId) || fadedNodeIds.has(toId);
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
      // An edge that appears with a graph change fades in rather than popping into view.
      initial={{ opacity: 0 }}
      animate={{ opacity: isFaded ? 0 : 1 }}
      transition={EDGE_FADE_TRANSITION}
      fill="none"
      stroke={theme.edge.color}
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth={1.5}
      markerEnd="url(#line-arrow)"
    />
  );
}
