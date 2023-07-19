// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import React, { createContext, RefObject, useEffect, useRef } from "react";
import cytoscape, { Core, LayoutOptions, Layouts, Stylesheet } from "cytoscape";
import elk from "cytoscape-elk";

export interface ZoomOptions {
  minLevel: number;
  maxLevel: number;
  sensitivity: number;
}

export interface CreationOptions {
  containerRef: RefObject<HTMLDivElement>;
  layoutOptions: LayoutOptions;
  zoomOptions: ZoomOptions;
  onNodeDoubleTap: (event: cytoscape.EventObjectNode) => void;
}

export function useCytoscape(
  elements: cytoscape.ElementDefinition[],
  stylesheets: Stylesheet[],
  {
    containerRef,
    layoutOptions,
    zoomOptions,
    onNodeDoubleTap,
  }: CreationOptions,
): [
  React.MutableRefObject<Core | undefined>,
  React.MutableRefObject<Layouts | undefined>,
] {
  const cytoscapeRef = useRef<Core>();
  const layoutRef = useRef<Layouts>();

  // Initialize cytoscape.
  useEffect(() => {
    cytoscape.use(elk);
    const cy = cytoscape({
      container: containerRef.current,
      minZoom: zoomOptions.minLevel,
      maxZoom: zoomOptions.maxLevel,
      wheelSensitivity: zoomOptions.sensitivity,
      autounselectify: true,
    });

    cy.on("layoutstart", () => cy.maxZoom(1));
    cy.on("layoutstop", () => cy.maxZoom(zoomOptions.maxLevel));
    cy.addListener("dbltap", "node", onNodeDoubleTap);

    cytoscapeRef.current = cy;

    return () => {
      cy.removeListener("layoutstart");
      cy.removeListener("layoutstop");
      cy.removeListener("dbltap", "node", onNodeDoubleTap);
      cy.destroy();
    };
  }, []);

  // Update style.
  useEffect(() => {
    cytoscapeRef.current?.style(stylesheets);
  }, [stylesheets]);

  // Update layout.
  useEffect(() => {
    layoutRef.current?.stop();
    cytoscapeRef.current?.json({ elements });
    layoutRef.current = cytoscapeRef.current?.layout(layoutOptions).run();
  }, [elements]);

  return [cytoscapeRef, layoutRef];
}

export const cytoscapeContext = createContext<cytoscape.Core | undefined>(
  undefined,
);
