// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { equal } from "../comparison";

export interface Point {
  x: number;
  y: number;
}

export type Position = Point;

export function pointsEqual(a: Point, b: Point): boolean {
  return equal(a.x, b.x) && equal(a.y, b.y);
}

export function translatePoint({ x, y }: Point, dx: number, dy: number): Point {
  return {
    x: x + dx,
    y: y + dy,
  };
}
