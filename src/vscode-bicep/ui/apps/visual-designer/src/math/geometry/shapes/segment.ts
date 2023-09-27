import { createBox } from "./box";
import { createPoint, pointsEqual } from "./point";

import type { Box } from "./box";
import type { Point } from "./point";

export type Segment = Readonly<{
  start: Point;
  end: Point;
}>;

export function createSegment(start: Point, end: Point): Segment {
  return { start, end };
}

export function getBoundingBox({ start, end }: Segment): Box {
  const center = createPoint(start.x + (end.x - start.x) / 2, start.y + (end.y - start.y) / 2);
  const width = Math.abs(end.x - start.x);
  const height = Math.abs(end.y - start.y);

  return createBox(center, width, height);
}

export function hasZeroLength({ start, end }: Segment) {
  return pointsEqual(start, end);
}
