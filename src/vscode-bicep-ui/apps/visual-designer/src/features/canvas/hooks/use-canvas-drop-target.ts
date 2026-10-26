// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeReference } from "@/core";
import type { Point } from "@/lib/math";

import { useGetPanZoomTransform } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { useCallback, useMemo } from "react";
import { canvasElementAtom, useGraphActions } from "@/core";

function clientToGraphPoint(
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

/** The canvas as a drop target for resources dragged from the palette. Points are client coordinates. */
export interface CanvasDropTarget {
  /** Whether a resource dropped at this point would land on the canvas. */
  canDropResourceAt: (clientPoint: Point) => boolean;
  /** Create a resource where it was dropped, converting the point to graph coordinates. */
  dropResourceAt: (resourceType: ResourceTypeReference, clientPoint: Point) => Promise<void>;
}

/** Usable anywhere inside `PanZoomProvider` and `GraphActionsProvider`; the canvas need not be an ancestor. */
export function useCanvasDropTarget(): CanvasDropTarget {
  const canvasElement = useAtomValue(canvasElementAtom);
  const getPanZoomTransform = useGetPanZoomTransform();
  const { createResourceAt } = useGraphActions();

  const canDropResourceAt = useCallback(
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

  const dropResourceAt = useCallback(
    async (resourceType: ResourceTypeReference, clientPoint: Point) => {
      if (!canvasElement) {
        return;
      }

      const origin = clientToGraphPoint(clientPoint, canvasElement.getBoundingClientRect(), getPanZoomTransform());
      if (origin) {
        await createResourceAt(resourceType, origin);
      }
    },
    [canvasElement, createResourceAt, getPanZoomTransform],
  );

  return useMemo(() => ({ canDropResourceAt, dropResourceAt }), [canDropResourceAt, dropResourceAt]);
}
