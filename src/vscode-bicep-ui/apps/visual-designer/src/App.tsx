// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ComponentType } from "react";
import type { NodeKind } from "./features/graph-engine";

import { PanZoomProvider } from "@vscode-bicep-ui/components";
import { WebviewMessageChannelProvider, useWebviewMessageChannel, useWebviewNotification } from "@vscode-bicep-ui/messaging";
import { getDefaultStore } from "jotai";
import { Suspense, useCallback, useEffect } from "react";
import { styled, ThemeProvider } from "styled-components";
import { GlobalStyle } from "./GlobalStyle";
import { useTheme } from "./theming/use-theme";
import { GraphControlBar } from "./components/GraphControlBar";
import { StatusBar } from "./components/StatusBar";
import { ModuleDeclaration, ResourceDeclaration } from "./features/visualization";
import { nodeConfigAtom, Canvas, Graph } from "./features/graph-engine";
import { loadDevAppShell } from "./features/devtools";
import { useAutoLayout } from "./features/layout";
import { useApplyDeploymentGraph } from "./features/messaging";
import {
  DEPLOYMENT_GRAPH_NOTIFICATION,
  READY_NOTIFICATION,
  type DeploymentGraphPayload,
} from "./messages";

const DevAppShell = loadDevAppShell();

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
      },
      [applyGraph],
    ),
  );

  useAutoLayout();

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

function AppCore() {
  const theme = useTheme();

  return (
    <ThemeProvider theme={theme}>
      <GlobalStyle />
      <$AppContainer>
        <PanZoomProvider>
          <GraphContainer />
        </PanZoomProvider>
        <StatusBar />
      </$AppContainer>
    </ThemeProvider>
  );
}

export function App() {
  // In dev mode, the lazy-loaded DevAppShell provides
  // a FakeMessageChannel, the DevToolbar, and the message-channel
  // context.  In production, we render straight into the provider
  // which creates its own channel via acquireVsCodeApi.
  if (DevAppShell) {
    return (
      <Suspense>
        <DevAppShell>
          <AppCore />
        </DevAppShell>
      </Suspense>
    );
  }

  return (
    <WebviewMessageChannelProvider>
      <AppCore />
    </WebviewMessageChannelProvider>
  );
}
