import { useAtomValue } from "jotai";
import { isPrimitive, nodesByIdAtom } from "./nodes";
import { CompoundNode } from "./nodes/CompoundNode";
import { PrimitiveNode } from "./nodes/PrimitiveNode";

export function NodeLayer() {
  const nodes = useAtomValue(nodesByIdAtom);

  return Object.entries(nodes).map(([id, node]) =>
    isPrimitive(node) ? <PrimitiveNode key={id} {...node} /> : <CompoundNode key={id} {...node} />,
  );
}
