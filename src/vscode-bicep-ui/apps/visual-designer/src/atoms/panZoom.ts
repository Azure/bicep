import type { D3ZoomEvent } from "d3-zoom";
import type { Selection } from "d3-selection";

import { select } from "d3-selection";
import { zoom, zoomIdentity } from "d3-zoom";
import { frame } from "framer-motion";
import { atom } from "jotai";

const panZoomBehaviorAtom = atom(zoom());

const panZoomSelectionAtom = atom<Selection<Element, unknown, null, undefined> | undefined>(undefined);

const panZoomTansformAtom = atom({ x: 0, y: 0, k: 1 });

export const setPanZoomGesturesAtom = atom(null, (get, set, element: Element) => {
  const panZoomBehavior = get(panZoomBehaviorAtom)
    .scaleExtent([1 / 4, 4])
    .on("zoom", (event: D3ZoomEvent<Element, unknown>) => {
      frame.update(() => set(panZoomTansformAtom, event.transform));
    });

  set(panZoomSelectionAtom, select(element).call(panZoomBehavior));
});

export const clearPanZoomGesturesAtom = atom(null, (get) => {
  get(panZoomBehaviorAtom).on("zoom", null);
});

export const getPanZoomTransformAtom = atom((get) => get(panZoomTansformAtom));

export const resetPanZoomTransformAtom = atom(null, (get) => {
  const selection = get(panZoomSelectionAtom);

  if (selection) {
    get(panZoomBehaviorAtom).transform(selection, zoomIdentity.translate(0, 0).scale(1));
  }
});
