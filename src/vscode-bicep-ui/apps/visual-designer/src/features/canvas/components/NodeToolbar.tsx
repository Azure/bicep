// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { KeyboardEvent as ReactKeyboardEvent } from "react";
import type { NodeState } from "@/lib/graph";

import { Codicon, useGetPanZoomTransform, usePanZoomTransformListener } from "@vscode-bicep-ui/components";
import { useAtomValue, useStore } from "jotai";
import { useCallback, useEffect, useLayoutEffect, useRef, useState } from "react";
import { css, keyframes, styled } from "styled-components";
import { canvasElementAtom, selectedNodeIdsAtom, useGraphApi, useIsMotionReduced } from "@/core";
import { isExportOverlayOpenAtom } from "@/features/export";
import { DRAG_DISTANCE_THRESHOLD, nodesByIdAtom } from "@/lib/graph";
import { ARROW_CURSOR, FloatingPanel, ICON_BUTTON_ICON_SIZE, IconButton } from "@/ui";
import { isHandModeAtom, nodeMenuAtom } from "../atoms";
import { useCanvasNodeData } from "../hooks/use-canvas-node-data";
import { useNodeToggle } from "../hooks/use-node-toggle";
import { HEADER_HEIGHT } from "./nodes/ContainerNode";
import { NodeToggleIcon } from "./nodes/NodeToggleIcon";

/** Selects the toolbar, so canvas gestures and shortcuts leave presses and keys inside it alone. */
export const NODE_TOOLBAR_SELECTOR = "[data-node-toolbar]";

/** Screen pixels between the toolbar and its node. */
const GAP = 12;
/** How long the pointer rests on a node before its bar shows, or on another node before the bar moves there. */
const SHOW_DELAY = 150;
/** How long a bar outlasts the pointer leaving its node, so the pointer can cross the gap into it. */
const HIDE_DELAY = 300;
/** Screen pixels the toolbar keeps from the canvas edges. */
const EDGE_MARGIN = 4;
/** Graph units between a node's box and the frame drawn inside it: the content's margin. */
const FRAME_INSET = 4;

const fadeIn = keyframes`
  from {
    opacity: 0;
  }
`;

/** Fades in whenever it shows or moves to another node, unless motion is reduced. */
const $Toolbar = styled(FloatingPanel)<{ $animate: boolean }>`
  position: absolute;
  left: 0;
  top: 0;
  z-index: 300;
  flex-direction: row;
  /* Shown once placed, so it never paints at the canvas origin first. */
  visibility: hidden;
  animation: ${({ $animate }) =>
    $animate
      ? css`
          ${fadeIn} 120ms ease-out
        `
      : "none"};
`;

/** Disabled with `aria-disabled` rather than `disabled`, so its tooltip still says why. */
const $ToggleButton = styled(IconButton)<{ $isError: boolean }>`
  color: ${({ $isError, theme }) => ($isError ? theme.error : theme.iconButton.color)};

  &[aria-disabled="true"] {
    cursor: ${ARROW_CURSOR};
    background-color: transparent;
    transform: none;
  }

  &[aria-busy="true"] {
    cursor: progress;
  }

  &[aria-disabled="true"]:not([aria-busy="true"]) > * {
    opacity: 0.45;
  }
`;

function isInToolbar(target: EventTarget | null) {
  return target instanceof Element && target.closest(NODE_TOOLBAR_SELECTOR) !== null;
}

/**
 * Whether a press on the canvas has travelled far enough to drag a node, draw a marquee, or pan, which hides the
 * toolbar until it is released. A click does not, so selecting a node does not make its toolbar flicker.
 */
function useIsCanvasGesturing(canvas: HTMLElement | null) {
  const [isGesturing, setIsGesturing] = useState(false);

  useEffect(() => {
    if (!canvas) return;

    let start: { pointerId: number; x: number; y: number } | null = null;

    const onPointerDown = (event: PointerEvent) => {
      if (event.isPrimary && !isInToolbar(event.target)) {
        start = { pointerId: event.pointerId, x: event.clientX, y: event.clientY };
      }
    };
    const onPointerMove = (event: PointerEvent) => {
      if (
        start?.pointerId === event.pointerId &&
        Math.hypot(event.clientX - start.x, event.clientY - start.y) >= DRAG_DISTANCE_THRESHOLD
      ) {
        setIsGesturing(true);
      }
    };
    const onPointerEnd = (event: PointerEvent) => {
      if (start?.pointerId === event.pointerId) {
        start = null;
        setIsGesturing(false);
      }
    };

    // Captured, so a node or the pan-zoom surface cannot hide the press by stopping it.
    canvas.addEventListener("pointerdown", onPointerDown, true);
    window.addEventListener("pointermove", onPointerMove);
    window.addEventListener("pointerup", onPointerEnd);
    window.addEventListener("pointercancel", onPointerEnd);
    return () => {
      canvas.removeEventListener("pointerdown", onPointerDown, true);
      window.removeEventListener("pointermove", onPointerMove);
      window.removeEventListener("pointerup", onPointerEnd);
      window.removeEventListener("pointercancel", onPointerEnd);
    };
  }, [canvas]);

  return isGesturing;
}

