// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom } from "jotai";
import type { PropsWithChildren } from "react";
import type { GraphContextValue, GraphNodeEvents } from "../context/GraphContext";

import { PanZoomTransformed } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { useMemo } from "react";
import { styled } from "styled-components";
import { layoutReadyAtom } from "../atoms";
import { createPaintOrderAtom, GraphContext, ignoreNodeEvent } from "../context/GraphContext";
import { usePresence } from "../hooks/use-presence";
import { EdgeLayer } from "./EdgeLayer";
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

export type GraphProps = PropsWithChildren<
  Partial<GraphNodeEvents> & {
    /** The node painted above the others, with its subtree. Its ancestors are raised above their siblings. */
    raisedNodeIdAtom?: Atom<string | null>;
    /** Snap nodes to new positions and shapes, and show and hide them without fading. */
    reduceMotion?: boolean;
  }
>;

export function Graph({
  children,
  raisedNodeIdAtom,
  reduceMotion = false,
  onNodeDragStart = ignoreNodeEvent,
  onNodeDrag = ignoreNodeEvent,
  onNodeDragEnd = ignoreNodeEvent,
  onNodeDoubleClick = ignoreNodeEvent,
}: GraphProps) {
  const layoutReady = useAtomValue(layoutReadyAtom);
  const paintOrderAtom = useMemo(() => createPaintOrderAtom(raisedNodeIdAtom), [raisedNodeIdAtom]);
  const { exitingNodes, exitingEdges, enteringNodeIds } = usePresence(reduceMotion);
  const context = useMemo<GraphContextValue>(
    () => ({
      events: { onNodeDragStart, onNodeDrag, onNodeDragEnd, onNodeDoubleClick },
      reduceMotion,
      paintOrderAtom,
    }),
    [onNodeDoubleClick, onNodeDrag, onNodeDragEnd, onNodeDragStart, paintOrderAtom, reduceMotion],
  );

  return (
    <GraphContext.Provider value={context}>
      <$PanZoomTransformed $visible={layoutReady} data-export-graph="">
        <$SharedDefs>
          <EdgeMarkerDefs />
        </$SharedDefs>
        {children}
        <EdgeLayer exitingNodes={exitingNodes} exitingEdges={exitingEdges} enteringNodeIds={enteringNodeIds} />
        <NodeLayer exitingNodes={exitingNodes} enteringNodeIds={enteringNodeIds} />
      </$PanZoomTransformed>
    </GraphContext.Provider>
  );
}
