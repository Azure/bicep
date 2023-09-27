import { createPoint, pointsEqual, translatePoint } from "./point";
import { createSegment } from "./segment";
import { equal } from "../../numeric/comparison-operators";

import type { Point } from "./point";
import type { Segment } from "./segment";


export type Box = Readonly<{
  center: Point;
  width: number;
  height: number;
}>;

export function createBox(center: Point, width: number, height: number): Box {
  if (width < 0 || height < 0) {
    throw new Error("Box width and height must be non-negative.");
  }

  return { center, width, height };
}

export function boxesEqual(first: Box, second: Box) {
  return (
    pointsEqual(first.center, second.center) && equal(first.width, second.width) && equal(first.height, second.height)
  );
}

export function translateBox(box: Box, dx: number, dy: number) {
  return createBox(translatePoint(box.center, dx, dy), box.width, box.height);
}

export function getMinX(box: Box) {
  return box.center.x - box.width / 2;
}

export function getMinY(box: Box) {
  return box.center.y - box.height / 2;
}

export function getMaxX(box: Box) {
  return box.center.x + box.width / 2;
}

export function getMaxY(box: Box) {
  return box.center.y + box.height / 2;
}

export function getLeftSegment(box: Box): Segment {
  const minX = getMinX(box);
  return createSegment(createPoint(minX, getMinY(box)), createPoint(minX, getMaxY(box)));
}

export function getTopSegment(box: Box): Segment {
  const minY = getMinY(box);
  return createSegment(createPoint(getMinX(box), minY), createPoint(getMaxX(box), minY));
}

export function getRightSegment(box: Box): Segment {
  const maxX = getMaxX(box);
  return createSegment(createPoint(maxX, getMinY(box)), createPoint(maxX, getMaxY(box)));
}

export function getBottomSegment(box: Box): Segment {
  const maxY = getMaxY(box);
  return createSegment(createPoint(getMinX(box), maxY), createPoint(getMaxX(box), maxY));
}

export function* enumerateSegments(box: Box): Generator<Segment> {
  yield getLeftSegment(box);
  yield getTopSegment(box);
  yield getRightSegment(box);
  yield getBottomSegment(box);
}
