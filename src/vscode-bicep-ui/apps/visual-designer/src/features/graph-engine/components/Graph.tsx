// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { PanZoomTransformed } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { styled } from "styled-components";
import { layoutReadyAtom } from "../atoms";
import { InnerEdgeLayer, OuterEdgeLayer } from "./EdgeLayer";
import { NodeLayer } from "./NodeLayer";

const $PanZoomTransformed = styled(PanZoomTransformed)<{ $visible: boolean }>`
  transform-origin: 0 0;
  height: 0px;
  width: 0px;
  opacity: ${({ $visible }) => ($visible ? 1 : 0)};
`;

export function Graph() {
  const layoutReady = useAtomValue(layoutReadyAtom);

  return (
    <$PanZoomTransformed $visible={layoutReady}>
      <OuterEdgeLayer />
      <NodeLayer />
      <InnerEdgeLayer />
    </$PanZoomTransformed>
  );
}
