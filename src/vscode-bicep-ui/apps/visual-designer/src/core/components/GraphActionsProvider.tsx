// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { GetGraphResult } from "../api";
import type { GraphActions } from "../context/GraphActionsContext";

import { useGetPanZoomDimensions } from "@vscode-bicep-ui/components";
import { useNotification } from "@vscode-bicep-ui/messaging";
import { useCallback, useMemo, useState } from "react";
import { useFitViewToBounds } from "@/lib/graph";
import { documentDidChange } from "../api";
import { GraphActionsContext } from "../context/GraphActionsContext";
import { GraphUpdateCoordinator } from "../graph-update-coordinator";
import { useGraphSync } from "../hooks/use-graph-sync";
import { useModuleCollapse } from "../hooks/use-module-collapse";
import { useNodeDrag } from "../hooks/use-node-drag";
import { useResourceCreation } from "../hooks/use-resource-creation";
import { useUndoRedo } from "../hooks/use-undo-redo";
import { useUndoRedoShortcuts } from "../hooks/use-undo-redo-shortcuts";

/**
 * Starts keeping the graph in step with the Bicep file and gives features the actions that change
 * it. Mounted once, by the app, inside `PanZoomProvider`: layout reads the viewport size and fits
 * the camera to a new graph.
 *
 * The hooks share state only through atoms, and order their host requests through one
 * `GraphUpdateCoordinator`.
 */
export function GraphActionsProvider({ children }: { children: ReactNode }) {
  const getPanZoomDimensions = useGetPanZoomDimensions();
  const getViewportCenter = useCallback(() => {
    const { width, height } = getPanZoomDimensions();
    return { x: width / 2, y: height / 2 };
  }, [getPanZoomDimensions]);
  const fitViewToBounds = useFitViewToBounds();
  const [coordinator] = useState(() => new GraphUpdateCoordinator<GetGraphResult>());

  const { handleDocumentChange, resetGraphLayout } = useGraphSync(coordinator, getViewportCenter, fitViewToBounds);
  const createResourceAt = useResourceCreation(coordinator);
  const toggleModuleCollapsed = useModuleCollapse(coordinator, getViewportCenter);
  const { handleNodeDragStart, handleNodeDrag, handleNodeDragEnd } = useNodeDrag();
  const { undo, redo } = useUndoRedo(coordinator);

  useNotification(documentDidChange, handleDocumentChange);

  useUndoRedoShortcuts(undo, redo);

  const actions = useMemo<GraphActions>(
    () => ({
      resetGraphLayout,
      createResourceAt,
      toggleModuleCollapsed,
      handleNodeDragStart,
      handleNodeDrag,
      handleNodeDragEnd,
      undo,
      redo,
    }),
    [
      createResourceAt,
      handleNodeDrag,
      handleNodeDragEnd,
      handleNodeDragStart,
      redo,
      resetGraphLayout,
      toggleModuleCollapsed,
      undo,
    ],
  );

  return <GraphActionsContext.Provider value={actions}>{children}</GraphActionsContext.Provider>;
}
