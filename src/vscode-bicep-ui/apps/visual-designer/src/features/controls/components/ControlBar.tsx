// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon, usePanZoomControl } from "@vscode-bicep-ui/components";
import { useAtomValue, useSetAtom } from "jotai";
import { styled } from "styled-components";
import { useCanvasActions } from "@/features/canvas";
import { openExportOverlayAtom } from "@/features/export";
import { useFitView } from "@/lib/graph";
import { FloatingPanel, IconButton } from "@/ui";
import { graphControlAvailabilityAtom } from "../atoms";
import { useResetGraphLayout } from "../hooks/use-reset-graph-layout";

const $ControlBar = styled(FloatingPanel)`
  position: absolute;
  top: 16px;
  right: 16px;
  z-index: 100;
`;

const $Divider = styled.div`
  height: 1px;
  margin: 2px 4px;
  background-color: ${({ theme }) => theme.panel.border};
`;

export function ControlBar() {
  const { zoomIn, zoomOut } = usePanZoomControl();
  const fitView = useFitView();
  const { resetGraphLayout: requestResetGraphLayout } = useCanvasActions();
  const resetGraphLayout = useResetGraphLayout(requestResetGraphLayout);
  const controls = useAtomValue(graphControlAvailabilityAtom);
  const openExportOverlay = useSetAtom(openExportOverlayAtom);

  return (
    <$ControlBar data-testid="control-bar">
      <IconButton onClick={() => zoomIn(1.5)} title="Zoom In" aria-label="Zoom In" data-testid="control-zoom-in">
        <Codicon name="zoom-in" size={16} />
      </IconButton>
      <IconButton onClick={() => zoomOut(1.5)} title="Zoom Out" aria-label="Zoom Out" data-testid="control-zoom-out">
        <Codicon name="zoom-out" size={16} />
      </IconButton>
      <IconButton
        onClick={fitView}
        title="Fit View"
        aria-label="Fit View"
        disabled={!controls.canFitView}
        data-testid="control-fit-view"
      >
        <Codicon name="screen-full" size={16} />
      </IconButton>
      <IconButton
        onClick={resetGraphLayout}
        title="Reset Layout"
        aria-label="Reset Layout"
        disabled={!controls.canResetGraphLayout}
        data-testid="control-reset-layout"
      >
        <Codicon name="type-hierarchy-sub" size={16} />
      </IconButton>
      <$Divider />
      <IconButton
        onClick={() => openExportOverlay()}
        title="Export Graph"
        aria-label="Export Graph"
        disabled={!controls.canExportGraph}
        data-testid="control-export"
      >
        <Codicon name="desktop-download" size={16} />
      </IconButton>
    </$ControlBar>
  );
}
