// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue, useSetAtom } from "jotai";
import { useCallback } from "react";
import { styled, ThemeProvider } from "styled-components";
import { isGraphChangeInProgressAtom, useGraphActions } from "@/core";
import {
  effectiveExportThemeAtom,
  ExportAreaCover,
  exportCanvasElementAtom,
  ExportPreviewLayer,
} from "@/features/export";
import { Graph, Viewport } from "@/lib/graph";
import { canvasElementAtom } from "../atoms";
import { NodeContentProvider } from "./nodes/NodeContentProvider";
import { PendingResourceLayer } from "./PendingResourceLayer";
import { ScopeIndicator } from "./ScopeIndicator";

const $CanvasWrapper = styled.div`
  position: absolute;
  inset: 0;

  &:focus-visible {
    outline: 2px solid ${({ theme }) => theme.focusBorder};
    outline-offset: -2px;
  }
`;

const $InteractionShield = styled.div`
  position: absolute;
  inset: 0;
  z-index: 80;
  cursor: progress;
`;

/** The surface that renders the graph. */
export function Canvas() {
  const { handleNodeDragStart, handleNodeDragEnd } = useGraphActions();
  const isGraphChangeInProgress = useAtomValue(isGraphChangeInProgressAtom);
  const exportTheme = useAtomValue(effectiveExportThemeAtom);
  const setCanvasElement = useSetAtom(canvasElementAtom);
  const setExportCanvasElement = useSetAtom(exportCanvasElementAtom);

  const handleCanvasRef = useCallback(
    (element: HTMLDivElement | null) => {
      setCanvasElement(element);
      // Export captures this element, but cannot import it from canvas without a dependency cycle.
      setExportCanvasElement(element);
    },
    [setCanvasElement, setExportCanvasElement],
  );

  return (
    <>
      <NodeContentProvider>
        <ThemeProvider theme={exportTheme}>
          <$CanvasWrapper
            ref={handleCanvasRef}
            role="region"
            aria-label="Visual designer canvas"
            aria-busy={isGraphChangeInProgress}
            tabIndex={0}
          >
            <Viewport>
              <PendingResourceLayer />
              <Graph onNodeDragStart={handleNodeDragStart} onNodeDragEnd={handleNodeDragEnd}>
                <ExportAreaCover />
              </Graph>
            </Viewport>
            {/* Blocks node gestures while a source edit or layout reset is in flight. */}
            {isGraphChangeInProgress && <$InteractionShield />}
          </$CanvasWrapper>
        </ThemeProvider>
      </NodeContentProvider>
      <ExportPreviewLayer />
      <ScopeIndicator />
    </>
  );
}
