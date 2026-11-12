// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";
import type { Point } from "@/lib/math";

import { PanZoom, useGetPanZoomTransform } from "@vscode-bicep-ui/components";
import { useStore } from "jotai";
import { useCallback, useEffect, useEffectEvent, useLayoutEffect, useRef } from "react";
import styled, { useTheme } from "styled-components";
import { isGraphInteractiveTarget } from "../interactive";
import { getNodeIdsInMarquee, getSelectableNodes } from "../selection";
import { ViewportBackground } from "./ViewportBackground";

const CURSOR_SIZE = 32;

/** Total pointer travel, in pixels, at which a press stops being a click and becomes a drag. */
const DRAG_DISTANCE_THRESHOLD = 4;

const PRIMARY_BUTTON = 0;
const SECONDARY_BUTTON = 2;

/**
 * What a left press does on the canvas.
 *
 * - `pan`: on the background it pans; on a node it moves the node. Right presses do nothing.
 * - `select`: on the background it draws a marquee; on a node it moves the node, unless Alt, Ctrl, or Cmd is held,
 *   which draws a marquee from there too, such as inside a module's frame.
 * - `hand`: it pans wherever it starts, and clicks are not reported.
 *
 * In `select` and `hand`, a right press pans, and a right click without travel is reported as a context menu request.
 */
export type ViewportMode = "pan" | "select" | "hand";

export interface NodeClickModifiers {
  /** Shift, Ctrl, or Cmd was held: add to or remove from the selection rather than replace it. */
  additive: boolean;
}

const $Container = styled.div<{ $cursor?: string; $grabbingCursor?: string }>`
  position: absolute;
  left: 0;
  top: 0;
  right: 0;
  bottom: 0;
  overflow: hidden;
  /* Presses on the canvas pan, drag, or draw a marquee; none of them should select the text in nodes. */
  user-select: none;
  -webkit-user-select: none;

  ${({ $cursor }) => ($cursor ? `&, & [data-node-id] { cursor: ${$cursor}; }` : "")}

  /* While panning, the pointer is the grabbing cursor, or hidden behind the grab indicator. */
  &.grabbing,
  &.grabbing * {
    cursor: ${({ $grabbingCursor }) => $grabbingCursor ?? "none"} !important;
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

const $Marquee = styled.div`
  position: absolute;
  left: 0;
  top: 0;
  display: none;
  box-sizing: border-box;
  border: 1px solid ${({ theme }) => theme.marquee.border};
  background: ${({ theme }) => theme.marquee.background};
  pointer-events: none;
  z-index: 9998;
