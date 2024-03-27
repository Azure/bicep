import { getNodeComponent } from "./Nodes";
import { useStore } from "../stores";

export function NodeLayer() {
  const nodes = useStore(x => x.nodesById);
  const nodeVariant = useStore(x => x.nodeVariant);

  return (
    <div>
      {Object.values(nodes).map((node) => {
        const NodeComponent = getNodeComponent(node, nodeVariant);
        return <NodeComponent key={node.id} />;
      })}
    </div>
  );
}
