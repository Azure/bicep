// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon, useGetPanZoomDimensions, usePanZoomControl } from "@vscode-bicep-ui/components";
import { useStore } from "jotai";
import { useCallback, useRef } from "react";
import { styled } from "styled-components";
import { useFitView } from "../../../hooks/useFitView";
import { applyLayout, computeLayout } from "../../graph-engine/layout/elk-layout";

const $GraphControlBar = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2px;
  padding: 4px;
  background-color: ${({ theme }) => theme.controlBar.background};
  border: 1px solid ${({ theme }) => theme.controlBar.border};
  border-radius: 6px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  backdrop-filter: blur(8px);
`;

const $ControlButton = styled.button`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  padding: 0;
  border: none;
  border-radius: 4px;
  background-color: transparent;
  color: ${({ theme }) => theme.controlBar.icon};
  cursor: pointer;
  transition: all 0.15s ease;

  &:hover {
    background-color: ${({ theme }) => theme.controlBar.hoverBackground};
  }

  &:active {
    background-color: ${({ theme }) => theme.controlBar.activeBackground};
    transform: scale(0.95);
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

export interface GraphControlBarProps {
  onExport?: () => void;
}

export function GraphControlBar({ onExport }: GraphControlBarProps) {
  const { zoomIn, zoomOut } = usePanZoomControl();
  const getPanZoomDimensions = useGetPanZoomDimensions();
  const store = useStore();
  const fitView = useFitView();
  const layoutInFlight = useRef(false);

  const resetLayout = useCallback(async () => {
    if (layoutInFlight.current) return;
    layoutInFlight.current = true;
    try {
      const viewport = getPanZoomDimensions();
      const result = await computeLayout(store, viewport);
      await applyLayout(store, result);
    } finally {
      layoutInFlight.current = false;
    }
  }, [store, getPanZoomDimensions]);

  return (
    <$GraphControlBar>
      <$ControlButton onClick={() => zoomIn(1.5)} title="Zoom In" aria-label="Zoom In">
        <Codicon name="zoom-in" size={16} />
      </$ControlButton>
      <$ControlButton onClick={() => zoomOut(1.5)} title="Zoom Out" aria-label="Zoom Out">
        <Codicon name="zoom-out" size={16} />
      </$ControlButton>
      <$ControlButton onClick={fitView} title="Fit View" aria-label="Fit View">
        <Codicon name="screen-full" size={16} />
      </$ControlButton>
      <$ControlButton onClick={resetLayout} title="Reset Layout" aria-label="Reset Layout">
        <Codicon name="type-hierarchy-sub" size={16} />
      </$ControlButton>
      {onExport && (
        <>
          <$Divider />
          <$ControlButton onClick={onExport} title="Export Graph" aria-label="Export Graph">
            <Codicon name="desktop-download" size={16} />
          </$ControlButton>
        </>
      )}
    </$GraphControlBar>
  );
}
