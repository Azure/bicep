// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { Point } from "@/lib/math";
import type { ResourceTypeReference } from "../types";

import { useGetPanZoomDimensions, useGetPanZoomTransform } from "@vscode-bicep-ui/components";
import { useNotification } from "@vscode-bicep-ui/messaging";
import { useAtomValue, useSetAtom } from "jotai";
import { useCallback, useMemo, useState } from "react";
import { styled, ThemeProvider } from "styled-components";
import { effectiveExportThemeAtom, exportCanvasElementAtom } from "@/features/export";
import { documentDidChange } from "@/hooks";
import { Graph, useFitViewToBounds, Viewport } from "@/lib/graph";
import { useGraphUpdate } from "../hooks/use-graph-update";
import { viewportToGraphPoint } from "../utils/viewport";
import { NodeContentProvider } from "./nodes/NodeContentProvider";
import { PendingResourceLayer } from "./PendingResourceLayer";

const $CanvasWrapper = styled.div`
  position: absolute;
  inset: 0;
`;

export interface CanvasSurface {
  /**
   * Create a resource at a client-coordinate point. Omit `clientPoint` to use the surface's default
   * placement, which is how keyboard activation creates a resource.
   */
  createResource: (resourceType: ResourceTypeReference, clientPoint?: Point) => Promise<void>;
  /** Whether a client-coordinate point falls on the graph surface. */
  canPlaceAt: (clientPoint: Point) => boolean;
  resetLayout: () => Promise<void>;
}

export interface CanvasViewProps {
  /** Rendered inside the canvas, beneath the graph, for export overlays. */
  canvasOverlay?: ReactNode;
  children: (surface: CanvasSurface) => ReactNode;
}

/**
 * The Bicep design surface: owns the update loop, the canvas subtree, and the pending resource layer.
 *
 * The surface handed to `children` is stated in client coordinates on purpose. Converting a pointer
 * position into a graph position needs the canvas rect and the pan/zoom transform, both of which are
 * graph knowledge; exposing them would push that geometry into whichever feature happened to call.
 *
 * Actions are passed as explicit props rather than exposed as a free hook because `useGraphUpdate` is
 * a single-instance state machine holding the client's mirror of the server's canonical graph, and a
 * second instance would diverge and corrupt patch application.
 */
export function CanvasView({ canvasOverlay, children }: CanvasViewProps) {
  const getPanZoomDimensions = useGetPanZoomDimensions();
  const getPanZoomTransform = useGetPanZoomTransform();
  const getViewportCenter = useCallback(() => {
    const { width, height } = getPanZoomDimensions();
    return { x: width / 2, y: height / 2 };
  }, [getPanZoomDimensions]);
  const fitViewToBounds = useFitViewToBounds();
  const {
    requestGraphUpdate,
    createResource: createResourceAtOrigin,
    resetLayout,
  } = useGraphUpdate(getViewportCenter, fitViewToBounds);
  const exportTheme = useAtomValue(effectiveExportThemeAtom);
  const setExportCanvasElement = useSetAtom(exportCanvasElementAtom);
  const [canvasElement, setCanvasElement] = useState<HTMLDivElement | null>(null);

  // "The graph may have changed." The webview pulls the update itself, submitting the graph it
  // currently displays and applying the patches. Other features subscribe to this same notification
  // independently for their own concerns.
  useNotification(
    documentDidChange,
    useCallback(() => {
      void requestGraphUpdate();
    }, [requestGraphUpdate]),
  );

  const handleCanvasRef = useCallback(
    (element: HTMLDivElement | null) => {
      setCanvasElement(element);
      setExportCanvasElement(element);
    },
    [setExportCanvasElement],
  );

  const canPlaceAt = useCallback(
    ({ x, y }: Point) => {
      if (!canvasElement) {
        return false;
      }

      const bounds = canvasElement.getBoundingClientRect();
      const elementAtPoint = document.elementFromPoint(x, y);

      return (
        !!elementAtPoint &&
        canvasElement.contains(elementAtPoint) &&
        x >= bounds.left &&
        x <= bounds.right &&
        y >= bounds.top &&
        y <= bounds.bottom
      );
    },
    [canvasElement],
  );

  const createResource = useCallback(
    async (resourceType: ResourceTypeReference, clientPoint?: Point) => {
      if (!canvasElement) {
        return;
      }

      const bounds = canvasElement.getBoundingClientRect();
      // No point means "wherever this surface puts things by default", which for keyboard
      // activation is the middle of the visible canvas.
      const point = clientPoint ?? {
        x: bounds.left + bounds.width / 2,
        y: bounds.top + bounds.height / 2,
      };
      const origin = viewportToGraphPoint(point, bounds, getPanZoomTransform());

      if (origin) {
        await createResourceAtOrigin(resourceType, origin);
      }
    },
    [canvasElement, createResourceAtOrigin, getPanZoomTransform],
  );

  const surface = useMemo<CanvasSurface>(
    () => ({ createResource, canPlaceAt, resetLayout }),
    [canPlaceAt, createResource, resetLayout],
  );

  return (
    <NodeContentProvider>
      <ThemeProvider theme={exportTheme}>
        <$CanvasWrapper ref={handleCanvasRef}>
          <Viewport>
            {canvasOverlay}
            <PendingResourceLayer />
            <Graph />
          </Viewport>
        </$CanvasWrapper>
      </ThemeProvider>
      {children(surface)}
    </NodeContentProvider>
  );
}
