// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { PanZoomProvider } from "@vscode-bicep-ui/components";
import { styled } from "styled-components";
import { Canvas, ResourceCreationError } from "@/features/canvas";
import { ControlBar } from "@/features/controls";
import { Dock } from "@/features/dock";
import { StatusBar } from "@/features/status";
import { AppEnvironment } from "./AppEnvironment";

const $AppContainer = styled.div`
  flex: 1 1 auto;
  position: relative;
  overflow: hidden;
`;

export function App() {
  return (
    <AppEnvironment>
      <$AppContainer data-testid="app-root">
        <PanZoomProvider>
          <Canvas>
            <ControlBar />
            <Dock />
          </Canvas>
        </PanZoomProvider>
        <ResourceCreationError />
        <StatusBar />
      </$AppContainer>
    </AppEnvironment>
  );
}
