// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ComponentType } from "react";
import type { NodeKind } from "./features/graph-engine/atoms";

import { PanZoomProvider, useGetPanZoomDimensions } from "@vscode-bicep-ui/components";
import { WebviewMessageChannelProvider, useWebviewMessageChannel, useWebviewNotification } from "@vscode-bicep-ui/messaging";
import type { WebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { getDefaultStore } from "jotai";
import { lazy, Suspense, useCallback, useEffect, useRef, useState } from "react";
import { styled, ThemeProvider } from "styled-components";
import { GlobalStyle } from "./GlobalStyle";
import { useTheme } from "./theming/useTheme";
import { GraphControlBar } from "./features/design-view/components/GraphControlBar";
import { ModuleDeclaration } from "./features/design-view/components/ModuleDeclaration";
import { ResourceDeclaration } from "./features/design-view/components/ResourceDeclaration";
import { nodeConfigAtom } from "./features/graph-engine/atoms";
import { Canvas, Graph } from "./features/graph-engine/components";
import { runLayout } from "./features/graph-engine/layout/elk-layout";
import { useApplyDeploymentGraph } from "./hooks/useDeploymentGraph";
import { useFitView } from "./hooks/useFitView";
import {
  DEPLOYMENT_GRAPH_NOTIFICATION,
  READY_NOTIFICATION,
  type DeploymentGraphPayload,
} from "./messages";

const isDev = typeof acquireVsCodeApi === "undefined";

// Lazy-load dev-only modules so they are code-split into a separate
// chunk and never downloaded in production (where acquireVsCodeApi exists).
const LazyDevToolbar = isDev
  ? lazy(() => import("./dev/DevToolbar").then((m) => ({ default: m.DevToolbar })))
  : undefined;

const loadFakeMessageChannel = isDev
  ? () => import("./dev/FakeMessageChannel").then((m) => new m.FakeMessageChannel())
  : undefined;

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
  getContentComponent: (kind: NodeKind, data: unknown) => {
    if (kind === "compound") {
      return ModuleDeclaration as ComponentType<{ id: string; data: unknown }>;
    }
    // An atomic node with a "path" field is a module demoted to leaf (no children).
    const record = data as Record<string, unknown> | null;
    if (record && "path" in record) {
      return ModuleDeclaration as ComponentType<{ id: string; data: unknown }>;
    }
    return ResourceDeclaration as ComponentType<{ id: string; data: unknown }>;
  },
});

/**
 * Inner component that lives inside PanZoomProvider so it can
 * access both the messaging channel and the pan-zoom controls.
 */
function GraphContainer() {
  const applyGraph = useApplyDeploymentGraph();
  const messageChannel = useWebviewMessageChannel();
  const fitView = useFitView();
  const getPanZoomDimensions = useGetPanZoomDimensions();
  const isFirstGraph = useRef(true);

  // Send READY notification on mount
  useEffect(() => {
    messageChannel.sendNotification({
      method: READY_NOTIFICATION,
    });
  }, [messageChannel]);

  // Listen for deployment graph updates
  useWebviewNotification(
    DEPLOYMENT_GRAPH_NOTIFICATION,
    useCallback(
      (params: unknown) => {
        const payload = params as DeploymentGraphPayload;
        applyGraph(payload.deploymentGraph);

        // Schedule ELK layout after DOM measurement (two frames),
        // then fit the view so the graph is centered in the viewport.
        const frame1 = requestAnimationFrame(() => {
          requestAnimationFrame(() => {
            const isFirst = isFirstGraph.current;
            // On the first layout, pass viewport dimensions so runLayout
            // can compute a centering offset.  Subsequent layouts reuse
            // the same offset automatically.
            const viewport = isFirst ? getPanZoomDimensions() : undefined;
            void runLayout(store, /* animate */ true, viewport).then(() => {
              if (isFirst) {
                isFirstGraph.current = false;
              }
            });
          });
        });

        return () => cancelAnimationFrame(frame1);
      },
      [applyGraph, fitView, getPanZoomDimensions],
    ),
  );

  return (
    <>
      <$ControlBarContainer>
        <GraphControlBar />
      </$ControlBarContainer>
      <Canvas>
        <Graph />
      </Canvas>
    </>
  );
}

const $AppContainer = styled.div`
  flex: 1 1 auto;
  position: relative;
  overflow: hidden;
`;

function VisualDesignerApp() {
  const theme = useTheme();

  return (
    <ThemeProvider theme={theme}>
      <GlobalStyle />
      <$AppContainer>
        <PanZoomProvider>
          <GraphContainer />
        </PanZoomProvider>
      </$AppContainer>
    </ThemeProvider>
  );
}

export function App() {
  const [fakeChannel, setFakeChannel] = useState<WebviewMessageChannel | undefined>(undefined);
  const fakeChannelRef = useRef<unknown>(undefined);

  useEffect(() => {
    if (!loadFakeMessageChannel || fakeChannelRef.current) return;
    fakeChannelRef.current = true; // prevent double-loading in StrictMode
    void loadFakeMessageChannel().then((ch) => {
      fakeChannelRef.current = ch;
      setFakeChannel(ch as unknown as WebviewMessageChannel);
    });
  }, []);

  // In production, render immediately. In dev, wait for the fake channel to load.
  if (isDev && !fakeChannel) return null;

  return (
    <WebviewMessageChannelProvider messageChannel={fakeChannel as unknown as WebviewMessageChannel}>
      {isDev && LazyDevToolbar && (
        <Suspense>
          <LazyDevToolbar channel={fakeChannelRef.current as never} />
        </Suspense>
      )}
      <VisualDesignerApp />
    </WebviewMessageChannelProvider>
  );
}
