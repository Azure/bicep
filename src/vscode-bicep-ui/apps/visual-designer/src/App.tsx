/* eslint-disable @typescript-eslint/no-unused-vars */
import type { MouseEvent } from "react";

import { PanZoomProvider } from "@vscode-bicep-ui/components";
import { useSetAtom } from "jotai";
import { useAtomCallback } from "jotai/utils";
import { useCallback, useEffect } from "react";
import { addCompoundNodeAtom, addEdgeAtom, addPrimitiveNodeAtom, edgesAtom, nodesAtom } from "./features/graph/atoms";
import { Canvas } from "./features/graph/components";
import { useSetNodeConfig } from "./features/graph/hooks";

function DummyComponent(_: { data: unknown }) {
  return <div />
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

  const updateSize = useAtomCallback(
    useCallback((get, set, event: MouseEvent<HTMLButtonElement>) => {
      event.stopPropagation();

      const nodes = get(nodesAtom);
      for (const node of Object.values(nodes)) {
        if (node.kind === "primitive") {
          set(node.dataAtom, { resourceType: "Bar" });
          console.log(get(node.dataAtom));
        }
      }
    }, []),
  );

  useSetNodeConfig({
    resolveNodeComponent: (_) => DummyComponent,
  });

  useEffect(() => {
    addPrimitiveNode("A", { x: 200, y: 200 }, { resourceType: "Foo" });
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
      <Canvas />
      <button style={{ position: "absolute", zIndex: 100, left: 10, top: 10 }} onClick={layout}>
        Layout
      </button>
      <button style={{ position: "absolute", zIndex: 100, left: 100, top: 10 }} onClick={updateSize}>
        UpdateSize
      </button>
    </PanZoomProvider>
  );
}
