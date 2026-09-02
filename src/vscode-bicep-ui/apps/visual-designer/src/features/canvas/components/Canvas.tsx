// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { Point } from "@/lib/math";
import type { CanvasActions } from "../context/CanvasActionsContext";
import type { ResourceTypeReference } from "../types";

import { useGetPanZoomDimensions, useGetPanZoomTransform } from "@vscode-bicep-ui/components";
import { useNotification } from "@vscode-bicep-ui/messaging";
import { useAtomValue, useSetAtom } from "jotai";
import { useCallback, useMemo, useState } from "react";
import { styled, ThemeProvider } from "styled-components";
import {
  effectiveExportThemeAtom,
  ExportAreaCover,
  exportCanvasElementAtom,
  ExportPreviewLayer,
} from "@/features/export";
import { documentDidChange } from "@/hooks";
import { Graph, useFitViewToBounds, Viewport } from "@/lib/graph";
import { CanvasActionsContext } from "../context/CanvasActionsContext";
import { useCanvasController } from "../hooks/use-canvas-controller";
import { NodeContentProvider } from "./nodes/NodeContentProvider";
import { PendingResourceLayer } from "./PendingResourceLayer";

const $CanvasWrapper = styled.div`
  position: absolute;
  inset: 0;
`;

function viewportToGraphPoint(
  clientPoint: Point,
  canvasBounds: Pick<DOMRect, "left" | "top">,
  transform: { x: number; y: number; scale: number },
): Point | null {
  if (
    !Number.isFinite(clientPoint.x) ||
    !Number.isFinite(clientPoint.y) ||
    !Number.isFinite(transform.x) ||
    !Number.isFinite(transform.y) ||
    !Number.isFinite(transform.scale) ||
    transform.scale <= 0
  ) {
    return null;
  }

  return {
    x: (clientPoint.x - canvasBounds.left - transform.x) / transform.scale,
    y: (clientPoint.y - canvasBounds.top - transform.y) / transform.scale,
  };
}

export interface CanvasProps {
  /** Layered over the canvas and able to call `useCanvasActions`. */
  children: ReactNode;
}

/** The Bicep design surface, its runtime, and the actions exposed to layered features. */
export function Canvas({ children }: CanvasProps) {
  const getPanZoomDimensions = useGetPanZoomDimensions();
  const getPanZoomTransform = useGetPanZoomTransform();
  const getViewportCenter = useCallback(() => {
    const { width, height } = getPanZoomDimensions();
    return { x: width / 2, y: height / 2 };
  }, [getPanZoomDimensions]);
  const fitViewToBounds = useFitViewToBounds();
  const { requestGraphUpdate, resetGraphLayout, createResourceAt } = useCanvasController(
    getViewportCenter,
    fitViewToBounds,
  );
  const exportTheme = useAtomValue(effectiveExportThemeAtom);
  const setExportCanvasElement = useSetAtom(exportCanvasElementAtom);
  const [canvasElement, setCanvasElement] = useState<HTMLDivElement | null>(null);

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

  const canPlaceResourceAt = useCallback(
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
        await createResourceAt(resourceType, origin);
      }
    },
    [canvasElement, createResourceAt, getPanZoomTransform],
  );

  const actions = useMemo<CanvasActions>(
    () => ({ createResource, canPlaceResourceAt, resetGraphLayout }),
    [canPlaceResourceAt, createResource, resetGraphLayout],
  );

  return (
    <CanvasActionsContext.Provider value={actions}>
      <NodeContentProvider>
        <ThemeProvider theme={exportTheme}>
          <$CanvasWrapper ref={handleCanvasRef}>
            <Viewport>
              <ExportAreaCover />
              <PendingResourceLayer />
              <Graph />
            </Viewport>
          </$CanvasWrapper>
        </ThemeProvider>
      </NodeContentProvider>
      <ExportPreviewLayer />
      {children}
    </CanvasActionsContext.Provider>
  );
}
