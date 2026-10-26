// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { RefObject } from "react";

import { useContext, useEffect, useEffectEvent } from "react";
import { GraphContext } from "../context/GraphContext";
import { isGraphInteractiveTarget } from "../interactive";
import { useDragListener } from "./use-drag-listener";

const INTERACTIVE_PRESS_EVENTS = ["pointerdown", "mousedown", "touchstart"] as const;

/**
 * Reports a node's drags and double-clicks through the graph's events.
 *
 * A press on interactive content inside the node stops at the node, so the canvas neither focuses the
 * node nor pans. Double-clicks never reach the canvas, which would otherwise zoom.
 */
export function useNodeGestures(ref: RefObject<HTMLDivElement | null>, id: string) {
  const { events } = useContext(GraphContext);
  const onDoubleClick = useEffectEvent(() => events.onNodeDoubleClick(id));

  useDragListener(ref, {
    onDragStart: () => events.onNodeDragStart(id),
    onDrag: (dx, dy) => events.onNodeDrag(id, dx, dy),
    onDragEnd: () => events.onNodeDragEnd(id),
  });

  useEffect(() => {
    const element = ref.current;
    if (!element) {
      return;
    }

    const stopInteractivePress = (event: Event) => {
      if (isGraphInteractiveTarget(event.target)) {
        event.stopPropagation();
      }
    };
    const handleDoubleClick = (event: MouseEvent) => {
      event.stopPropagation();
      if (!isGraphInteractiveTarget(event.target)) {
        onDoubleClick();
      }
    };

    for (const type of INTERACTIVE_PRESS_EVENTS) {
      element.addEventListener(type, stopInteractivePress);
    }
    element.addEventListener("dblclick", handleDoubleClick);

    return () => {
      for (const type of INTERACTIVE_PRESS_EVENTS) {
        element.removeEventListener(type, stopInteractivePress);
      }
      element.removeEventListener("dblclick", handleDoubleClick);
    };
  }, [ref]);
}
