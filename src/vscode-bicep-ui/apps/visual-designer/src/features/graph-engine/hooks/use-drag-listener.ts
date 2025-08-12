// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { D3DragEvent, SubjectPosition } from "d3-drag";
import type { RefObject } from "react";

import { useGetPanZoomTransform } from "@vscode-bicep-ui/components";
import { drag } from "d3-drag";
import { select } from "d3-selection";
import { useEffect } from "react";

export function useDragListener(ref: RefObject<HTMLDivElement>, onDrag: (dx: number, dy: number) => void) {
  const getPanZoomTransform = useGetPanZoomTransform();

  useEffect(() => {
    if (!ref.current) {
      return;
    }

    const selection = select(ref.current);
    const dragBehavior = drag<HTMLDivElement, unknown>().on(
      "drag",
      ({ dx, dy }: D3DragEvent<HTMLDivElement, unknown, SubjectPosition>) => {
        const { scale } = getPanZoomTransform();

        onDrag(dx / scale, dy / scale);
      },
    );

    selection.call(dragBehavior);

    return () => {
      selection.on("drag", null);
    };
  }, [ref, onDrag, getPanZoomTransform]);
}
