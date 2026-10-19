// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodeState } from "../atoms";

import { atom, useStore } from "jotai";
import { motion } from "motion/react";
import { useEffect, useMemo, useRef } from "react";
import { styled, useTheme } from "styled-components";
import { boxesOverlap, getBoxCenter, getBoxCenterSegmentIntersection } from "@/lib/math";
import { NODE_ENTER_TRANSITION, NODE_EXIT_TRANSITION } from "../motion";

const EDGE_FADE_TRANSITION = { duration: 0.1, ease: "easeOut" } as const;

const $EdgePath = styled(motion.path)`
  transition: stroke 180ms ease;
`;

export interface StraightEdgeProps {
  from: NodeState;
  to: NodeState;
  /** The edge appeared with a node that fades in, so it fades in with that node. */
  isEntering: boolean;
  /** The edge has left the graph and fades out. */
  isExiting: boolean;
}

/** A straight edge between the boxes its nodes are drawn at, following them as they move. */
export function StraightEdge({ from, to, isEntering, isExiting }: StraightEdgeProps) {
  const theme = useTheme();
  const ref = useRef<SVGPathElement>(null);
  const store = useStore();
  const segmentAtom = useMemo(
    () =>
      atom((get) => {
        const fromBox = get(from.boxAtom);
        const toBox = get(to.boxAtom);

        if (!fromBox || !toBox || boxesOverlap(fromBox, toBox)) {
          return null;
        }

        const segmentFrom = getBoxCenterSegmentIntersection(fromBox, getBoxCenter(toBox));
        const segmentTo = getBoxCenterSegmentIntersection(toBox, getBoxCenter(fromBox));

        return segmentFrom && segmentTo ? { from: segmentFrom, to: segmentTo } : null;
      }),
    [from, to],
  );

  useEffect(() => {
    const draw = () => {
      if (!ref.current) {
        return;
      }

      const segment = store.get(segmentAtom);

      if (!segment || [segment.from.x, segment.from.y, segment.to.x, segment.to.y].some(isNaN)) {
        ref.current.removeAttribute("d");
      } else {
        ref.current.setAttribute("d", `M ${segment.from.x} ${segment.from.y} L ${segment.to.x} ${segment.to.y}`);
      }
    };

    draw();

    return store.sub(segmentAtom, draw);
  }, [store, segmentAtom]);

  return (
    <$EdgePath
      ref={ref}
      // An edge that appears with a graph change fades in rather than popping into view.
      initial={{ opacity: 0 }}
      animate={{ opacity: isExiting ? 0 : 1 }}
      transition={isExiting ? NODE_EXIT_TRANSITION : isEntering ? NODE_ENTER_TRANSITION : EDGE_FADE_TRANSITION}
      fill="none"
      stroke={theme.edge.color}
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth={1.5}
      markerEnd="url(#line-arrow)"
    />
  );
}
