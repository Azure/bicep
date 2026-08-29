// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";

export function viewportToGraphPoint(
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
