import { CompactNode } from "./CompactNode";
import { CompoundNode } from "./CompoundNode";
import { InformativeNode } from "./InformativeNode";

import type { NodeState, NodeVariant } from "../../stores/types";

const nodeComponentsByVariant: Record<NodeVariant, React.ComponentType> = {
  Compact: CompactNode,
  Informative: InformativeNode,
};

export function getNodeComponent(node: NodeState, nodeVariant: NodeVariant) {
  if (node.compound) {
    return CompoundNode;
  }
  
  return nodeComponentsByVariant[nodeVariant];
}
