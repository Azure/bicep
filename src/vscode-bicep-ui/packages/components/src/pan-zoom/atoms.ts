// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Getter } from "jotai/vanilla";

import { atom } from "jotai";
import { createIsolation } from "jotai-scope";

const isolation = createIsolation();

// Workaround for "error TS4023: Exported variable 'Provider' has or is using name 'PrdStore' from external module "/bicep/src/vscode-bicep-ui/node_modules/jotai/esm/vanilla/store" but cannot be named".
export const Provider = isolation.Provider as ReturnType<typeof createIsolation>["Provider"];

export const { useAtom, useAtomValue, useSetAtom, useStore } = isolation;

export const panZoomTransformAtom = atom({ x: 0, y: 0, scale: 1 });

export const panZoomDimensionsAtom = atom({ height: 0, width: 0 });

type PanZoomController = {
  zoomIn: (scaleFactor?: number) => void;
  zoomOut: (scaleFactor?: number) => void;
  reset: () => void;
  transform: (x: number, y: number, scale: number) => void;
};

export const panZoomControlAtom = atom<PanZoomController | null>(null);

function getPanZoomController(get: Getter): PanZoomController {
  const controller = get(panZoomControlAtom);
  if (controller === null) {
    throw new Error("The pan-zoom controller is not mounted.");
  }
  return controller;
}

export const zoomInAtom = atom(null, (get, _set, scaleFactor?: number) =>
  getPanZoomController(get).zoomIn(scaleFactor),
);
export const zoomOutAtom = atom(null, (get, _set, scaleFactor?: number) =>
  getPanZoomController(get).zoomOut(scaleFactor),
);
export const resetPanZoomAtom = atom(null, (get) => getPanZoomController(get).reset());
export const transformPanZoomAtom = atom(null, (get, _set, x: number, y: number, scale: number) =>
  getPanZoomController(get).transform(x, y, scale),
);
