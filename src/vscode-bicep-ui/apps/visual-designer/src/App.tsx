// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ComponentType } from "react";
import type { DefaultTheme } from "styled-components";
import type { NodeKind } from "./features/graph-engine";

import { PanZoomProvider, useGetPanZoomDimensions } from "@vscode-bicep-ui/components";
import { WebviewMessageChannelProvider, useWebviewMessageChannel, useWebviewNotification } from "@vscode-bicep-ui/messaging";
import { getDefaultStore } from "jotai";
import { Suspense, useCallback, useEffect, useRef, useState } from "react";
import { styled, ThemeProvider } from "styled-components";
import { GlobalStyle } from "./GlobalStyle";
import { useTheme } from "./theming/use-theme";
import { GraphControlBar } from "./components/GraphControlBar";
import { StatusBar } from "./components/StatusBar";
import { ExportAreaCover, ExportAreaPreview, ExportOverlay, type ExportFormat } from "./features/export";
import { getThemeByName } from "./theming/themes";
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

function deriveExportFileStem(documentPath?: string, documentFileName?: string): string {
  const fileName = (documentFileName || documentPath || "").split(/[\\/]/).pop() ?? "";
  const stem = fileName.replace(/\.[^.]+$/, "").trim();

  return stem || "bicep-graph";
}

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
  const getPanZoomDimensions = useGetPanZoomDimensions();
  const getViewportCenter = useCallback(() => {
    const { width, height } = getPanZoomDimensions();
    return { x: width / 2, y: height / 2 };
  }, [getPanZoomDimensions]);
  const applyGraph = useApplyDeploymentGraph(getViewportCenter);
  const messageChannel = useWebviewMessageChannel();
  const [isExporting, setIsExporting] = useState(false);
  const [exportPadding, setExportPadding] = useState(40);
  const [exportFormat, setExportFormat] = useState<ExportFormat>("png");
  const [exportThemeName, setExportThemeName] = useState<DefaultTheme["name"] | null>(null);
  const [exportFileStem, setExportFileStem] = useState("bicep-graph");
  const canvasRef = useRef<HTMLDivElement>(null);

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
        setExportFileStem(deriveExportFileStem(payload.documentPath, payload.documentFileName));
      },
      [applyGraph],
    ),
  );

  useAutoLayout();

  const handleOpenExport = useCallback(() => {
    setIsExporting(true);
  }, []);

  const handleCloseExport = useCallback(() => {
    setIsExporting(false);
    setExportThemeName(null);
  }, []);

  const currentTheme = useTheme();
  const exportTheme = exportThemeName ? getThemeByName(exportThemeName) : null;
  const canvasTheme = exportTheme ?? currentTheme;

  return (
    <>
      <$ControlBarContainer>
        <GraphControlBar onExport={handleOpenExport} />
      </$ControlBarContainer>
      {isExporting && (
        <ExportOverlay
          canvasElement={canvasRef.current}
          exportFileStem={exportFileStem}
          onClose={handleCloseExport}
          onPaddingChange={setExportPadding}
          onFormatChange={setExportFormat}
          onThemeChange={setExportThemeName}
        />
      )}
      <ThemeProvider theme={canvasTheme}>
        <div ref={canvasRef} style={{ position: "absolute", inset: 0 }}>
          <Canvas>
            {isExporting && exportFormat === "jpeg" && <ExportAreaCover padding={exportPadding} />}
            <Graph />
          </Canvas>
        </div>
      </ThemeProvider>
      {isExporting && <ExportAreaPreview padding={exportPadding} />}
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
