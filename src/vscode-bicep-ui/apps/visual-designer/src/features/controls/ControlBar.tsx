// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon, usePanZoomControl } from "@vscode-bicep-ui/components";
import { useAtomValue, useSetAtom } from "jotai";
import { styled } from "styled-components";
import { openExportOverlayAtom } from "@/features/export";
import { useFitView } from "@/lib/graph";
import { graphControlAvailabilityAtom } from "./atoms";
import { ControlButton, ControlSurface } from "./ControlPrimitives";
import { useResetLayout } from "./use-reset-layout";

const $Divider = styled.div`
  height: 1px;
  margin: 2px 4px;
  background-color: ${({ theme }) => theme.controlBar.border};
`;

interface ControlBarProps {
  requestLayout: () => Promise<void>;
}

export function ControlBar({ requestLayout }: ControlBarProps) {
  const { zoomIn, zoomOut } = usePanZoomControl();
  const fitView = useFitView();
  const resetLayout = useResetLayout(requestLayout);
  const controls = useAtomValue(graphControlAvailabilityAtom);
  const openExportOverlay = useSetAtom(openExportOverlayAtom);

  return (
    <ControlSurface data-testid="control-bar">
      <ControlButton onClick={() => zoomIn(1.5)} title="Zoom In" aria-label="Zoom In" data-testid="control-zoom-in">
        <Codicon name="zoom-in" size={16} />
      </ControlButton>
      <ControlButton onClick={() => zoomOut(1.5)} title="Zoom Out" aria-label="Zoom Out" data-testid="control-zoom-out">
        <Codicon name="zoom-out" size={16} />
      </ControlButton>
      <ControlButton
        onClick={fitView}
        title="Fit View"
        aria-label="Fit View"
        disabled={!controls.canFitView}
        data-testid="control-fit-view"
      >
        <Codicon name="screen-full" size={16} />
      </ControlButton>
      <ControlButton
        onClick={resetLayout}
        title="Reset Layout"
        aria-label="Reset Layout"
        disabled={!controls.canResetLayout}
        data-testid="control-reset-layout"
      >
        <Codicon name="type-hierarchy-sub" size={16} />
      </ControlButton>
      <$Divider />
      <ControlButton
        onClick={() => openExportOverlay()}
        title="Export Graph"
        aria-label="Export Graph"
        disabled={!controls.canExportGraph}
        data-testid="control-export"
      >
        <Codicon name="desktop-download" size={16} />
      </ControlButton>
    </ControlSurface>
  );
}
