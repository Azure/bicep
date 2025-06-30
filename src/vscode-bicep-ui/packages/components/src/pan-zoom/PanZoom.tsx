import type { D3ZoomEvent } from "d3-zoom";
import type { PropsWithChildren } from "react";

import { select } from "d3-selection";
import { zoom, zoomIdentity } from "d3-zoom";
import { frame } from "motion/react";
import { RESET } from "jotai/utils";
import { useEffect, useRef } from "react";
import { panZoomControlAtom, panZoomTransformAtom, useSetAtom } from "./atoms.ts";

import "d3-transition";

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
  scaleFactor?: number;

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
  scaleFactor = 1.4,
  transition = { duration: 400 },
  className,
  children,
}: PanZoomProps): JSX.Element {
  const ref = useRef<HTMLDivElement>(null);
  const setPanZoomTransform = useSetAtom(panZoomTransformAtom);
  const setPanZoomControl = useSetAtom(panZoomControlAtom);

  useEffect(() => {
    if (!ref.current) {
      return;
    }

    const panZoomBehavior = zoom<HTMLDivElement, unknown>()
      .scaleExtent([minimumScale, maximumScale])
      .on("zoom", ({ transform }: D3ZoomEvent<HTMLDivElement, unknown>) => {
        frame.render(() => {
          const { x, y, k: scale } = transform;
          setPanZoomTransform({ x, y, scale });
        });
      });

    const selection = select(ref.current).call(panZoomBehavior);

    setPanZoomControl({
      reset: () => selection.transition().duration(transition.duration).call(panZoomBehavior.transform, zoomIdentity),
      zoomIn: () => selection.transition().duration(transition.duration).call(panZoomBehavior.scaleBy, scaleFactor),
      zoomOut: () => selection.transition().duration(transition.duration).call(panZoomBehavior.scaleBy, 1 / scaleFactor),
    });

    return () => {
      panZoomBehavior.on("zoom", null);
      setPanZoomControl(RESET);
    };
  }, [maximumScale, minimumScale, scaleFactor, setPanZoomControl, setPanZoomTransform, transition.duration]);

  return (
    <div className={className} ref={ref} data-testid="pan-zoom">
      {children}
    </div>
  );
}
