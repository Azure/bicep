import { getEdgeComponent } from "./Edges";
import { useStore } from "../stores";

export function EdgeLayer() {
  const edges = useStore(x => x.edges);
  const edgeShape = useStore(x => x.edgeShape);

  return (
    <div>
      {edges.map((_edge, index) => {
        const EdgeComponent = getEdgeComponent(edgeShape);
        return <EdgeComponent key={index} />;
      })}
    </div>
  );
}
