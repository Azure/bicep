/* eslint-disable sort-imports */
import { equalsZero, greaterThan, lessThan } from "../../numeric/comparison-operators";
import { getMinX, getMaxX, getMinY, getMaxY, enumerateSegments } from "../shapes/box";
import { createPoint } from "../shapes/point";
import { getBoundingBox, hasZeroLength } from "../shapes/segment";

import type { Box } from "../shapes/box";
import type { Point } from "../shapes/point";
import type { Segment } from "../shapes/segment";

export function boxesIntersect(first: Box, second: Box): boolean {
  return !(
    greaterThan(getMinX(first), getMaxX(second)) ||
    greaterThan(getMinX(second), getMaxX(first)) ||
    greaterThan(getMinY(first), getMaxY(second)) ||
    greaterThan(getMinY(second), getMaxY(first))
  );
}

export function findSegmentSegmentIntersection(first: Segment, second: Segment): Point | undefined {
  /*
  Note: The function is not designed to address scenarios properly where:
  - Any of the line segments is of zero length.
  - The line segments are parallel to each other.
  This is due to these cases being considered non-viable in the application context.
  Should these conditions arise, the function will return an empty array.
  */
  if (hasZeroLength(first) || hasZeroLength(second)) {
    return undefined;
  }

  if (!boxesIntersect(getBoundingBox(first), getBoundingBox(second))) {
    return undefined;
  }

  // See https://paulbourke.net/geometry/pointlineplane/
  const { x: x1, y: y1 } = first.start;
  const { x: x2, y: y2 } = first.end;
  const { x: x3, y: y3 } = second.start;
  const { x: x4, y: y4 } = second.end;

  const denominator = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);

  // Lines are parallel.
  if (equalsZero(denominator)) {
    return undefined;
  }

  const ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denominator;
  const ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denominator;

  if (lessThan(ua, 0) || greaterThan(ua, 1) || lessThan(ub, 0) || greaterThan(ub, 1)) {
    // The intersection is along the segments.
    return undefined;
  }

  const x = x1 + ua * (x2 - x1);
  const y = y1 + ua * (y2 - y1);

  return createPoint(x, y);
}

export function findFirstSegmentBoxIntersection(segment: Segment, box: Box): Point | undefined {
  for (const boxSegment of enumerateSegments(box)) {
    const intersection = findSegmentSegmentIntersection(segment, boxSegment);

    if (intersection !== undefined) {
      return intersection;
    }
  }

  return undefined;
}
