// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { DocumentDidChangePayload, ResourceTypeReference } from "@/lib/messaging";
import type { Point } from "@/lib/utils";

import { useGetPanZoomDimensions, useGetPanZoomTransform } from "@vscode-bicep-ui/components";
import { useWebviewMessageChannel, useWebviewNotification } from "@vscode-bicep-ui/messaging";
import { useAtomValue, useSetAtom } from "jotai";
import { useCallback, useEffect, useMemo, useState } from "react";
import { styled, ThemeProvider } from "styled-components";
import { effectiveExportThemeAtom, exportCanvasElementAtom, exportFileStemAtom } from "@/features/export";
import { Canvas, Graph, useFitViewToBounds, viewportToGraphPoint } from "@/lib/graph";
import { DOCUMENT_DID_CHANGE_NOTIFICATION, READY_NOTIFICATION } from "@/lib/messaging";
import { useGraphUpdate } from "../hooks/use-graph-update";
import { NodeContentProvider } from "./nodes/NodeContentProvider";
import { PendingResourceLayer } from "./PendingResourceLayer";

const $CanvasWrapper = styled.div`
  position: absolute;
  inset: 0;
`;

function deriveExportFileStem(documentPath?: string, documentFileName?: string): string {
  const fileName = (documentFileName || documentPath || "").split(/[\\/]/).pop() ?? "";
  const stem = fileName.replace(/\.[^.]+$/, "").trim();

  return stem || "bicep-graph";
}

export interface DeploymentGraphSurface {
  /**
   * Create a resource at a client-coordinate point. Omit `clientPoint` to use the surface's default
   * placement, which is how keyboard activation creates a resource.
   */
  createResource: (resourceType: ResourceTypeReference, clientPoint?: Point) => Promise<void>;
  /** Whether a client-coordinate point falls on the graph surface. */
  canPlaceAt: (clientPoint: Point) => boolean;
  resetLayout: () => Promise<void>;
}

export interface DeploymentGraphViewProps {
  /** Rendered inside the canvas, beneath the graph, for export overlays. */
  canvasOverlay?: ReactNode;
  children: (surface: DeploymentGraphSurface) => ReactNode;
}

/**
 * The Bicep deployment graph surface: owns the update loop, the canvas subtree, and the pending
 * resource layer.
 *
 * The surface handed to `children` is stated in client coordinates on purpose. Converting a pointer
 * position into a graph position needs the canvas rect and the pan/zoom transform, both of which are
 * graph knowledge; exposing them would push that geometry into whichever feature happened to call.
 *
 * Actions are passed as explicit props rather than exposed as a free hook because `useGraphUpdate` is
 * a single-instance state machine holding the client's mirror of the server's canonical graph, and a
 * second instance would diverge and corrupt patch application.
 */
export function DeploymentGraphView({ canvasOverlay, children }: DeploymentGraphViewProps) {
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
  const messageChannel = useWebviewMessageChannel();
  const exportTheme = useAtomValue(effectiveExportThemeAtom);
  const setExportFileStem = useSetAtom(exportFileStemAtom);
  const setExportCanvasElement = useSetAtom(exportCanvasElementAtom);
  const [canvasElement, setCanvasElement] = useState<HTMLDivElement | null>(null);

  useEffect(() => {
    messageChannel.sendNotification({ method: READY_NOTIFICATION });
  }, [messageChannel]);

  // Listen for "the graph may have changed" notifications. The webview pulls the update itself,
  // submitting the graph it currently displays and applying the patches.
  useWebviewNotification(
    DOCUMENT_DID_CHANGE_NOTIFICATION,
    useCallback(
      (params: unknown) => {
        const payload = params as DocumentDidChangePayload;
        messageChannel.setState({ documentPath: payload.documentUri });
        setExportFileStem(deriveExportFileStem(payload.documentUri));
        void requestGraphUpdate();
      },
      [messageChannel, requestGraphUpdate, setExportFileStem],
    ),
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

  const surface = useMemo<DeploymentGraphSurface>(
    () => ({ createResource, canPlaceAt, resetLayout }),
    [canPlaceAt, createResource, resetLayout],
  );

  return (
    <NodeContentProvider>
      <ThemeProvider theme={exportTheme}>
        <$CanvasWrapper ref={handleCanvasRef}>
          <Canvas>
            {canvasOverlay}
            <PendingResourceLayer />
            <Graph />
          </Canvas>
        </$CanvasWrapper>
      </ThemeProvider>
      {children(surface)}
    </NodeContentProvider>
  );
}
