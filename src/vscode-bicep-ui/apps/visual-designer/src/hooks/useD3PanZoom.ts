import type { D3ZoomEvent } from "d3-zoom";

import { select } from "d3-selection";
import { zoom } from "d3-zoom";
import { useEffect, useRef } from "react";

export type PanZoomEventListener = (event: D3ZoomEvent<HTMLDivElement, unknown>) => void;

const handleWheelDelta = (event: WheelEvent) => (-event.deltaY * (event.deltaMode ? 120 : 1)) / 1000;

export function useD3PanZoom(listener: PanZoomEventListener) {
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!ref.current) {
      return;
    }

    const selection = select(ref.current as Element);
    const { width, height } = ref.current.getBoundingClientRect();
    const zoomBehavior = zoom()
      .scaleExtent([1 / 4, 4])
      .extent([
        [0, 0],
        [width, height],
      ])
      .wheelDelta(handleWheelDelta)
      .on("zoom", listener);

    selection.call(zoomBehavior);

    return () => {
      zoomBehavior.on("zoom", null);
    };
  }, [listener]);

  return ref;
}
