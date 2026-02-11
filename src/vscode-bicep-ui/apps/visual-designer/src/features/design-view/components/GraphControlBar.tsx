// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon, useGetPanZoomDimensions, usePanZoomControl } from "@vscode-bicep-ui/components";
import { useStore } from "jotai";
import { useAtomCallback } from "jotai/utils";
import { useCallback } from "react";
import { styled } from "styled-components";
import { nodesAtom } from "../../graph-engine/atoms";
import { runLayout } from "../../graph-engine/layout/elk-layout";

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

export function GraphControlBar() {
  const getPanZoomDimensions = useGetPanZoomDimensions();
  const { zoomIn, zoomOut, transform } = usePanZoomControl();
  const store = useStore();

  const resetLayout = useCallback(async () => {
    await runLayout(store);
  }, [store]);

  const fitView = useAtomCallback(
    useCallback(
      (get) => {
        const nodes = get(nodesAtom);
        const nodeList = Object.values(nodes);

        if (nodeList.length === 0) {
          return;
        }

        // Calculate the bounding box of all nodes
        const boxes = nodeList.map((node) => get(node.boxAtom));
        const graphMinX = Math.min(...boxes.map((box) => box.min.x));
        const graphMinY = Math.min(...boxes.map((box) => box.min.y));
        const graphMaxX = Math.max(...boxes.map((box) => box.max.x));
        const graphMaxY = Math.max(...boxes.map((box) => box.max.y));

        const graphWidth = graphMaxX - graphMinX;
        const graphHeight = graphMaxY - graphMinY;
        const graphCenterX = graphMinX + graphWidth / 2;
        const graphCenterY = graphMinY + graphHeight / 2;

        const { width: viewportWidth, height: viewportHeight } = getPanZoomDimensions();

        // Calculate scale to fit content in viewport with some padding
        const padding = 100; // Padding around the content
        const scaleX = (viewportWidth - padding * 2) / graphWidth;
        const scaleY = (viewportHeight - padding * 2) / graphHeight;
        const scale = Math.min(scaleX, scaleY, 1); // Don't zoom in beyond 1:1

        // Calculate translation to center the content
        const viewportCenterX = viewportWidth / 2;
        const viewportCenterY = viewportHeight / 2;
        const translateX = viewportCenterX - graphCenterX * scale;
        const translateY = viewportCenterY - graphCenterY * scale;

        transform(translateX, translateY, scale);
      },
      [getPanZoomDimensions, transform],
    ),
  );

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
    </$GraphControlBar>
  );
}
