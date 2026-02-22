// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import { PanZoom } from "@vscode-bicep-ui/components";
import { useStore } from "jotai";
import { useEffect, useRef } from "react";
import styled, { useTheme } from "styled-components";
import { focusedNodeIdAtom } from "../atoms/nodes";
import { CanvasBackground } from "./CanvasBackground";

const CURSOR_SIZE = 22;

const $Container = styled.div`
  position: absolute;
  left: 0;
  top: 0;
  right: 0;
  bottom: 0;
  overflow: hidden;
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

export interface CanvasProps extends PropsWithChildren {
  /** When false the dot-pattern background is hidden. Defaults to true. */
  showBackground?: boolean;
}

export function Canvas({ children, showBackground = true }: CanvasProps) {
  const theme = useTheme();
  const store = useStore();
  const containerRef = useRef<HTMLDivElement>(null);
  const cursorRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const container = containerRef.current;
    const cursor = cursorRef.current;
    if (!container || !cursor) return;

    let isActive = false;
    let cachedRect: DOMRect | null = null;
    let didDrag = false;
    let pendingFocusId: string | null | undefined;

    const onPointerDown = (e: PointerEvent) => {
      // Remember the intended focus target but defer applying it
      // until pointerup so that a pan-drag doesn't clear focus.
      const nodeEl = (e.target as HTMLElement).closest<HTMLElement>("[data-node-id]");
      pendingFocusId = nodeEl?.dataset.nodeId ?? null;
      didDrag = false;

      isActive = true;
      cachedRect = container.getBoundingClientRect();
      cursor.style.display = "block";
      cursor.style.left = `${e.clientX - cachedRect.left}px`;
      cursor.style.top = `${e.clientY - cachedRect.top}px`;
      container.style.cursor = "none";
    };

    const DRAG_THRESHOLD = 4;

    const onPointerMove = (e: PointerEvent) => {
      if (!isActive || !cachedRect) return;

      if (!didDrag) {
        const dx = e.movementX;
        const dy = e.movementY;
        if (Math.abs(dx) > DRAG_THRESHOLD || Math.abs(dy) > DRAG_THRESHOLD) {
          didDrag = true;
        }
      }

      cursor.style.left = `${e.clientX - cachedRect.left}px`;
      cursor.style.top = `${e.clientY - cachedRect.top}px`;
    };

    const onPointerUp = () => {
      // Apply focus only on click (no drag). Clicking a node focuses
      // it; clicking empty canvas clears focus. Drags leave focus
      // unchanged.
      if (!didDrag && pendingFocusId !== undefined) {
        store.set(focusedNodeIdAtom, pendingFocusId);
      }

      isActive = false;
      cachedRect = null;
      didDrag = false;
      pendingFocusId = undefined;
      cursor.style.display = "none";
      container.style.cursor = "";
    };

    container.addEventListener("pointerdown", onPointerDown);
    window.addEventListener("pointermove", onPointerMove);
    window.addEventListener("pointerup", onPointerUp);
    window.addEventListener("pointercancel", onPointerUp);

    return () => {
      container.removeEventListener("pointerdown", onPointerDown);
      window.removeEventListener("pointermove", onPointerMove);
      window.removeEventListener("pointerup", onPointerUp);
      window.removeEventListener("pointercancel", onPointerUp);
    };
  }, [store]);

  return (
    <$Container ref={containerRef}>
      <$PanZoom>
        {showBackground && <CanvasBackground />}
        {children}
      </$PanZoom>
      <$GrabCursor
        ref={cursorRef}
        $background={theme.grabCursor.background}
        $blur={theme.grabCursor.blur}
      />
    </$Container>
  );
}
