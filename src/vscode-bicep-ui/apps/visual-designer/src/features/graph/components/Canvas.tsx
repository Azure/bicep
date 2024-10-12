import { PanZoom } from "@vscode-bicep-ui/components";
import type { PropsWithChildren } from "react";
import styled from "styled-components";

const $PanZoom = styled(PanZoom)`
  position: absolute;
  left: 0px;
  top: 0px;
  right: 0px;
  bottom: 0px;
  overflow: hidden;
`;

export function Canvas({ children }: PropsWithChildren) {
  return <$PanZoom>{children}</$PanZoom>
}
