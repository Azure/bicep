// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";

import { atom } from "jotai";
import { documentUriAtom } from "@/hooks";
import { activeThemeAtom, getThemeByName } from "@/ui/theme";

export type ExportBackgroundMode = "transparent" | "solid";

export const DEFAULT_EXPORT_FILE_STEM = "bicep-graph";
export const DEFAULT_EXPORT_PADDING = 40;

export const isExportOverlayOpenAtom = atom(false);
export const exportPaddingAtom = atom(DEFAULT_EXPORT_PADDING);
export const exportBackgroundModeAtom = atom<ExportBackgroundMode>("transparent");
export const exportThemeOverrideAtom = atom<DefaultTheme["name"] | null>(null);
export const isExportInProgressAtom = atom(false);
export const exportCanvasElementAtom = atom<HTMLElement | null>(null);

/**
 * The exported file is named after the document it was captured from: "main.bicep" -> "main".
 *
 * Derived from `documentUriAtom` rather than written by whoever handles the change notification, so
 * the name cannot drift from the document.
 */
export const exportFileStemAtom = atom((get) => {
  const fileName = (get(documentUriAtom) ?? "").split(/[\\/]/).pop() ?? "";
  const stem = fileName.replace(/\.[^.]+$/, "").trim();

  return stem || DEFAULT_EXPORT_FILE_STEM;
});

export const effectiveExportThemeAtom = atom((get) => {
  const override = get(exportThemeOverrideAtom);

  return override ? getThemeByName(override) : get(activeThemeAtom);
});

export const exportBackgroundColorAtom = atom((get) => get(effectiveExportThemeAtom).viewport.background);

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
