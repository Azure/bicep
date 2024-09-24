import { useAtomValue } from "jotai";
import { nodesAtom } from "./atoms";
import { Node } from "./Node";

export function NodeLayer() {
  const nodes = useAtomValue(nodesAtom);

  return Object.entries(nodes).map(([id, node]) => (
    <Node key={id} id={id} originAtom={node.origin} boxAtom={node.box} />
  ));
}
