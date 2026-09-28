// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MouseEvent } from "react";

import { PanZoomProvider } from "@vscode-bicep-ui/components";
import { styled } from "styled-components";
import { GraphActionsProvider } from "@/core";
import { Canvas } from "@/features/canvas";
import { Controls } from "@/features/controls";
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
 * Lays out the chrome along the bottom edge: the status at the left and the dock at the center.
 *
 * Both sit on the same 16px inset from the bottom as the other chrome does from the top, so their
 * bottom edges line up whatever their heights. The equal side columns keep the dock centered while the
 * status truncates before reaching it. The layer covers the canvas so the dock's popover can grow
 * upward; it is a size container so the popover can size itself against that space.
 */
const $BottomChrome = styled.div`
  position: absolute;
  inset: 16px;
  z-index: 200;
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto minmax(0, 1fr);
  grid-template-rows: minmax(0, 1fr) auto;
  grid-template-areas:
    ". . ."
    "status dock .";
  align-items: end;
  column-gap: 12px;
  container-type: size;
  pointer-events: none;
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
          <GraphActionsProvider>
            <Canvas />
            <Controls />
            <$BottomChrome>
              <StatusBar />
              <Dock />
            </$BottomChrome>
          </GraphActionsProvider>
        </PanZoomProvider>
      </$AppContainer>
    </AppEnvironment>
  );
}
