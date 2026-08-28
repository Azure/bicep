// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { PanZoomProvider } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { styled } from "styled-components";
import { ControlBar } from "@/features/controls";
import { DeploymentGraphView, ResourceCreationError } from "@/features/deployment-graph";
import {
  ExportAreaCover,
  ExportAreaPreview,
  ExportOverlay,
  isExportCanvasCoverVisibleAtom,
  isExportPreviewVisibleAtom,
} from "@/features/export";
import { Palette } from "@/features/palette";
import { StatusBar } from "@/features/status";
import { AppProviders } from "./AppProviders";

const $AppContainer = styled.div`
  flex: 1 1 auto;
  position: relative;
  overflow: hidden;
`;

const $ControlBarContainer = styled.div`
  position: absolute;
  top: 16px;
  right: 16px;
  z-index: 100;
`;

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

export function App() {
  return (
    <AppProviders>
      <$AppContainer data-testid="app-root">
        <PanZoomProvider>
          <DeploymentGraphView canvasOverlay={<ExportCanvasCoverLayer />}>
            {({ canPlaceAt, createResource, resetLayout }) => (
              <>
                <$ControlBarContainer>
                  <ControlBar requestLayout={resetLayout} />
                </$ControlBarContainer>
                <ExportUILayer />
                <Palette createResource={createResource} canPlaceAt={canPlaceAt} />
              </>
            )}
          </DeploymentGraphView>
        </PanZoomProvider>
        <ResourceCreationError />
        <StatusBar />
      </$AppContainer>
    </AppProviders>
  );
}
