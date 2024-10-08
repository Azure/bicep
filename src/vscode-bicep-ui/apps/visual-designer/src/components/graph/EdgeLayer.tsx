import { useAtomValue } from "jotai";
import { edgesAtom } from "./edges/atoms";
import { StraightEdge } from "./edges/StraightEdge";

export function EdgeLayer() {
  const edges = useAtomValue(edgesAtom);

  return edges.map(edge => (
    <StraightEdge key={edge.id} {...edge} />
  ));
}
