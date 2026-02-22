// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import { PanZoom } from "@vscode-bicep-ui/components";
import { useEffect, useRef } from "react";
import styled, { useTheme } from "styled-components";
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
  const containerRef = useRef<HTMLDivElement>(null);
  const cursorRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const container = containerRef.current;
    const cursor = cursorRef.current;
    if (!container || !cursor) return;

    let isActive = false;
    let cachedRect: DOMRect | null = null;

    const onPointerDown = (e: PointerEvent) => {
      isActive = true;
      cachedRect = container.getBoundingClientRect();
      cursor.style.display = "block";
      cursor.style.left = `${e.clientX - cachedRect.left}px`;
      cursor.style.top = `${e.clientY - cachedRect.top}px`;
      container.style.cursor = "none";
    };

    const onPointerMove = (e: PointerEvent) => {
      if (!isActive || !cachedRect) return;
      cursor.style.left = `${e.clientX - cachedRect.left}px`;
      cursor.style.top = `${e.clientY - cachedRect.top}px`;
    };

    const onPointerUp = () => {
      isActive = false;
      cachedRect = null;
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
  }, []);

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
