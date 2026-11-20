// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue, useSetAtom } from "jotai";
import { useCallback } from "react";
import { styled, ThemeProvider, useTheme } from "styled-components";
import {
  canvasElementAtom,
  isGraphChangeInProgressAtom,
  raisedNodeIdsAtom,
  useGraphActions,
  useGraphApi,
  useIsMotionReduced,
} from "@/core";
import { effectiveExportThemeAtom, ExportAreaCover, ExportPreviewLayer } from "@/features/export";
import { Graph, Viewport } from "@/lib/graph";
import { getCursors } from "@/ui";
import { isHandModeAtom } from "../atoms";
import { useNavigationShortcuts } from "../hooks/use-navigation-shortcuts";
import { useSelectionGestures } from "../hooks/use-selection-gestures";
import { NodeContextMenu } from "./NodeContextMenu";
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

  /* Node drags and module toggles are ignored while a source edit or Reset Layout is in flight; say so. */
  &[aria-busy="true"],
  &[aria-busy="true"] [data-node-id],
  &[aria-busy="true"] [data-node-id] button {
    cursor: progress;
  }
`;

/** The surface that renders the graph. */
export function Canvas() {
  const { handleNodeDragStart, handleNodeDrag, handleNodeDragEnd } = useGraphActions();
  const api = useGraphApi();
  const selectionGestures = useSelectionGestures();
  const cursors = getCursors(useTheme());
  const handMode = useAtomValue(isHandModeAtom);
  useNavigationShortcuts();
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
            <Viewport
              mode={handMode ? "hand" : "select"}
              cursor={handMode ? cursors.hand : cursors.arrow}
              grabbingCursor={cursors.grabbing}
              {...selectionGestures}
            >
              <PendingResourceLayer />
              <Graph
                raisedNodeIdsAtom={raisedNodeIdsAtom}
                reduceMotion={reduceMotion}
                panOnNodeDrag={handMode}
                marqueeOnModifiedDrag={!handMode}
                onNodeDragStart={handleNodeDragStart}
                onNodeDrag={handleNodeDrag}
                onNodeDragEnd={handleNodeDragEnd}
                onNodeDoubleClick={revealNode}
              >
                <ExportAreaCover />
              </Graph>
            </Viewport>
            <NodeContextMenu />
          </$CanvasWrapper>
        </ThemeProvider>
      </NodeContentProvider>
      <ExportPreviewLayer />
      <ScopeIndicator />
    </>
  );
}
