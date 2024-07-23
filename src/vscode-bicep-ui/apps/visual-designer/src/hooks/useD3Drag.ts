import type { D3DragEvent, SubjectPosition } from "d3-drag";

import { drag } from "d3-drag";
import { select } from "d3-selection";
import { useEffect, useRef } from "react";

export default function useD3Drag<T extends Element = HTMLDivElement>(
  onDrag: (event: D3DragEvent<T, unknown, SubjectPosition>) => void,
) {
  const elementRef = useRef<T>(null);

  useEffect(() => {
    if (!elementRef.current) {
      return;
    }

    const selection = select(elementRef.current);
    const dragBehavior = drag<T, unknown>().on("drag", onDrag);

    selection.call(dragBehavior);

    return () => {
      selection.on("drag", null);
    };
  }, [onDrag]);

  return elementRef;
}