type NodeToolbarContentProps = {
  node: NodeState;
  canvas: HTMLElement;
  /** Shown because the pointer rests on the node rather than because it is selected. */
  isTransient: boolean;
};

function NodeToolbarContent({ node, canvas, isTransient }: NodeToolbarContentProps) {
  const store = useStore();
  const api = useGraphApi();
  const getPanZoomTransform = useGetPanZoomTransform();
  const data = useCanvasNodeData(node.id);
  const { toggle, isRestoring, activate } = useNodeToggle(node.id, data);
  const ref = useRef<HTMLDivElement>(null);
  const reduceMotion = useIsMotionReduced();

  // Written straight to the element, since it follows the node on every frame of a pan, zoom, drag, or layout.
  const place = useCallback(() => {
    const element = ref.current;
    const box = store.get(node.boxAtom);
    if (!element) return;

    if (!box) {
      element.style.visibility = "hidden";
      return;
    }

    const { x, y, scale } = getPanZoomTransform();
    const left = (box.min.x + FRAME_INSET) * scale + x;
    const right = (box.max.x - FRAME_INSET) * scale + x;
    const top = (box.min.y + FRAME_INSET) * scale + y;
    const bottom = (box.max.y - FRAME_INSET) * scale + y;
    // An expanded container's toolbar belongs to its header, not the frame around its children.
    const isContainer = store.get(node.childIdsAtom).length > 0 && store.get(node.isPlacedAtom);
    const anchorBottom = isContainer ? Math.min(bottom, top + HEADER_HEIGHT * scale) : bottom;
    const { clientWidth: width, clientHeight: height } = canvas;
    const { offsetWidth: toolbarWidth, offsetHeight: toolbarHeight } = element;

    const above = top - GAP - toolbarHeight;
    const preferred = above >= EDGE_MARGIN ? above : anchorBottom + GAP;
    // Right-aligned with the part of the node in view, so a wide container's bar does not drift off screen.
    const rightX = Math.min(right, width);
    const toolbarX = Math.min(Math.max(rightX - toolbarWidth, EDGE_MARGIN), width - toolbarWidth - EDGE_MARGIN);
    const toolbarY = Math.min(Math.max(preferred, EDGE_MARGIN), height - toolbarHeight - EDGE_MARGIN);
    const isInView = right > 0 && left < width && bottom > 0 && top < height;

    element.style.transform = `translate(${Math.round(toolbarX)}px, ${Math.round(toolbarY)}px)`;
    element.style.visibility = isInView ? "visible" : "hidden";
  }, [canvas, getPanZoomTransform, node, store]);

  useLayoutEffect(() => {
    place();
    const unsubscribe = store.sub(node.boxAtom, place);
    const resizeObserver = new ResizeObserver(place);
    resizeObserver.observe(canvas);
    if (ref.current) resizeObserver.observe(ref.current);

    return () => {
      unsubscribe();
      resizeObserver.disconnect();
    };
  }, [canvas, node, place, store]);

  usePanZoomTransformListener(place);

  const goToSource = useCallback(() => api.revealNode(node.id), [api, node.id]);

  const handleKeyDown = useCallback(
    (event: ReactKeyboardEvent<HTMLDivElement>) => {
      const buttons = [...event.currentTarget.querySelectorAll<HTMLButtonElement>("button")];
      const index = buttons.findIndex((button) => button === document.activeElement);
      const focusAt = (next: number) => buttons[(next + buttons.length) % buttons.length]?.focus();

      switch (event.key) {
        case "ArrowRight":
          focusAt(index + 1);
          break;
        case "ArrowLeft":
          focusAt(index - 1);
          break;
        case "Home":
          focusAt(0);
          break;
        case "End":
          focusAt(buttons.length - 1);
          break;
        case "Escape":
          canvas.focus({ preventScroll: true });
          break;
        default:
          return;
      }

      event.preventDefault();
      event.stopPropagation();
    },
    [canvas],
  );

  if (!data) return null;

  return (
    <$Toolbar
      ref={ref}
      role="toolbar"
      aria-label={`Actions for ${data.symbolicName}`}
      aria-orientation="horizontal"
      data-export-exclude=""
      data-node-toolbar=""
      data-transient={isTransient}
      $animate={!reduceMotion}
      onKeyDown={handleKeyDown}
    >
      <IconButton type="button" title="Go to Source" aria-label="Go to Source" onClick={goToSource}>
        <Codicon name="go-to-file" size={ICON_BUTTON_ICON_SIZE} />
      </IconButton>
      {toggle && (
        <$ToggleButton
          type="button"
          title={toggle.title}
          aria-label={`${toggle.action} ${data.nodeKind === "module" ? "module" : "resource"} ${data.symbolicName}`}
          aria-expanded={toggle.action === "Collapse"}
          aria-busy={isRestoring}
          aria-disabled={toggle.isDisabled === true}
          data-source-status={data.canCollapse || isRestoring ? undefined : (data.sourceStatus ?? undefined)}
          $isError={toggle.isError === true}
          onClick={activate}
        >
          <NodeToggleIcon toggle={toggle} isRestoring={isRestoring} progressTestId="module-restore-progress" />
        </$ToggleButton>
      )}
    </$Toolbar>
  );
}

