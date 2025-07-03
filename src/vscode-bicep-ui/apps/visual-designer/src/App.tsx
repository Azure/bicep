// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ComponentType } from "react";
import type { NodeKind } from "./features/graph-engine/atoms";

import { PanZoomProvider } from "@vscode-bicep-ui/components";
import { getDefaultStore, useSetAtom } from "jotai";
import { useEffect } from "react";
import { styled } from "styled-components";
import { ModuleDeclaration } from "./features/design-view/components/ModuleDeclaration";
import { ResourceDeclaration } from "./features/design-view/components/ResourceDeclaration";
import { GraphControlBar } from "./features/design-view/components/GraphControlBar";
import {
  addCompoundNodeAtom,
  addEdgeAtom,
  addAtomicNodeAtom,
  edgesAtom,
  nodeConfigAtom,
  nodesAtom,
} from "./features/graph-engine/atoms";
import { Canvas, Graph } from "./features/graph-engine/components";

const store = getDefaultStore();
const nodeConfig = store.get(nodeConfigAtom);

const $ControlBarContainer = styled.div`
  position: absolute;
  top: 16px;
  right: 16px;
  z-index: 100;
`;

store.set(nodeConfigAtom, {
  ...nodeConfig,
  padding: {
    ...nodeConfig.padding,
    top: 50,
  },
  getContentComponent: (kind: NodeKind) =>
    (kind === "atomic" ? ResourceDeclaration : ModuleDeclaration) as ComponentType<{ id: string; data: unknown }>,
});

export function App() {
  const setNodesAtom = useSetAtom(nodesAtom);
  const setEdgesAtom = useSetAtom(edgesAtom);
  const addAtomicNode = useSetAtom(addAtomicNodeAtom);
  const addCompoundNode = useSetAtom(addCompoundNodeAtom);
  const addEdge = useSetAtom(addEdgeAtom);

  useEffect(() => {
    addAtomicNode(
      "A",
      { x: 200, y: 200 },
      { symbolicName: "foobar", resourceType: "Microsoft.Compute/virtualMachines" },
    );
    addAtomicNode("B", { x: 500, y: 200 }, { symbolicName: "bar", resourceType: "Foo" });
    addAtomicNode("C", { x: 800, y: 500 }, { symbolicName: "someRandomStorage", resourceType: "Foo" });
    addAtomicNode("D", { x: 1200, y: 700 }, { symbolicName: "Tricep", resourceType: "Foo" });
    addCompoundNode("E", ["A", "C"], { symbolicName: "myMod", path: "modules/foooooooooooooooooooooooooooooooooooooobar.bicep" });

    addEdge("A->B", "A", "B");
    addEdge("E->D", "E", "D");
    addEdge("C->B", "C", "B");

    return () => {
      setEdgesAtom([]);
      setNodesAtom({});
    };
  }, [addCompoundNode, addAtomicNode, addEdge, setNodesAtom, setEdgesAtom]);

  return (
    <PanZoomProvider>
      <$ControlBarContainer>
        <GraphControlBar />
      </$ControlBarContainer>
      <Canvas>
        <Graph />
      </Canvas>
    </PanZoomProvider>
  );
}
