// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { PanZoomTransformed } from "@vscode-bicep-ui/components";
import { styled } from "styled-components";
import { InnerEdgeLayer, OuterEdgeLayer } from "./EdgeLayer";
import { NodeLayer } from "./NodeLayer";

const $PanZoomTransformed = styled(PanZoomTransformed)`
  transform-origin: 0 0;
  height: 0px;
  width: 0px;
`;

export function Graph() {
  return (
    <$PanZoomTransformed>
      <OuterEdgeLayer />
      <NodeLayer />
      <InnerEdgeLayer />
    </$PanZoomTransformed>
  );
}
