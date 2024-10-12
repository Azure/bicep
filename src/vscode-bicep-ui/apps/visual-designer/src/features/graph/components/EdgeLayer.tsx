import { useAtomValue } from "jotai";
import { edgesAtom } from "../atoms/edges";
import { StraightEdge } from "./StraightEdge";

export function EdgeLayer() {
  const edges = useAtomValue(edgesAtom);

  return edges.map(edge => (
    <StraightEdge key={edge.id} {...edge} />
  ));
}
