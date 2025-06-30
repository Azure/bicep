// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";
import { createIsolation } from "jotai-scope";
import { atomWithReset } from "jotai/utils";

const isolation = createIsolation();

// Workaround for "error TS4023: Exported variable 'Provider' has or is using name 'PrdStore' from external module "/bicep/src/vscode-bicep-ui/node_modules/jotai/esm/vanilla/store" but cannot be named".
export const Provider = isolation.Provider as ReturnType<typeof createIsolation>["Provider"];

export const { useAtom, useAtomValue, useSetAtom, useStore } = isolation;

export const panZoomTransformAtom = atom({ x: 0, y: 0, scale: 1 });

export const panZoomControlAtom = atomWithReset({
  zoomIn: () => {},
  zoomOut: () => {},
  reset: () => {},
});
