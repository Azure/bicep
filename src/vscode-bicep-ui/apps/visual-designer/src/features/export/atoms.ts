// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";
import type { ExportBackgroundMode } from "./types";

import { atom } from "jotai";
import { activeThemeAtom, getThemeByName } from "@/lib/theming";

export const DEFAULT_EXPORT_FILE_STEM = "bicep-graph";
export const DEFAULT_EXPORT_PADDING = 40;

export const isExportOverlayOpenAtom = atom(false);
export const exportPaddingAtom = atom(DEFAULT_EXPORT_PADDING);
export const exportBackgroundModeAtom = atom<ExportBackgroundMode>("transparent");
export const exportThemeOverrideAtom = atom<DefaultTheme["name"] | null>(null);
export const exportFileStemAtom = atom(DEFAULT_EXPORT_FILE_STEM);
export const isExportInProgressAtom = atom(false);
export const exportCanvasElementAtom = atom<HTMLElement | null>(null);

export const effectiveExportThemeAtom = atom((get) => {
  const override = get(exportThemeOverrideAtom);

  return override ? getThemeByName(override) : get(activeThemeAtom);
});

export const exportBackgroundColorAtom = atom((get) => get(effectiveExportThemeAtom).canvas.background);

export const isExportPreviewVisibleAtom = atom((get) => get(isExportOverlayOpenAtom));

export const isExportCanvasCoverVisibleAtom = atom(
  (get) => get(isExportOverlayOpenAtom) && get(exportBackgroundModeAtom) === "solid",
);

export const openExportOverlayAtom = atom(null, (_, set) => {
  set(isExportOverlayOpenAtom, true);
});

export const closeExportOverlayAtom = atom(null, (_, set) => {
  set(isExportOverlayOpenAtom, false);
  set(exportThemeOverrideAtom, null);
});
