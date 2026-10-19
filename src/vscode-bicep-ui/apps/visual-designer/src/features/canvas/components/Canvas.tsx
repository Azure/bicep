// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue, useSetAtom } from "jotai";
import { useCallback } from "react";
import { styled, ThemeProvider } from "styled-components";
import {
  canvasElementAtom,
  focusNodeAtom,
  isGraphChangeInProgressAtom,
  raisedNodeIdAtom,
  useGraphActions,
  useGraphApi,
  useIsMotionReduced,
} from "@/core";
import { effectiveExportThemeAtom, ExportAreaCover, ExportPreviewLayer } from "@/features/export";
import { Graph, Viewport } from "@/lib/graph";
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
  const { handleNodeDragStart, handleNodeDrag, handleNodeDragEnd } = useGraphActions();
  const api = useGraphApi();
  const focusNode = useSetAtom(focusNodeAtom);
  const revealNode = useCallback((nodeId: string) => api.revealNode(nodeId), [api]);
  const reduceMotion = useIsMotionReduced();
  const isGraphChangeInProgress = useAtomValue(isGraphChangeInProgressAtom);
  const exportTheme = useAtomValue(effectiveExportThemeAtom);
  const setCanvasElement = useSetAtom(canvasElementAtom);

  return (
    <>
      <NodeContentProvider>
        <ThemeProvider theme={exportTheme}>
          <$CanvasWrapper
            ref={setCanvasElement}
            role="region"
            aria-label="Visual designer canvas"
            aria-busy={isGraphChangeInProgress}
            tabIndex={0}
          >
            <Viewport onNodeClick={focusNode}>
              <PendingResourceLayer />
              <Graph
                raisedNodeIdAtom={raisedNodeIdAtom}
                reduceMotion={reduceMotion}
                onNodeDragStart={handleNodeDragStart}
                onNodeDrag={handleNodeDrag}
                onNodeDragEnd={handleNodeDragEnd}
                onNodeDoubleClick={revealNode}
              >
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