/**
 * The node the pointer rests on, or null. It changes only after the pointer settles, so sweeping across a dense graph
 * does not flash a bar over every node, and it outlasts a short trip across the gap into the toolbar, which keeps the
 * node it shows for. Touch has no hover.
 */
function useHoveredNodeId(canvas: HTMLElement | null) {
  const [hoveredNodeId, setHoveredNodeId] = useState<string | null>(null);

  useEffect(() => {
    if (!canvas) return;

    let current: string | null = null;
    let pending: string | null = null;
    let timer: ReturnType<typeof setTimeout> | undefined;

    const settle = (nodeId: string | null) => {
      if (nodeId === pending) return;

      pending = nodeId;
      clearTimeout(timer);
      if (nodeId === current) return;

      timer = setTimeout(
        () => {
          current = nodeId;
          setHoveredNodeId(nodeId);
        },
        nodeId === null ? HIDE_DELAY : SHOW_DELAY,
      );
    };

    const onPointerMove = (event: PointerEvent) => {
      if (event.pointerType === "touch") return;
      if (isInToolbar(event.target)) {
        settle(current);
        return;
      }

      const element = event.target instanceof Element ? event.target.closest<HTMLElement>("[data-node-id]") : null;
      settle(element?.dataset.nodeId ?? null);
    };
    const onPointerLeave = () => settle(null);

    canvas.addEventListener("pointermove", onPointerMove);
    canvas.addEventListener("pointerleave", onPointerLeave);
    return () => {
      clearTimeout(timer);
      canvas.removeEventListener("pointermove", onPointerMove);
      canvas.removeEventListener("pointerleave", onPointerLeave);
    };
  }, [canvas]);

  return hoveredNodeId;
}

/**
 * The actions for a node, floating above it at screen size whatever the zoom: for the node the pointer rests on, and
 * otherwise for the one selected node. One bar shows at a time. It shows only with the select tool, and hides while the
 * canvas is being dragged, panned, or marqueed, while a context menu is open, and while exporting. It is outside the
 * graph and the export subtree, so it never affects layout or an export.
 */
export function NodeToolbar() {
  const canvas = useAtomValue(canvasElementAtom);
  const selected = useAtomValue(selectedNodeIdsAtom);
  const nodesById = useAtomValue(nodesByIdAtom);
  const isHandMode = useAtomValue(isHandModeAtom);
  const isMenuOpen = useAtomValue(nodeMenuAtom) !== null;
  const isExportOpen = useAtomValue(isExportOverlayOpenAtom);
  const isGesturing = useIsCanvasGesturing(canvas);
  const hoveredNodeId = useHoveredNodeId(canvas);
  const [selectedNodeId] = selected;
  const hoveredNode = hoveredNodeId === null ? undefined : nodesById[hoveredNodeId];
  const node =
    hoveredNode ?? (selected.size === 1 && selectedNodeId !== undefined ? nodesById[selectedNodeId] : undefined);

  if (!node || !canvas || isHandMode || isMenuOpen || isExportOpen || isGesturing) {
    return null;
  }

  return <NodeToolbarContent key={node.id} node={node} canvas={canvas} isTransient={!selected.has(node.id)} />;
}
