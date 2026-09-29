// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { D3DragEvent, SubjectPosition } from "d3-drag";
import type { RefObject } from "react";

import { useGetPanZoomTransform } from "@vscode-bicep-ui/components";
import { drag } from "d3-drag";
import { select } from "d3-selection";
import { useEffect, useEffectEvent } from "react";

export interface DragListeners {
  /** Called once, on the first movement. A press that is released without moving is a click. */
  onDragStart: () => void;
  /** Called on every movement with the distance moved in graph coordinates. */
  onDrag: (dx: number, dy: number) => void;
  /** Called when a drag that started moving is released. */
  onDragEnd: () => void;
}

export function useDragListener(ref: RefObject<HTMLDivElement | null>, listeners: DragListeners) {
  const getPanZoomTransform = useGetPanZoomTransform();
  const onDragStart = useEffectEvent(listeners.onDragStart);
  const onDrag = useEffectEvent(listeners.onDrag);
  const onDragEnd = useEffectEvent(listeners.onDragEnd);

  useEffect(() => {
    if (!ref.current) {
      return;
    }

    const selection = select(ref.current);
    let hasMoved = false;
    const dragBehavior = drag<HTMLDivElement, unknown>()
      .on("start", () => {
        hasMoved = false;
      })
      .on("drag", ({ dx, dy }: D3DragEvent<HTMLDivElement, unknown, SubjectPosition>) => {
        if (dx === 0 && dy === 0) {
          return;
        }

        if (!hasMoved) {
          hasMoved = true;
          onDragStart();
        }

        const { scale } = getPanZoomTransform();

        onDrag(dx / scale, dy / scale);
      })
      .on("end", () => {
        if (hasMoved) {
          hasMoved = false;
          onDragEnd();
        }
      });

    selection.call(dragBehavior);

    return () => {
      selection.on(".drag", null);
    };
  }, [ref, getPanZoomTransform]);
}
