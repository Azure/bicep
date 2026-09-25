// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PointerEvent as ReactPointerEvent } from "react";
import type { PaletteDragState } from "../atoms";

import { useSetAtom } from "jotai";
import { useCallback, useEffect, useRef } from "react";
import { paletteDragAtom } from "../atoms";

interface PendingDrag extends PaletteDragState {
  pointerId: number;
  originX: number;
  originY: number;
  /** A press only becomes a drag once the pointer travels past the threshold. */
  dragging: boolean;
}

/** Pointer travel (px) that separates a click from a drag. */
const DRAG_THRESHOLD = 4;

export function usePaletteDrag(
  canPlaceResourceAt: (clientPoint: { x: number; y: number }) => boolean,
  onDrop: (item: PaletteDragState["item"], clientX: number, clientY: number) => void,
) {
  const activeDragRef = useRef<PendingDrag | null>(null);
  const setDragState = useSetAtom(paletteDragAtom);

  const cancelDrag = useCallback(() => {
    activeDragRef.current = null;
    setDragState(null);
  }, [setDragState]);

  useEffect(() => cancelDrag, [cancelDrag]);

  useEffect(() => {
    const handlePointerMove = (event: PointerEvent) => {
      const drag = activeDragRef.current;
      if (!drag || drag.pointerId !== event.pointerId) {
        return;
      }
      if (!drag.dragging && Math.hypot(event.clientX - drag.originX, event.clientY - drag.originY) < DRAG_THRESHOLD) {
        return;
      }

      drag.dragging = true;
      drag.clientX = event.clientX;
      drag.clientY = event.clientY;
      setDragState({ item: drag.item, clientX: event.clientX, clientY: event.clientY });
    };
    const handlePointerUp = (event: PointerEvent) => {
      const drag = activeDragRef.current;
      if (!drag || drag.pointerId !== event.pointerId) {
        return;
      }

      // A press that never crossed the threshold is not a drag and inserts nothing.
      if (drag.dragging && canPlaceResourceAt({ x: event.clientX, y: event.clientY })) {
        onDrop(drag.item, event.clientX, event.clientY);
      }
      cancelDrag();
    };
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape" && activeDragRef.current) {
        // Escape cancels the drag only; it must not also dismiss the surrounding palette.
        event.preventDefault();
        event.stopPropagation();
        cancelDrag();
      }
    };

    window.addEventListener("pointermove", handlePointerMove);
    window.addEventListener("pointerup", handlePointerUp);
    window.addEventListener("pointercancel", cancelDrag);
    window.addEventListener("keydown", handleKeyDown, true);
    return () => {
      window.removeEventListener("pointermove", handlePointerMove);
      window.removeEventListener("pointerup", handlePointerUp);
      window.removeEventListener("pointercancel", cancelDrag);
      window.removeEventListener("keydown", handleKeyDown, true);
    };
  }, [canPlaceResourceAt, cancelDrag, onDrop, setDragState]);

  const startDrag = useCallback((item: PaletteDragState["item"], event: ReactPointerEvent<HTMLElement>) => {
    if (event.button !== 0) {
      return;
    }

    event.preventDefault();
    event.currentTarget.setPointerCapture(event.pointerId);
    activeDragRef.current = {
      item,
      pointerId: event.pointerId,
      originX: event.clientX,
      originY: event.clientY,
      clientX: event.clientX,
      clientY: event.clientY,
      dragging: false,
    };
  }, []);

  return { startDrag };
}
