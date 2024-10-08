import { PanZoom } from "@vscode-bicep-ui/components";
import styled from "styled-components";
import { Graph } from "./Graph";

const $CanvasPanel = styled(PanZoom)`
  position: absolute;
  left: 0px;
  top: 0px;
  right: 0px;
  bottom: 0px;
  overflow: hidden;
`;

export function Canvas() {
  return (
    <$CanvasPanel>
      <Graph />
    </$CanvasPanel>
  );
}
