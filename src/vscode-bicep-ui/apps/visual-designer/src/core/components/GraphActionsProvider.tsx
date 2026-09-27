// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { GraphActions } from "../context/GraphActionsContext";

import { useGetPanZoomDimensions } from "@vscode-bicep-ui/components";
import { useNotification } from "@vscode-bicep-ui/messaging";
import { useCallback, useMemo } from "react";
import { useFitViewToBounds } from "@/lib/graph";
import { documentDidChange } from "../api";
import { GraphActionsContext } from "../context/GraphActionsContext";
import { useGraphSync } from "../hooks/use-graph-sync";
import { useUndoRedoShortcuts } from "../hooks/use-undo-redo-shortcuts";

/**
 * Starts keeping the graph in step with the Bicep file and gives features the actions that change
 * it. Mounted once, by the app, inside `PanZoomProvider`: layout reads the viewport size and fits
 * the camera to a new graph.
 */
export function GraphActionsProvider({ children }: { children: ReactNode }) {
  const getPanZoomDimensions = useGetPanZoomDimensions();
  const getViewportCenter = useCallback(() => {
    const { width, height } = getPanZoomDimensions();
    return { x: width / 2, y: height / 2 };
  }, [getPanZoomDimensions]);
  const fitViewToBounds = useFitViewToBounds();
  const { requestGraphUpdate, resetGraphLayout, createResourceAt, handleNodeDragStart, handleNodeDragEnd, undo, redo } =
    useGraphSync(getViewportCenter, fitViewToBounds);

  useNotification(
    documentDidChange,
    useCallback(() => {
      void requestGraphUpdate();
    }, [requestGraphUpdate]),
  );

  useUndoRedoShortcuts(undo, redo);

  const actions = useMemo<GraphActions>(
    () => ({ resetGraphLayout, createResourceAt, handleNodeDragStart, handleNodeDragEnd, undo, redo }),
    [createResourceAt, handleNodeDragEnd, handleNodeDragStart, redo, resetGraphLayout, undo],
  );

  return <GraphActionsContext.Provider value={actions}>{children}</GraphActionsContext.Provider>;
}
