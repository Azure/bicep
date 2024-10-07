import { useAtomValue } from "jotai";
import { isSubgraph, nodesAtom } from "./atoms";
import { PrimitiveNode } from "./nodes/PrimitiveNode";
import { CompoundNode } from "./nodes/CompoundNode";

export function NodeLayer() {
  const nodes = useAtomValue(nodesAtom);

  return Object.entries(nodes).map(([id, node]) => (
    isSubgraph(node)
    ? <CompoundNode key={id} id={id} childIdsAtom={node.childNodeIds} boxAtom={node.box} />
    : <PrimitiveNode key={id} id={id} originAtom={node.origin} boxAtom={node.box} />
  ));
}
