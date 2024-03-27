import { equal } from "../../numeric/comparison-operators";

export type Point = Readonly<{
  x: number;
  y: number;
}>;

export type Position = Point;

export function createPoint(x: number, y: number): Point {
  return { x, y };
}

export function translatePoint(point: Point, dx: number, dy: number): Point {
  return createPoint(point.x + dx, point.y + dy);
}

export function pointsEqual(first: Point, second: Point): boolean {
  return equal(first.x, second.x) && equal(first.y, second.y);
}
