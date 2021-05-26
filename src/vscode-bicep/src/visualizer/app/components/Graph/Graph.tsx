// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { useEffect, useRef, VFC, memo } from "react";
import cytoscape from "cytoscape";
import elk from "cytoscape-elk";
import styled from "styled-components";
import { stylesheet } from "./style";

interface GraphProps {
  elements: cytoscape.ElementDefinition[];
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

const GraphContainer = styled.div`
  position: absolute;
  left: 0px;
  top: 0px;
  bottom: 0px;
  right: 0px;
`;

const GraphComponent: VFC<GraphProps> = ({ elements }) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const cytoscapeRef = useRef<cytoscape.Core>();
  const layoutRef = useRef<cytoscape.Layouts>();

  useEffect(() => {
    if (!cytoscapeRef.current) {
      cytoscape.use(elk);
      const cy = cytoscape({
        container: containerRef.current,
        layout: layoutOptions,
        elements,
        minZoom: 0.2,
        maxZoom: 1,
        wheelSensitivity: 0.1,
        autounselectify: true,
        style: stylesheet,
      });

      cy.on("layoutstart", () => cy.maxZoom(1));
      cy.on("layoutstop", () => cy.maxZoom(2));

      cytoscapeRef.current = cy;
    } else {
      layoutRef.current?.stop();
      cytoscapeRef.current.json({ elements });
      layoutRef.current = cytoscapeRef.current.layout(layoutOptions);
      layoutRef.current.run();
    }
  }, [elements]);

  useEffect(() => () => cytoscapeRef.current?.destroy(), []);

  return <GraphContainer ref={containerRef} />;
};

export const Graph = memo(
  GraphComponent,
  (prevProps, nextProps) =>
    prevProps.elements.length === nextProps.elements.length &&
    prevProps.elements.every((prevElement, i) => {
      const prevData = prevElement.data;
      const nextData = nextProps.elements[i].data;

      return (
        prevData.id === nextData.id &&
        prevData.hasError === nextData.hasError &&
        prevData.backgroundDataUri === nextData.backgroundDataUri &&
        prevData.source === nextData.source &&
        prevData.target === nextData.target
      );
    })
);