`;

export interface ViewportProps extends PropsWithChildren {
  /** When false the dot-pattern background is hidden. Defaults to true. */
  showBackground?: boolean;
  /** What a left press does. Defaults to `pan`. */
  mode?: ViewportMode;
  /** A CSS cursor for the canvas and its nodes, or undefined for the default. */
  cursor?: string;
  /**
   * A CSS cursor shown while panning. Without one, the pointer is hidden behind a translucent grab
   * indicator instead.
   */
  grabbingCursor?: string;
  /**
   * A left press was released without travelling far enough to drag: on a node, with its ID, or on the
   * background, with null. Not reported in `hand` mode.
   */
  onNodeClick?: (nodeId: string | null, modifiers: NodeClickModifiers) => void;
  /** A right press was released without panning: on a node, with its ID, or on the background, with null. */
  onNodeContextMenu?: (nodeId: string | null, clientPoint: Point) => void;
  /** A marquee started, in `select` mode. */
  onMarqueeStart?: (modifiers: NodeClickModifiers) => void;
  /** The nodes the marquee covers changed. Reported as the marquee moves. */
  onMarqueeChange?: (nodeIds: string[]) => void;
}

function ignore() {}

function isAdditive(event: PointerEvent) {
  return event.shiftKey || event.ctrlKey || event.metaKey;
}

type Gesture = {
  pointerId: number;
  kind: "press" | "marquee" | "secondary";
  start: Point;
  rect: DOMRect;
  pressedNodeId: string | null;
  didDrag: boolean;
  additive: boolean;
};

export function Viewport({
  children,
  showBackground = true,
  mode = "pan",
  cursor,
  grabbingCursor,
  onNodeClick = ignore,
  onNodeContextMenu = ignore,
  onMarqueeStart = ignore,
  onMarqueeChange = ignore,
}: ViewportProps) {
  const theme = useTheme();
  const store = useStore();
  const getPanZoomTransform = useGetPanZoomTransform();
  const modeRef = useRef(mode);
  const handleClick = useEffectEvent(onNodeClick);
  const handleContextMenu = useEffectEvent(onNodeContextMenu);
  const handleMarqueeStart = useEffectEvent(onMarqueeStart);
  const handleMarqueeChange = useEffectEvent(onMarqueeChange);
  const containerRef = useRef<HTMLDivElement>(null);
  const cursorRef = useRef<HTMLDivElement>(null);
  const marqueeRef = useRef<HTMLDivElement>(null);
  const grabbingCursorRef = useRef(grabbingCursor);

  useLayoutEffect(() => {
    modeRef.current = mode;
    grabbingCursorRef.current = grabbingCursor;
  });

  // A right press pans outside the viewer. A left press pans unless the select tool draws a marquee instead.
  const panZoomFilter = useCallback((event: Event) => {
    const { ctrlKey, button } = event as MouseEvent;
    if (event.type === "mousedown") {
      const mode = modeRef.current;
      return (
        (button === SECONDARY_BUTTON && mode !== "pan") || (button === PRIMARY_BUTTON && !ctrlKey && mode !== "select")
      );
    }

    return (!ctrlKey || event.type === "wheel") && !button;
  }, []);

  useEffect(() => {
    const container = containerRef.current;
    const grabCursor = cursorRef.current;
    const marquee = marqueeRef.current;
    if (!container || !grabCursor || !marquee) return;

    let gesture: Gesture | null = null;

    const travelled = (event: PointerEvent, from: Point) =>
      Math.hypot(event.clientX - from.x, event.clientY - from.y) >= DRAG_DISTANCE_THRESHOLD;

    const showGrabCursor = (event: PointerEvent, rect: DOMRect) => {
      grabCursor.style.display = grabbingCursorRef.current ? "none" : "block";
      grabCursor.style.left = `${event.clientX - rect.left}px`;
      grabCursor.style.top = `${event.clientY - rect.top}px`;
      container.classList.add("grabbing");
    };

    const drawMarquee = (event: PointerEvent, { start, rect }: Gesture) => {
      const left = Math.min(start.x, event.clientX) - rect.left;
      const top = Math.min(start.y, event.clientY) - rect.top;
      const width = Math.abs(event.clientX - start.x);
      const height = Math.abs(event.clientY - start.y);
      marquee.style.display = "block";
      marquee.style.transform = `translate(${left}px, ${top}px)`;
      marquee.style.width = `${width}px`;
      marquee.style.height = `${height}px`;

      const { x, y, scale } = getPanZoomTransform();
      const toGraph = (screen: number, offset: number) => (screen - offset) / scale;
      handleMarqueeChange(
        getNodeIdsInMarquee(getSelectableNodes(store), {
          min: { x: toGraph(left, x), y: toGraph(top, y) },
          max: { x: toGraph(left + width, x), y: toGraph(top + height, y) },
        }),
      );
    };

    const endGesture = () => {
      gesture = null;
      grabCursor.style.display = "none";
      marquee.style.display = "none";
      container.classList.remove("grabbing");
    };

    const onPointerDown = (event: PointerEvent) => {
      if (gesture !== null || !event.isPrimary || isGraphInteractiveTarget(event.target)) return;
      if (event.button !== PRIMARY_BUTTON && (event.button !== SECONDARY_BUTTON || modeRef.current === "pan")) return;

      const rect = container.getBoundingClientRect();
      const nodeElement = event.target instanceof Element ? event.target.closest<HTMLElement>("[data-node-id]") : null;
      const pressedNodeId = nodeElement?.dataset.nodeId ?? null;
      const isMarquee =
        event.button === PRIMARY_BUTTON &&
        modeRef.current === "select" &&
        (pressedNodeId === null || event.altKey || event.ctrlKey || event.metaKey) &&
        event.pointerType !== "touch";

      gesture = {
        pointerId: event.pointerId,
        kind: event.button === SECONDARY_BUTTON ? "secondary" : isMarquee ? "marquee" : "press",
        start: { x: event.clientX, y: event.clientY },
        rect,
        pressedNodeId,
        didDrag: false,
        additive: isAdditive(event),
      };

      if (gesture.kind === "marquee") {
        handleMarqueeStart({ additive: gesture.additive });
      } else if (gesture.kind === "press" && modeRef.current !== "select") {
        // With the select tool, a press on a node moves or selects it: nothing is grabbed until a pan.
        showGrabCursor(event, rect);
      }
    };

    const onPointerMove = (event: PointerEvent) => {
      if (!gesture || event.pointerId !== gesture.pointerId) return;

      if (!gesture.didDrag && travelled(event, gesture.start)) {
        gesture.didDrag = true;
        if (gesture.kind === "secondary") {
          showGrabCursor(event, gesture.rect);
        }
      }

      if (gesture.kind === "marquee") {
        if (gesture.didDrag) drawMarquee(event, gesture);
      } else if (container.classList.contains("grabbing")) {
        grabCursor.style.left = `${event.clientX - gesture.rect.left}px`;
        grabCursor.style.top = `${event.clientY - gesture.rect.top}px`;
      }
    };

    const onPointerUp = (event: PointerEvent) => {
      if (!gesture || event.pointerId !== gesture.pointerId) return;

      const current = gesture;
      const isClick = !current.didDrag && !travelled(event, current.start);
      endGesture();

      if (!isClick) return;

      if (current.kind === "secondary") {
        handleContextMenu(current.pressedNodeId, { x: event.clientX, y: event.clientY });
      } else if (modeRef.current !== "hand") {
        // A marquee that never left its starting point is a click on the background.
        handleClick(current.pressedNodeId, { additive: current.additive });
      }
    };

    const onPointerCancel = (event: PointerEvent) => {
      if (gesture && event.pointerId === gesture.pointerId) endGesture();
    };

    // The context menu, if any, opens on release, after it is clear the press did not pan.
    const onContextMenu = (event: MouseEvent) => event.preventDefault();

    // A marquee can reach past the canvas, over text in the chrome around it, which must not be selected either.
    const onSelectStart = (event: Event) => {
      if (gesture?.kind === "marquee") event.preventDefault();
    };

    container.addEventListener("pointerdown", onPointerDown);
    container.addEventListener("contextmenu", onContextMenu);
    document.addEventListener("selectstart", onSelectStart, true);
    window.addEventListener("pointermove", onPointerMove);
    window.addEventListener("pointerup", onPointerUp);
    window.addEventListener("pointercancel", onPointerCancel);

    return () => {
      container.removeEventListener("pointerdown", onPointerDown);
      container.removeEventListener("contextmenu", onContextMenu);
      document.removeEventListener("selectstart", onSelectStart, true);
      window.removeEventListener("pointermove", onPointerMove);
      window.removeEventListener("pointerup", onPointerUp);
      window.removeEventListener("pointercancel", onPointerCancel);
    };
  }, [getPanZoomTransform, store]);

  return (
    <$Container
      ref={containerRef}
      $cursor={cursor}
      $grabbingCursor={grabbingCursor}
      data-testid="graph-canvas"
      data-export-viewport=""
    >
      <$PanZoom filter={panZoomFilter}>
        {showBackground && <ViewportBackground />}
        {children}
      </$PanZoom>
      <$Marquee ref={marqueeRef} data-testid="selection-marquee" data-export-exclude="" />
      <$GrabCursor ref={cursorRef} $background={theme.grabCursor.background} $blur={theme.grabCursor.blur} />
    </$Container>
  );
}
