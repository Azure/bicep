// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { D3ZoomEvent } from "d3-zoom";
import type { PropsWithChildren } from "react";

import { select } from "d3-selection";
import { zoom, zoomIdentity } from "d3-zoom";
import { frame } from "motion/react";
import { RESET } from "jotai/utils";
import { useEffect, useLayoutEffect, useRef } from "react";
import { panZoomControlAtom, panZoomDimensionsAtom, panZoomTransformAtom, useSetAtom } from "./atoms";

import "d3-transition";
import useResizeObserver from "@react-hook/resize-observer";

/**
 * Props for the PanZoom component.
 */
type PanZoomProps = PropsWithChildren<{
  /**
   * The minimum scale value.
   */
  minimumScale?: number;

  /**
   * The maximum scale value.
   */
  maximumScale?: number;

  /**
   * The factor by which to zoom in or out when triggered programmatically.
   */
  defaultScaleFactor?: number;

  /**
   * The zoom transition settings to use when triggered programmatically.
   */
  transition?: {
    /**
     * The duration of the transition.
     */
    duration: number;
  };

  /**
   * The class name for styling the component.
   */
  className?: string;
}>;

/**
 * Component for panning and zooming gestures.
 *
 * @component
 * @param {PanZoomProps} props - The component props.
 * @param {number} [props.minimumScale=1/4] - The minimum scale value.
 * @param {number} [props.maximumScale=4] - The maximum scale value.
 * @param {number} [props.scaleFactor=1.4] - The factor by which to zoom in or out when triggered programmatically.
 * @param {Object} [props.transition={ duration: 400 }] - The zoom transition settings to use when triggered programmatically.
 * @param {number} [props.transition.duration=400] - The duration of the transition.
 * @param {string} [props.className] - The CSS class name for the component.
 * @param {ReactNode} props.children - The child components.
 * @returns {JSX.Element} The rendered component.
 */
export function PanZoom({
  minimumScale = 1 / 4,
  maximumScale = 4,
  defaultScaleFactor = 1.15,
  transition = { duration: 250 },
  className,
  children,
}: PanZoomProps): JSX.Element {
  const ref = useRef<HTMLDivElement>(null);
  const setPanZoomTransform = useSetAtom(panZoomTransformAtom);
  const setPanZoomDimensions = useSetAtom(panZoomDimensionsAtom);
  const setPanZoomControl = useSetAtom(panZoomControlAtom);

  useLayoutEffect(() => {
    if (!ref.current) {
      return;
    }

    const { width, height } = ref.current.getBoundingClientRect();
    console.log("PanZoom dimensions:", { width, height });
    setPanZoomDimensions({ width, height });
  });

  useResizeObserver(ref, (entry) => {
    const { width, height } = entry.contentRect;
    console.log("PanZoom dimensions:", { width, height });
    setPanZoomDimensions({ width, height });
  });

  useEffect(() => {
    if (!ref.current) {
      return;
    }

    const panZoomBehavior = zoom<HTMLDivElement, unknown>()
      .scaleExtent([minimumScale, maximumScale])
      .wheelDelta((event) => -event.deltaY * (event.deltaMode === 1 ? 0.05 : 0.0015))
      .on("zoom", ({ transform }: D3ZoomEvent<HTMLDivElement, unknown>) => {
        frame.render(() => {
          const { x, y, k: scale } = transform;
          setPanZoomTransform({ x, y, scale });
        });
      });

    const selection = select(ref.current).call(panZoomBehavior);

    setPanZoomControl({
      reset: () => selection.transition().duration(transition.duration).call(panZoomBehavior.transform, zoomIdentity),
      zoomIn: (scaleFactor?: number) => selection.transition().duration(transition.duration).call(panZoomBehavior.scaleBy, scaleFactor ?? defaultScaleFactor),
      zoomOut: (scaleFactor?: number) => selection.transition().duration(transition.duration).call(panZoomBehavior.scaleBy, 1 / (scaleFactor ?? defaultScaleFactor)),
      transform: (x: number, y: number, scale: number) => {
        const clammpedScale = Math.min(Math.max(scale, minimumScale), maximumScale);
        const transform = zoomIdentity.translate(x, y).scale(clammpedScale);
        selection.transition().duration(transition.duration).call(panZoomBehavior.transform, transform);
      },
    });

    return () => {
      panZoomBehavior.on("zoom", null);
      setPanZoomControl(RESET);
    };
  }, [maximumScale, minimumScale, defaultScaleFactor, setPanZoomControl, setPanZoomTransform, transition.duration]);

  return (
    <div className={className} ref={ref} data-testid="pan-zoom">
      {children}
    </div>
  );
}
