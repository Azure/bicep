// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodeState } from "../atoms";
import type { EdgeStackAnchor } from "../paint-order";

import { atom, useStore } from "jotai";
import { motion } from "motion/react";
import { useContext, useEffect, useLayoutEffect, useMemo, useRef } from "react";
import { styled, useTheme } from "styled-components";
import { boxesOverlap, getBoxCenter, getBoxCenterSegmentIntersection } from "@/lib/math";
import { GraphContext } from "../context/GraphContext";
import { cancelWrite, scheduleWrite } from "../hooks/batched-writes";
import { NODE_ENTER_TRANSITION, NODE_EXIT_TRANSITION } from "../motion";
import { getEdgeZIndex } from "../paint-order";

const EDGE_FADE_TRANSITION = { duration: 0.1, ease: "easeOut" } as const;

/** One SVG per edge, so each edge takes its own place in the stack of nodes. */
const $EdgeSvg = styled.svg`
  overflow: visible;
  position: absolute;
  pointer-events: none;
`;

const $EdgePath = styled(motion.path)`
  transition: stroke 180ms ease;
`;

export interface StraightEdgeProps {
  id: string;
  from: NodeState;
  to: NodeState;
  /** What the edge is stacked against as nodes are restacked. */
  stackAnchors: readonly EdgeStackAnchor[];
  /** The edge appeared with a node that fades in, so it fades in with that node. */
  isEntering: boolean;
  /** The edge has left the graph and fades out. */
  isExiting: boolean;
}

/** A straight edge between the boxes its nodes are drawn at, following them as they move. */
export function StraightEdge({ id, from, to, stackAnchors, isEntering, isExiting }: StraightEdgeProps) {
  const theme = useTheme();
  const svgRef = useRef<SVGSVGElement>(null);
  const ref = useRef<SVGPathElement>(null);
  const store = useStore();
  const { paintOrderAtom } = useContext(GraphContext);
  const zIndexAtom = useMemo(
    () => atom((get) => getEdgeZIndex(get(paintOrderAtom), stackAnchors)),
    [paintOrderAtom, stackAnchors],
  );

  // Restacking writes the z-index directly, so it re-renders no edges.
  useLayoutEffect(() => {
    const key = {};
    const write = () => {
      if (svgRef.current) {
        svgRef.current.style.zIndex = String(store.get(zIndexAtom));
      }
    };

    scheduleWrite(key, write);
    const unsubscribe = store.sub(zIndexAtom, () => scheduleWrite(key, write));

    return () => {
      unsubscribe();
      cancelWrite(key);
    };
  }, [store, zIndexAtom]);
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
    <$EdgeSvg ref={svgRef} data-edge-id={id}>
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
    </$EdgeSvg>
  );
}
