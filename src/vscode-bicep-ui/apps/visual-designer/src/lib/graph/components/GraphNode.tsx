// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodeState } from "../atoms";

import { useAtomValue } from "jotai";
import { useRef } from "react";
import { useNodeGeometry, useNodeGestures, useNodeZIndex } from "../hooks";
import { BaseNode } from "./BaseNode";
import { NodeContent } from "./NodeContent";

export interface GraphNodeProps {
  node: NodeState;
  isEntering?: boolean;
  isExiting?: boolean;
}

/** A node in the graph. The same element is kept when the node becomes a container or stops being one. */
export function GraphNode({ node, isEntering = false, isExiting = false }: GraphNodeProps) {
  const ref = useRef<HTMLDivElement>(null);
  const isContainer = useAtomValue(node.childIdsAtom).length > 0;
  const kind = isContainer ? "compound" : "atomic";

  useNodeZIndex(ref, node.id);
  useNodeGestures(ref, node.id);
  useNodeGeometry(ref, node, isContainer, isExiting);

  return (
    <BaseNode ref={ref} id={node.id} kind={kind} isEntering={isEntering} isExiting={isExiting}>
      <NodeContent id={node.id} kind={kind} dataAtom={node.dataAtom} />
    </BaseNode>
  );
}
