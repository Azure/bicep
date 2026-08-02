// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ComponentType } from "react";
import type { NodeKind } from "./lib/graph";
import type { DocumentDidChangePayload } from "./lib/messaging";

import { PanZoomProvider, useGetPanZoomDimensions } from "@vscode-bicep-ui/components";
import {
  useWebviewMessageChannel,
  useWebviewNotification,
  WebviewMessageChannelProvider,
} from "@vscode-bicep-ui/messaging";
import { getDefaultStore, useAtomValue, useSetAtom } from "jotai";
import { Suspense, useCallback, useEffect } from "react";
import { styled, ThemeProvider } from "styled-components";
import { ControlBar } from "./features/controls";
import { loadDevAppShell } from "./features/devtools";
import {
  effectiveExportThemeAtom,
  ExportAreaCover,
  ExportAreaPreview,
  exportCanvasElementAtom,
  exportFileStemAtom,
  ExportOverlay,
  isExportCanvasCoverVisibleAtom,
  isExportPreviewVisibleAtom,
} from "./features/export";
import { StatusBar } from "./features/status";
import { ModuleDeclaration, ResourceDeclaration } from "./features/visualization";
import { GlobalStyle } from "./GlobalStyle";
import { Canvas, Graph, nodeConfigAtom } from "./lib/graph";
import { useFitViewToBounds } from "./lib/graph/hooks";
import { DOCUMENT_DID_CHANGE_NOTIFICATION, READY_NOTIFICATION, useGraphUpdate } from "./lib/messaging";
import { useTheme } from "./lib/theming";

const DevAppShell = loadDevAppShell();

const store = getDefaultStore();
const nodeConfig = store.get(nodeConfigAtom);

const $ControlBarContainer = styled.div`
  position: absolute;
  top: 16px;
  right: 16px;
  z-index: 100;
`;

const $CanvasWrapper = styled.div`
  position: absolute;
  inset: 0;
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
  getContentComponent: (kind: NodeKind) => {
    if (kind === "compound") {
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
  const fitViewToBounds = useFitViewToBounds();
  const { requestGraphUpdate, resetLayout } = useGraphUpdate(getViewportCenter, fitViewToBounds);
  const messageChannel = useWebviewMessageChannel();
  const exportTheme = useAtomValue(effectiveExportThemeAtom);
  const setExportFileStem = useSetAtom(exportFileStemAtom);
  const setExportCanvasElement = useSetAtom(exportCanvasElementAtom);

  // Send READY notification on mount
  useEffect(() => {
    messageChannel.sendNotification({
      method: READY_NOTIFICATION,
    });
  }, [messageChannel]);

  // Listen for "the graph may have changed" notifications. The webview
  // pulls the update itself, submitting the graph it currently displays and applying the patches.
  useWebviewNotification(
    DOCUMENT_DID_CHANGE_NOTIFICATION,
    useCallback(
      (params: unknown) => {
        const payload = params as DocumentDidChangePayload;
        setExportFileStem(deriveExportFileStem(payload.documentUri));
        void requestGraphUpdate();
      },
      [requestGraphUpdate, setExportFileStem],
    ),
  );

  const canvasTheme = exportTheme;

  const handleCanvasRef = useCallback(
    (element: HTMLDivElement | null) => {
      setExportCanvasElement(element);
    },
    [setExportCanvasElement],
  );

  return (
    <>
      <$ControlBarContainer>
        <ControlBar requestLayout={resetLayout} />
      </$ControlBarContainer>
      <ExportUILayer />
      <ThemeProvider theme={canvasTheme}>
        <$CanvasWrapper ref={handleCanvasRef}>
          <Canvas>
            <ExportCanvasCoverLayer />
            <Graph />
          </Canvas>
        </$CanvasWrapper>
      </ThemeProvider>
    </>
  );
}

function ExportUILayer() {
  const isExportPreviewVisible = useAtomValue(isExportPreviewVisibleAtom);

  if (!isExportPreviewVisible) {
    return null;
  }

  return (
    <>
      <ExportOverlay />
      <ExportAreaPreview />
    </>
  );
}

function ExportCanvasCoverLayer() {
  const isExportCanvasCoverVisible = useAtomValue(isExportCanvasCoverVisibleAtom);

  if (!isExportCanvasCoverVisible) {
    return null;
  }

  return <ExportAreaCover />;
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
      <$AppContainer data-testid="app-root">
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
