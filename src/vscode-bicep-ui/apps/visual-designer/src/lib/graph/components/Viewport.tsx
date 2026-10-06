// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import { PanZoom } from "@vscode-bicep-ui/components";
import { useStore } from "jotai";
import { useEffect, useRef } from "react";
import styled, { useTheme } from "styled-components";
import { focusedNodeIdAtom } from "../atoms/nodes";
import { ViewportBackground } from "./ViewportBackground";

const CURSOR_SIZE = 32;

/** Total pointer travel, in pixels, at which a press stops being a click and becomes a drag. */
const DRAG_DISTANCE_THRESHOLD = 4;

const $Container = styled.div`
  position: absolute;
  left: 0;
  top: 0;
  right: 0;
  bottom: 0;
  overflow: hidden;

  &.grabbing,
  &.grabbing * {
    cursor: none !important;
  }
`;

const $PanZoom = styled(PanZoom)`
  width: 100%;
  height: 100%;
`;

const $GrabCursor = styled.div<{ $background: string; $blur: number }>`
  position: absolute;
  pointer-events: none;
  display: none;
  width: ${CURSOR_SIZE}px;
  height: ${CURSOR_SIZE}px;
  border-radius: 50%;
  transform: translate(-50%, -50%);
  background: ${({ $background }) => $background};
  backdrop-filter: blur(${({ $blur }) => $blur}px);
  -webkit-backdrop-filter: blur(${({ $blur }) => $blur}px);
  z-index: 9999;
`;

export interface ViewportProps extends PropsWithChildren {
  /** When false the dot-pattern background is hidden. Defaults to true. */
  showBackground?: boolean;
}

export function Viewport({ children, showBackground = true }: ViewportProps) {
  const theme = useTheme();
  const store = useStore();
  const containerRef = useRef<HTMLDivElement>(null);
  const cursorRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const container = containerRef.current;
    const cursor = cursorRef.current;
    if (!container || !cursor) return;

    let activePointerId: number | null = null;
    let pointerDownX = 0;
    let pointerDownY = 0;
    let cachedRect: DOMRect | null = null;
    let didDrag = false;
    let pendingFocusId: string | null | undefined;

    const hasTravelledDragDistance = (e: PointerEvent) =>
      Math.hypot(e.clientX - pointerDownX, e.clientY - pointerDownY) >= DRAG_DISTANCE_THRESHOLD;

    const endPointerGesture = () => {
      activePointerId = null;
      pointerDownX = 0;
      pointerDownY = 0;
      cachedRect = null;
      didDrag = false;
      pendingFocusId = undefined;
      cursor.style.display = "none";
      container.classList.remove("grabbing");
    };

    const onPointerDown = (e: PointerEvent) => {
      if (activePointerId !== null || !e.isPrimary || e.button !== 0) return;

      activePointerId = e.pointerId;
      pointerDownX = e.clientX;
      pointerDownY = e.clientY;
      // Remember the intended focus target but defer applying it
      // until pointerup so that a pan-drag doesn't clear focus.
      const nodeEl = (e.target as HTMLElement).closest<HTMLElement>("[data-node-id]");
      pendingFocusId = nodeEl?.dataset.nodeId ?? null;
      didDrag = false;

      cachedRect = container.getBoundingClientRect();
      cursor.style.display = "block";
      cursor.style.left = `${e.clientX - cachedRect.left}px`;
      cursor.style.top = `${e.clientY - cachedRect.top}px`;
      container.classList.add("grabbing");
    };

    const onPointerMove = (e: PointerEvent) => {
      if (e.pointerId !== activePointerId || !cachedRect) return;

      if (hasTravelledDragDistance(e)) didDrag = true;

      cursor.style.left = `${e.clientX - cachedRect.left}px`;
      cursor.style.top = `${e.clientY - cachedRect.top}px`;
    };

    const onPointerUp = (e: PointerEvent) => {
      if (e.pointerId !== activePointerId) return;

      // Apply focus only on click (no drag). Clicking a node focuses
      // it; clicking empty canvas clears focus. Drags leave focus
      // unchanged.
      if (!didDrag && !hasTravelledDragDistance(e) && pendingFocusId !== undefined) {
        store.set(focusedNodeIdAtom, pendingFocusId);
      }

      endPointerGesture();
    };

    const onPointerCancel = (e: PointerEvent) => {
      if (e.pointerId !== activePointerId) return;
      endPointerGesture();
    };

    container.addEventListener("pointerdown", onPointerDown);
    window.addEventListener("pointermove", onPointerMove);
    window.addEventListener("pointerup", onPointerUp);
    window.addEventListener("pointercancel", onPointerCancel);

    return () => {
      container.removeEventListener("pointerdown", onPointerDown);
      window.removeEventListener("pointermove", onPointerMove);
      window.removeEventListener("pointerup", onPointerUp);
      window.removeEventListener("pointercancel", onPointerCancel);
    };
  }, [store]);

  return (
    <$Container ref={containerRef} data-testid="graph-canvas" data-export-viewport="">
      <$PanZoom>
        {showBackground && <ViewportBackground />}
        {children}
      </$PanZoom>
      <$GrabCursor ref={cursorRef} $background={theme.grabCursor.background} $blur={theme.grabCursor.blur} />
    </$Container>
  );
}
