import type { MouseEvent } from "react";

import { useSetAtom } from "jotai";
import { useAtomCallback } from "jotai/utils";
import { useCallback, useEffect } from "react";
import { Canvas } from "./graph";
import { addCompoundNodeAtom, addPrimitiveNodeAtom, isPrimitive, nodesAtom } from "./graph/nodes";

export function App() {
  const addPrimitiveNode = useSetAtom(addPrimitiveNodeAtom);
  const addCompoundNode = useSetAtom(addCompoundNodeAtom);

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

    addCompoundNode(
      "E",
      ["A", "C"],
      { resourceType: "Foobar" },
    );
  }, [addCompoundNode, addPrimitiveNode]);

  return (
    <>
      <Canvas />
      <button style={{ position: "absolute", zIndex: 100, left: 10, top: 10 }} onClick={layout}>Layout</button>
    </>
  );
}
