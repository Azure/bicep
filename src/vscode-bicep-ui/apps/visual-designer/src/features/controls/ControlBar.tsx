// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon, usePanZoomControl } from "@vscode-bicep-ui/components";
import { useAtomValue, useSetAtom } from "jotai";
import { styled } from "styled-components";
import { openExportOverlayAtom } from "@/features/export";
import { useFitView } from "@/lib/graph";
import { graphControlAvailabilityAtom } from "./atoms";
import { useResetLayout } from "./use-reset-layout";

const $ControlBar = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1px;
  padding: 4px;
  background-color: ${({ theme }) => theme.controlBar.background};
  border: 1px solid ${({ theme }) => theme.controlBar.border};
  border-radius: 8px;
  box-shadow:
    0 1px 3px rgba(0, 0, 0, 0.08),
    0 4px 12px rgba(0, 0, 0, 0.06);
  backdrop-filter: blur(12px);
`;

const $ControlButton = styled.button`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  padding: 0;
  border: none;
  border-radius: 6px;
  background-color: transparent;
  color: ${({ theme }) => theme.controlBar.icon};
  cursor: pointer;
  transition:
    background-color 150ms ease,
    transform 150ms ease;

  &:hover {
    background-color: ${({ theme }) => theme.controlBar.hoverBackground};
  }

  &:active {
    background-color: ${({ theme }) => theme.controlBar.activeBackground};
    transform: scale(0.95);
  }

  &:disabled {
    opacity: 0.45;
    cursor: not-allowed;
    transform: none;
  }

  &:disabled:hover,
  &:disabled:active {
    background-color: transparent;
    transform: none;
  }

  &:focus-visible {
    outline: 2px solid ${({ theme }) => theme.focusBorder};
    outline-offset: 1px;
  }
`;

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
    <$ControlBar data-testid="control-bar">
      <$ControlButton onClick={() => zoomIn(1.5)} title="Zoom In" aria-label="Zoom In" data-testid="control-zoom-in">
        <Codicon name="zoom-in" size={16} />
      </$ControlButton>
      <$ControlButton
        onClick={() => zoomOut(1.5)}
        title="Zoom Out"
        aria-label="Zoom Out"
        data-testid="control-zoom-out"
      >
        <Codicon name="zoom-out" size={16} />
      </$ControlButton>
      <$ControlButton
        onClick={fitView}
        title="Fit View"
        aria-label="Fit View"
        disabled={!controls.canFitView}
        data-testid="control-fit-view"
      >
        <Codicon name="screen-full" size={16} />
      </$ControlButton>
      <$ControlButton
        onClick={resetLayout}
        title="Reset Layout"
        aria-label="Reset Layout"
        disabled={!controls.canResetLayout}
        data-testid="control-reset-layout"
      >
        <Codicon name="type-hierarchy-sub" size={16} />
      </$ControlButton>
      <$Divider />
      <$ControlButton
        onClick={() => openExportOverlay()}
        title="Export Graph"
        aria-label="Export Graph"
        disabled={!controls.canExportGraph}
        data-testid="control-export"
      >
        <Codicon name="desktop-download" size={16} />
      </$ControlButton>
    </$ControlBar>
  );
}
