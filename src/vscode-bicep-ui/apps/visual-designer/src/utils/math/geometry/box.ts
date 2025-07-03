// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


import { type Point } from "./point";

export interface Box {
  min: Point;
  max: Point;
}

export function getBoxCenter(box: Box): Point {
  return {
    x: (box.min.x + box.max.x) / 2,
    y: (box.min.y + box.max.y) / 2,
  };
}

export function getBoxWidth(box: Box): number {
  return box.max.x - box.min.x;
}

export function getBoxHeight(box: Box): number {
  return box.max.y - box.min.y;
}

export function translateBox(box: Box, dx: number, dy: number): Box {
  return {
    min: {
      x: box.min.x + dx,
      y: box.min.y + dy,
    },
    max: {
      x: box.max.x + dx,
      y: box.max.y + dy,
    },
  };
}

export function boxesOverlap(box: Box, other: Box): boolean {
  return box.min.x <= other.max.x && box.max.x >= other.min.x && box.min.y <= other.max.y && box.max.y >= other.min.y;
}

export function boxContainsPoint(box: Box, point: Point): boolean {
  return box.min.x <= point.x && point.x <= box.max.x && box.min.y <= point.y && point.y <= box.max.y;
}

export function getBoxCenterSegmentIntersection(fromBox: Box, toPoint: Point): Point | undefined {
  if (boxContainsPoint(fromBox, toPoint)) {
    return undefined;
  }

  const centerPoint = getBoxCenter(fromBox);

  const m = (centerPoint.y - toPoint.y) / (centerPoint.x - toPoint.x);
  const h = getBoxHeight(fromBox);
  const w = getBoxWidth(fromBox);

  if (Math.abs(m * w) < h) {
    const k = centerPoint.x > toPoint.x ? -1 : 1;

    return {
      x: centerPoint.x + (k * w) / 2,
      y: centerPoint.y + (k * (m * w)) / 2,
    };
  }

  const k = centerPoint.y > toPoint.y ? -1 : 1;

  return {
    x: centerPoint.x + k * h / 2 / m,
    y: centerPoint.y + k * h / 2,
  };
}
