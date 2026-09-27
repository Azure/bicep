// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";
import type { NodeDragCallbacks } from "../context/NodeDragCallbacksContext";

import { PanZoomTransformed } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { useMemo } from "react";
import { styled } from "styled-components";
import { layoutReadyAtom } from "../atoms";
import { ignoreNodeDrag, NodeDragCallbacksContext } from "../context/NodeDragCallbacksContext";
import { InnerEdgeLayer, OuterEdgeLayer } from "./EdgeLayer";
import { EdgeMarkerDefs } from "./EdgeMarkerDefs";
import { NodeLayer } from "./NodeLayer";

const $PanZoomTransformed = styled(PanZoomTransformed)<{ $visible: boolean }>`
  transform-origin: 0 0;
  height: 0px;
  width: 0px;
  opacity: ${({ $visible }) => ($visible ? 1 : 0)};
`;

const $SharedDefs = styled.svg`
  position: absolute;
  width: 0;
  height: 0;
  overflow: hidden;
  pointer-events: none;
`;

export type GraphProps = PropsWithChildren<Partial<NodeDragCallbacks>>;

export function Graph({ children, onNodeDragStart = ignoreNodeDrag, onNodeDragEnd = ignoreNodeDrag }: GraphProps) {
  const layoutReady = useAtomValue(layoutReadyAtom);
  const nodeDragCallbacks = useMemo(() => ({ onNodeDragStart, onNodeDragEnd }), [onNodeDragEnd, onNodeDragStart]);

  return (
    <NodeDragCallbacksContext.Provider value={nodeDragCallbacks}>
      <$PanZoomTransformed $visible={layoutReady} data-export-graph="">
        <$SharedDefs>
          <EdgeMarkerDefs />
        </$SharedDefs>
        {children}
        <OuterEdgeLayer />
        <NodeLayer />
        <InnerEdgeLayer />
      </$PanZoomTransformed>
    </NodeDragCallbacksContext.Provider>
  );
}
