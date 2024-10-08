import { pointTranslate, type Point } from "./point";

export interface Box {
  min: Point;
  max: Point;
}

export function boxTranslate(box: Box, dx: number, dy: number): Box {
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

export function boxTranslateTo(box: Box, min: Point): Box {
  return {
    min,
    max: pointTranslate(box.max, min.x - box.min.x, min.y - box.min.y),
  };
}

// prettier-ignore
export function boxContainsPoint(box: Box, point: Point): boolean {
  return (
    box.min.x <= point.x &&
    box.min.y <= point.y &&
    box.max.x >= point.x &&
    box.max.y >= point.y
  )
}

// prettier-ignore
export function boxContainsBox(box: Box, other: Box): boolean {
  return (
    box.min.x <= other.min.x &&
    box.min.y <= other.min.y &&
    box.max.x >= other.max.x &&
    box.max.y >= other.max.y
  )
}
