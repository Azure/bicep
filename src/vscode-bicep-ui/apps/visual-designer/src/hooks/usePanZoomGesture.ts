import { useSetAtom } from "jotai";
import { useEffect, useRef } from "react";
import { clearPanZoomGesturesAtom, setPanZoomGesturesAtom } from "../atoms/panZoom";

export function usePanZoomGesture() {
  const ref = useRef<Element>(null);
  const setPanZoomGestures = useSetAtom(setPanZoomGesturesAtom);
  const clearPanZoomGestures = useSetAtom(clearPanZoomGesturesAtom);

  useEffect(() => {
    if (ref.current) {
      setPanZoomGestures(ref.current);
    }

    return () => {
      clearPanZoomGestures();
    };
  }, [clearPanZoomGestures, setPanZoomGestures]);

  return ref;
}
