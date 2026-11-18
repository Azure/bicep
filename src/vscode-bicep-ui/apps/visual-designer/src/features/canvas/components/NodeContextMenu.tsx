// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { KeyboardEvent as ReactKeyboardEvent } from "react";

import { Codicon } from "@vscode-bicep-ui/components";
import { useAtom, useAtomValue, useStore } from "jotai";
import { useCallback, useEffect, useLayoutEffect, useRef } from "react";
import { styled } from "styled-components";
import { canvasElementAtom, selectedNodeIdsAtom, shownGraphAtom, useGraphApi } from "@/core";
import { isExportOverlayOpenAtom } from "@/features/export";
import { FloatingPanel, ICON_BUTTON_ICON_SIZE, IconButton } from "@/ui";
import { isHandModeAtom, nodeMenuAtom } from "../atoms";

const $Menu = styled(FloatingPanel).attrs<{ $x: number; $y: number }>(({ $x, $y }) => ({
  style: {
    left: `max(4px, min(${$x}px, calc(100% - 168px)))`,
    top: `max(4px, min(${$y}px, calc(100% - 42px)))`,
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
        itemRef.current?.focus();
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
      onKeyDown={handleKeyDown}
    >
      <$MenuItem ref={itemRef} role="menuitem" title="Go to Source" aria-label="Go to Source" onClick={goToSource}>
        <Codicon name="go-to-file" size={ICON_BUTTON_ICON_SIZE} />
        Go to Source
      </$MenuItem>
    </$Menu>
  );
}
