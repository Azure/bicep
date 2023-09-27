import { StraightEdge } from "./StraightEdge";

import type { EdgeShape } from "../../stores/types";

export function getEdgeComponent(edgeShape: EdgeShape) {
  if (edgeShape === "Straight") {
    return StraightEdge;
  }
  
  throw new Error(`Unsupported edge shape: ${edgeShape}`);
}
