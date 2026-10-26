// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon, usePanZoomControl } from "@vscode-bicep-ui/components";
import { useAtomValue, useSetAtom } from "jotai";
import { graphHasNodesAtom, isGraphChangeInProgressAtom, useGraphActions } from "@/core";
import { openExportOverlayAtom } from "@/features/export";
import { useFitView } from "@/lib/graph";
import { FloatingPanel, FloatingPanelDivider, ICON_BUTTON_ICON_SIZE, IconButton } from "@/ui";

/** View controls (zoom, fit, reset layout) and export. */
export function ControlBar() {
  const { zoomIn, zoomOut } = usePanZoomControl();
  const fitView = useFitView();
  const { resetGraphLayout } = useGraphActions();
  const hasNodes = useAtomValue(graphHasNodesAtom);
  const isGraphChangeInProgress = useAtomValue(isGraphChangeInProgressAtom);
  const openExportOverlay = useSetAtom(openExportOverlayAtom);

  return (
    <FloatingPanel data-testid="control-bar">
      <IconButton onClick={() => zoomIn(1.5)} title="Zoom In" aria-label="Zoom In" data-testid="control-zoom-in">
        <Codicon name="zoom-in" size={ICON_BUTTON_ICON_SIZE} />
      </IconButton>
      <IconButton onClick={() => zoomOut(1.5)} title="Zoom Out" aria-label="Zoom Out" data-testid="control-zoom-out">
        <Codicon name="zoom-out" size={ICON_BUTTON_ICON_SIZE} />
      </IconButton>
      <IconButton
        onClick={fitView}
        title="Fit View"
        aria-label="Fit View"
        disabled={!hasNodes}
        data-testid="control-fit-view"
      >
        <Codicon name="screen-full" size={ICON_BUTTON_ICON_SIZE} />
      </IconButton>
      <IconButton
        onClick={resetGraphLayout}
        title="Reset Layout"
        aria-label="Reset Layout"
        disabled={!hasNodes || isGraphChangeInProgress}
        data-testid="control-reset-layout"
      >
        <Codicon name="type-hierarchy-sub" size={ICON_BUTTON_ICON_SIZE} />
      </IconButton>
      <FloatingPanelDivider />
      <IconButton
        onClick={() => openExportOverlay()}
        title="Export Graph"
        aria-label="Export Graph"
        disabled={!hasNodes}
        data-testid="control-export"
      >
        <Codicon name="desktop-download" size={ICON_BUTTON_ICON_SIZE} />
      </IconButton>
    </FloatingPanel>
  );
}
