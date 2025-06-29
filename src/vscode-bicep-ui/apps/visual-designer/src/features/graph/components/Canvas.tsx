// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import { PanZoom } from "@vscode-bicep-ui/components";
import styled from "styled-components";
import { CanvasBackground } from "./CanvasBackground";

const $PanZoom = styled(PanZoom)`
  position: absolute;
  left: 0px;
  top: 0px;
  right: 0px;
  bottom: 0px;
  overflow: hidden;
  &:active {
  cursor: url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" fill="%23000000" fill-opacity="0.6" width="32px" height="32px" viewBox="0 0 10 10"><circle cx="5" cy="5" r="4"/></svg>') 16 16, auto;
  }
`;

export function Canvas({ children }: PropsWithChildren) {
  return (
    <$PanZoom>
      <CanvasBackground />
      {children}
    </$PanZoom>
  );
}
