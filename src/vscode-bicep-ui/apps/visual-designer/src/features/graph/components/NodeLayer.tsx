import { useAtomValue } from "jotai";
import { nodesAtom } from "../atoms";
import { CompoundNode } from "./nodes/CompoundNode";
import { PrimitiveNode } from "./nodes/PrimitiveNode";

export function NodeLayer() {
  const nodes = useAtomValue(nodesAtom);

  return Object.entries(nodes).map(([id, node]) =>
    node.kind === "primitive" ? <PrimitiveNode key={id} {...node} /> : <CompoundNode key={id} {...node} />,
  );
}
