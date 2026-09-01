// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PointerEvent as ReactPointerEvent } from "react";
import type { PaletteDragState } from "./atoms";

import { useSetAtom } from "jotai";
import { useCallback, useEffect, useRef } from "react";
import { paletteDragAtom } from "./atoms";

interface PendingDrag extends PaletteDragState {
  pointerId: number;
}

export function usePaletteDrag(
  getCanvasElement: () => HTMLElement | null,
  onDrop: (item: PaletteDragState["item"], clientX: number, clientY: number) => void,
) {
  const activeDragRef = useRef<PendingDrag | null>(null);
  const setDragState = useSetAtom(paletteDragAtom);

  const cancelDrag = useCallback(() => {
    activeDragRef.current = null;
    setDragState(null);
  }, [setDragState]);

  useEffect(() => {
    const handlePointerMove = (event: PointerEvent) => {
      const drag = activeDragRef.current;
      if (!drag || drag.pointerId !== event.pointerId) {
        return;
      }

      drag.clientX = event.clientX;
      drag.clientY = event.clientY;
      setDragState({ item: drag.item, clientX: event.clientX, clientY: event.clientY });
    };
    const handlePointerUp = (event: PointerEvent) => {
      const drag = activeDragRef.current;
      if (!drag || drag.pointerId !== event.pointerId) {
        return;
      }

      const canvas = getCanvasElement();
      const bounds = canvas?.getBoundingClientRect();
      const elementAtPointer = document.elementFromPoint(event.clientX, event.clientY);
      if (
        canvas &&
        bounds &&
        elementAtPointer &&
        canvas.contains(elementAtPointer) &&
        event.clientX >= bounds.left &&
        event.clientX <= bounds.right &&
        event.clientY >= bounds.top &&
        event.clientY <= bounds.bottom
      ) {
        onDrop(drag.item, event.clientX, event.clientY);
      }
      cancelDrag();
    };
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        cancelDrag();
      }
    };

    window.addEventListener("pointermove", handlePointerMove);
    window.addEventListener("pointerup", handlePointerUp);
    window.addEventListener("pointercancel", cancelDrag);
    window.addEventListener("keydown", handleKeyDown);
    return () => {
      window.removeEventListener("pointermove", handlePointerMove);
      window.removeEventListener("pointerup", handlePointerUp);
      window.removeEventListener("pointercancel", cancelDrag);
      window.removeEventListener("keydown", handleKeyDown);
    };
  }, [cancelDrag, getCanvasElement, onDrop, setDragState]);

  const startDrag = useCallback(
    (item: PaletteDragState["item"], event: ReactPointerEvent<HTMLElement>) => {
      if (event.button !== 0) {
        return;
      }

      event.preventDefault();
      event.currentTarget.setPointerCapture(event.pointerId);
      const drag = {
        item,
        pointerId: event.pointerId,
        clientX: event.clientX,
        clientY: event.clientY,
      };
      activeDragRef.current = drag;
      setDragState({ item, clientX: event.clientX, clientY: event.clientY });
    },
    [setDragState],
  );

  return { startDrag };
}
