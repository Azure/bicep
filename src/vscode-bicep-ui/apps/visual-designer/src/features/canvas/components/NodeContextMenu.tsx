// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { KeyboardEvent as ReactKeyboardEvent } from "react";

import { Codicon } from "@vscode-bicep-ui/components";
import { useAtom, useAtomValue, useStore } from "jotai";
import { useCallback, useEffect, useLayoutEffect, useRef } from "react";
import { styled } from "styled-components";
import { canvasElementAtom, selectedNodeIdsAtom, shownGraphAtom, useGraphApi } from "@/core";
import { isExportOverlayOpenAtom } from "@/features/export";
import {
  ARROW_CURSOR,
  FLOATING_PANEL_GAP,
  FloatingPanel,
  ICON_BUTTON_ICON_SIZE,
  ICON_BUTTON_SIZE,
  IconButton,
} from "@/ui";
import { isHandModeAtom, nodeMenuAtom } from "../atoms";
import { useCanvasNodeData } from "../hooks/use-canvas-node-data";
import { useNodeToggle } from "../hooks/use-node-toggle";
import { NodeToggleIcon } from "./nodes/NodeToggleIcon";

const $Menu = styled(FloatingPanel).attrs<{ $x: number; $y: number; $itemCount: number }>(({ $x, $y, $itemCount }) => ({
  style: {
    left: `max(4px, min(${$x}px, calc(100% - 168px)))`,
    // The panel's padding, its items, and the gaps between them, plus the 4px kept from the edge.
    top: `max(4px, min(${$y}px, calc(100% - ${8 + $itemCount * ICON_BUTTON_SIZE + ($itemCount - 1) * FLOATING_PANEL_GAP + 4}px)))`,
  },
}))`
  position: absolute;
  z-index: 301;
  min-width: 156px;
`;

const $MenuItem = styled(IconButton)`
  width: auto;
  justify-content: flex-start;
  padding: 0 8px;
  gap: 6px;
  white-space: nowrap;
  font: inherit;
  font-size: 12px;

  &[aria-disabled="true"] {
    opacity: 0.45;
    cursor: ${ARROW_CURSOR};
    background-color: transparent;
    transform: none;
  }
`;

/** A node's context menu, in screen space outside the transformed graph and its export subtree. */
export function NodeContextMenu() {
  const store = useStore();
  const [menu, setMenu] = useAtom(nodeMenuAtom);
  const canvas = useAtomValue(canvasElementAtom);
  const graph = useAtomValue(shownGraphAtom);
  const api = useGraphApi();
  const menuRef = useRef<HTMLDivElement>(null);
  const itemRef = useRef<HTMLButtonElement>(null);
  const node = menu ? graph.nodes.get(menu.nodeId) : undefined;
  const nodeId = menu?.nodeId ?? null;
  const { toggle, isRestoring, activate } = useNodeToggle(nodeId ?? "", useCanvasNodeData(nodeId));

  const dismiss = useCallback(() => {
    setMenu(null);
    canvas?.focus({ preventScroll: true });
  }, [canvas, setMenu]);

  const goToSource = useCallback(() => {
    if (menu) {
      api.revealNode(menu.nodeId);
      dismiss();
    }
  }, [api, dismiss, menu]);

  const toggleNode = useCallback(() => {
    if (toggle?.isDisabled !== true) {
      activate();
      dismiss();
    }
  }, [activate, dismiss, toggle]);

  useLayoutEffect(() => {
    if (node) itemRef.current?.focus({ preventScroll: true });
  }, [node]);

  // A change of selection, tool, or export state makes the menu stale.
  useEffect(() => {
    const close = () => store.set(nodeMenuAtom, null);
    const unsubscribe = [
      store.sub(selectedNodeIdsAtom, close),
      store.sub(isHandModeAtom, close),
      store.sub(isExportOverlayOpenAtom, close),
      store.sub(shownGraphAtom, () => {
        const current = store.get(nodeMenuAtom);
        if (current && !store.get(shownGraphAtom).nodes.has(current.nodeId)) close();
      }),
    ];
    return () => unsubscribe.forEach((dispose) => dispose());
  }, [store]);

  useEffect(() => {
    if (!menu) return;
    const closeOutside = (event: PointerEvent) => {
      if (event.target instanceof Node && !menuRef.current?.contains(event.target)) setMenu(null);
    };
    const close = () => setMenu(null);
    document.addEventListener("pointerdown", closeOutside);
    canvas?.addEventListener("wheel", close, { passive: true });
    window.addEventListener("resize", close);
    window.addEventListener("blur", close);
    return () => {
      document.removeEventListener("pointerdown", closeOutside);
      canvas?.removeEventListener("wheel", close);
      window.removeEventListener("resize", close);
      window.removeEventListener("blur", close);
    };
  }, [canvas, menu, setMenu]);

  const handleKeyDown = useCallback(
    (event: ReactKeyboardEvent) => {
      if (event.key === "Escape") {
        event.preventDefault();
        event.stopPropagation();
        dismiss();
      } else if (["ArrowDown", "ArrowUp", "Home", "End"].includes(event.key)) {
        event.preventDefault();
        event.stopPropagation();
        const items = [...(menuRef.current?.querySelectorAll<HTMLElement>('[role="menuitem"]') ?? [])];
        const index = items.findIndex((item) => item === document.activeElement);
        const next =
          event.key === "Home"
            ? 0
            : event.key === "End"
              ? items.length - 1
              : index + (event.key === "ArrowDown" ? 1 : -1);
        items[(next + items.length) % items.length]?.focus();
      } else if (event.key === "Tab") {
        dismiss();
      }
    },
    [dismiss],
  );

  if (!menu || !node || !canvas) return null;

  const bounds = canvas.getBoundingClientRect();

  return (
    <$Menu
      ref={menuRef}
      role="menu"
      aria-label={`Actions for ${node.symbolName}`}
      $x={menu.clientPoint.x - bounds.left}
      $y={menu.clientPoint.y - bounds.top}
      $itemCount={toggle ? 2 : 1}
      onKeyDown={handleKeyDown}
    >
      <$MenuItem ref={itemRef} role="menuitem" title="Go to Source" aria-label="Go to Source" onClick={goToSource}>
        <Codicon name="go-to-file" size={ICON_BUTTON_ICON_SIZE} />
        Go to Source
      </$MenuItem>
      {toggle && (
        <$MenuItem
          role="menuitem"
          title={toggle.title}
          aria-label={toggle.action}
          aria-disabled={toggle.isDisabled === true}
          onClick={toggleNode}
        >
          <NodeToggleIcon toggle={toggle} isRestoring={isRestoring} />
          {toggle.action}
        </$MenuItem>
      )}
    </$Menu>
  );
}
