import type { Position } from "../math/geometry";

interface EdgeProps {
  sourceIntersection: Position;
  targetIntersection: Position;
}

export function Edge({ sourceIntersection, targetIntersection }: EdgeProps) {
  return (
    <line
      stroke={"black"}
      strokeWidth={1}
      x1={sourceIntersection.x}
      y1={sourceIntersection.y}
      x2={targetIntersection.x}
      y2={targetIntersection.y}
    />
  );
}
