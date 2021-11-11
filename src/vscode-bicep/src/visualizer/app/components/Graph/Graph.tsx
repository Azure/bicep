// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { useRef, VFC, memo, useCallback, useMemo } from "react";
import cytoscape from "cytoscape";
import styled, { DefaultTheme, withTheme } from "styled-components";

import { createStylesheet } from "./style";
import { createRevealFileRangeMessage } from "../../../messages";
import { vscode } from "../../vscode";
import { useCytoscape } from "../../hooks";

interface GraphProps {
  elements: cytoscape.ElementDefinition[];
  theme: DefaultTheme;
}

const layoutOptions = {
  name: "elk",
  padding: 100,
  fit: true,
  animate: true,
  animationDuration: 1000,
  animationEasing: "cubic-bezier(0.33, 1, 0.68, 1)",
  elk: {
    algorithm: "layered",
    "layered.layering.strategy": "INTERACTIVE",
    "layered.nodePlacement.bk.fixedAlignment": "BALANCED",
    "layered.cycleBreaking.strategy": "DEPTH_FIRST",
    "elk.direction": "DOWN",
    "elk.aspectRatio": 2.5,
    "spacing.nodeNodeBetweenLayers": 80,
    "spacing.baseValue": 120,
    "spacing.componentComponent": 100,
  },
};

const zoomOptions = {
  minLevel: 0.2,
  maxLevel: 2,
  sensitivity: 0.1,
};

const GraphContainer = styled.div`
  position: absolute;
  left: 0px;
  top: 0px;
  bottom: 0px;
  right: 0px;
  overflow: hidden;
  background-color: ${({ theme }) => theme.canvas.backgroundColor};
  background-image: ${({ theme }) => theme.canvas.backgroundImage};
  background-size: 24px 24px;
  background-position: 12px 12px;
`;

const GraphComponent: VFC<GraphProps> = ({ elements, theme }) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const styleSheet = useMemo(() => createStylesheet(theme), [theme]);

  useCytoscape(elements, styleSheet, {
    containerRef,
    layoutOptions,
    zoomOptions,

    onNodeDoubleTap: useCallback((event: cytoscape.EventObjectNode) => {
      const filePath = event.target.data("filePath");
      const range = event.target.data("range");
      vscode.postMessage(createRevealFileRangeMessage(filePath, range));
    }, []),
  });

  return <GraphContainer ref={containerRef} theme={theme} />;
};

export const Graph = memo(
  withTheme(GraphComponent),
  (prevProps, nextProps) =>
    prevProps.theme === nextProps.theme &&
    prevProps.elements.length === nextProps.elements.length &&
    prevProps.elements.every((prevElement, i) => {
      const prevData = prevElement.data;
      const nextData = nextProps.elements[i].data;

      return (
        prevData.id === nextData.id &&
        prevData.hasError === nextData.hasError &&
        prevData.filePath === nextData.filePath &&
        prevData.range?.start?.line === nextData.range?.start?.line &&
        prevData.range?.start?.character === nextData.range?.start?.character &&
        prevData.range?.end?.line === nextData.range?.end?.line &&
        prevData.range?.end?.character === nextData.range?.end?.character &&
        prevData.backgroundDataUri === nextData.backgroundDataUri &&
        prevData.source === nextData.source &&
        prevData.target === nextData.target
      );
    })
);
