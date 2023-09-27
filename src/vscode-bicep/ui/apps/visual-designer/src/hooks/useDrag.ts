import { drag } from "d3-drag";
import { select } from "d3-selection";
import { useEffect, useRef } from "react";

import { useStore } from "../stores";

import type { D3DragEvent, SubjectPosition } from "d3-drag";

export default function useDrag(nodeId: string) {
  const elementRef = useRef<HTMLDivElement>(null);
  const translateNode = useStore(x => x.translateNode);

  useEffect(() => {
    if (elementRef.current) {
      const selection = select(elementRef.current as Element);
      const dragBehavior = drag().on("drag", (event: D3DragEvent<Element, null, SubjectPosition>) => {
        translateNode(nodeId, event.dx, event.dy);
      });

      selection.call(dragBehavior);

      return () => {
        selection.on("drag", null);
      };
    }
  }, [nodeId, translateNode]);

  return elementRef;
}
