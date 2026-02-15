// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ComponentType } from "react";
import type { NodeKind } from "./features/graph-engine/atoms";

import { PanZoomProvider } from "@vscode-bicep-ui/components";
import { getDefaultStore, useSetAtom } from "jotai";
import { useEffect } from "react";
import { styled, ThemeProvider } from "styled-components";
import { GraphControlBar } from "./features/design-view/components/GraphControlBar";
import { ModuleDeclaration } from "./features/design-view/components/ModuleDeclaration";
import { ResourceDeclaration } from "./features/design-view/components/ResourceDeclaration";
import {
  addAtomicNodeAtom,
  addCompoundNodeAtom,
  addEdgeAtom,
  edgesAtom,
  nodeConfigAtom,
  nodesAtom,
} from "./features/graph-engine/atoms";
import { Canvas, Graph } from "./features/graph-engine/components";
import { runLayout } from "./features/graph-engine/layout/elk-layout";
import { GlobalStyle } from "./GlobalStyle";
import { useTheme } from "./theming/useTheme";

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
    addCompoundNode("E", ["A", "C"], {
      symbolicName: "myMod",
      path: "modules/foooooooooooooooooooooooooooooooooooooobar.bicep",
    });

    addEdge("A->B", "A", "B");
    addEdge("E->D", "E", "D");
    addEdge("C->B", "C", "B");

    // Wait for DOM measurement (two frames) then run auto-layout
    const frame1 = requestAnimationFrame(() => {
      const frame2 = requestAnimationFrame(() => {
        void runLayout(store);
      });

      cleanup = () => cancelAnimationFrame(frame2);
    });

    let cleanup: (() => void) | undefined;

    return () => {
      cancelAnimationFrame(frame1);
      cleanup?.();
      setEdgesAtom([]);
      setNodesAtom({});
    };
  }, [addCompoundNode, addAtomicNode, addEdge, setNodesAtom, setEdgesAtom]);

  const theme = useTheme();

  return (
    <ThemeProvider theme={theme}>
      <GlobalStyle />
      <PanZoomProvider>
        <$ControlBarContainer>
          <GraphControlBar />
        </$ControlBarContainer>
        <Canvas>
          <Graph />
        </Canvas>
      </PanZoomProvider>
    </ThemeProvider>
  );
}
