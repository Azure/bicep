// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MouseEvent } from "react";

import { PanZoomProvider } from "@vscode-bicep-ui/components";
import { styled } from "styled-components";
import { Canvas, ResourceCreationError } from "@/features/canvas";
import { ControlBar } from "@/features/controls";
import { Dock } from "@/features/dock";
import { StatusBar } from "@/features/status";
import { isInTextInput } from "@/utils";
import { AppEnvironment } from "./AppEnvironment";

const $AppContainer = styled.div`
  flex: 1 1 auto;
  position: relative;
  overflow: hidden;
`;

/**
 * VS Code opens a Cut/Copy/Paste menu on right-click in webviews unless the event is already
 * handled. Those commands do nothing on the graph, so only text fields keep the menu.
 */
function suppressHostContextMenu(event: MouseEvent) {
  if (!isInTextInput(event.target)) {
    event.preventDefault();
  }
}

export function App() {
  return (
    <AppEnvironment>
      <$AppContainer data-testid="app-root" onContextMenu={suppressHostContextMenu}>
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
