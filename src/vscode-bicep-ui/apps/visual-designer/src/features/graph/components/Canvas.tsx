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
`;

export function Canvas({ children }: PropsWithChildren) {
  return (
    <$PanZoom>
      <CanvasBackground />
      {children}
    </$PanZoom>
  );
}
