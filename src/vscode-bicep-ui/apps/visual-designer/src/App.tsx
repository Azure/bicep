/* eslint-disable @typescript-eslint/no-unused-vars */
import type { ComponentType, MouseEvent } from "react";

import { PanZoomProvider } from "@vscode-bicep-ui/components";
import { useSetAtom } from "jotai";
import { useAtomCallback } from "jotai/utils";
import { useCallback, useEffect } from "react";
import { addCompoundNodeAtom, addEdgeAtom, addPrimitiveNodeAtom, edgesAtom, nodesAtom } from "./features/graph/atoms";
import { Canvas, Graph } from "./features/graph/components";
import { useSetNodeConfig } from "./features/graph/hooks";

function DummyComponent({ id, data }: { id: string; data: { resourceType: string } }) {
  return <div>{id}:{data.resourceType}</div>;
}

export function App() {
  const setNodesAtom = useSetAtom(nodesAtom);
  const setEdgesAtom = useSetAtom(edgesAtom);
  const addPrimitiveNode = useSetAtom(addPrimitiveNodeAtom);
  const addCompoundNode = useSetAtom(addCompoundNodeAtom);
  const addEdge = useSetAtom(addEdgeAtom);

  const layout = useAtomCallback(
    useCallback((get, set, event: MouseEvent<HTMLButtonElement>) => {
      event.stopPropagation();

      const nodes = get(nodesAtom);
      for (const node of Object.values(nodes)) {
        if (node.kind === "primitive") {
          set(node.originAtom, { ...get(node.originAtom) });
        }
      }
    }, []),
  );

  useSetNodeConfig({
    resolveNodeContentComponent: (_data) => DummyComponent as ComponentType<{ id: string, data: unknown }>,
  });

  useEffect(() => {
    addPrimitiveNode("A", { x: 200, y: 200 }, { resourceType: "Foobar" });
    addPrimitiveNode("B", { x: 500, y: 200 }, { resourceType: "Foo" });
    addPrimitiveNode("C", { x: 800, y: 500 }, { resourceType: "Foo" });
    addPrimitiveNode("D", { x: 1200, y: 700 }, { resourceType: "Foo" });
    addCompoundNode("E", ["A", "C"], { resourceType: "Foo" });

    addEdge("A->B", "A", "B");
    addEdge("E->D", "E", "D");
    addEdge("C->B", "C", "B");

    return () => {
      setEdgesAtom([]);
      setNodesAtom({});
    };
  }, [addCompoundNode, addPrimitiveNode, addEdge, setNodesAtom, setEdgesAtom]);

  return (
    <PanZoomProvider>
      <Canvas>
        <Graph />
        <button style={{ position: "absolute", zIndex: 100, left: 10, top: 10 }} onClick={layout}>
          Layout
        </button>
      </Canvas>
    </PanZoomProvider>
  );
}
