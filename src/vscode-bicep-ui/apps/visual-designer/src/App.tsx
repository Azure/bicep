// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ComponentType, MouseEvent } from "react";
import type { NodeKind } from "./features/graph/atoms";

import { PanZoomProvider } from "@vscode-bicep-ui/components";
import { getDefaultStore, useSetAtom } from "jotai";
import { useAtomCallback } from "jotai/utils";
import { useCallback, useEffect } from "react";
import { ModuleDeclaration } from "./features/declarations/components/ModuleDeclaration";
import { ResourceDeclaration } from "./features/declarations/components/ResourceDeclaration";
import {
  addCompoundNodeAtom,
  addEdgeAtom,
  addPrimitiveNodeAtom,
  edgesAtom,
  nodeConfigAtom,
  nodesAtom,
} from "./features/graph/atoms";
import { Canvas, Graph } from "./features/graph/components";

const store = getDefaultStore();
const nodeConfig = store.get(nodeConfigAtom);

store.set(nodeConfigAtom, {
  ...nodeConfig,
  padding: {
    ...nodeConfig.padding,
    top: 50,
  },
  getContentComponent: (kind: NodeKind) =>
    (kind === "primitive" ? ResourceDeclaration : ModuleDeclaration) as ComponentType<{ id: string; data: unknown }>,
});

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

  useEffect(() => {
    addPrimitiveNode(
      "A",
      { x: 200, y: 200 },
      { symbolicName: "foobar", resourceType: "Microsoft.Compute/virtualMachines" },
    );
    addPrimitiveNode("B", { x: 500, y: 200 }, { symbolicName: "bar", resourceType: "Foo" });
    addPrimitiveNode("C", { x: 800, y: 500 }, { symbolicName: "someRandomStorage", resourceType: "Foo" });
    addPrimitiveNode("D", { x: 1200, y: 700 }, { symbolicName: "Tricep", resourceType: "Foo" });
    addCompoundNode("E", ["A", "C"], { symbolicName: "myMod", path: "modules/foooooooooooooooooooooooooooooooooooooobar.bicep" });

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
