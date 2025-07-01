// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomCallback } from "jotai/utils";
import { useCallback } from "react";
import { usePanZoomControl } from "@vscode-bicep-ui/components";
import { styled } from "styled-components";
import { nodesAtom } from "../../graph-core/atoms";

const $GraphControlBar = styled.div`
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 8px;
  background-color: #ffffff;
  border: 1px solid #e1e4e8;
  border-radius: 4px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
`;

const $ControlButton = styled.button`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  padding: 0;
  border: 1px solid #d1d5da;
  border-radius: 4px;
  background-color: #f6f8fa;
  color: #24292f;
  cursor: pointer;
  font-size: 16px;
  font-weight: bold;
  transition: all 0.2s ease;

  &:hover {
    background-color: #f3f4f6;
    border-color: #0969da;
  }

  &:active {
    transform: translateY(1px);
  }

  &:focus {
    outline: 1px solid #0969da;
    outline-offset: 2px;
  }
`;

const $Icon = styled.span`
  display: inline-block;
  line-height: 1;
`;

export function GraphControlBar() {
  const { zoomIn, zoomOut, reset } = usePanZoomControl();

  const resetLayout = useAtomCallback(
    useCallback((get, set) => {
      const nodes = get(nodesAtom);
      for (const node of Object.values(nodes)) {
        if (node.kind === "atomic") {
          set(node.originAtom, { ...get(node.originAtom) });
        }
      }
    }, []),
  );

  return (
    <$GraphControlBar>
      <$ControlButton onClick={zoomIn} title="Zoom In" aria-label="Zoom In">
        <$Icon>+</$Icon>
      </$ControlButton>
      <$ControlButton onClick={zoomOut} title="Zoom Out" aria-label="Zoom Out">
        <$Icon>-</$Icon>
      </$ControlButton>
      <$ControlButton onClick={reset} title="Fit View" aria-label="Fit View">
        <$Icon>↺</$Icon>
      </$ControlButton>
      <$ControlButton onClick={resetLayout} title="Reset Layout" aria-label="Reset Layout">
        <$Icon>⟲</$Icon>
      </$ControlButton>
    </$GraphControlBar>
  );
}
