// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ExportFormat } from "./types";

import { atom, useAtomValue, useStore } from "jotai";
import { useCallback, useLayoutEffect, useState } from "react";
import { styled, useTheme } from "styled-components";
import { nodesAtom } from "../graph-engine/atoms";
import { ExportToolbar } from "./ExportToolbar";
import { captureGraphElement, saveDataUrl } from "./captureElement";

const DEFAULT_PADDING = 40;

const $OverlayContainer = styled.div`
  position: absolute;
  top: 16px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 200;
`;

/** Derived atom that reactively computes graph bounds from all node boxAtoms. */
const graphBoundsAtom = atom((get) => {
  const nodes = get(nodesAtom);
  const nodeList = Object.values(nodes);
  if (nodeList.length === 0) return null;

  const boxes = nodeList.map((node) => get(node.boxAtom));
  const minX = Math.min(...boxes.map((b) => b.min.x));
  const minY = Math.min(...boxes.map((b) => b.min.y));
  const maxX = Math.max(...boxes.map((b) => b.max.x));
  const maxY = Math.max(...boxes.map((b) => b.max.y));

  return { minX, minY, width: maxX - minX, height: maxY - minY };
});

export interface ExportOverlayProps {
  /** The DOM element wrapping the graph canvas (used for capture). */
  canvasElement: HTMLElement | null;
  onClose: () => void;
}

/**
 * Floating export toolbar that appears over the existing canvas.
 * No modal, no isolated store — the user works on the live graph
 * and exports it directly.
 */
export function ExportOverlay({ canvasElement, onClose }: ExportOverlayProps) {
  const theme = useTheme();
  const store = useStore();
  const [exporting, setExporting] = useState(false);
  const [padding, setPadding] = useState(DEFAULT_PADDING);

  // Reactively subscribe to graph bounds — updates as nodes are dragged.
  const graphBounds = useAtomValue(graphBoundsAtom);

  // Derived export dimensions.
  const canvasWidth = graphBounds ? Math.round(graphBounds.width + padding * 2) : 0;
  const canvasHeight = graphBounds ? Math.round(graphBounds.height + padding * 2) : 0;

  const handleExport = useCallback(
    async (format: ExportFormat) => {
      if (!canvasElement || exporting) return;
      setExporting(true);
      try {
        const dataUrl = await captureGraphElement(
          canvasElement,
          store,
          format,
          padding,
          theme.canvas.background,
        );
        await saveDataUrl(dataUrl, `bicep-graph.${format}`, format);
      } catch (error) {
        console.error("Export failed:", error);
      } finally {
        setExporting(false);
      }
    },
    [canvasElement, exporting, store, padding, theme.canvas.background],
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
        canvasWidth={canvasWidth}
        canvasHeight={canvasHeight}
        exporting={exporting}
      />
    </$OverlayContainer>
  );
}
