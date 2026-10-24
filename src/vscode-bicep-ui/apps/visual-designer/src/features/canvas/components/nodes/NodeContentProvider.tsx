// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { CanvasNodeData } from "@/core";
import type { NodeContentRenderProps, NodeKind } from "@/lib/graph";

import { useStore } from "jotai";
import { useHydrateAtoms } from "jotai/utils";
import { nodeConfigAtom } from "@/lib/graph";
import { ModuleNode } from "./ModuleNode";
import { ResourceNode } from "./ResourceNode";

/** Extra headroom above a compound node's children, so the module label has room to sit. */
const COMPOUND_NODE_LABEL_INSET = 50;

function renderNodeContent(kind: NodeKind, { id, data }: NodeContentRenderProps) {
  // Core shows every node with `CanvasNodeData`.
  const nodeData = data as CanvasNodeData;

  return nodeData.nodeKind === "module" ? (
    <ModuleNode id={id} kind={kind} data={nodeData} />
  ) : (
    <ResourceNode id={id} data={nodeData} />
  );
}

/**
 * Teaches the generic graph engine how to render Bicep node content.
 *
 * `lib/graph` is Bicep-agnostic and reaches product code only through `nodeConfigAtom`. Hydrating
 * during render rather than in an effect guarantees the config is in place before any node mounts,
 * and scoping the write to the store from context keeps it out of module scope so tests can supply
 * their own store.
 */
export function NodeContentProvider({ children }: { children: ReactNode }) {
  const store = useStore();
  const defaults = store.get(nodeConfigAtom);

  useHydrateAtoms([
    [
      nodeConfigAtom,
      {
        ...defaults,
        padding: { ...defaults.padding, top: COMPOUND_NODE_LABEL_INSET },
        renderContent: renderNodeContent,
      },
    ],
  ] as const);

  return <>{children}</>;
}
