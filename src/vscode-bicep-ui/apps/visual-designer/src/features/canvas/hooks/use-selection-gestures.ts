// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodeClickModifiers } from "@/lib/graph";
import type { Point } from "@/lib/math";

import { useAtomValue, useStore } from "jotai";
import { useCallback, useEffect, useMemo, useRef } from "react";
import {
  canvasElementAtom,
  selectedNodeIdsAtom,
  selectNodeAtom,
  setSelectedNodeIdsAtom,
  toggleNodeSelectedAtom,
} from "@/core";
import { isExportOverlayOpenAtom } from "@/features/export";
import { getNodeTargetBox } from "@/lib/graph";
import { isInTextInput } from "@/utils";
import { isHandModeAtom, nodeMenuAtom } from "../atoms";

export interface SelectionGestures {
  onNodeClick: (nodeId: string | null, modifiers: NodeClickModifiers) => void;
  onNodeContextMenu: (nodeId: string | null, clientPoint: Point) => void;
  onMarqueeStart: (modifiers: NodeClickModifiers) => void;
  onMarqueeChange: (nodeIds: string[]) => void;
}

/**
 * What clicks, marquees, and context-menu requests on the canvas do.
 *
 * A click selects one node, Shift/Ctrl/Cmd+click adds or removes one, and a marquee selects what it covers, added to the
 * selection when started with a modifier. A right click opens a node's context menu only when it leaves
 * exactly one node selected. Shift+F10 or the Context Menu key opens it for the selected node, and Escape
 * clears the selection.
 */
export function useSelectionGestures(): SelectionGestures {
  const store = useStore();
  const canvas = useAtomValue(canvasElementAtom);
  const marqueeBaseRef = useRef<ReadonlySet<string>>(new Set());

  const onNodeClick = useCallback(
    (nodeId: string | null, { additive }: NodeClickModifiers) => {
      if (nodeId === null) {
        if (!additive) store.set(selectNodeAtom, null);
      } else if (additive) {
        store.set(toggleNodeSelectedAtom, nodeId);
      } else {
        store.set(selectNodeAtom, nodeId);
      }
    },
    [store],
  );

  const onNodeContextMenu = useCallback(
    (nodeId: string | null, clientPoint: Point) => {
      if (store.get(isHandModeAtom) || store.get(isExportOverlayOpenAtom)) {
        return;
      }

      if (nodeId === null) {
        store.set(nodeMenuAtom, null);
        return;
      }

      // Right-clicking a node outside the selection selects it, as a click would; within a multiple selection it
      // keeps the selection, and offers no single-node actions.
      const selected = store.get(selectedNodeIdsAtom);
      if (!selected.has(nodeId)) {
        store.set(selectNodeAtom, nodeId);
      }

      store.set(nodeMenuAtom, store.get(selectedNodeIdsAtom).size === 1 ? { nodeId, clientPoint } : null);
    },
    [store],
  );

  const onMarqueeStart = useCallback(
    ({ additive }: NodeClickModifiers) => {
      marqueeBaseRef.current = additive ? store.get(selectedNodeIdsAtom) : new Set();
    },
    [store],
  );

  const onMarqueeChange = useCallback(
    (nodeIds: string[]) => {
      const next = new Set([...marqueeBaseRef.current, ...nodeIds]);
      const current = store.get(selectedNodeIdsAtom);
      // The marquee reports on every pointer move; most moves do not change what it covers.
      if (next.size !== current.size || [...next].some((id) => !current.has(id))) {
        store.set(setSelectedNodeIdsAtom, next);
      }
    },
    [store],
  );

  useEffect(() => {
    if (!canvas) return;

    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.defaultPrevented || isInTextInput(event.target) || store.get(isHandModeAtom)) return;
      if (event.target instanceof Element && event.target.closest('[role="menu"]')) return;

      const selected = store.get(selectedNodeIdsAtom);
      if (event.key === "ContextMenu" || (event.shiftKey && event.key === "F10")) {
        event.preventDefault();
        event.stopPropagation();
        const [nodeId] = selected;
        const box = selected.size === 1 && nodeId !== undefined ? getNodeTargetBox(store, nodeId) : null;
        if (nodeId === undefined || !box || store.get(isExportOverlayOpenAtom)) return;

        // The node's top center, in client coordinates.
        const element = canvas.querySelector<HTMLElement>(`[data-node-id="${CSS.escape(nodeId)}"]`);
        const rect = element?.getBoundingClientRect();
        if (rect) {
          store.set(nodeMenuAtom, { nodeId, clientPoint: { x: rect.left + rect.width / 2, y: rect.top } });
        }
      } else if (event.key === "Escape" && selected.size > 0 && !store.get(isExportOverlayOpenAtom)) {
        event.preventDefault();
        event.stopPropagation();
        store.set(nodeMenuAtom, null);
        store.set(selectNodeAtom, null);
      }
    };

    canvas.addEventListener("keydown", handleKeyDown);
    return () => canvas.removeEventListener("keydown", handleKeyDown);
  }, [canvas, store]);

  return useMemo(
    () => ({ onNodeClick, onNodeContextMenu, onMarqueeStart, onMarqueeChange }),
    [onMarqueeChange, onMarqueeStart, onNodeClick, onNodeContextMenu],
  );
}
