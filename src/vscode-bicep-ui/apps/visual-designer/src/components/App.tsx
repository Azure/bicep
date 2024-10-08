import type { MouseEvent } from "react";

import { useSetAtom } from "jotai";
import { useAtomCallback } from "jotai/utils";
import { useCallback, useEffect } from "react";
import { Canvas } from "./graph";
import { addEdgeAtom, edgesAtom } from "./graph/edges/atoms";
import { addCompoundNodeAtom, addPrimitiveNodeAtom, isPrimitive, nodesAtom } from "./graph/nodes";

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
        if (isPrimitive(node)) {
          set(node.originAtom, { ...get(node.originAtom) });
        }
      }
    }, []),
  );

  useEffect(() => {
    addPrimitiveNode(
      "A",
      { x: 200, y: 200 },
      { min: { x: 200, y: 200 }, max: { x: 300, y: 300 } },
      { resourceType: "Foo" },
    );

    addPrimitiveNode(
      "B",
      { x: 500, y: 200 },
      { min: { x: 500, y: 200 }, max: { x: 600, y: 300 } },
      { resourceType: "Bar" },
    );

    addPrimitiveNode(
      "C",
      { x: 800, y: 500 },
      { min: { x: 800, y: 500 }, max: { x: 900, y: 600 } },
      { resourceType: "Baz" },
    );

    addPrimitiveNode(
      "D",
      { x: 1200, y: 700 },
      { min: { x: 1000, y: 800 }, max: { x: 1200, y: 900 } },
      { resourceType: "Foobar" },
    );

    addCompoundNode("E", ["A", "C"], { resourceType: "Foobar" });

    addEdge("A->B", "A", "B");
    addEdge("E->D", "E", "D");
    addEdge("C->B", "C", "B");

    return () => {
      setEdgesAtom([]);
      setNodesAtom({});
    }
  }, [addCompoundNode, addPrimitiveNode, addEdge, setNodesAtom, setEdgesAtom]);

  return (
    <>
      <Canvas />
      <button style={{ position: "absolute", zIndex: 100, left: 10, top: 10 }} onClick={layout}>
        Layout
      </button>
    </>
  );
}
