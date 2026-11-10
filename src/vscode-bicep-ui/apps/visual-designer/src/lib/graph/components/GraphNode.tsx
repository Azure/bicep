// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodeState } from "../atoms";

import { useAtomValue, useAtomValueRawSync } from "jotai";
import { memo, useRef } from "react";
import { useNodeGeometry, useNodeGestures, useNodeZIndex } from "../hooks";
import { BaseNode } from "./BaseNode";
import { NodeContent } from "./NodeContent";

export interface GraphNodeProps {
  node: NodeState;
  isEntering?: boolean;
  isExiting?: boolean;
}

/**
 * A node in the graph. The same element is kept when the node becomes a container or stops being one.
 * It renders when its own data or children change, and measures itself whenever it does. A node whose
 * children are not placed yet, such as a module whose children are still being laid out, stays drawn as a
 * leaf, so it changes shape once, when they arrive.
 */
export const GraphNode = memo(function GraphNode({ node, isEntering = false, isExiting = false }: GraphNodeProps) {
  const ref = useRef<HTMLDivElement>(null);
  const hasChildren = useAtomValue(node.childIdsAtom).length > 0;
  // A node is often placed between its render and its effects, as when a layout answers within the frame, which
  // `useAtomValue` would miss: it subscribes in an effect. `useSyncExternalStore` checks again once subscribed.
  const isPlaced = useAtomValueRawSync(node.isPlacedAtom);
  const data = useAtomValue(node.dataAtom);
  // An exiting node keeps its shape while it fades, even once its children's positions are forgotten.
  const isContainer = hasChildren && (isPlaced || isExiting);
  const kind = isContainer ? "compound" : "atomic";

  useNodeZIndex(ref, node.id);
  useNodeGestures(ref, node.id);
  useNodeGeometry(ref, node, isContainer, isExiting);

  return (
    <BaseNode ref={ref} id={node.id} kind={kind} isEntering={isEntering} isPlaced={isPlaced} isExiting={isExiting}>
      <NodeContent id={node.id} kind={kind} data={data} />
    </BaseNode>
  );
});
