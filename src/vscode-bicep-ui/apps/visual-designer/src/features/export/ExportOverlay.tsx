// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ExportFormat } from "./types";

import type { DefaultTheme } from "styled-components";

import { useStore } from "jotai";
import { useCallback, useLayoutEffect, useState } from "react";
import { styled, useTheme } from "styled-components";
import { getThemeByName } from "../../theming/themes";
import { ExportToolbar } from "./ExportToolbar";
import { captureGraphElement, saveDataUrl } from "./capture-element";

const DEFAULT_PADDING = 40;

const $OverlayContainer = styled.div`
  position: absolute;
  top: 16px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 200;
`;

export interface ExportOverlayProps {
  /** The DOM element wrapping the graph canvas (used for capture). */
  canvasElement: HTMLElement | null;
  /** Preferred filename stem for exported graph files (without extension). */
  exportFileStem?: string;
  onClose: () => void;
  /** Called when the user changes the padding value. */
  onPaddingChange?: (padding: number) => void;
  /** Called when the user changes the export format. */
  onFormatChange?: (format: ExportFormat) => void;
  /** Called when the user changes the export theme. null = current VS Code theme. */
  onThemeChange?: (themeName: DefaultTheme["name"] | null) => void;
}

/**
 * Floating export toolbar that appears over the existing canvas.
 * No modal, no isolated store — the user works on the live graph
 * and exports it directly.
 */
export function ExportOverlay({
  canvasElement,
  exportFileStem,
  onClose,
  onPaddingChange: notifyPaddingChange,
  onFormatChange: notifyFormatChange,
  onThemeChange: notifyThemeChange,
}: ExportOverlayProps) {
  const theme = useTheme();
  const store = useStore();
  const [exporting, setExporting] = useState(false);
  const [padding, setPaddingInternal] = useState(DEFAULT_PADDING);
  const [exportThemeName, setExportThemeName] = useState<DefaultTheme["name"] | null>(null);

  const setTheme = useCallback(
    (name: DefaultTheme["name"] | null) => {
      setExportThemeName(name);
      notifyThemeChange?.(name);
    },
    [notifyThemeChange],
  );

  const setPadding = useCallback(
    (value: number) => {
      setPaddingInternal(value);
      notifyPaddingChange?.(value);
    },
    [notifyPaddingChange],
  );


  const handleExport = useCallback(
    async (format: ExportFormat) => {
      if (!canvasElement || exporting) return;
      setExporting(true);
      try {
        const exportBg = exportThemeName
          ? getThemeByName(exportThemeName).canvas.background
          : theme.canvas.background;
        const dataUrl = await captureGraphElement(
          canvasElement,
          store,
          format,
          padding,
          exportBg,
        );
        const fileStem = (exportFileStem ?? "bicep-graph").trim() || "bicep-graph";
        await saveDataUrl(dataUrl, `${fileStem}.${format}`, format);
      } catch (error) {
        console.error("Export failed:", error);
      } finally {
        setExporting(false);
      }
    },
    [canvasElement, exporting, store, padding, theme.canvas.background, exportThemeName, exportFileStem],
  );

  // Close on Escape key.
  useLayoutEffect(() => {
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, [onClose]);

  return (
    <$OverlayContainer>
      <ExportToolbar
        onExport={handleExport}
        onClose={onClose}
        padding={padding}
        onPaddingChange={setPadding}
        exporting={exporting}
        onFormatChange={notifyFormatChange}
        onThemeChange={setTheme}
      />
    </$OverlayContainer>
  );
}
