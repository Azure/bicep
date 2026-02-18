// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import { PanZoom } from "@vscode-bicep-ui/components";
import styled, { useTheme } from "styled-components";
import { CanvasBackground } from "./CanvasBackground";

function buildGrabCursor(fill: string, opacity: number): string {
  return `url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" fill="${fill}" fill-opacity="${opacity}" width="32px" height="32px" viewBox="0 0 10 10"><circle cx="5" cy="5" r="4"/></svg>') 16 16, auto`;
}

const $PanZoom = styled(PanZoom)<{ $grabCursorUrl: string }>`
  position: absolute;
  left: 0px;
  top: 0px;
  right: 0px;
  bottom: 0px;
  overflow: hidden;
  &:active {
    cursor: ${({ $grabCursorUrl }) => $grabCursorUrl};
  }
`;

export function Canvas({ children }: PropsWithChildren) {
  const theme = useTheme();
  const grabCursorUrl = buildGrabCursor(theme.grabCursor.fill, theme.grabCursor.opacity);

  return (
    <$PanZoom $grabCursorUrl={grabCursorUrl}>
      <CanvasBackground />
      {children}
    </$PanZoom>
  );
}
